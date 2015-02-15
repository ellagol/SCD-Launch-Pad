using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ATSBusinessObjects;
using ATSBusinessObjects.ContentMgmtModels;
using ATSDomain;

namespace ATSBusinessLogic
{
    public  class VersionBLL
    {

        #region GetVersion
        public static ObservableCollection<VersionModel> GetAllVersions(long HierarchyId)
        {
            // Initialize work fields
            ObservableCollection<VersionModel> VersionList = new ObservableCollection<VersionModel>();
            VersionModel EmptyVersion = Domain.GetBusinessObject<VersionModel>();
            // Build The Query String
            System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
            QryStr.Append("Select * FROM PE_Version t1 Where HierarchyId = '"+ HierarchyId.ToString()+ "' ");
            QryStr.Append("and not (");
            QryStr.Append("CreationDate = (select MIN(CreationDate) from PE_Version Where HierarchyId = '" + HierarchyId.ToString() + "') ");
            QryStr.Append("and not exists ");
            QryStr.Append("(select 1 from PE_VersionContent t2 where t1.VersionId=t2.VersionId) and VersionName = 'New Version') ");
            QryStr.Append("order by t1.CreationDate desc");
           
            string Qry = QryStr.ToString();
            // Fetch the DataTable from the database
            DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
            // Populate the collection
            if (ResTable != null)
            {
                foreach (DataRow DataRow in ResTable.Rows)
                {
                    VersionModel Version = (VersionModel)Domain.DeepCopy(EmptyVersion); // DeepCopy an empty instance to save on the Reflection work
                    //
                    //VersionModel Version = new VersionModel();
                    Version.VersionId = (int)DataRow["VersionId"];
                    Version.IsNew = false;
                    Version.IsDirty = false;

                    Version.HierarchyId = (int)DataRow["HierarchyId"];

                    Version.Sequence = Convert.ToInt32(DataRow["VersionSeqNo"]);
                    Version.VersionName = (string)DataRow["VersionName"];
                    Version.VersionStatus = (string)DataRow["VersionStatus"].ToString().Trim();
                    Version.TargetPath = (string)DataRow["TargetPath"];
                    Version.Description = (string)DataRow["Description"];
                    Version.CreationDate = (DateTime)DataRow["CreationDate"];
                    Version.LastUpdateTime = (DateTime)DataRow["LastUpdateTime"];
                    Version.LastUpdateUser = (string)DataRow["LastUpdateUser"];
                    Version.EcrId = (DataRow["ECRId"] is System.DBNull) ? "" : (string)DataRow["ECRId"];
                    
                    if (!(DataRow["DefaultTargetPathInd"] is System.DBNull))
                    {
                        Version.DefaultTargetPathInd = (Boolean)DataRow["DefaultTargetPathInd"];
                    }



                    VersionList.Add(Version);
                    

                    }

                    // TODO: Continue populating all properties  
                
                    
                }
            return VersionList;
        }

        public static ObservableCollection<VersionModel> GetAllVersionsFromDataTable(long HierarchyId, DataTable PE_Version)
        {
            // Initialize work fields
            ObservableCollection<VersionModel> VersionList = new ObservableCollection<VersionModel>();
            VersionModel EmptyVersion = Domain.GetBusinessObject<VersionModel>();

            string selectCondition = "HierarchyId = " + HierarchyId;
            DataRow[] versionDataRowArray = PE_Version.Select(selectCondition);
            // Populate the collection
            if (versionDataRowArray != null)
            {
                foreach (DataRow dr in versionDataRowArray)
                {
                    VersionModel Version = (VersionModel)Domain.DeepCopy(EmptyVersion); // DeepCopy an empty instance to save on the Reflection work
                    //
                    //VersionModel Version = new VersionModel();
                    Version.VersionId = (int)dr["VersionId"];
                    Version.IsNew = false;
                    Version.IsDirty = false;

                    Version.HierarchyId = (int)dr["HierarchyId"];

                    Version.Sequence = Convert.ToInt32(dr["VersionSeqNo"]);
                    Version.VersionName = (string)dr["VersionName"];
                    Version.VersionStatus = (string)dr["VersionStatus"].ToString().Trim();
                    Version.TargetPath = (string)dr["TargetPath"];
                    Version.Description = (string)dr["Description"];
                    Version.CreationDate = (DateTime)dr["CreationDate"];
                    Version.LastUpdateTime = (DateTime)dr["LastUpdateTime"];
                    Version.LastUpdateUser = (string)dr["LastUpdateUser"];
                    Version.EcrId = (dr["ECRId"] is System.DBNull) ? "" : (string)dr["ECRId"];

                    if (!(dr["DefaultTargetPathInd"] is System.DBNull))
                    {
                        Version.DefaultTargetPathInd = (Boolean)dr["DefaultTargetPathInd"];
                    }
                    VersionList.Add(Version);
                }
                // TODO: Continue populating all properties  
            }
            return VersionList;
        }

