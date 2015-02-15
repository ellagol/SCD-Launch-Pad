using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using ATSBusinessLogic.ContentMgmtBLL;
using ATSBusinessObjects;
using ATSBusinessObjects.ContentMgmtModels;
using ATSDomain;

namespace ATSBusinessLogic
{
    public class ContentBLL
    {

        #region Variables

        private long VersionId;
        private DataTable ResTable;
        public static Dictionary<int, CMFolderModel> folders = new Dictionary<int, CMFolderModel>();
        public static Dictionary<int, CMContentModel> contents = new Dictionary<int, CMContentModel>();
        public static Dictionary<int, CMVersionModel> versions = new Dictionary<int, CMVersionModel>();
        public static List<CMTreeNode> allContents;

        //For CR3600
        //public static bool isCMFlyoutOpen = false;
    
        #endregion

        #region Constructor

        public ContentBLL(long VersionId)
        {
            this.VersionId = VersionId;
            this.ResTable = new DataTable();
            //this.folders = new Dictionary<int, CMFolderModel>();
            //this.contents = new Dictionary<int, CMContentModel>();
            //this.versions = new Dictionary<int, CMVersionModel>();
            //get content tree

            //ella performance
            //Domain.CallingAppName = Domain.AppName;
            //allContents = CMTreeNodeBLL.GetTreeObjects(out folders, out contents, out versions);
            //Domain.CallingAppName = "";
        }

        //public ContentBLL()
        //{
        //    allContents = CMTreeNodeBLL.GetTreeObjects(out folders, out contents, out versions);
        //    //Domain.CallingAppName = Domain.AppName;
        //    //allContents = CMTreeNodeBLL.GetTreeObjects(out folders, out contents, out versions);
        //    //Domain.CallingAppName = "";
        //}

        #endregion

        #region Get all Contents

        public ObservableCollection<ContentModel> getActiveContents()
        {
            ObservableCollection<ContentModel> contentList = new ObservableCollection<ContentModel>();
            try
            {
                //Filter contents
                var SBstep = new StringBuilder(string.Empty);
                SBstep.Append("select * from PE_VersionContent where VersionId=" + VersionId.ToString() + "order by [ContentSeqNo];");
                string QrySteps = SBstep.ToString();
                // Fetch the DataTable from the database
                ResTable = Domain.PersistenceLayer.FetchDataTable(SBstep.ToString(), CommandType.Text, null);
            }
            catch (Exception e) 
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return contentList; 
            }
            if (ResTable != null)
            {
                foreach (DataRow DataRow in ResTable.Rows)
                {
                    try
                    {
                        CMContentModel parent = null;
                        int versionId = Convert.ToInt32(DataRow.ItemArray[2]);

                        //If versionId or parent ContentId do not exist in Domain memory
                        if (!versions.Keys.Contains(versionId) || !contents.Keys.Contains(versions[versionId].ParentID))
                        {
                        //Refresh Contents tree from CM - API
                            String logMessage = "Missing information for versionId " + versionId + ".";
                            logMessage = logMessage + "\n                  Refreshing Contents tree data.";
                            Domain.SaveGeneralWarningLog(logMessage);
                            
                            try
                            {
                                Domain.CallingAppName = Domain.AppName;
                                allContents = CMTreeNodeBLL.GetTreeObjects(out folders, out contents, out versions);
                                Domain.CallingAppName = "";
                            }
                            catch (Exception e)
                            {
                                String exLogMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                                Domain.SaveGeneralErrorLog(exLogMessage);
                                throw new Exception("144");
                            }
                        }
                        int versionParentId = versions[versionId].ParentID;
                        parent = contents[versionParentId];
                        int seq = Convert.ToInt32(DataRow.ItemArray[3]);
                        var info = new ContentModel(parent.Name.ToString(), versions[versionId].Name.ToString(), versionId, seq, DateTime.Now.ToString(), parent.IconPath);
                        info.status = versions[versionId].Status.Name;
                        info.contentCategory = contents[versionParentId].ContentType.Name;
                        if (!contentList.Contains(info))
                            contentList.Add(info);
                    }
                    catch (Exception e)
                    {
                        String exLogMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                        Domain.SaveGeneralErrorLog(exLogMessage);
                        throw new Exception("CMDataError");
                    }
                }
            }
            return contentList;
        }

