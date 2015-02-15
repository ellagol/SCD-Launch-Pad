using System;
using System.Collections.Generic;
using System.Data;
using ATSDomain;

namespace ATSBusinessLogic.ContentMgmtBLL
{
    public class CMSecurityBLL
    {
        #region Get Application Permission

        public static List<String> GetApplicationPermission(string User)
        {
            List<String> cmPermissions = new List<string>();

            try
            {
                //Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("SELECT ATS_Privilege.PrivilegeDescription FROM Applications INNER JOIN ATS_Privilege INNER JOIN ATS_ProfilePrivilege INNER JOIN ATS_UserProfile ON ATS_ProfilePrivilege.ProfileId = ATS_UserProfile.ProfileId ");
                QryStr.Append("ON ATS_Privilege.PrivilegeCode = ATS_ProfilePrivilege.PrivilegeCode ON Applications.AP_ID = ATS_UserProfile.SubSystem WHERE (ATS_UserProfile.UserId = '" + User + "') AND ");
                QryStr.Append("(Applications.AP_Name = '" +  "Content manager" + "') AND (ATS_UserProfile.ExpirationDate IS NULL)");
                // Fetch the DataTable from the database
                DataTable Priviliges = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), System.Data.CommandType.Text, null);

                foreach (DataRow Element in Priviliges.Rows)
                {
                    cmPermissions.Add((string)Element["PrivilegeDescription"]);
                }

                return cmPermissions;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return null;
            }           
        }

        #endregion
    }
}