        #endregion GetVersion

        #region GetVersionRow By hierarchy id

        public static VersionModel GetActiveVersion(long Id)
        {
            // Initialize work fields
            VersionModel Version = Domain.GetBusinessObject<VersionModel>();
            // Build The Query String
            System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
            QryStr.Append("Select * FROM PE_Version WHERE HierarchyId = '" + Id + "' AND VersionStatus='A'");
            string Qry = QryStr.ToString();
            // Fetch the DataTable from the database
            DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
            // Populate the collection
            if (ResTable != null && ResTable.Rows.Count == 1)
            {
                DataRow DataRow = ResTable.Rows[0];
                //

                Version.VersionId = (int)DataRow["VersionId"];
                Version.IsNew = false;
                Version.IsDirty = false;

                Version.HierarchyId = (int)DataRow["HierarchyId"];

                Version.Sequence = Convert.ToInt32(DataRow["VersionSeqNo"]);
                Version.VersionName = (string)DataRow["VersionName"];
                Version.VersionStatus = (string)DataRow["VersionStatus"].ToString().Trim();
                Version.TargetPath = (string)DataRow["TargetPath"];
                Version.Description = (string)DataRow["Description"];
                Version.CreationDate = (DateTime)DataRow["CreationDate"];
                Version.LastUpdateTime = (DateTime)DataRow["LastUpdateTime"];
                Version.LastUpdateUser = (string)DataRow["LastUpdateUser"];
                Version.EcrId = (DataRow["ECRId"] is System.DBNull) ? "" : (string)DataRow["ECRId"];
                if (!(DataRow["DefaultTargetPathInd"] is System.DBNull))
                {
                    Version.DefaultTargetPathInd = (Boolean)DataRow["DefaultTargetPathInd"];
                }


            }
            return Version;
        }

        public static VersionModel GetActiveVersionFromDataTable(long Id, DataTable PE_Version)
        {
            // Initialize work fields
            VersionModel Version = Domain.GetBusinessObject<VersionModel>();
            string selectCondition = "HierarchyId = '" + Id + "' AND VersionStatus='A'";
            DataRow[] versionDataRowArray = PE_Version.Select(selectCondition);
            if (versionDataRowArray != null && versionDataRowArray.Length == 1)
            {
                DataRow DataRow = versionDataRowArray[0];
                //

                Version.VersionId = (int)DataRow["VersionId"];
                Version.IsNew = false;
                Version.IsDirty = false;

                Version.HierarchyId = (int)DataRow["HierarchyId"];

                Version.Sequence = Convert.ToInt32(DataRow["VersionSeqNo"]);
                Version.VersionName = (string)DataRow["VersionName"];
                Version.VersionStatus = (string)DataRow["VersionStatus"].ToString().Trim();
                Version.TargetPath = (string)DataRow["TargetPath"];
                Version.Description = (string)DataRow["Description"];
                Version.CreationDate = (DateTime)DataRow["CreationDate"];
                Version.LastUpdateTime = (DateTime)DataRow["LastUpdateTime"];
                Version.LastUpdateUser = (string)DataRow["LastUpdateUser"];
                Version.EcrId = (DataRow["ECRId"] is System.DBNull) ? "" : (string)DataRow["ECRId"];
                if (!(DataRow["DefaultTargetPathInd"] is System.DBNull))
                {
                    Version.DefaultTargetPathInd = (Boolean)DataRow["DefaultTargetPathInd"];
                }
            }
            return Version;
        }

