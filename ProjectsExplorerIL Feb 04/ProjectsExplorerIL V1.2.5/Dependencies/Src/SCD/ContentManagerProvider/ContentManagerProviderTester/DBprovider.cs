using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using TraceExceptionWrapper;

namespace ContentManagerProvider
{
    public class DBprovider 
    {
        private string ApplicationName { get; set; } 
        private bool SelfConnection { get; set; } 
        private SqlCommand Command { get; set; }
        public SqlConnection Connection { get; private set; }
        public SqlTransaction Transaction { get; set; }

        public DBprovider(String connectionString, String applicationName)
        {
            Command = null;
            Transaction = null;
            SelfConnection = true;
            ApplicationName = applicationName;
            
            Connection = new SqlConnection(connectionString);
        }

        public DBprovider(SqlConnection connection, SqlTransaction transaction, String applicationName)
        {
            Transaction = transaction;
            SelfConnection = false;
            ApplicationName = applicationName;

            Connection = connection;
            Command = Connection.CreateCommand();
        }

        public void OpenConnection()
        {
            if (!SelfConnection)
                return;

            try
            {
                Connection.Open();
            }
            catch (Exception e)
            {
                throw new TraceException("Open DB connection", new List<string>() { e.Message }, new StackFrame(1, true), e, ApplicationName);
            }

            Command = Connection.CreateCommand();
        }

        public void CloseConnection()
        {
            if (!SelfConnection)
                return;

            Connection.Close();
        }

        public void BeginTransaction()
        {
            if (SelfConnection && Transaction == null)
                Transaction = Connection.BeginTransaction();
        }

        public void RollbackTransaction()
        {
            if (SelfConnection && Transaction != null)
            {
                Transaction.Rollback();
                Transaction = null;
            }
        }

        public void ComitTransaction()
        {
            if (SelfConnection && Transaction != null)
            {
                Transaction.Commit();
                Transaction = null;
            }
        }

        public void ExecuteCommand(String sqlString)
        {
            Command.Transaction = Transaction;
            Command.CommandText = sqlString;
            Command.ExecuteNonQuery();
        }

        public DataTable ExecuteSelectCommand(String sqlString)
        {

            SqlDataReader dataReader;
            DataTable dataTable = new DataTable();

            Command.Transaction = Transaction;
            Command.CommandText = sqlString;

            dataReader = Command.ExecuteReader();
            dataTable.Load(dataReader);
            dataReader.Close();

            return dataTable;
        }

        public int ExecuteScalarCommand(String sqlString)
        {
            Command.Transaction = Transaction;

            Command.CommandText = sqlString;
            return (int)Command.ExecuteScalar();
        }

        public static String ConvertTimeToStringFormat(DateTime time)
        {
            return time.Month + "/" + time.Day + "/" + time.Year + " " + time.Hour + ":" + time.Minute + ":" + time.Second + "." + time.Millisecond;
        }

        public static String GetStringParam(DataRow row, String paramName) 
        {
            if(row.IsNull(paramName))
                return String.Empty;

            return (String)row[paramName];
        }

        public static int GetIntParam(DataRow row, String paramName) 
        {
            if(row.IsNull(paramName))
                return 0;

            return (int)row[paramName];
        }

        public static DateTime GetDateParam(DataRow row, string paramName)
        {
            if (row.IsNull(paramName))
                return DateTime.MinValue;

            return (DateTime)row[paramName];
        }

        public static bool IsTimeSame(DateTime first, DateTime second)
        {

            if (first.Year != second.Year ||
                first.Month != second.Month ||
                first.Day != second.Day ||
                first.Hour != second.Hour ||
                first.Minute != second.Minute ||
                first.Second != second.Second)
                return false;

            return true;
        }
    }
}