        public ObservableCollection<ContentModel> getActiveContents(String activeVersionName)
        {
            ObservableCollection<ContentModel> contentList = new ObservableCollection<ContentModel>();
            try
            {
                //Filter contents
                var SBstep = new StringBuilder(string.Empty);
                SBstep.Append("select * from [PE_Version] where [HierarchyId]=" + VersionId.ToString() + " and [VersionName]='" + activeVersionName + "';");
                string QrySteps = SBstep.ToString();
                // Fetch the DataTable from the database
                ResTable = Domain.PersistenceLayer.FetchDataTable(SBstep.ToString(), CommandType.Text, null);
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return contentList;
            }
            if (ResTable != null)
            {
                foreach (DataRow DataRow in ResTable.Rows)
                {
                    try
                    {
                        this.VersionId = Convert.ToInt32(DataRow.ItemArray[0]);
                        contentList = getActiveContents();
                    }
                    catch (Exception e)
                    {
                        String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                        Domain.SaveGeneralErrorLog(logMessage);
                        throw new Exception("Refresh");
                    }
                }
            }
            return contentList;
        }
        #endregion

        #region Check if Content is free

        public static CMApiReturnCode checkCertificateFree(int contentVersionId, Dictionary<int, CMContentModel> contents, Dictionary<int, CMVersionModel> versions)
        {
            try
            {
                int versionParentId = versions[Convert.ToInt32(contentVersionId)].ParentID;
                bool ContentCertFreeFlag = contents[versionParentId].CertificateFree;
                if (ContentCertFreeFlag)
                {
                    return CMApiReturnCode.Success;
                }
                else
                {
                    return CMApiReturnCode.ContentNotFree;
                }
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return CMApiReturnCode.ContentVersionNotFound;
            }
        }
        #endregion

        #region Generate file to copy list