        #endregion GetVersionRow

        #region GetVersionRow By hierarchy id and version id

        public static VersionModel GetVersionByIdAndHierarchyId(long versionId, long HierarchyId)
        {
            VersionModel Version = Domain.GetBusinessObject<VersionModel>();
            try
            {
                // Initialize work fields
                //VersionModel Version = Domain.GetBusinessObject<VersionModel>();
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("Select * FROM PE_Version WHERE HierarchyId = '" + HierarchyId + "' AND VersionId = '" + versionId + "'");
                string Qry = QryStr.ToString();
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null && ResTable.Rows.Count == 1)
                {
                    DataRow DataRow = ResTable.Rows[0];
                    //

                    Version.VersionId = (int)DataRow["VersionId"];
                    Version.IsNew = false;
                    Version.IsDirty = false;

                    Version.HierarchyId = (int)DataRow["HierarchyId"];

                    Version.Sequence = Convert.ToInt32(DataRow["VersionSeqNo"]);
                    Version.VersionName = (string)DataRow["VersionName"];
                    Version.VersionStatus = (string)DataRow["VersionStatus"].ToString().Trim();
                    Version.TargetPath = (string)DataRow["TargetPath"];
                    Version.Description = (string)DataRow["Description"];
                    Version.CreationDate = (DateTime)DataRow["CreationDate"];
                    Version.LastUpdateTime = (DateTime)DataRow["LastUpdateTime"];
                    Version.LastUpdateUser = (string)DataRow["LastUpdateUser"];
                    Version.EcrId = (DataRow["ECRId"] is System.DBNull) ? "" : (string)DataRow["ECRId"];
                    if (!(DataRow["DefaultTargetPathInd"] is System.DBNull))
                    {
                        Version.DefaultTargetPathInd = (Boolean)DataRow["DefaultTargetPathInd"];
                    }


                }
                return Version;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return Version;
            }
        }

        #endregion GetVersionRow

        #region Get Versions DataTable

        public static DataTable GetVersionsDataByProjectIds(List<int> projectIds)
        {
            try
            {
                string strPrIds = string.Join(",", projectIds);
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();

                QryStr.Append("select pv.HierarchyId, pv.VersionId, pvc.ContentVersionId from PE_Version pv ");
                QryStr.Append("join PE_VersionContent pvc on pv.VersionId = pvc.VersionId ");
                QryStr.Append("where pv.HierarchyId in (" + strPrIds + ") ");

                string Qry = QryStr.ToString();
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                if (ResTable != null)
                {
                    return ResTable;
                }
                return null;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return null;
            }

        }

        public static DataTable GetVersionsDataTable()
        {
            try
            {
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();

                QryStr.Append("select * from PE_Version ");

                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                if (ResTable != null)
                {
                    return ResTable;
                }
                else
                {
                    String logMessage = "Failed to retrieve data from PE_Version table.";
                    Domain.SaveGeneralErrorLog(logMessage);
                    return null;
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return null;
            }

        }
        #endregion

        #region ReOpen Version

        public static Boolean reOpenVersion(long AversionID, long CversionID)
        {
            try
            {
                var SB = new StringBuilder(string.Empty);
                SB.Append("Update PE_Version set VersionStatus='C', LastUpdateTime=GETDATE(), LastUpdateUser='" + Domain.User + "', LastUpdateComputer='" + Domain.Workstn + "', LastUpdateapplication='" + Domain.AppName + "' where VersionId = '" + AversionID + "'");
                SB.Append(" Update PE_Version set VersionStatus='A', LastUpdateTime=GETDATE(), LastUpdateUser='" + Domain.User + "', LastUpdateComputer='" + Domain.Workstn + "', LastUpdateapplication='" + Domain.AppName + "' where VersionId = '" + CversionID + "';");
                long result = Convert.ToInt64(Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null));
                if (result < -1)
                    return false;
                else
                    return true;
            }
            catch (Exception e)
            {
                throw new Exception("DB Error");
            }
        }

