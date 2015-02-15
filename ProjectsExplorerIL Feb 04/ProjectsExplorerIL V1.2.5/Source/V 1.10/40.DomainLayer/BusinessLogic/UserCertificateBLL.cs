using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ATSBusinessObjects;

namespace ATSBusinessLogic
{
    public class UserCertificateBLL
    {
        #region GetAllUserCertificate

        public static string GetAllUserCertificate(out ObservableCollection<UserCertificateApiModel> UserCertificates)
        {
            UserCertificates = new ObservableCollection<UserCertificateApiModel>();
            try
            {
                Dictionary<string, string> UserCertificateDictonary;
               UserCertificateDictonary = UserCertificateApiBLL.GetAllUserCertificates(ATSDomain.Domain.DbConnString);
               if (UserCertificateDictonary.Count > 0)
               {
                   foreach (var i in UserCertificateDictonary)
                   {
                       UserCertificateApiModel UC = new UserCertificateApiModel();
                       UC.UserCertificateId = i.Key;
                       UC.UserCertificateName = i.Value;
                       UserCertificates.Add(UC);
                   }
               }
                return string.Empty;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                return "151";
            }
        }
        #endregion GetAllUserCertificate

        #region GetUserCertificateForHierachy
        public static void GetUserCertificateForHierachy(ref HierarchyModel Hierarchy)
        {
       
            var SB = new StringBuilder(string.Empty);
            try
            {

                SB.Append("SELECT fc.UserCertificateId, fc.LastUpdateTime  FROM PE_FolderUserCertificate fc inner join PE_Hierarchy h on fc.HierarchyId = h.Id ");
                SB.Append(" WHERE h.Id = '" + Hierarchy.Id + "'");
                DataTable ResTable = ATSDomain.Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection 
                if (ResTable != null && ResTable.Rows.Count > 0)
                {
                    Dictionary<string, string> AllCertificates = UserCertificateApiBLL.GetAllUserCertificates(ATSDomain.Domain.DbConnString);
                    foreach (DataRow DataRow in ResTable.Rows)
                    {

                        string UserCertificateId = (string)DataRow["UserCertificateId"];
                        string UserCertificateName = AllCertificates[UserCertificateId];
                        UserCertificateApiModel UCM = new UserCertificateApiModel();
                        UCM.UserCertificateId = UserCertificateId;
                        UCM.UserCertificateName = UserCertificateName;
                        UCM.LastUpdateTime = (DateTime)DataRow["LastUpdateTime"];
                        UCM.IsNew = false;
                        Hierarchy.UserCertificates.Add(UCM);
                    }
                }

            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);

            }
        }
        #endregion GetUserCertificateForHierachy

        #region DeleteUserCertificate

