using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using ATSBusinessObjects.ContentMgmtModels;
using ATSDomain;
using TraceExceptionWrapper;

namespace ATSBusinessLogic.ContentMgmtBLL
{
    public class CMTreeNodeBLL
    {
        #region Get Tree Objects 

        public static List<CMTreeNode> GetTreeObjects(out Dictionary<int, CMFolderModel> folders, out Dictionary<int, CMContentModel> contents, out Dictionary<int, CMVersionModel> versions)
        {
            return GetTreeObjectsData(true, out folders, out contents, out versions);
        }
        //Performance09
        public static List<CMTreeNode> GetSubTreeObjects(List<int> contentIds, out Dictionary<int, CMFolderModel> folders, out Dictionary<int, CMContentModel> contents, out Dictionary<int, CMVersionModel> versions)
        {
            return GetSubTreeObjectsData(contentIds, out folders, out contents, out versions);
        }

        public static List<CMTreeNode> GetSubTreeObjects(List<int> contentIds, List<int> versionIds, out Dictionary<int, CMFolderModel> folders, out Dictionary<int, CMContentModel> contents, out Dictionary<int, CMVersionModel> versions)
        {
            return GetSubTreeObjectsData(contentIds, versionIds, out folders, out contents, out versions);
        }

        #endregion

        #region Get Tree Objects Data

        public static List<CMTreeNode> GetTreeObjectsData(bool fullData, out Dictionary<int, CMFolderModel> folders, out Dictionary<int, CMContentModel> contents, out Dictionary<int, CMVersionModel> versions)
        {
            try
            {
                List<CMTreeNode> tree;
                CMContentsReaderBLL contentsReader = new CMContentsReaderBLL();

                tree = contentsReader.GetContentsTree(fullData, out folders, out contents, out versions);
                return tree;
            }
            catch (TraceException te)
            {
                String logMessage = te.Message + "\n" + "Source: " + te.Source + "\n" + te.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                te.AddTrace(new StackFrame(1, true));
                throw te;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                TraceException te = new TraceException(new StackFrame(1, true), e, "Content manager");

                throw te;
            }
        }

        //Performance09
        public static List<CMTreeNode> GetSubTreeObjectsData(List<int> contentIds, out Dictionary<int, CMFolderModel> folders, out Dictionary<int, CMContentModel> contents, out Dictionary<int, CMVersionModel> versions)
        {
            try
            {
                List<CMTreeNode> tree;
                CMContentsReaderBLL contentsReader = new CMContentsReaderBLL();

                tree = contentsReader.GetContentsSubTree(contentIds, out folders, out contents, out versions);
                return tree;
            }
            catch (TraceException te)
            {
                String logMessage = te.Message + "\n" + "Source: " + te.Source + "\n" + te.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                te.AddTrace(new StackFrame(1, true));
                throw te;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                TraceException te = new TraceException(new StackFrame(1, true), e, "Content manager");

                throw te;
            }
        }

        //CR4046
        public static List<CMTreeNode> GetSubTreeObjectsData(List<int> contentIds, List<int> versionIds, out Dictionary<int, CMFolderModel> folders, out Dictionary<int, CMContentModel> contents, out Dictionary<int, CMVersionModel> versions)
        {
            try
            {
                List<CMTreeNode> tree;
                CMContentsReaderBLL contentsReader = new CMContentsReaderBLL();

                tree = contentsReader.GetContentsSubTree(contentIds, versionIds, out folders, out contents, out versions);
                return tree;
            }
            catch (TraceException te)
            {
                String logMessage = te.Message + "\n" + "Source: " + te.Source + "\n" + te.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                te.AddTrace(new StackFrame(1, true));
                throw te;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                TraceException te = new TraceException(new StackFrame(1, true), e, "Content manager");

                throw te;
            }
        }


        #endregion

        #region get Root Path

