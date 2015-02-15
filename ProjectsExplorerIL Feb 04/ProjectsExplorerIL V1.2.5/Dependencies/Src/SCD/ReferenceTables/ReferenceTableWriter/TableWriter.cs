using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DatabaseProvider;
using LastUpdateUtilities;
using MessagesManager;
using ReferenceTableReader;
using TraceExceptionWrapper;

namespace ReferenceTableWriter
{
	public class TableWriter
	{
		private static void GetRecordUpdateInfoFromDatabase(ReferenceTable.TableProperties properties, string key, out DateTime newDateTime, out string newUser)
		{
			var dataTable = GetDataTable(properties, key);
			if (dataTable.Rows.Count == 0)
				throw new TraceException("Table key not exist", new List<string> { key, properties.TableName }, LastUpdateWrite.Details.Application);
			GetUpdateInfoFromDataRow(dataTable.Rows[0], out newDateTime, out newUser);
		}

		private static DataTable GetDataTable(ReferenceTable.TableProperties properties, string key)
		{
			string sqlCommand;

			sqlCommand = "Select ";
			sqlCommand += properties.TitleTime + " as UpdateTime, ";
			sqlCommand += properties.TitleUser + " as UpdateUser ";
			sqlCommand += "From ";
			sqlCommand += properties.TableName;
			sqlCommand += " Where ";
			sqlCommand += properties.TitleKey + " = '" + key + "' ";

			return DatabaseConnection.DBaseRTable.ExecuteSelectCommand(sqlCommand);
		}

		private static void GetUpdateInfoFromDataRow(DataRow row, out DateTime newDateTime, out string newUser)
		{
			newDateTime = DBprovider.GetDateParam(row, "UpdateTime");
			newUser = DBprovider.GetStringParam(row, "UpdateUser");
		}

		public static void UpdateRecord(ReferenceTable.TableProperties properties, string key, DateTime readTime, Dictionary<string, object> newData)
		{
			try
			{
				DatabaseConnection.DBaseRTable.OpenConnection();

				CheckExistingValues(properties, key, newData, false);

				/*** check if already being updated ***/
				DateTime readDateTime;
				string readUser;
				GetRecordUpdateInfoFromDatabase(properties, key, out readDateTime, out readUser);
				if (!DBprovider.IsTimeSame(readTime, readDateTime))
					throw new TraceException("Table changed", new List<string> { key, properties.TableName, readUser },
											   LastUpdateWrite.Details.Application);
				UpdateRecordInDatabase(properties, key, newData);
			}
			catch (TraceException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new TraceException("System error",
										   new List<string> { ex.Message },
										   LastUpdateWrite.Details.Application);
			}
			finally
			{
				DatabaseConnection.DBaseRTable.CloseConnection();
			}
		}

		private static void UpdateRecordInDatabase(ReferenceTable.TableProperties properties, string key, Dictionary<string, object> newData)
		{
			string sqlCommand;

			sqlCommand = "Update ";
			sqlCommand += properties.TableName;
			sqlCommand += " Set ";
			sqlCommand += properties.TitleTime + " = '" + DBprovider.ConvertTimeToStringFormat((DateTime)newData[properties.TitleTime]) + "', ";
			sqlCommand += properties.TitleUser + " = '" + newData[properties.TitleUser] + "', ";
			sqlCommand += properties.TitleComputer + " = '" + newData[properties.TitleComputer] + "', ";
			sqlCommand += properties.TitleApplication + " = '" + newData[properties.TitleApplication] + "'";
			if (properties.NumDataColumns > 0)
			{
				for (var i = 0; i < properties.NumDataColumns; i++)
				{
					var title = properties.ColumnNames[i];
					sqlCommand += ", " + title + " = '" + newData[title] + "'";
				}
			}
			sqlCommand += " Where ";
			sqlCommand += properties.TitleKey + " = '" + key + "' ";

			DatabaseConnection.DBaseRTable.ExecuteCommand(sqlCommand);
		}