        public static string DeleteUserCertificate(ObservableCollection<UserCertificateApiModel> RemovedCertificate, long HierarchyId)
        {
            var SB = new StringBuilder(string.Empty);
            try
            {
                if (RemovedCertificate.Count > 0)
                {
                    foreach (var i in RemovedCertificate)
                    {
                        //TODO : LAST UPDATE CHECK
                        SB.Clear();
                        SB.Append("Delete from PE_FolderUserCertificate where HierarchyId='" + HierarchyId + "' and UserCertificateId='"+i.UserCertificateId+"';");
                        long RV = 0;
                        RV = (long)ATSDomain.Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, null);
                        if (RV < 1)
                            return "104";
                    }
                }
                    return string.Empty;
            }
             catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("DB Error");
                
            }
        }
        #endregion DeleteUserCertificate

        #region LastUpdateUserCertificate

        //public static string CheckLastUpdateUserCertificateDelete(ObservableCollection<UserCertificateApiModel> UserCertificates, long HierarchyID, ObservableCollection<UserCertificateApiModel>  DeleteUserCertificate)
        //{
            
        //    System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
        //    QryStr.Append("SELECT HierarchyId, UserCertificateId, LastUpdateTime FROM PE_FolderUserCertificate WHERE HierarchyId = " + HierarchyID + " ");

        //    DataTable ResTable = ATSDomain.Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
        //    Dictionary<string, UserCertificateApiModel> UserCertificateDicatonary = new Dictionary<string, UserCertificateApiModel>();
        //    foreach (var i in UserCertificates)
        //    {
        //        if (i.IsNew == false)
        //        {
        //            UserCertificateDicatonary.Add(i.UserCertificateId, i);
        //        }
        //    }
        //    foreach (var i in DeleteUserCertificate)
        //    {
        //        UserCertificateDicatonary.Add(i.UserCertificateId, i);
        //    }
        //    if (ResTable != null)
        //    {
        //        if (ResTable.Rows.Count == UserCertificateDicatonary.Count)
        //        {
        //            foreach (DataRow DataRow in ResTable.Rows)
        //            {

        //                if (!(UserCertificateDicatonary.ContainsKey((string)DataRow["UserCertificateId"])))
        //                    return "104";
        //            }
        //        }
        //        else
        //            return "104";

        //    }

        //    else
        //        return "105";
        //    return string.Empty;
        //}

        //public static string CheckLastUpdateUserCertificateAdd(ObservableCollection<UserCertificateApiModel> UserCertificates, long HierarchyID)
        //{
        //    try
        //    {
        //        System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
        //        QryStr.Append("SELECT HierarchyId, UserCertificateId, LastUpdateTime FROM PE_FolderUserCertificate WHERE HierarchyId = " + HierarchyID + " ");

        //        // Fetch the DataTable from the database
        //        DataTable ResTable = ATSDomain.Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
        //        Dictionary<string, UserCertificateApiModel> UserCertificateDictionary = new Dictionary<string, UserCertificateApiModel>();
        //        //Dictionary<string, UserCertificateApiModel> AddUserCertificateDictionary = new Dictionary<string, UserCertificateApiModel>();
        //        //foreach (var i in UserCertificates)
        //        //{
        //        //    if (i.IsNew == false)
        //        //    {
        //        //        UserCertificateDictionary.Add(i.UserCertificateId, i);
        //        //    }
        //        //    else
        //        //    {
        //        //        AddUserCertificateDictionary.Add(i.UserCertificateId, i);
        //        //    }
        //        //}


        //        // Temporary fix for LastUpdate error. Currently will only verify that added certificate was not previously added by another user 
        //        foreach (var i in UserCertificates)
        //        {
        //            UserCertificateDictionary.Add(i.UserCertificateId, i);
        //        }
        //        if (ResTable != null)
        //        {
        //            //if (ResTable.Rows.Count == UserCertificateDictionary.Count)
        //            //{
        //            //    foreach (DataRow DataRow in ResTable.Rows)
        //            //    {
        //            //        //Not exists on DB
        //            //        if (!(UserCertificateDictionary.ContainsKey((string)DataRow["UserCertificateId"])))
        //            //            return "104";
        //            //        //Exists in Certificates to add and in the DB.
        //            //        if (AddUserCertificateDictionary.ContainsKey((string)DataRow["UserCertificateId"]))
        //            //            return "104";

        //            //    }
        //            //}
        //            //else
        //            //    return "104";

        //            foreach (DataRow DataRow in ResTable.Rows)
        //            {
        //                //Exists in Certificates to add and in the DB.
        //                if (UserCertificateDictionary.ContainsKey((string)DataRow["UserCertificateId"]))
        //                    return "104";
        //            }

        //        }

        //        else
        //            return "105";
        //        return string.Empty;
        //    }
        //    catch (Exception e)
        //    {
        //        ApiBLL.TraceExceptionParameterValue.Add(e.Message);
        //        String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
        //        ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
        //        throw new Exception("Failed to check Last Updated Date for User Certificates");
        //    }
        //}

        public static string CheckLastUpdateUserCertificate(ObservableCollection<UserCertificateApiModel> InitUserCertificates, long HierarchyID)
        {
            try
            {
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("SELECT HierarchyId, UserCertificateId, LastUpdateTime FROM PE_FolderUserCertificate WHERE HierarchyId = " + HierarchyID + " ");

                // Fetch the DataTable from the database
                DataTable ResTable = ATSDomain.Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);

                Dictionary<string, UserCertificateApiModel> UserCertificateDictionary = new Dictionary<string, UserCertificateApiModel>();
                foreach (var i in InitUserCertificates)
                {
                    UserCertificateDictionary.Add(i.UserCertificateId, i);
                }
                if (ResTable != null)
                {
                    if (ResTable.Rows.Count == InitUserCertificates.Count)
                    {
                    foreach (DataRow DataRow in ResTable.Rows)
                    {
                        if (UserCertificateDictionary.ContainsKey((string)DataRow["UserCertificateId"]))
                            {
                                //Saved in Hierarchy model certificates exist in db and have not changed
                                var diff = (Convert.ToDateTime(DataRow["LastUpdateTime"]) - Convert.ToDateTime(UserCertificateDictionary[(string)DataRow["UserCertificateId"]].LastUpdateTime)).TotalSeconds;
                                if (diff < 1)
                                    return string.Empty;
                            }
                            else
                            return "104";
                    }
                    }
                    else
                        return "104";
                }
                else
                    return "105";
                return string.Empty;
            }
            catch (Exception e)
            {
                ApiBLL.TraceExceptionParameterValue.Add(e.Message);
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("Failed to check Last Updated Date for User Certificates");
            }
        }


        #endregion LastUpdateCertificate

        #region GetHierarchyBranchCertificatesByProjectId

        public static string GetHierarchyBranchCertificatesByProjectId(long projectId, out List<String> HierarchyBranchCertificates)
        {
            HierarchyBranchCertificates = new List<string>();
            string branchIds = "";
            HierarchyBLL.HierarchyBLLReturnCode getIdsStatus= HierarchyBLL.HierarchyBLLReturnCode.CommonException;
            try
            {
                getIdsStatus = HierarchyBLL.GetHierarchyBranchIdsProjectId(projectId, out branchIds);
                if (getIdsStatus != HierarchyBLL.HierarchyBLLReturnCode.Success)
                {
                    return "105";
                }
                string selectCertQry = "Select distinct UserCertificateId from dbo.PE_FolderUserCertificate where ExpirationDate is null and HierarchyId in (" + branchIds + ");";
                // Fetch the DataTable from the database
                DataTable listOfProjectCertificates = ATSDomain.Domain.PersistenceLayer.FetchDataTable(selectCertQry.ToString(), CommandType.Text, null);

                if (listOfProjectCertificates != null)
                {
                    if (listOfProjectCertificates.Rows.Count > 0)
                    {
                        foreach (DataRow DataRow in listOfProjectCertificates.Rows)
                        {
                            HierarchyBranchCertificates.Add(DataRow.ItemArray[0].ToString());
                        }
                    }
                  }
                else
                {
                    return "105";
                }
                return "0";
            }
            catch (Exception e)
            {
                ApiBLL.TraceExceptionParameterValue.Add(e.Message);
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                return "105";
            }
        }

        #endregion

        #region Get Certificates by User Id

        public static string GetCertificatesByUserId(out List<String> userCertificates)
        {

            userCertificates = new List<string>();
            try
            {
                userCertificates = UserCertificateApiBLL.GetUserCertificatesByUserName(ATSDomain.Domain.DbConnString, ATSDomain.Domain.User);
                return "0";
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e);
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                return "105"; //returns Exception enum value to indicate RM exception
            }

        }

        #endregion

        #region Compare User certificates

        public static List<String> IsWorkstationCertified(List<String> branchCertificates, List<String> certificateUser)
        {
            List<String> missingCertificates = new List<String>(); //List of missing certificates
            List<String> missingCertificatesDesc = new List<String>(); //List of missing certificates descriptions
            if (branchCertificates.Count > 0 && certificateUser.Count > 0)
            {
                foreach (String c in branchCertificates)
                {
                    if (!certificateUser.Contains(c))
                    {
                        missingCertificates.Add(c); //Adding missing certificates to the list
                    }
                }
            }
            else if (branchCertificates.Count > 0 && certificateUser.Count == 0)
            {
                missingCertificates.AddRange(branchCertificates);
            }

            if (missingCertificates.Count > 0)
            {
                Dictionary<string, string> UserCertificateDictonary;
                UserCertificateDictonary = UserCertificateApiBLL.GetAllUserCertificates(ATSDomain.Domain.DbConnString);

                if (UserCertificateDictonary == null)
                {
                    throw new Exception("Failed to retrieve list of user certificates");
                }
                foreach (string c in missingCertificates)
                {
                    missingCertificatesDesc.Add(UserCertificateDictonary[c]);
                }
            }
            return missingCertificatesDesc;
        }

        #endregion
    }

   
}