        #endregion

        public static int ParentId = 0;
        public static int ID = 0;
        public static string getParentName(string parentId2)
        {
           
            var Target = new StringBuilder(string.Empty);
          
                var TargetQry = new StringBuilder(string.Empty);
                TargetQry.Append("select Name, ParentId, Id from PE_Hierarchy where Id='" + parentId2.ToString().Trim() + "'");
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(TargetQry.ToString(), CommandType.Text, null);
                if (ResTable != null && ResTable.Rows.Count == 1)
                {
                    DataRow DataRow = ResTable.Rows[0];

                    string TargetParent = (string)DataRow["Name"];
                    //Target.Append("/" + TargetParent.ToString().Trim());
                    //int parentId = 0;

                    if (!(DataRow["ParentId"] is System.DBNull))
                    {
                        ParentId = (int)DataRow["ParentId"];
                        string Tr = getParentName(ParentId.ToString());
                        Target.Append(Tr.ToString().Trim());
                        ID = (int)DataRow["Id"];
                        ParentId = ID;
                    }
                    else if (ParentId == 0)
                    {
                        ParentId = Convert.ToInt32(parentId2);
                      
                    }


                   
                    Target.Append("/" + TargetParent.ToString().Trim());
                 //getParentId(parentId);
                } 

        
            return Target.ToString().Trim();
        }
    
        #region VersionBLL return codes enumeration
        public enum VersionBLLReturnCode : int
        {
            DBException,
            EmptyDataTable,
            Success, //Common for all BLLs
            CommonException, //Common for all BLLs
            VersionToActivateNotFound,
            MoreThanOneVerForCntName
        }
        #endregion

        #region Bootstrap API

        #region Get VersionContents by version name and Project Id

        public static VersionBLLReturnCode GetVersionInfoByProjctIdAndVersionName(long projId, string VerName, out Dictionary<long, int> activeVerContens, out string targetPath, out long verId)
        {
            activeVerContens = new Dictionary<long, int>();
            targetPath = string.Empty;
            verId = -1;
            Dictionary<string, string> activeVerContensStr = new Dictionary<string, string>();
            VersionBLLReturnCode status = VersionBLLReturnCode.DBException;
            try
            {
                StringBuilder getContentsQry = new System.Text.StringBuilder();
                getContentsQry.Append("Select distinct ContentVersionId, ContentSeqNo, TargetPath, av.VersionId ");
                getContentsQry.Append("from dbo.PE_VersionContent avc Join dbo.PE_Version av ");
                getContentsQry.Append("on av.versionId=avc.VersionId ");
                getContentsQry.Append("where av.VersionName='" + VerName + "' and av.HierarchyId=" + projId + ";");

                DataTable contentsList = Domain.PersistenceLayer.FetchDataTable(getContentsQry.ToString(), CommandType.Text, null);
                if (contentsList.Rows.Count > 0)
                {

                    foreach (DataRow DataRow in contentsList.Rows)
                    {
                        activeVerContens.Add(Convert.ToInt32(DataRow["ContentVersionId"]), Convert.ToInt32(DataRow["ContentSeqNo"]));
                        targetPath = Convert.ToString(DataRow["TargetPath"]);
                        verId = Convert.ToInt32(DataRow["VersionId"]);
                        status = VersionBLLReturnCode.Success; //Success
                    }
                }
                else
                {
                    status = VersionBLLReturnCode.EmptyDataTable; // no rows selected
                }
                return status;
            }
            catch (Exception)
            {
                return status; // DB error - future handling
            }
        }
        #endregion

        #region Get Content Version Id by Content name and Project Version name

