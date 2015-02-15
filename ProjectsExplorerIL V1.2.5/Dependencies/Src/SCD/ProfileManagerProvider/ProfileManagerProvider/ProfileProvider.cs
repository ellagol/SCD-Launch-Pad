using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using DatabaseProvider;

namespace ProfileManagerProvider
{
    public class ProfileProvider
    {
        private String ApplicationName { get; set; }
        private readonly DBprovider _providerDb;

        #region DB provider init

        public ProfileProvider(String connectionString, String applicationName)
        {
            ApplicationName = applicationName;
            _providerDb = new DBprovider(connectionString, applicationName);
        }

        public ProfileProvider(SqlConnection connection, SqlTransaction transaction, String applicationName)
        {
            ApplicationName = applicationName;
            _providerDb = new DBprovider(connection, transaction, applicationName);
        }

        public void UpdateTransaction(SqlTransaction transaction)
        {
            _providerDb.Transaction = transaction;
        }

        #endregion

        #region Access manager provider functions

        public List<String> GetApplicationPermission(String user)
        {
            List<String> applicationPermission = new List<String>();

            string sqlCommand = "SELECT ATS_Privilege.PrivilegeDescription AS Name ";
            sqlCommand += "FROM Applications INNER JOIN ";
            sqlCommand += "ATS_Privilege INNER JOIN ";
            sqlCommand += "ATS_ProfilePrivilege INNER JOIN ";
            sqlCommand += "ATS_UserProfile ON ATS_ProfilePrivilege.ProfileId = ATS_UserProfile.ProfileId ON ";
            sqlCommand += "ATS_Privilege.PrivilegeCode = ATS_ProfilePrivilege.PrivilegeCode ON Applications.AP_ID = ATS_UserProfile.SubSystem ";

            sqlCommand += "WHERE (ATS_UserProfile.UserId = '" + DBprovider.UpdateStringForSqlFormat(user) + "') AND ";
            sqlCommand += "(Applications.AP_Name = '" + DBprovider.UpdateStringForSqlFormat(ApplicationName) + "') AND";
            sqlCommand += " (ATS_UserProfile.ExpirationDate IS NULL)";

            DataTable dataTable = _providerDb.ExecuteSelectCommand(sqlCommand);

            if (dataTable == null)
                return applicationPermission;

            foreach (DataRow row in dataTable.Rows)
                applicationPermission.Add(DBprovider.GetStringParam(row, "Name"));

            return applicationPermission;
        }

        #endregion

    }
}