		public static void AddRecord(ReferenceTable.TableProperties properties, Dictionary<string, object> data)
		{
			try
			{
				DatabaseConnection.DBaseRTable.OpenConnection();

				CheckExistingValues(properties, data[properties.TitleKey].ToString(), data, true);

				AddRecordToDatabase(properties, data);
			}
			catch (TraceException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new TraceException("System error",
										   new List<string> { ex.Message },
										   LastUpdateWrite.Details.Application);
			}
			finally
			{
				DatabaseConnection.DBaseRTable.CloseConnection();
			}
		}

		private static void AddRecordToDatabase(ReferenceTable.TableProperties properties, Dictionary<string, object> data)
		{
			string sqlCommand;

			sqlCommand = "Insert Into ";
			sqlCommand += properties.TableName + " (";
			sqlCommand += properties.TitleKey + ", ";
			sqlCommand += properties.TitleTime + ", ";
			sqlCommand += properties.TitleUser + ", ";
			sqlCommand += properties.TitleComputer + ", ";
			sqlCommand += properties.TitleApplication;
			if (properties.NumDataColumns > 0)
			{
				for (var i = 0; i < properties.NumDataColumns; i++)
					sqlCommand += ", " + properties.ColumnNames[i];
			}
			sqlCommand += ") Values (";
			sqlCommand += "'" + data[properties.TitleKey] + "', ";
			sqlCommand += "'" + DBprovider.ConvertTimeToStringFormat((DateTime)data[properties.TitleTime]) + "', ";
			sqlCommand += "'" + data[properties.TitleUser] + "', ";
			sqlCommand += "'" + data[properties.TitleComputer] + "', ";
			sqlCommand += "'" + data[properties.TitleApplication] + "' ";
			if (properties.NumDataColumns > 0)
			{
				for (var i = 0; i < properties.NumDataColumns; i++)
					sqlCommand += ", '" + data[properties.ColumnNames[i]] + "'";
			}
			sqlCommand += ") ";

			DatabaseConnection.DBaseRTable.ExecuteCommand(sqlCommand);
		}

		public static bool ValueAlreadyExists(string tableName, string title, object data)
		{
			string sqlCommand;

			sqlCommand = "Select * From ";
			sqlCommand += tableName;
			sqlCommand += " Where ";
			sqlCommand += title + " = '" + data + "'";

			var dataTable = DatabaseConnection.DBaseRTable.ExecuteSelectCommand(sqlCommand);
			return (dataTable.Rows.Count > 0);
		}

		private static bool ValueAlreadyExists(string tableName, string titleKey, string key, string title, object data)
		{
			string sqlCommand;

			sqlCommand = "Select * From ";
			sqlCommand += tableName;
			sqlCommand += " Where ";
			sqlCommand += title + " = '" + data + "' AND " + titleKey + " != '" + key + "'";

			var dataTable = DatabaseConnection.DBaseRTable.ExecuteSelectCommand(sqlCommand);
			return (dataTable.Rows.Count > 0);
		}

		private static void CheckExistingValues(ReferenceTable.TableProperties properties, string key, Dictionary<string, object> data, bool includingKey)
		{
			var titleKey = properties.TitleKey;
			if (includingKey)
			{
				if (ValueAlreadyExists(properties.TableName, titleKey, key))
					throw new TraceException("Record already exists",
											   new List<string> { data[titleKey].ToString(), titleKey, properties.TableName },
											   LastUpdateWrite.Details.Application);
			}

			if (properties.NumDataColumns > 0)
			{
				for (var i = 0; i < properties.NumDataColumns; i++)
				{
					var title = properties.ColumnNames[i];
					if (properties.ColumnInfo[title].Unique)
						if (ValueAlreadyExists(properties.TableName, titleKey, key, title, data[title]))
							throw new TraceException("Record already exists", new List<string> { data[title].ToString(), title, properties.TableName },
											LastUpdateWrite.Details.Application);
				}

			}
		}

	}
}