        //Receives Content name and Project Id. 
        //Returns status 1 - success, 0 - no rows selected, -1 - exception
        //Populates activated content version id and Content CertificateFree flag
        public static VersionBLLReturnCode GetActivatedCntVersionIdByCntNameAndPrVersionName(string cntName, string PrVersionName, long projId, Dictionary<int, CMContentModel> cmContents, Dictionary<int, CMVersionModel> cmVersions, out long conVersionToActivate)
        {
            conVersionToActivate = -1;
            VersionBLLReturnCode callStatus = VersionBLL.VersionBLLReturnCode.CommonException;//Exception

            try
            {
                Dictionary<long, int> activeVersionContens;
                string versionTargetPath = string.Empty;
                long prVersionId = -1;

                //retrieve Version contents and Target path by Project Id and Version Name
                VersionBLL.VersionBLLReturnCode dbCallStatus = VersionBLL.GetVersionInfoByProjctIdAndVersionName(projId, PrVersionName, out activeVersionContens, out versionTargetPath, out prVersionId);

                if (dbCallStatus == VersionBLL.VersionBLLReturnCode.Success) //Success - at least 1 row returned in activeVersionContens
                {
                    //Look for ContentVersionId by Content Name and make sure only one match found
                    int countMatchingVersions = 0; //
                    foreach (long v in activeVersionContens.Keys)
                    {
                        //foreach (KeyValuePair<int, ContentVersion> ver in cmVersions)
                        //{
                        //    if (ver.Key == v)
                        //    {
                        //        foreach (KeyValuePair<int, Content> cmCnt in cmContents)
                        //        {
                        //            if (cmCnt.Value.Name == cntName && cmCnt.Key == ver.Value.ParentID)
                        //            {
                        //                conVersionToActivate = v; // versionId of the activated version
                        //                countMatchingVersions++; //counter to validate that only one version Id was found
                        //            }
                        //        }
                        //    }
                        //}

                        int versionParentId = cmVersions[Convert.ToInt32(v)].ParentID;
                        string versionContentName = cmContents[versionParentId].Name;
                        if (versionContentName.Equals(cntName))
                        {
                            conVersionToActivate = v; // versionId of the activated version
                            countMatchingVersions++; //counter to validate that only one version Id was found
                        }
                    }
                    if (countMatchingVersions == 1)
                    {
                        callStatus = VersionBLL.VersionBLLReturnCode.Success;
                    }
                    else if (countMatchingVersions > 1)
                    {
                        callStatus = VersionBLL.VersionBLLReturnCode.MoreThanOneVerForCntName;
                    }
                    else callStatus = VersionBLL.VersionBLLReturnCode.VersionToActivateNotFound;
                }
                else
                {
                    callStatus = dbCallStatus;//Empty Data Table or connection error 
                }
                return callStatus;
            }
            catch (Exception ex)
            {
                ApiBLL.TraceExceptionParameterValue.Add(ex.Message);
                return callStatus;
            }
        }

        public static VersionBLLReturnCode GetRequiredDiskspace(out int reqSpace)
        {
            VersionBLLReturnCode callStatus = VersionBLL.VersionBLLReturnCode.CommonException;//Exception
            reqSpace = -1;
            try
            {

                string reqDiskSpaceQry = "select value from PE_SystemParameters where variable='RequiredDiskSpace';";


                reqSpace = Convert.ToInt32(Domain.PersistenceLayer.FetchDataValue(reqDiskSpaceQry, CommandType.Text, null));
                if (reqSpace >= 0)
                {
                    callStatus = VersionBLLReturnCode.Success;
                }
                return callStatus;
            }
            catch (Exception e)
            {
                ApiBLL.TraceExceptionParameterValue.Add(e.Message);
                return callStatus;
            }
        }

        #endregion

        #endregion

        #region project full path

