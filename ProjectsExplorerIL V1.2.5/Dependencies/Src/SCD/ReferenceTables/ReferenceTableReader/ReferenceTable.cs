using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using DatabaseProvider;

namespace ReferenceTableReader
{
	public class ReferenceTable
	{
		private DBprovider DBase;
		public const int NumCommonColumns = 5;

		#region DB provider init

		public class OneColumnInfo
		{
			[DefaultValue(false)]
			public bool Unique { get; set; }													// should field have unique value (must not be same value in another record)

			public int ColumnLength { get; set; }												// column length in characters (for instance: varchar(5) will return 5, nvarchar(50) will return 50 and nvarchar(MAX) will return -1)
			public string DefaultValue { get; set; }											// default value can be used to add a new record
			[DefaultValue(false)]
			public bool DefaultNotLegal { get; set; }											// if this is true, this feature can be used to paint a cell red if it is default value
		}

		public class TableProperties
		{
			public string TableName { get; set; }												// table name
			public string TitleKey { get; set; }												// column title for key
			public string TitleTime { get; set; }												// column title for datetime of last update
			public string TitleUser { get; set; }												// column title for user who performed last update
			public string TitleComputer { get; set; }											// column title for computer on which last update was performed
			public string TitleApplication { get; set; }										// column title for application which performed last update
			public int NumDataColumns { get; set; }												// number of data columns (not including the above)
			public List<string> ColumnNames { get; set; }										// column titles of data columns
			public Dictionary<string, OneColumnInfo> CommonColumnInfo { get; set; }				// info for common columns (key, datetime, user, computer, application)
			public Dictionary<string, OneColumnInfo> ColumnInfo { get; set; }					// info for data columns
			public string QueryLine;															// query for returning record with selected key (key value should be concatenated to this string)
			public string QueryTable;															// query for returning complete table
		}

		public void PrepareTableProperties(out TableProperties tableProperties, string tableId)
		{
			tableProperties = new TableProperties();
			var tableInfoLine = GetReferenceTableLineDictionary(TableInfo.TableName, new List<string> { tableId });
			tableProperties.TableName = tableInfoLine[TableInfo.TableNameColumn].ToString();
			tableProperties.TitleKey = tableInfoLine[TableInfo.KeyColumn].ToString();
			tableProperties.TitleTime = tableInfoLine[TableInfo.TimeColumn].ToString();
			tableProperties.TitleUser = tableInfoLine[TableInfo.UserColumn].ToString();
			tableProperties.TitleComputer = tableInfoLine[TableInfo.ComputerColumn].ToString();
			tableProperties.TitleApplication = tableInfoLine[TableInfo.ApplicationColumn].ToString();
			tableProperties.CommonColumnInfo = new Dictionary<string, OneColumnInfo>();
			var tableName = tableProperties.TableName;
			tableProperties.CommonColumnInfo.Add(tableProperties.TitleKey, new OneColumnInfo { ColumnLength = GetTableColumnLength(tableName, tableProperties.TitleKey), Unique = true, DefaultValue = "", DefaultNotLegal = true });
			tableProperties.CommonColumnInfo.Add(tableProperties.TitleTime, new OneColumnInfo { ColumnLength = GetTableColumnLength(tableName, tableProperties.TitleTime) });
			tableProperties.CommonColumnInfo.Add(tableProperties.TitleUser, new OneColumnInfo { ColumnLength = GetTableColumnLength(tableName, tableProperties.TitleUser) });
			tableProperties.CommonColumnInfo.Add(tableProperties.TitleComputer, new OneColumnInfo { ColumnLength = GetTableColumnLength(tableName, tableProperties.TitleComputer) });
			tableProperties.CommonColumnInfo.Add(tableProperties.TitleApplication, new OneColumnInfo { ColumnLength = GetTableColumnLength(tableName, tableProperties.TitleApplication) });

			var tableColumnsData = GetReferenceTableTableDictionary("TableColumnsData", new List<string> { tableId });
			if (tableColumnsData.Count > 0)
			{
				tableProperties.NumDataColumns = tableColumnsData.Count;
				tableProperties.ColumnInfo = new Dictionary<string, OneColumnInfo>();
				tableProperties.ColumnNames = new List<string>();
				foreach (var d in tableColumnsData)
				{
					var columnName = d[TableColumnsData.NameColumn].ToString();
					tableProperties.ColumnNames.Add(columnName);

					tableProperties.ColumnInfo.Add(columnName, new OneColumnInfo
					{
						ColumnLength = GetTableColumnLength(tableName, columnName),
						Unique = (bool)d[TableColumnsData.UniqueColumn],
						DefaultValue = d[TableColumnsData.DefaultValueColumn].ToString(),
						DefaultNotLegal = (d[TableColumnsData.DefaultValueColumn] != DBNull.Value && d[TableColumnsData.DefaultNotLegalColumn] != DBNull.Value && (bool)d[TableColumnsData.DefaultNotLegalColumn])
					});
				}
			}
			else
				tableProperties.NumDataColumns = 0;
			tableProperties.QueryTable = string.Format("Select {0}, {1}, {2}, {3}, {4}", tableProperties.TitleKey, tableProperties.TitleTime, tableProperties.TitleUser, tableProperties.TitleComputer, tableProperties.TitleApplication);
			tableProperties.QueryLine = string.Format("Select {0}, {1}, {2}, {3}", tableProperties.TitleTime, tableProperties.TitleUser, tableProperties.TitleComputer, tableProperties.TitleApplication);
			if (tableProperties.NumDataColumns > 0)
				for (var i = 0; i < tableProperties.NumDataColumns; i++)
				{
					tableProperties.QueryTable += string.Format(", {0}", tableProperties.ColumnNames[i]);
					tableProperties.QueryLine += string.Format(", {0}", tableProperties.ColumnNames[i]);
				}
			tableProperties.QueryTable += " From " + tableProperties.TableName;
			tableProperties.QueryLine += string.Format(" From {0} Where {1} = ", tableProperties.TableName, tableProperties.TitleKey);
		}

