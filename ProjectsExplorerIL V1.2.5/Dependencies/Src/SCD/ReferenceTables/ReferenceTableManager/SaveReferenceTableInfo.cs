using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DatabaseProvider;
using ReferenceTableManager.Properties;
using ReferenceTableReader;
using ReferenceTableWriter;

namespace ReferenceTableManager
{
	internal class SaveReferenceTableInfo
	{
		/*** call this function if changing column information only ***/
		internal static void UpdateReferenceTable(string connectionString, string application, ReferenceTable.TableProperties tableProperties, string tableId, bool isCreate)
		{
			var db = new DBprovider(connectionString, application);
			try
			{
				db.OpenConnection();
				db.BeginTransaction();
				if (isCreate)
					CreateTableInfo(db, tableProperties, tableId);
				else
					SaveTableInfo(db, tableProperties, tableId);
				SaveTableColumnsData(db, tableProperties, tableId);
				db.CommitTransaction();
			}
			catch (Exception)
			{
				db.RollbackTransaction();
				throw;
			}
			finally
			{
				db.CloseConnection();
			}
		}

		/*** call this function if updating table name ***/
		internal static void UpdateReferenceTableName(string connectionString, string application, string newTableName, string tableId)
		{
			var db = new DBprovider(connectionString, application);
			try
			{
				db.OpenConnection();
				/*** check if table ID exists ***/
				if (ValueAlreadyExists(db, TableInfo.TableName, TableInfo.TableIdColumn, tableId))
				{
					/*** read current table name from metadata ***/
					var readTableName =
						db.ExecuteStringCommand(string.Format("Select {0} From {1} Where {2} = '{3}'", TableInfo.TableNameColumn,
						                                      TableInfo.TableName, TableInfo.TableIdColumn, tableId));

					/*** if it is really different from existing table name ***/
					if (readTableName != newTableName)
					{
						/*** then check if new name already exists elsewhere ***/
						if (ValueAlreadyExists(db, TableInfo.TableName, TableInfo.TableNameColumn, newTableName))
						{
							MessageHandler.MsgReport.DisplayInformation("Table already exists", new List<string> { newTableName }, "");
							return;
						}
						db.ExecuteCommand(string.Format("Update {0} Set {1} = '{2}' Where {3} = '{4}'", TableInfo.TableName, TableInfo.TableNameColumn, newTableName, TableInfo.TableIdColumn, tableId));
					}
				}
			}
			finally
			{
				db.CloseConnection();
			}
		}

		private static void SaveTableInfo(DBprovider db, ReferenceTable.TableProperties properties, string tableId)
		{
			string sqlCommand;

			sqlCommand = string.Format("Update {0} Set ", TableInfo.TableName);
			sqlCommand += string.Format("{0} = '{1}', ", TableInfo.KeyColumn, properties.TitleKey);
			sqlCommand += string.Format("{0} = '{1}', ", TableInfo.TimeColumn, properties.TitleTime);
			sqlCommand += string.Format("{0} = '{1}', ", TableInfo.UserColumn, properties.TitleUser);
			sqlCommand += string.Format("{0} = '{1}', ", TableInfo.ComputerColumn, properties.TitleComputer);
			sqlCommand += string.Format("{0} = '{1}' ", TableInfo.ApplicationColumn, properties.TitleApplication);
			sqlCommand += "Where ";
			sqlCommand += string.Format("{0} = '{1}' ", TableInfo.TableIdColumn, tableId);

			db.ExecuteCommand(sqlCommand);
		}

		private static void CreateTableInfo(DBprovider db, ReferenceTable.TableProperties properties, string tableId)
		{
			string sqlCommand;

			sqlCommand = string.Format("Insert Into {0} (", TableInfo.TableName);
			sqlCommand += string.Format("{0}, ", TableInfo.TableIdColumn);
			sqlCommand += string.Format("{0}, ", TableInfo.TableNameColumn);
			sqlCommand += string.Format("{0}, ", TableInfo.KeyColumn);
			sqlCommand += string.Format("{0}, ", TableInfo.TimeColumn);
			sqlCommand += string.Format("{0}, ", TableInfo.UserColumn);
			sqlCommand += string.Format("{0}, ", TableInfo.ComputerColumn);
			sqlCommand += string.Format("{0}", TableInfo.ApplicationColumn);
			sqlCommand += ") Values ";
			sqlCommand += string.Format("('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}')", tableId, properties.TableName, properties.TitleKey,
					properties.TitleTime, properties.TitleUser, properties.TitleComputer, properties.TitleApplication);

			db.ExecuteCommand(sqlCommand);
		}

		private static void SaveTableColumnsData(DBprovider db, ReferenceTable.TableProperties properties, string tableId)
		{
			string sqlCommand;

			/*** Delete all rows for this reference table from TableColumnsData table ***/
			if (ValueAlreadyExists(db, TableColumnsData.TableName, TableColumnsData.TableIdColumn, tableId))
			{
				sqlCommand = string.Format("Delete From {0} ", TableColumnsData.TableName);
				sqlCommand += "Where ";
				sqlCommand += string.Format("{0} = '{1}'", TableColumnsData.TableIdColumn, tableId);

				db.ExecuteCommand(sqlCommand);
			}
			/*** Add new rows ***/
			sqlCommand = string.Format("Insert Into {0} (", TableColumnsData.TableName);
			sqlCommand += string.Format("{0}, ", TableColumnsData.TableIdColumn);
			sqlCommand += string.Format("{0}, ", TableColumnsData.NameColumn);
			sqlCommand += string.Format("{0}, ", TableColumnsData.OrderColumn);
			sqlCommand += string.Format("{0}, ", TableColumnsData.UniqueColumn);
			sqlCommand += string.Format("{0}, ", TableColumnsData.DefaultValueColumn);
			sqlCommand += string.Format("{0}", TableColumnsData.DefaultNotLegalColumn);
			sqlCommand += ") Values ";
			{
				for (var i = 0; i < properties.NumDataColumns; i++)
					sqlCommand += string.Format("('{0}', '{1}', '{2}', '{3}', '{4}', '{5}'), ", tableId, properties.ColumnNames[i], (i + 1).ToString(), 
						properties.ColumnInfo[properties.ColumnNames[i]].Unique, properties.ColumnInfo[properties.ColumnNames[i]].DefaultValue,
						properties.ColumnInfo[properties.ColumnNames[i]].DefaultNotLegal);
			}
			sqlCommand = sqlCommand.TrimEnd(new char[] { ',', ' ' });

			db.ExecuteCommand(sqlCommand);
		}

		internal static bool ValueAlreadyExists(DBprovider db, string tableName, string title, object data)
		{
			string sqlCommand;

			sqlCommand = "Select * From ";
			sqlCommand += tableName;
			sqlCommand += " Where ";
			sqlCommand += title + " = '" + data + "'";

			var dataTable = db.ExecuteSelectCommand(sqlCommand);
			return (dataTable.Rows.Count > 0);
		}

		internal static bool ValueAlreadyExists(string connectionString, string application, string tableName, string title, object data)
		{
			bool isExists;
			var db = new DBprovider(connectionString, application);
			try
			{
				db.OpenConnection();
				isExists = ValueAlreadyExists(db, tableName, title, data);
			}
			finally
			{
				db.CloseConnection();
			}
			return isExists;
		}
	}
}