        public Hashtable filesToCopyList(ContentModel c, 
                                            Dictionary<int, CMVersionModel> versions, 
                                            IEnumerable<ContentModel> activeContents)
        {
            try
            {
            Hashtable filesToCopyList = new Hashtable();
            foreach (ContentModel cm in activeContents)
            {
                foreach (CMContentFileModel f in versions[cm.id].Files.Values)
                {
                    filesToCopyList.Add(f, f.FileRelativePath + "\\" + f.FileName);
                }
            }
            return filesToCopyList;
        }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return null;
            }
        }
        #endregion

        #region Update ContentList list

        public ObservableCollection<ContentModel> UpdateContents(ref ContentModel newCM)
        {
            ObservableCollection<ContentModel> contentList = new ObservableCollection<ContentModel>();
            try
            {
                //Get contents from API
                Domain.CallingAppName = Domain.AppName;
                allContents = CMTreeNodeBLL.GetTreeObjects(out folders, out contents, out versions);
                Domain.CallingAppName = "";
                //Filter contents
                var SBstep = new StringBuilder(string.Empty);
                SBstep.Append("select * from PE_VersionContent where VersionId=" + VersionId.ToString() + ";");
                string QrySteps = SBstep.ToString();
                // Fetch the DataTable from the database
                ResTable = Domain.PersistenceLayer.FetchDataTable(SBstep.ToString(), CommandType.Text, null);
            }
            catch (Exception e) 
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return contentList; 
            }
            if (ResTable != (null))
            {
                foreach (DataRow DataRow in ResTable.Rows)
                {
                    try
                    {
                        CMContentModel parent = null;
                        foreach (var v in versions) //Add the content
                        {
                            if (DataRow.ItemArray[2].Equals(v.Key))
                            {
                                foreach (var c in contents)
                                {
                                    if (v.Value.ParentID == c.Key)
                                        parent = c.Value;
                                }
                                var info = new ContentModel(parent.Name.ToString(), v.Value.Name.ToString(), parent.ID, DataRow.ItemArray[5].ToString(), parent.IconPath, parent.ContentType.Name);
                                if (!contentList.Contains(info))
                                    contentList.Add(info);
                            }
                        }
                    }

                    catch (Exception e) 
                    {
                        String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                        Domain.SaveGeneralErrorLog(logMessage);
                    }
                }
            }
            ContentModel CM = new ContentModel(newCM.name, newCM.version, newCM.id, newCM.lastUpdateTime, newCM.IconFileFullPath, newCM.contentCategory);
            contentList.Add(CM);
            return contentList;
        }

        #endregion

        #region getTreeObjectsApiCall - Common function

        // Returns status
        public enum CMApiReturnCode : int
        {
            GetTreeObjectsException, //144
            EmptyCMTree, //144
            Success,
            ConnectionError, //144
            ContentVersionNotFound, //144
            ContentNotFree //0
        }

        public static CMApiReturnCode GetContentsTree(out Dictionary<int, CMFolderModel> outFolders, out Dictionary<int, CMContentModel> outContents, out Dictionary<int, CMVersionModel> outVersions)
        {
            outFolders = new Dictionary<int, CMFolderModel>();
            outContents = new Dictionary<int, CMContentModel>();
            outVersions = new Dictionary<int, CMVersionModel>();

            CMApiReturnCode returnCode = CMApiReturnCode.GetTreeObjectsException;

            try
            {
                //Contetnt Manager Provider Object

                //Get folders, contents and versions from API
                Domain.CallingAppName = Domain.AppName;
                List<CMTreeNode> contentTree = CMTreeNodeBLL.GetTreeObjects(out outFolders, out outContents, out outVersions);
                Domain.CallingAppName = "";
                if (contentTree.Count > 0)
                {
                    returnCode = CMApiReturnCode.Success; //Success
                }
                else
                {
                    returnCode = CMApiReturnCode.EmptyCMTree; //API function failed - error 143
                }

                return returnCode;
            }
            catch (Exception cmEx)
            {
                ApiBLL.TraceExceptionParameterValue.Add(cmEx.Message);
                String logMessage = cmEx.Message + "\n" + "Source: " + cmEx.Source + "\n" + cmEx.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return returnCode; //returns GetTreeObjectsException enum value to indicate CM exception
            }
        }

        public static CMApiReturnCode GetContentsSubTree(List<int> contentIds, out Dictionary<int, CMFolderModel> outFolders, out Dictionary<int, CMContentModel> outContents, out Dictionary<int, CMVersionModel> outVersions)
        {
            outFolders = new Dictionary<int, CMFolderModel>();
            outContents = new Dictionary<int, CMContentModel>();
            outVersions = new Dictionary<int, CMVersionModel>();

            CMApiReturnCode returnCode = CMApiReturnCode.GetTreeObjectsException;

            try
            {
                //Contetnt Manager Provider Object

                //Get folders, contents and versions from API
                Domain.CallingAppName = Domain.AppName;
                List<CMTreeNode> contentTree = CMTreeNodeBLL.GetSubTreeObjects(contentIds, out outFolders, out outContents, out outVersions);
                Domain.CallingAppName = "";
                if (contentTree.Count > 0)
                {
                    returnCode = CMApiReturnCode.Success; //Success
                }
                else
                {
                    returnCode = CMApiReturnCode.EmptyCMTree; //API function failed - error 143
                }

                return returnCode;
            }
            catch (Exception cmEx)
            {
                //ApiBLL.TraceExceptionParameterValue.Add(cmEx.Message);
                String logMessage = cmEx.Message + "\n" + "Source: " + cmEx.Source + "\n" + cmEx.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return returnCode; //returns GetTreeObjectsException enum value to indicate CM exception
            }
        }

        #endregion


        public static CMApiReturnCode createAllFilesSortedList(Dictionary<long, int> SortedActiveVersionContents, 
                                                               Dictionary<int, CMVersionModel> cmVersions, 
                                                               out List<CMContentFileModel> DistinctFilesSorted)
        {
            
            DistinctFilesSorted = new List<CMContentFileModel>();
            List<CMContentFileModel> allFilesSorted = new List<CMContentFileModel>();
            try
            {
                foreach (int contentVersionID in SortedActiveVersionContents.Keys)
                {
                    allFilesSorted.AddRange(cmVersions[contentVersionID].Files.Values);
                }

                Dictionary<string, CMContentFileModel> distinctFilesList = new Dictionary<string, CMContentFileModel>();

                foreach (CMContentFileModel file in allFilesSorted)                
                {
                    distinctFilesList[file.FileRelativePath + "\\" + file.FileName] = file;
                }

                DistinctFilesSorted = distinctFilesList.Values.ToList();

                return CMApiReturnCode.Success;
            }
            catch (Exception e) 
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return CMApiReturnCode.ContentVersionNotFound; 
            }
        }

        #region Search by Content Name

        public static List<int>  GetContVerIdByContentName(string searchPattern)
        {
            List<int> ListOfContVerIdWithSearchPattern = new List<int>();
            try
            {
                if (!string.IsNullOrEmpty(searchPattern) && !string.IsNullOrWhiteSpace(searchPattern))
                {
                    foreach (var con in contents)
                    {
                        if (con.Value.Name.Contains(searchPattern))
                        {
                            ListOfContVerIdWithSearchPattern.AddRange(con.Value.Versions.Keys);
                        }
                    }
                }
                return ListOfContVerIdWithSearchPattern;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                System.Diagnostics.Debug.WriteLine(ex.Message);
                ListOfContVerIdWithSearchPattern.Clear();
                return ListOfContVerIdWithSearchPattern;
            }
        }

        #endregion

        public static List<CMTreeNode> LoadContentTreeToMemory( out Dictionary<int, CMFolderModel> folders, 
        out Dictionary<int, CMContentModel> contents ,out Dictionary<int, CMVersionModel> versions)
        {
            folders = new Dictionary<int, CMFolderModel>();
            contents = new Dictionary<int, CMContentModel>();
            versions = new Dictionary<int, CMVersionModel>();
            try
            {
                Domain.CallingAppName = Domain.AppName;
                allContents = CMTreeNodeBLL.GetTreeObjects(out folders, out contents, out versions);
                Domain.CallingAppName = "";
                return allContents;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return allContents;
            }
        
        }

        public static int CountProjectCMFiles(ObservableCollection<ContentModel> projectContents)
        {
            List<int> vIds = new List<int>();
            List<int> linkedVersionIds = new List<int>();
            List<int> listOfCMVersionsIds = new List<int>();

            int totalFiles = 0;

            if (projectContents != null && projectContents.Count > 0)
            {
                foreach (ContentModel v in projectContents)
                {
                    vIds.Add(v.id);
                }

                listOfCMVersionsIds = ContentBLL.GetVersionAllLinkedSubVersions(vIds);
                listOfCMVersionsIds.AddRange(vIds);

                totalFiles = CMVersionBLL.CountVersionFiles(listOfCMVersionsIds);
            }
            return totalFiles;
        }
        public static List<int> GetVersionAllLinkedSubVersions(List<int> activeContentVersions)
        {
            try
            {
                List<int> allVIds = CMVersionBLL.GetVersionAllSubVersions(activeContentVersions);
                return allVIds;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception(e.Message);
            }
        }

        public static List<int> GetAllContentIds(List<int> versionIds)
        {
            try
            {
                List<int> contentIds = CMContentBLL.GetContentIds(versionIds);
                return contentIds;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception(e.Message);
            }
        }

        public static DataTable GetProjectVersionContentsByNameAndProjectId(String versionName, long projectId)
        {
            ObservableCollection<ContentModel> contentList = new ObservableCollection<ContentModel>();
            try
            {
                var SBstep = new StringBuilder(string.Empty);

                SBstep.Append("select vc.* from PE_VersionContent vc ");
                SBstep.Append("join PE_Version v ");
                SBstep.Append("on v.VersionId = vc.VersionId ");
                SBstep.Append("where v.VersionName ='" + versionName + "' ");
                SBstep.Append("and v.HierarchyId = " + projectId + ";");

                string QrySteps = SBstep.ToString();
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(SBstep.ToString(), CommandType.Text, null);

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

        public static ObservableCollection<ContentModel> getActiveContents(DataTable contentsTable, long VersionId,
                                                                            Dictionary<int, CMVersionModel> versions,
                                                                            Dictionary<int, CMContentModel> contents)
        {
            ObservableCollection<ContentModel> contentList = new ObservableCollection<ContentModel>();
            if (contentsTable != null && VersionId > 0)
            {
                string selectCondition = "VersionId =" + VersionId;

                DataRow[] stepRow = contentsTable.Select(selectCondition);
                foreach (DataRow DataRow in stepRow)
                {
                    try
                    {
                        CMContentModel parent = null;
                        int versionId = Convert.ToInt32(DataRow.ItemArray[2]);
                        int versionParentId = versions[versionId].ParentID;
                        parent = contents[versionParentId];
                        int seq = Convert.ToInt32(DataRow.ItemArray[3]);
                        var info = new ContentModel(parent.Name.ToString(), versions[versionId].Name.ToString(), versionId, seq, DateTime.Now.ToString(), parent.IconPath);
                        info.status = versions[versionId].Status.Name;
                        info.contentCategory = contents[versionParentId].ContentType.Name;
                        if (!contentList.Contains(info))
                            contentList.Add(info);
                    }
                    catch (Exception e)
                    {
                        String exLogMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                        Domain.SaveGeneralErrorLog(exLogMessage);
                        throw new Exception("CMDataError");
                    }
                }
            }
            return contentList;
        }
        
        public static ObservableCollection<ContentModel> getActiveContentsPartialModel(String versionName, long projectId)
        {
            ObservableCollection<ContentModel> contentList = new ObservableCollection<ContentModel>();

            DataTable dt = GetProjectVersionContentsByNameAndProjectId(versionName, projectId);
            if (dt == null)
            {
                return contentList;
            }

            foreach (DataRow dr in dt.Rows)
            {
                ContentModel cmv = new ContentModel(null, null, Convert.ToInt32(dr["contentVersionId"]), Convert.ToInt32(dr["ContentSeqNo"]), null, null);
                contentList.Add(cmv);
            }


            return contentList;
        }

    }

}