		public ReferenceTable(String connection, String applicationString)
		{
			DBase = new DBprovider(connection, applicationString);
		}

		public ReferenceTable(SqlConnection connection, SqlTransaction transaction, String applicationString)
		{
			DBase = new DBprovider(connection, transaction, applicationString);
		}

		public void UpdateTransaction(SqlTransaction transaction)
		{
			DBase.Transaction = transaction;
		}

		#endregion

		public List<object> GetReferenceTableLine(String tableName, List<String> parameteres)
		{
			try
			{
				DBase.OpenConnection();
				String selectString = GetReferenceTableSelectString(tableName, true);

				if (selectString == "")
					return new List<object>();

				selectString = UpdateSelectStringParameters(selectString, parameteres);
				DataTable dataTable = GetReferenceTableData(selectString);

				DBase.CloseConnection();

				if (dataTable.Rows.Count != 1)
					return new List<object>();

				List<object> tableRow = new List<object>();
				tableRow.AddRange(dataTable.Rows[0].ItemArray);
				return tableRow;
			}
			catch
			{
				DBase.CloseConnection();
				return new List<object>();
			}
		}

		public Dictionary<string, object> GetReferenceTableLineDictionary(String tableName, List<String> parameteres)
		{
			try
			{
				DBase.OpenConnection();
				String selectString = GetReferenceTableSelectString(tableName, true);

				if (selectString == "")
					return new Dictionary<string, object>();

				selectString = UpdateSelectStringParameters(selectString, parameteres);
				DataTable dataTable = GetReferenceTableData(selectString);

				DBase.CloseConnection();

				if (dataTable.Rows.Count != 1)
					return new Dictionary<string, object>();

				Dictionary<string, object> tableRow = new Dictionary<string, object>();
				foreach (DataColumn dataColumn in dataTable.Columns)
					tableRow.Add(dataColumn.ColumnName, dataTable.Rows[0].ItemArray[dataColumn.Ordinal]);

				return tableRow;
			}
			catch
			{
				DBase.CloseConnection();
				return new Dictionary<string, object>();
			}
		}

		public List<List<object>> GetReferenceTableTable(String tableName, List<String> parameteres)
		{
			try
			{
				List<object> tableRow;
				List<List<object>> tableData = new List<List<object>>();

				DBase.OpenConnection();

				String selectString = GetReferenceTableSelectString(tableName, false);

				if (selectString == "")
					return new List<List<object>>();

				selectString = UpdateSelectStringParameters(selectString, parameteres);
				DataTable dataTable = GetReferenceTableData(selectString);

				DBase.CloseConnection();

				foreach (DataRow dataRow in dataTable.Rows)
				{
					tableRow = new List<object>();
					tableRow.AddRange(dataRow.ItemArray);
					tableData.Add(tableRow);
				}

				return tableData;
			}
			catch
			{
				DBase.CloseConnection();
				return new List<List<object>>();
			}
		}