        public static String getRootPath()
        {
            // Initialize work fields
            String RootPath = "";

            try
            {
                var SBrootPath = new StringBuilder(string.Empty);
                SBrootPath.Append("SELECT SP_Value FROM SystemParameters WHERE SP_Name = 'RootPathFS'");
                // Fetch the DataTable from the database
                object rootObj = Domain.PersistenceLayer.FetchDataValue(SBrootPath.ToString(), System.Data.CommandType.Text, null);
                // Populate the collection
                if (rootObj != null)
                {
                    RootPath = (string)rootObj;      
                    return RootPath;
                }
                else
                {
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

        #region get System Parameters

        public static Dictionary<string, string> getSystemParameters()
        {
            // Initialize work fields
            Dictionary<string, string> SystemParameters = new Dictionary<string, string>();

            try
            {
                var SBsystemParameters = new StringBuilder(string.Empty);

                SBsystemParameters.Append("SELECT * FROM SystemParameters");

                // Fetch the DataTable from the database
                DataTable ResTbl = Domain.PersistenceLayer.FetchDataTable(SBsystemParameters.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTbl != null)
                {
                    foreach (DataRow DataRow in ResTbl.Rows)
                    {
                        SystemParameters.Add((string)DataRow["SP_Name"], (string)DataRow["SP_Value"]);                       
                    }
                    return SystemParameters;
                }
                else
                {
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

        #region Get Content Version Status Types Data Table

        public static DataTable GetContentVersionStatusDataTable()
        {
            try
            {
                var SB = new StringBuilder(string.Empty);

                SB.Append("SELECT CVS_ID, CVS_LastUpdateUser, CVS_LastUpdateComputer, CVS_LastUpdateApplication, CVS_LastUpdateTime, CVS_Name, CVS_Icon FROM ContentVersionStatus");

                // Fetch the DataTable from the database
                DataTable ResTbl = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTbl != null)
                {
                    return ResTbl;
                }
                else
                {
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

        #region Get Content Types Data Table

        public static DataTable GetContentTypesDataTable()
        {
            try
            {
                var SB = new StringBuilder(string.Empty);

                SB.Append("SELECT CTY_ID, CTY_LastUpdateUser, CTY_LastUpdateComputer, CTY_LastUpdateApplication, CTY_LastUpdateTime, CTY_Name FROM ContentType");

                // Fetch the DataTable from the database
                DataTable ResTbl = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTbl != null)
                {
                    return ResTbl;
                }
                else
                {
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

        #region Get User Group Types Data Table

        public static DataTable GetUserGroupTypesDataTable()
        {
            try
            {
                var SB = new StringBuilder(string.Empty);

                SB.Append("SELECT GT_ID, GT_LastUpdateUser, GT_LastUpdateComputer, GT_LastUpdateApplication, GT_LastUpdateTime, GT_Name FROM GroupTypes");

                // Fetch the DataTable from the database
                DataTable ResTbl = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTbl != null)
                {
                    return ResTbl;
                }
                else
                {
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

        #region Get User Group Type

        public static String GetUserGroupType()
        {
            // Initialize work fields
            String Type = "";

            try
            {
                var SB = new StringBuilder(string.Empty);

                SB.Append("SELECT UGT_id_GroupTypes FROM UserGroupType WHERE (UGT_id_UserName = '" + Domain.User + "')");

                // Fetch the DataTable from the database
                DataTable ResTbl = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTbl != null && ResTbl.Rows.Count>0)
                {
                    Type = ResTbl.Rows[0]["UGT_id_GroupTypes"].ToString();
                    return Type;               
                }
                else
                {
                    return "";
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return "";
            }
        }

        #endregion

        #region Get Version Links With Lock Data Table

        public static DataTable GetVersionLinksWithLock()
        {
            try
            {
                var SB = new StringBuilder(string.Empty);

                SB.Append("SELECT ContentVersionVersionLink.CVVL_id_ContentVersion_Parent, ContentVersionVersionLink.CVVL_id_ContentVersion_Link, ContentVersion.CV_id_Content FROM ContentVersionVersionLink INNER JOIN ContentVersion WITH (UPDLOCK) ON ContentVersionVersionLink.CVVL_id_ContentVersion_Link = ContentVersion.CV_ID");

                // Fetch the DataTable from the database
                DataTable ResTbl = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTbl != null)
                {
                    return ResTbl;
                }
                else
                {
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

        #region Get Content Version SubVersions By ContentVersion ID Data Table

        public static DataTable GetContentVersionSubVersionsByContentVersionID(long contentVersionID)
        {
            try
            {
                var SB = new StringBuilder(string.Empty);

                SB.Append("SELECT CVVL_id_ContentVersion_Link, CVVL_ChildNumber, CVVL_LastUpdateUser, CVVL_LastUpdateComputer, CVVL_LastUpdateApplication, CVVL_LastUpdateTime FROM ContentVersionVersionLink WHERE CVVL_id_ContentVersion_Parent = '" + contentVersionID + "' ");
                SB.Append("ORDER BY CVVL_ChildNumber");

                // Fetch the DataTable from the database
                DataTable ResTbl = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTbl != null)
                {
                    return ResTbl;
                }
                else
                {
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

        #region Get Content Version Files Data Table

        public static DataTable GetContentVersionFilesDataTable(long contentVersionID)
        {
            try
            {
                var SB = new StringBuilder(string.Empty);

                SB.Append("SELECT CVF_ID, CVF_LastUpdateUser, CVF_LastUpdateComputer, CVF_LastUpdateApplication, CVF_LastUpdateTime, CVF_Name, CVF_Path, CVF_Size, CVF_FileLastWriteTime FROM ContentVersionFile WHERE CVF_id_ContentVersion  = '" + contentVersionID + "' ");
      
                // Fetch the DataTable from the database
                DataTable ResTbl = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTbl != null)
                {
                    return ResTbl;
                }
                else
                {
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

        #region Get Content Versions DataTable By Content ID

        public static DataTable GetContentVersionsDataTableByContentID(int contentID)
        {
            try
            {
                var SB = new StringBuilder(string.Empty);

                SB.Append("SELECT CV_ID, CV_LastUpdateUser, CV_LastUpdateComputer, CV_LastUpdateApplication, ");
                SB.Append("CV_LastUpdateTime, CV_Name, CV_id_Content, CV_ECR, CV_DocumentID, CV_id_ContentVersionStatus, ");
                SB.Append("CV_Description, CV_Path, CV_ChildNumber, CV_CommandLine, CV_LockWithDescription, CV_id_PathType, CV_PDMDocVersion, CV_ConfManagementLink FROM ContentVersion WHERE CV_id_Content = " + contentID + " ");
                SB.Append("ORDER BY CV_CreationDate desc");
                // Fetch the DataTable from the database
                DataTable ResTbl = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTbl != null)
                {
                    return ResTbl;
                }
                else
                {
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

        #region Get Contents Data Table By ID

        public static DataTable GetContentsDataTableByID(List<int> contentsID)
        {
            try
            {
                String whereInList = "";

                if (contentsID == null)
                    whereInList = "";
                else
                {
                    for (int i = 0; i < contentsID.Count; i++)
                    {
                        whereInList += contentsID[i].ToString();

                        if (i + 1 != contentsID.Count)
                            whereInList += ",";
                    }

                    whereInList = "Where CO_ID IN (" + whereInList + ") ";
                }

                var SB = new StringBuilder(string.Empty);

                SB.Append("SELECT CO_ID, CO_LastUpdateUser, CO_LastUpdateComputer, CO_LastUpdateApplication, ");
                SB.Append("CO_LastUpdateTime, CO_Name, CO_id_ContentTree, CO_id_ContentType, CO_CertificateFree, ");
                SB.Append("CO_ChildNumber, CO_Icon, CO_Description FROM Content " + whereInList + " ");
                SB.Append("ORDER BY CO_ChildNumber");
                // Fetch the DataTable from the database
                DataTable ResTbl = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTbl != null)
                {
                    return ResTbl;
                }
                else
                {
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

        #region Get Folders Data Table

        public static DataTable GetFoldersDataTable()
        {
            try
            {
                var SB = new StringBuilder(string.Empty);

                SB.Append("SELECT CT_ID, CT_Name, CT_LastUpdateUser, CT_LastUpdateApplication, CT_LastUpdateComputer, ");
                SB.Append("CT_LastUpdateTime, CT_ParentID, CT_ChildNumber, CT_Description, CT_DefaultVNPrefix, CT_DefaultVNStartValue, CT_DefaultVNIncrement ");
                SB.Append("FROM ContentTree WHERE (CT_ID <> 0)");
                SB.Append("ORDER BY CT_ChildNumber");
                // Fetch the DataTable from the database
                DataTable ResTbl = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTbl != null)
                {
                    return ResTbl;
                }
                else
                {
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

        //CR4046
        public static DataTable GetFoldersDataTable(List<int> contentIds)
        {
            try
            {
                var SB = new StringBuilder(string.Empty);

                if (contentIds != null && contentIds.Count > 0)
                {
                    List<int> parentFolders = new List<int>();
                    CMFolderBLL.GetListOfAllContentsParentFolders(contentIds, out parentFolders);

                    String strParentFolders = string.Join(",", parentFolders);

                    SB.Append("SELECT CT_ID, CT_Name, CT_LastUpdateUser, CT_LastUpdateApplication, CT_LastUpdateComputer, ");
                    SB.Append("CT_LastUpdateTime, CT_ParentID, CT_ChildNumber, CT_Description, CT_DefaultVNPrefix, CT_DefaultVNStartValue, CT_DefaultVNIncrement ");
                    SB.Append("FROM ContentTree ct WHERE (CT_ID <> 0) ");
                    SB.Append("and CT_ID in (" + strParentFolders + ") ");
                    SB.Append("ORDER BY CT_ChildNumber");
                    // Fetch the DataTable from the database
                    DataTable ResTbl = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                    // Populate the collection
                    if (ResTbl != null)
                    {
                        return ResTbl;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
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

        #region Get Folder User Group Types Data Table

        public static DataTable GetFolderUserGroupTypesDataTable(int folderID)
        {
            try
            {
                var SB = new StringBuilder(string.Empty);

                SB.Append("SELECT CTUGT_id_UserGroupType, CTUGT_LastUpdateUser, CTUGT_LastUpdateComputer, ");
                SB.Append("CTUGT_LastUpdateApplication, CTUGT_LastUpdateTime ");
                SB.Append("FROM ContentTreeUserGroupType WHERE CTUGT_id_ContentTree = " + folderID + " ");

                // Fetch the DataTable from the database
                DataTable ResTbl = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTbl != null)
                {
                    return ResTbl;
                }
                else
                {
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

        public static DataTable GetFolderUserGroupTypesDataTable()
        {
            try
            {
                var SB = new StringBuilder(string.Empty);

                SB.Append("SELECT CTUGT_id_ContentTree, CTUGT_id_UserGroupType, CTUGT_LastUpdateUser, CTUGT_LastUpdateComputer, ");
                SB.Append("CTUGT_LastUpdateApplication, CTUGT_LastUpdateTime ");
                SB.Append("FROM ContentTreeUserGroupType ");

                // Fetch the DataTable from the database
                DataTable ResTbl = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTbl != null)
                {
                    return ResTbl;
                }
                else
                {
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

        #region Performance#2

        public static DataTable GetContentVersionsDataTable()
        {
            try
            {
                var SB = new StringBuilder(string.Empty);

                SB.Append("SELECT CV_ID, CV_LastUpdateUser, CV_LastUpdateComputer, CV_LastUpdateApplication, ");
                SB.Append("CV_LastUpdateTime, CV_Name, CV_id_Content, CV_ECR, CV_DocumentID, CV_id_ContentVersionStatus, ");
                SB.Append("CV_Description, CV_Path, CV_ChildNumber, CV_CommandLine, CV_LockWithDescription, CV_id_PathType, CV_CreationDate, ");
                SB.Append(" CV_PDMDocVersion, CV_ConfManagementLink FROM ContentVersion ");
                SB.Append("ORDER BY CV_CreationDate desc");
                // Fetch the DataTable from the database
                DataTable ResTbl = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTbl != null)
                {
                    return ResTbl;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static DataTable GetContentVersionsDataTableByContentIDPerf(int contentID)
        {
            try
            {
                if (CMContentsReaderBLL.ContentVersionsDataTable == null ||
                    CMContentsReaderBLL.ContentVersionsDataTable.Rows == null ||
                    CMContentsReaderBLL.ContentVersionsDataTable.Rows.Count == 0)
                {
                    CMContentsReaderBLL.ContentVersionsDataTable = CMTreeNodeBLL.GetContentVersionsDataTable();
                }

                DataTable ContentVersions = CMContentsReaderBLL.ContentVersionsDataTable.Clone();
                Dictionary<long, DataRow> CVDictionary = new Dictionary<long, DataRow>();

                string selectCondition = "CV_id_Content=" + contentID.ToString();

                DataRow[] temp = CMContentsReaderBLL.ContentVersionsDataTable.Select(selectCondition);
                if (temp != null && temp.Length > 0)
                {
                    ContentVersions = temp.CopyToDataTable();
                }
                return ContentVersions;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #endregion

        #region Performance#3
        public static DataTable GetContentVersionSubVersionsDataTable()
        {
            try
            {
                var SB = new StringBuilder(string.Empty);

                SB.Append("SELECT CVVL_id_ContentVersion_Link, CVVL_ChildNumber, CVVL_LastUpdateUser, ");
                SB.Append("CVVL_LastUpdateComputer, CVVL_LastUpdateApplication, CVVL_LastUpdateTime, ");
                SB.Append("CVVL_id_ContentVersion_Parent FROM ContentVersionVersionLink ");
                SB.Append("ORDER BY CVVL_ChildNumber");

                // Fetch the DataTable from the database
                CMContentsReaderBLL.ContentVersionLinkedTable = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection

                return CMContentsReaderBLL.ContentVersionLinkedTable;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static DataTable GetContentVersionSubVersionsByContentVersionIDPerf(long contentVersionID)
        {
            try
            {
                // Populate dataTable for contentVersionID
                if (CMContentsReaderBLL.ContentVersionLinkedTable == null ||
                    CMContentsReaderBLL.ContentVersionLinkedTable.Rows == null ||
                    CMContentsReaderBLL.ContentVersionLinkedTable.Rows.Count == 0)
                {
                    CMContentsReaderBLL.ContentVersionLinkedTable = CMTreeNodeBLL.GetContentVersionSubVersionsDataTable();
                }

                DataTable ResTbl = CMContentsReaderBLL.ContentVersionLinkedTable.Clone();
                // Populate the collection

                string selectCondition = "CVVL_id_ContentVersion_Parent=" + contentVersionID.ToString();
                DataRow[] temp = CMContentsReaderBLL.ContentVersionLinkedTable.Select(selectCondition);
                if (temp != null && temp.Length > 0)
                {
                    ResTbl = temp.CopyToDataTable();
                }

                return ResTbl;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #endregion

        #region Performance#4

        public static DataTable GetContentVersionFilesDataTable()
        {
            try
            {
                var SB = new StringBuilder(string.Empty);

                SB.Append("SELECT CVF_ID, CVF_LastUpdateUser, ");
                SB.Append("CVF_LastUpdateComputer, CVF_LastUpdateApplication, ");
                SB.Append("CVF_LastUpdateTime, CVF_Name, CVF_Path, CVF_id_ContentVersion, CVF_Size, ");
                SB.Append("CVF_FileLastWriteTime FROM ContentVersionFile ");

                // Fetch the DataTable from the database
                CMContentsReaderBLL.ContentVersionFilesTable = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection
                if (CMContentsReaderBLL.ContentVersionFilesTable != null)
                {
                    return CMContentsReaderBLL.ContentVersionFilesTable;
                }
                else
                {
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

        public static DataTable GetContentVersionFilesDataTablePerf(long contentVersionID)
        {
            try
            {
                // Populate dataTable for contentVersionID
                if (CMContentsReaderBLL.ContentVersionFilesTable == null || CMContentsReaderBLL.ContentVersionFilesTable.Rows == null ||
                    CMContentsReaderBLL.ContentVersionFilesTable.Rows.Count == 0)
                {
                    CMContentsReaderBLL.ContentVersionFilesTable = CMTreeNodeBLL.GetContentVersionFilesDataTable();
                }
                DataTable ResTbl = CMContentsReaderBLL.ContentVersionFilesTable.Clone();
                // Populate the collection

                string selectCondition = "CVF_id_ContentVersion=" + contentVersionID.ToString();
                DataRow[] temp = CMContentsReaderBLL.ContentVersionFilesTable.Select(selectCondition);
                if (temp != null && temp.Length > 0)
                {
                    ResTbl = temp.CopyToDataTable();
                }
                return ResTbl;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        #endregion

        #region Import

        public static List<CMTreeNode> GetTreeObjectsFromXml(string parentFolderPath, out Dictionary<int, CMFolderModel> folders, out Dictionary<int, CMContentModel> contents, out Dictionary<int, CMVersionModel> versions)
        {
            try
            {
                return GetTreeObjectsDataFromXml(parentFolderPath, out folders, out contents, out versions);
            }
            catch (TraceException te)
            {
                String logMessage = te.Message + "\n" + "Source: " + te.Source + "\n" + te.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                te.AddTrace(new StackFrame(1, true));
                throw te;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                TraceException te = new TraceException(new StackFrame(1, true), e, "Content manager");

                throw te;
            }
        }

        public static List<CMTreeNode> GetTreeObjectsDataFromXml(string parentFolderPath, out Dictionary<int, CMFolderModel> folders, out Dictionary<int, CMContentModel> contents, out Dictionary<int, CMVersionModel> versions)
        {
            try
            {
                List<CMTreeNode> tree;
                CMContentsReaderBLL contentsReader = new CMContentsReaderBLL();

                tree = contentsReader.GetContentsTreeFromXml(parentFolderPath, out folders, out contents, out versions);
                return tree;
            }
            catch (TraceException te)
            {
                String logMessage = te.Message + "\n" + "Source: " + te.Source + "\n" + te.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                te.AddTrace(new StackFrame(1, true));
                throw te;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                TraceException te = new TraceException(new StackFrame(1, true), e, "Content manager");

                throw te;
            }
        }

        public static DataTable GetContentVersionsDataTableFromXml(string parentFolderPath)
        {
            try
            {
                DataTable ResTbl = FileSystemBLL.ImportDataFromXml(parentFolderPath, ImportProjectBLL.archiveCMFolderName, ImportProjectBLL.cmContentVersionXmlFileName);
                if (ResTbl != null)
                {
                    return ResTbl;
                }
                else
                {
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

        public static DataTable GetContentVersionSubVersionsDataTableFromXml(string parentFolderPath)
        {
            try
            {
                CMContentsReaderBLL.ContentVersionLinkedTable = FileSystemBLL.ImportDataFromXml(parentFolderPath, ImportProjectBLL.archiveCMFolderName, ImportProjectBLL.cmContentVersionVersionLinkXmlFileName);
                return CMContentsReaderBLL.ContentVersionLinkedTable;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return null;
            }
        }

        public static DataTable GetContentVersionFilesDataTableFromXml(string parentFolderPath)
        {
            try
            {
                DataTable ResTbl = FileSystemBLL.ImportDataFromXml(parentFolderPath, ImportProjectBLL.archiveCMFolderName, ImportProjectBLL.cmContentVersionFileXmlFileName);
                if (ResTbl != null)
                {
                    return ResTbl;
                }
                else
                {
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

    }
}
