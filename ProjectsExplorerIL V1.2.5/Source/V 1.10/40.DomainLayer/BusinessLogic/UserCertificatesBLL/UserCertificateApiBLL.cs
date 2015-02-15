using System;
using System.Collections.Generic;
using System.Data;
using ATSDomain;

namespace ATSBusinessLogic
{
    public class UserCertificateApiBLL
    {
        #region GetAllUserCertificates

        public static Dictionary<string, string> GetAllUserCertificates(string connectionString)
        {
            Dictionary<string, string> AllUserCertificates = new Dictionary<string, string>();
            System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
            try
            {
                if (connectionString != null && connectionString != string.Empty)
                {

                    // Initialize the Domain
                    Domain.DomainInitForAPI(connectionString);
                    QryStr.Append("select Id, Name from UserCertificates where Status='A'");

                    // Fetch the DataTable from the database
                    DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                    // Populate the collection
                    if (ResTable != null)
                    {
                        foreach (DataRow DataRow in ResTable.Rows)
                        {
                            if ((!(DataRow["Id"] is System.DBNull)) && (!(DataRow["Name"] is System.DBNull)))
                            {
                                string Id = (string)DataRow["Id"];
                                string Name = (string)DataRow["Name"];
                                AllUserCertificates.Add(Id, Name);
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Invalid connection string");
                }

                return AllUserCertificates;
            }
            catch (Exception e)
            {
                
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                return AllUserCertificates;
            }
        }
        
        #endregion GetAllUserCertificates

        #region GetUserCertificatesByUserName

        public static List<String> GetUserCertificatesByUserName(string connectionString, string UserName)
        {
            List<String> AssignedUserCertificatesByUser = new List<String>();
            System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
            try
            {
                if (connectionString != null && connectionString != string.Empty)
                {
                    // Initialize the Domain
                    Domain.DomainInitForAPI(connectionString);

                    if (!(String.IsNullOrEmpty(UserName)))
                    {
                        QryStr.Append("select CertificateId from AssignedUserCertificates ");
                        QryStr.Append("where UserName = '" + UserName + "' ");
                        DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                        // Populate the collection
                        if (ResTable != null)
                        {
                            foreach (DataRow DataRow in ResTable.Rows)
                            {
                                if (!(DataRow["CertificateId"] is System.DBNull))
                                {
                                    string CertificateId = (string)DataRow["CertificateId"];
                                    AssignedUserCertificatesByUser.Add(CertificateId);
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Invalid User Name");
                    }
                

                }
                else
                {
                    throw new Exception("Invalid connection string");
                }


                return AssignedUserCertificatesByUser;
            }
               
            catch (Exception e)
            {

                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                throw new Exception();
            }
        }
        #endregion GetUserCertificatesByUserName
    }
}