		public List<Dictionary<string, object>> GetReferenceTableTableDictionary(String tableName, List<String> parameteres)
		{
			try
			{
				Dictionary<string, object> tableRow;
				List<Dictionary<string, object>> tableData = new List<Dictionary<string, object>>();

				DBase.OpenConnection();

				String selectString = GetReferenceTableSelectString(tableName, false);

				if (selectString == "")
					return new List<Dictionary<string, object>>();

				selectString = UpdateSelectStringParameters(selectString, parameteres);
				DataTable dataTable = GetReferenceTableData(selectString);

				DBase.CloseConnection();

				foreach (DataRow dataRow in dataTable.Rows)
				{
					tableRow = new Dictionary<string, object>();
					foreach (DataColumn dataColumn in dataTable.Columns)
						tableRow.Add(dataColumn.ColumnName, dataRow.ItemArray[dataColumn.Ordinal]);
					tableData.Add(tableRow);
				}

				return tableData;
			}
			catch
			{
				DBase.CloseConnection();
				return new List<Dictionary<string, object>>();
			}
		}

		public List<Dictionary<string, object>> GetReferenceTableTableDictionary(String query)
		{
			try
			{
				var tableData = new List<Dictionary<string, object>>();

				DBase.OpenConnection();

				if (query == "")
					return new List<Dictionary<string, object>>();

				DataTable dataTable = GetReferenceTableData(query);

				DBase.CloseConnection();

				foreach (DataRow dataRow in dataTable.Rows)
				{
					Dictionary<string, object> tableRow = new Dictionary<string, object>();
					foreach (DataColumn dataColumn in dataTable.Columns)
						tableRow.Add(dataColumn.ColumnName, dataRow.ItemArray[dataColumn.Ordinal]);
					tableData.Add(tableRow);
				}

				return tableData;
			}
			catch
			{
				DBase.CloseConnection();
				return new List<Dictionary<string, object>>();
			}
		}

		private string GetReferenceTableSelectString(String tableName, bool selectLine)
		{
			string sqlCommand = "Select ";
			sqlCommand += "RT_LineSelectString as LineSelectString, ";
			sqlCommand += "RT_TableSelectString as TableSelectString ";
			sqlCommand += "From ";
			sqlCommand += "ReferenceTable ";
			sqlCommand += "Where RT_Name = '" + tableName + "'";

			DataTable dt = DBase.ExecuteSelectCommand(sqlCommand);

			if (dt.Rows.Count < 1)
				return "";

			if (selectLine)
				return DBprovider.GetStringParam(dt.Rows[0], "LineSelectString");
			else
				return DBprovider.GetStringParam(dt.Rows[0], "TableSelectString");
		}

		private string UpdateSelectStringParameters(String selectString, List<String> parameteres)
		{
			if (parameteres == null)
				return selectString;

			for (int i = 0; i < parameteres.Count; i++)
				selectString = selectString.Replace("{" + i + "}", parameteres[i]);

			return selectString;
		}

		private DataTable GetReferenceTableData(String selectString)
		{
			return DBase.ExecuteSelectCommand(selectString);
		}

		public int GetTableColumnLength(string tableName, string columnName)
		{
			try
			{
				DBase.OpenConnection();
				var sqlCommand = string.Format("Select COLUMNPROPERTY(OBJECT_ID('{0}'), '{1}', 'PRECISION') as ColumnLength",
				                               tableName, columnName);

				var dataTable = DBase.ExecuteSelectCommand(sqlCommand);
				DBase.CloseConnection();
				if ((dataTable == null) || (dataTable.Rows.Count == 0))
					return 0;
				/*** In the following, the length in characters of the column is returned. Return value of -1 means very large length. ***/
				var length = DBprovider.GetIntParam(dataTable.Rows[0], "ColumnLength");
				return length;
			}
			catch
			{
				DBase.CloseConnection();
				return 0;
			}
		}

		public List<string> GetTableColumnNames(string tableName)
		{
			try
			{
				var colNames = new List<string>();
				DBase.OpenConnection();
				var sqlCommand = string.Format("Select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '{0}'",
											   tableName);

				var dataTable = DBase.ExecuteSelectCommand(sqlCommand);
				DBase.CloseConnection();
				if ((dataTable != null) && (dataTable.Rows.Count > 0))
				{
					foreach (DataRow r in dataTable.Rows)
						colNames.Add(DBprovider.GetStringParam(r, "COLUMN_NAME"));
				}

				return colNames;
			}
			catch
			{
				DBase.CloseConnection();
				return new List<string>();
			}
		}
		public List<string> GetAllTableNames()
		{
			try
			{
				var colNames = new List<string>();
				DBase.OpenConnection();
				var sqlCommand = "Select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME";

				var dataTable = DBase.ExecuteSelectCommand(sqlCommand);
				DBase.CloseConnection();
				if ((dataTable != null) && (dataTable.Rows.Count > 0))
				{
					foreach (DataRow r in dataTable.Rows)
						colNames.Add(DBprovider.GetStringParam(r, "TABLE_NAME"));
				}

				return colNames;
			}
			catch
			{
				DBase.CloseConnection();
				return new List<string>();
			}
		}
	}
}