        public static string GetProjectFullPathByProjectId(long HierarchyId, String VersionName)
        {
            String fullPath=null;
            // Build The Query String
            System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
            QryStr.Append("  select [TargetPath] from [PE_Version] where [HierarchyId]='" + HierarchyId.ToString() + "' and [VersionName]='" + VersionName.ToString() + "';");

            string Qry = QryStr.ToString();

            DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
            // Populate the collection
            if (ResTable != null)
            {
                foreach (DataRow DataRow in ResTable.Rows)
                {
                    fullPath = DataRow[0].ToString();
                }
                return fullPath;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region LastUpadateVersionCheck

        public static string LastUpadateVersionCheck(ref HierarchyModel HM)
        {
            try
            {
                if (!HM.IsNew)
                {
                    System.Text.StringBuilder LastUpdateSB = new System.Text.StringBuilder();
                    LastUpdateSB.Append("SELECT LastUpdateTime FROM PE_Version WHERE VersionId = '" + HM.VM.VersionId + "'");
                    object LastDateObj = Domain.PersistenceLayer.FetchDataValue(LastUpdateSB.ToString(), System.Data.CommandType.Text, null);
                    if (LastDateObj != null)
                    {
                        string LastDate = Convert.ToDateTime(LastDateObj).ToString();
                        string ObjDate = HM.VM.LastUpdateTime.ToString();
                        if (LastDate != ObjDate) //If TimeStamp on the file is AFTER what we had when row was loaded into memory
                        {
                            return "104";
                        }
                    }
                    else
                        return "104";
                }
                return string.Empty;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                throw new Exception("DB Error");
            }
        }
        #endregion LastUpadateVersionCheck

        #region LastUpadateVersionCheck

        public static string LastUpadateReOpenVersionCheck(ref VersionModel VM)
        {
            try
            {
                System.Text.StringBuilder LastUpdateSB = new System.Text.StringBuilder();
                LastUpdateSB.Append("SELECT LastUpdateTime FROM PE_Version WHERE VersionId = '" + VM.VersionId + "'");
                object LastDateObj = Domain.PersistenceLayer.FetchDataValue(LastUpdateSB.ToString(), System.Data.CommandType.Text, null);
                if (LastDateObj != null)
                {
                    string LastDate = Convert.ToDateTime(LastDateObj).ToString();
                    string ObjDate = VM.LastUpdateTime.ToString();
                    if (LastDate != ObjDate) //If TimeStamp on the file is AFTER what we had when row was loaded into memory
                    {
                        return "104";
                    }

                }
                return string.Empty;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                throw new Exception("DB Error");
            }
        }
        #endregion LastUpadateVersionCheck

        #region get list of used content versions

        public static void GetListOfUsedContentVersionsPE(ref List<int> listOfUsedContentVersions)
        {
            listOfUsedContentVersions = new List<int>();
            try
            {
                string qry = "select distinct ContentVersionId from PE_VersionContent";
                DataTable contentVersions = Domain.PersistenceLayer.FetchDataTable(qry.ToString(), CommandType.Text, null);
                if (contentVersions != null)
                {
                    foreach (DataRow DataRow in contentVersions.Rows)
                    {
                        int cvID = Convert.ToInt32(DataRow.ItemArray[0]);
                        listOfUsedContentVersions.Add(cvID);
   
                    }
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("Failed to get list of content versions");
            }
        }

        public static List<string> GetListOfVersionNamesByProjectId(int projectId)
        {
            List<string>  listOfVersionNames = new List<string>();
            try
            {
                StringBuilder QryStr = new StringBuilder();

                QryStr.Append("select versionname, v.CreationDate from pe_version v ");
                QryStr.Append("where HierarchyId = " + projectId);
                QryStr.Append(" and versionid > (select MIN(versionid) from pe_version v where HierarchyId = " + projectId + ")");
                QryStr.Append(" union all ");
                QryStr.Append("select distinct versionname, v.CreationDate from pe_version v ");
                QryStr.Append("left outer join PE_VersionContent vc ");
                QryStr.Append("on vc.VersionId = v.VersionId ");
                QryStr.Append("where HierarchyId = " + projectId);
                QryStr.Append(" and v.CreationDate = (select MIN(CreationDate) from pe_version v where HierarchyId = " + projectId + ")");
                QryStr.Append(" and v.VersionName != 'New Version'");
                QryStr.Append(" order by v.CreationDate desc"); //latest first

                DataTable VersionNames = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                if (VersionNames != null)
                {
                    foreach (DataRow DataRow in VersionNames.Rows)
                    {
                        string vName = DataRow.ItemArray[0].ToString();
                        listOfVersionNames.Add(vName);
                    }
                }
                return listOfVersionNames;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("Failed to get list of versions names.");
            }
        }

        #endregion get list of used content versions

        #region Export-Import

        public static DataTable GetActiveVersionDataTable(long Id)
        {
            try
            {
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("Select * FROM PE_Version WHERE HierarchyId = '" + Id + "' AND VersionStatus='A'");
                string Qry = QryStr.ToString();
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                if (ResTable != null && ResTable.Rows.Count <= 1)
                {
                    return ResTable;
                }
                return null;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return null;
            }
        }

        public static VersionModel GetActiveVersionModel(DataRow versionRow)
        {
            // Initialize work fields
            VersionModel Version = Domain.GetBusinessObject<VersionModel>();

            if (versionRow != null)
            {
                DataRow DataRow = versionRow;
                //

                Version.VersionId = (int)DataRow["VersionId"];
                Version.IsNew = false;
                Version.IsDirty = false;

                Version.HierarchyId = (int)DataRow["HierarchyId"];

                Version.Sequence = Convert.ToInt32(DataRow["VersionSeqNo"]);
                Version.VersionName = (string)DataRow["VersionName"];
                Version.VersionStatus = (string)DataRow["VersionStatus"].ToString().Trim();
                Version.TargetPath = (string)DataRow["TargetPath"];
                Version.Description = (string)DataRow["Description"];
                Version.CreationDate = (DateTime)DataRow["CreationDate"];
                Version.LastUpdateTime = (DateTime)DataRow["LastUpdateTime"];
                Version.LastUpdateUser = (string)DataRow["LastUpdateUser"];
                Version.EcrId = (DataRow["ECRId"] is System.DBNull) ? "" : (string)DataRow["ECRId"];
                if (!(DataRow["DefaultTargetPathInd"] is System.DBNull))
                {
                    Version.DefaultTargetPathInd = (Boolean)DataRow["DefaultTargetPathInd"];
                }
            }
            return Version;
        }

        #endregion Export-Import

        #region Default Version name
        public static string GenerateDefaultVersionName(int projectId)
        {
            string defaultVersionName = string.Empty;
            double defaultVersionNameNumeric = 0;
            try
            {
                //string versionNamePrefix = Domain.PE_SystemParameters["DefaultVNPrefix"];
                string versionNamePrefix = string.Empty;
                string strFirstNewVersionName = string.Empty;
                string strVersionNameIncrement = string.Empty;
                string strVersionIncrementPrecision = string.Empty;

                double firstNewVersionName = 0;
                double versionNameIncrement = 0;
                try
                {
                    versionNamePrefix = Domain.PE_SystemParameters["DefaultVNPrefix"];
                    if (versionNamePrefix != null)
                    {
                        versionNamePrefix = versionNamePrefix.Trim();
                    }
                }
                catch
                { //ignore if failed
                }

                try
                {
                    strFirstNewVersionName = Domain.PE_SystemParameters["DefaultVNStart"];
                    firstNewVersionName = Convert.ToDouble(strFirstNewVersionName);
                }
                catch (Exception e)
                {
                    String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                    logMessage = logMessage + "\nFailed to generate default version name." +
                     "\nDefaultVNStart parameter is not valid. Please check PE_SystemParameters table";
                    ATSDomain.Domain.SaveGeneralWarningLog(logMessage);
                    return defaultVersionName;
                }

                try
                {
                    strVersionNameIncrement = Domain.PE_SystemParameters["DefaultVNIncrement"];
                    versionNameIncrement = Convert.ToDouble(strVersionNameIncrement);
                    if (strVersionNameIncrement.Split('.')[0].Length != strVersionNameIncrement.Length)//with precision
                    {
                        strVersionIncrementPrecision = strVersionNameIncrement.Split('.')[1];
                    }
                }
                catch (Exception e)
                {
                    String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                    logMessage = logMessage + "\nFailed to generate default version name." +
                     "\nDefaultVNIncrement parameter is not valid. Please check PE_SystemParameters table";
                    ATSDomain.Domain.SaveGeneralWarningLog(logMessage);
                    return defaultVersionName;
                }

                if (projectId > 0)
                {
                    List<string> versionsNames = GetListOfVersionNamesByProjectId(projectId);
                    if (versionsNames != null && versionsNames.Count > 0)
                    {
                        string latestVersionName = versionsNames.ToArray().GetValue(0).ToString();
                        try
                        {
                            if (!string.IsNullOrEmpty(versionNamePrefix) && !string.IsNullOrWhiteSpace(versionNamePrefix))
                            {
                                string latestVersionNamePrefix = latestVersionName.Substring(0, versionNamePrefix.Length);
                                if (latestVersionNamePrefix == versionNamePrefix)
                                {
                                    double latestVersionNameNumeric = Convert.ToDouble(latestVersionName.Substring(versionNamePrefix.Length));
                                    double div = latestVersionNameNumeric / versionNameIncrement;
                                    double rem = div - Convert.ToInt32(div);
                                    if (Math.Round(rem, 8) == 0) //name is according to the policy
                                    {
                                        defaultVersionNameNumeric = latestVersionNameNumeric + versionNameIncrement;
                                        defaultVersionName = versionNamePrefix + defaultVersionNameNumeric.ToString();
                                        defaultVersionName = AddTrailingZeros(defaultVersionName, strVersionIncrementPrecision);
                                        return defaultVersionName;
                                    }
                                }
                            }
                            else
                            {
                                double latestVersionNameNumeric = Convert.ToDouble(latestVersionName);
                                double div = latestVersionNameNumeric / versionNameIncrement;
                                double rem = div - Convert.ToInt32(div);
                                if (Math.Round(rem, 8) == 0) //name is according to the policy
                                {
                                    defaultVersionNameNumeric = latestVersionNameNumeric + versionNameIncrement;
                                    defaultVersionName = versionNamePrefix + defaultVersionNameNumeric.ToString();
                                    defaultVersionName = AddTrailingZeros(defaultVersionName, strVersionIncrementPrecision);
                                    return defaultVersionName;
                                }
                            }
                        }
                        catch
                        { } //ignore, version name is not according to the policy
                    }
                    int versionsCount = versionsNames.Count;
                    defaultVersionNameNumeric = firstNewVersionName + versionsCount * versionNameIncrement;
                    defaultVersionName = versionNamePrefix + defaultVersionNameNumeric.ToString();
                }
                else
                {
                    defaultVersionName = versionNamePrefix + firstNewVersionName.ToString();
                }
                defaultVersionName = AddTrailingZeros(defaultVersionName, strVersionIncrementPrecision);
                return defaultVersionName;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                logMessage = logMessage + "\nFailed to generate default version name";
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                return defaultVersionName;
            }
        }

        static string AddTrailingZeros(string defaultVersionName, string incrementPrecision)
        {
            try
            {
                if (!string.IsNullOrEmpty(defaultVersionName) && !string.IsNullOrEmpty(incrementPrecision))
                {
                    string[] tempPr = defaultVersionName.Split('.');
                    if (tempPr[0].Length == defaultVersionName.Length) //no decimal point
                    {
                        defaultVersionName = defaultVersionName + ".";
                        for (int i = 0; i < incrementPrecision.Length; i++)
                        {
                            defaultVersionName = defaultVersionName + "0";
                        }
                    }
                }
                return defaultVersionName;
            }
            catch 
            {
                return defaultVersionName;
            } //ignore, no failure
        }

        #endregion

    }
}
