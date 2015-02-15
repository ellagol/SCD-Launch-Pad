using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ATSBusinessObjects;
using ATSBusinessObjects.ContentMgmtModels;
using ATSDomain;
using Infra.DAL;
using TraceExceptionWrapper;


namespace ATSBusinessLogic.ContentMgmtBLL
{
    public class CMVersionBLL
    {       
        #region Retrieve Version Row from database and return as CMVersionModel

        public static CMVersionModel GetVersionRow(long VersionId)
        {
            // Initialize work fields
            CMVersionModel Node = new CMVersionModel();
            try
            {
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("Select * FROM ContentVersion WHERE CV_ID = " + VersionId);
                string Qry = QryStr.ToString();
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null && ResTable.Rows.Count == 1)
                {
                    DataRow DataRow = ResTable.Rows[0];
                    
                    Node.Id = (int)DataRow["CV_ID"];
                    Node.Name = (string)DataRow["CV_Name"];
                    Node.Description = (string)DataRow["CV_Description"];
                    Node.ECR = (string)DataRow["CV_ECR"];
                    Node.id_ContentVersionStatus = (string)DataRow["CV_id_ContentVersionStatus"];
                    Node.DocumentID = (string)DataRow["CV_DocumentID"];

                    if (DataRow["CV_PDMDocVersion"] == DBNull.Value)
                    {
                        Node.PDMDocumentVersion = "";
                    }
                    else
                    {
                        Node.PDMDocumentVersion = (string)DataRow["CV_PDMDocVersion"];
                    }

                    if (DataRow["CV_ConfManagementLink"] == DBNull.Value)
                    {
                        Node.ConfigurationManagementLink = "";
                    }
                    else
                    {
                        Node.ConfigurationManagementLink = (string)DataRow["CV_ConfManagementLink"];
                    }

                    Node.id_Content = (int)DataRow["CV_id_Content"];
                    Node.Path.Name = (string)DataRow["CV_Path"];
                    Node.ChildNumber = (int)DataRow["CV_ChildNumber"];
                    Node.id_PathType = (string)DataRow["CV_id_PathType"];
                    Node.CommandLine = (string)DataRow["CV_CommandLine"];
                    Node.LockWithDescription = (string)DataRow["CV_LockWithDescription"];              
                    Node.LastUpdateTime = (DateTime)DataRow["CV_LastUpdateTime"];
                    Node.LastUpdateUser = (string)DataRow["CV_LastUpdateUser"];
                    Node.LastUpdateComputer = (string)DataRow["CV_LastUpdateComputer"];
                    Node.LastUpdateApplication = (string)DataRow["CV_LastUpdateApplication"];

                    if (DataRow["CV_CreationDate"] == DBNull.Value)
                    {
                        Node.CreationDate = DateTime.MinValue;
                    }
                    else
                    {
                        Node.CreationDate = (DateTime)DataRow["CV_CreationDate"];
                    }

                    GetVersiontStatus(ref Node); //get version status
                }
                return Node;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return Node;
            }
        }

        #endregion

        #region Retrieve Version Id

        public static long GetVersionId(long VersionId)
        {
            // Initialize work fields
            CMVersionModel Node = new CMVersionModel();
            try
            {
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("SELECT CV_ID FROM ContentVersion WHERE CV_ID = " + VersionId);
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null && ResTable.Rows.Count == 1)
                {
                    return 1;
                }
                else
                {
                    throw new TraceException("Version deleted", null, "Content manager");
                }

            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new TraceException("Version deleted", null, "Content manager");
            }
        }

        #endregion

        #region Delete Version

        public static long DeleteVersion(long VersionId)
        {
            //Work Fields
            long RV = 0;

            try
            {
                CMVersionModel Version = GetVersionRow(VersionId);

                CheckVersionLinkedVersion(Version); // if there are no linked versions
                
                CheckVersionLinkedProject(Version);

                RV = DeleteContentVersionSubVersions(Version);
                if (RV < 0)
                    return -1;

                RV = DeleteContentVersionFilesDb(Version);
                if (RV < 0)
                    return -1;

                RV = DeleteContentVersionData(Version);
                if (RV < 0)
                    return -1;

                DeleteContentVersionFilesFs(Version);

                return RV;
                
            }
            catch (TraceException te)
            {
                throw te;
            }
        }

        #endregion

        #region Clone Version

        public static CMVersionModel CloneVersion(CMVersionModel VM)
        {
            try
            {
                // Work fields
                CMVersionModel v = new CMVersionModel();
                var SB = new StringBuilder(string.Empty);
                List<ParamStruct> CommandParams;
                Boolean DatabaseSupportsBatchQueries = Domain.PersistenceLayer.GetSupportsBatchQueries();

                v.Name = VM.Name;
                v.id_Content = VM.id_Content;
                v.ECR = VM.ECR;
                v.id_ContentVersionStatus = VM.id_ContentVersionStatus;
                v.DocumentID = VM.DocumentID;
                v.PDMDocumentVersion = VM.PDMDocumentVersion;
                v.ConfigurationManagementLink = VM.ConfigurationManagementLink;
                v.Path = VM.Path;
                v.ChildNumber = VM.ChildNumber;
                v.Description = VM.Description;
                v.id_PathType = VM.id_PathType;
                v.CommandLine = VM.CommandLine;
                v.LockWithDescription = VM.LockWithDescription;
                v.ParentID = VM.ParentID;
                v.Files = VM.Files;
                v.CreationDate = DateTime.Now;
                UpdateControlFields(ref v);

                SB.Clear();
                // Build the Query
                SB.Append(" INSERT INTO ContentVersion (CV_Name, CV_id_Content, CV_DocumentID, ");
                SB.Append(" CV_Path, CV_id_PathType, CV_ECR, CV_id_ContentVersionStatus, CV_ChildNumber, ");
                SB.Append(" CV_Description, CV_LockWithDescription, CV_CommandLine, CV_LastUpdateUser, ");
                SB.Append(" CV_LastUpdateTime, CV_LastUpdateComputer, CV_LastUpdateApplication, ");
                SB.Append(" CV_PDMDocVersion, CV_ConfManagementLink, CV_CreationDate) ");
                SB.Append(" VALUES (@CV_Name, @CV_id_Content, @CV_DocumentID, ");
                SB.Append(" @CV_Path, @CV_id_PathType, @CV_ECR, @CV_id_ContentVersionStatus, ");
                SB.Append(" @CV_ChildNumber, @CV_Description, @CV_LockWithDescription, ");
                SB.Append(" @CV_CommandLine, @CV_LastUpdateUser, @CV_LastUpdateTime, ");
                SB.Append("@CV_LastUpdateComputer, @CV_LastUpdateApplication, @CV_PDMDocVersion, @CV_ConfManagementLink, @CV_CreationDate) ");
                if (DatabaseSupportsBatchQueries)
                {
                    SB.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                }

                // Set the parameters
                CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "CV_ID", DataType = DbType.Int32, Value = v.Id },
                new ParamStruct { ParamName = "CV_Name", DataType = DbType.String, Value = v.Name },
                new ParamStruct { ParamName = "CV_id_Content", DataType = DbType.Int32, Value = v.id_Content },
                new ParamStruct { ParamName = "CV_ECR", DataType = DbType.String, Value = v.ECR },
                new ParamStruct { ParamName = "CV_id_ContentVersionStatus", DataType = DbType.String, Value = v.id_ContentVersionStatus },
                new ParamStruct { ParamName = "CV_DocumentID", DataType = DbType.String, Value = v.DocumentID },
                new ParamStruct { ParamName = "CV_PDMDocVersion", DataType = DbType.String, Value = v.PDMDocumentVersion },
                new ParamStruct { ParamName = "CV_ConfManagementLink", DataType = DbType.String, Value = v.ConfigurationManagementLink },
                new ParamStruct { ParamName = "CV_Path", DataType = DbType.String, Value = v.Path.Name },
                new ParamStruct { ParamName = "CV_ChildNumber", DataType = DbType.Int32, Value = v.ChildNumber},
                new ParamStruct { ParamName = "CV_Description", DataType = DbType.String, Value = v.Description },
                new ParamStruct { ParamName = "CV_id_PathType", DataType = DbType.String, Value = v.id_PathType },
                new ParamStruct { ParamName = "CV_CommandLine", DataType = DbType.String, Value = v.CommandLine },
                new ParamStruct { ParamName = "CV_LockWithDescription", DataType = DbType.String, Value = v.LockWithDescription },
                new ParamStruct { ParamName = "CV_LastUpdateUser", DataType = DbType.String, Value = v.LastUpdateUser},   
                new ParamStruct { ParamName = "CV_LastUpdateTime", DataType = DbType.DateTime, Value = v.LastUpdateTime },
                new ParamStruct { ParamName = "CV_LastUpdateComputer", DataType = DbType.String, Value = v.LastUpdateComputer },
                new ParamStruct { ParamName = "CV_LastUpdateApplication", DataType = DbType.String, Value = v.LastUpdateApplication },
                new ParamStruct { ParamName = "CV_CreationDate", DataType = DbType.DateTime, Value = v.CreationDate }            
                };

                //Execute the query
                object RV = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());

                if (RV != null)
                {
                    v.Id = Convert.ToInt64(RV);
                    GetVersiontStatus(ref v);
          
                    //clone linked versions     

                    if (VM.ContentVersions != null)
                    {
                        v.ContentVersions = new Dictionary<int, CMContentVersionSubVersionModel>();

                        foreach (KeyValuePair<int, CMContentVersionSubVersionModel> subVersion in VM.ContentVersions)
                        {

                            CMContentVersionSubVersionModel versionSubVersion = new CMContentVersionSubVersionModel()
                            {
                                Order = subVersion.Value.Order,
                                Content = subVersion.Value.Content,
                                ContentSubVersion = subVersion.Value.ContentSubVersion
                            };
                            versionSubVersion.ContentSubVersion.Id = subVersion.Key;

                            //UpdateLastUpdate(subVersion.Value, versionSubVersion);
                            v.ContentVersions.Add(subVersion.Key, versionSubVersion);
                            AddContentVersionLink(v, versionSubVersion);
                        }
                    }

                    if (VM.Files != null)
                    {
                        v.Files = new Dictionary<int, CMContentFileModel>();

                        foreach (KeyValuePair<int, CMContentFileModel> contentFile in VM.Files)
                        {
                            CMContentFileModel file = new CMContentFileModel()
                            {
                                ID = contentFile.Value.ID,
                                FileName = contentFile.Value.FileName,
                                FileFullPath = contentFile.Value.FileFullPath,
                                FileRelativePath = contentFile.Value.FileRelativePath
                            };

                            //UpdateLastUpdate(contentFile.Value, file);
                            v.Files.Add(contentFile.Key, file);
                        }
                    }

                    return v;
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

        #region Get Content Id By Version Id

        public static int GetContentIdByVersionId(int versionID)
        {
            if (versionID == 0)
                return 0;

            try
            {
                var SB = new StringBuilder(string.Empty);

                SB.Append("SELECT CV_id_Content FROM ContentVersion WHERE CV_ID  = '" + versionID + "' ");      
                // Fetch the DataTable from the database
                DataTable ResTbl = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTbl != null)
                {
                    DataRow DataRow = ResTbl.Rows[0];
                    return (int)DataRow["CV_id_Content"];
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return 0;
            }
        }

        #endregion

        #region Get Version Name

        public static String GetVersionName(int versionID)
        {
            try
            {
                var SB = new StringBuilder(string.Empty);

                SB.Append("SELECT CV_Name FROM ContentVersion WHERE CV_ID  = '" + versionID + "' ");
                // Fetch the DataTable from the database
                DataTable ResTbl = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTbl != null)
                {
                    DataRow DataRow = ResTbl.Rows[0];
                    return (string)DataRow["CV_Name"];
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

        public static String GetLastVersionName(int parentContentID)
        {
            try
            {
                var SB = new StringBuilder(string.Empty);

                SB.Append(" SELECT CV_Name FROM ContentVersion ");
                SB.Append(" WHERE CV_id_Content = " +parentContentID);
                SB.Append(" order by CV_CreationDate desc ");

                // Fetch the DataTable from the database
                DataTable ResTbl = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTbl != null)
                {
                    DataRow DataRow = ResTbl.Rows[0];
                    return (string)DataRow["CV_Name"];
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

        #region Get Version Status

        public static void GetVersiontStatus(ref CMVersionModel v)
        {
            try
            {
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Clear();
                QryStr.Append("Select * FROM ContentVersionStatus WHERE CVS_ID = '" + v.id_ContentVersionStatus + "' ");
                string Qry = QryStr.ToString();
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null && ResTable.Rows.Count == 1)
                {
                    DataRow DataRow = ResTable.Rows[0];

                    v.Status.Icon = (string)DataRow["CVS_Icon"];
                    v.Status.ID = (string)DataRow["CVS_ID"];
                    v.Status.Name = (string)DataRow["CVS_Name"];
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return;
            }
        }

        #endregion

        #region Get Version Status List

        public static List<KeyValuePair<string, string>>GetVersiontStatusList()
        {
            try
            {
                List<KeyValuePair<string, string>> VersionStatusTypesList = new List<KeyValuePair<string, string>>();
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Clear();
                QryStr.Append("Select CVS_ID, CVS_Name FROM ContentVersionStatus");
                string Qry = QryStr.ToString();
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null)
                {
                    foreach (DataRow DataRow in ResTable.Rows)
                    {
                        KeyValuePair<string, string> TypeKeyValue = new KeyValuePair<string, string>((string)DataRow["CVS_ID"], (string)DataRow["CVS_Name"]);
                        VersionStatusTypesList.Add(TypeKeyValue);
                    }
                }
                return VersionStatusTypesList;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return null;
            }
        }

        #endregion

        #region Add New Version

        public static long AddNewVersion(ref CMVersionModel v)
        {
            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                List<ParamStruct> CommandParams;
                Boolean DatabaseSupportsBatchQueries = Domain.PersistenceLayer.GetSupportsBatchQueries();

                UpdateControlFields(ref v);
                v.CreationDate = DateTime.Now;

                v.ChildNumber = CMVersionBLL.GetChildIDForAddVersion(v.id_Content);

                SB.Clear();
                // Build the Query
                SB.Append("INSERT INTO ContentVersion (CV_Name, CV_id_Content, CV_DocumentID, CV_Path, CV_id_PathType, CV_ECR, CV_id_ContentVersionStatus, CV_ChildNumber, CV_Description, CV_LockWithDescription, CV_CommandLine, CV_LastUpdateUser, CV_LastUpdateTime, CV_LastUpdateComputer, CV_LastUpdateApplication, CV_CreationDate, CV_PDMDocVersion, CV_ConfManagementLink) ");
                SB.Append("VALUES (@CV_Name, @CV_id_Content, @CV_DocumentID, @CV_Path, @CV_id_PathType, @CV_ECR, @CV_id_ContentVersionStatus, @CV_ChildNumber, @CV_Description, @CV_LockWithDescription, @CV_CommandLine, @CV_LastUpdateUser, @CV_LastUpdateTime, @CV_LastUpdateComputer, @CV_LastUpdateApplication, @CV_CreationDate, @CV_PDMDocVersion, @CV_ConfManagementLink) ");
                if (DatabaseSupportsBatchQueries)
                {
                    SB.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                }

                // Set the parameters
                CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "CV_ID", DataType = DbType.Int32, Value = v.Id },
                new ParamStruct { ParamName = "CV_Name", DataType = DbType.String, Value = v.Name },
                new ParamStruct { ParamName = "CV_id_Content", DataType = DbType.Int32, Value = v.id_Content },
                new ParamStruct { ParamName = "CV_ECR", DataType = DbType.String, Value = v.ECR },
                new ParamStruct { ParamName = "CV_id_ContentVersionStatus", DataType = DbType.String, Value = v.id_ContentVersionStatus },
                new ParamStruct { ParamName = "CV_DocumentID", DataType = DbType.String, Value = v.DocumentID },
                new ParamStruct { ParamName = "CV_PDMDocVersion", DataType = DbType.String, Value = v.PDMDocumentVersion },
                new ParamStruct { ParamName = "CV_ConfManagementLink", DataType = DbType.String, Value = v.ConfigurationManagementLink },
                new ParamStruct { ParamName = "CV_Path", DataType = DbType.String, Value = v.Path.Name },
                new ParamStruct { ParamName = "CV_ChildNumber", DataType = DbType.Int32, Value = v.ChildNumber},
                new ParamStruct { ParamName = "CV_Description", DataType = DbType.String, Value = v.Description },
                new ParamStruct { ParamName = "CV_id_PathType", DataType = DbType.String, Value = v.id_PathType },
                new ParamStruct { ParamName = "CV_CommandLine", DataType = DbType.String, Value = v.CommandLine },
                new ParamStruct { ParamName = "CV_LockWithDescription", DataType = DbType.String, Value = v.LockWithDescription },
                new ParamStruct { ParamName = "CV_LastUpdateUser", DataType = DbType.String, Value = v.LastUpdateUser},   
                new ParamStruct { ParamName = "CV_LastUpdateTime", DataType = DbType.DateTime, Value = v.LastUpdateTime },
                new ParamStruct { ParamName = "CV_LastUpdateComputer", DataType = DbType.String, Value = v.LastUpdateComputer },
                new ParamStruct { ParamName = "CV_LastUpdateApplication", DataType = DbType.String, Value = v.LastUpdateApplication },
                new ParamStruct { ParamName = "CV_CreationDate", DataType = DbType.DateTime, Value = v.CreationDate }
                };

                //Execute the query
                object RV = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());

                if (RV != null)
                {
                    v.Id = Convert.ToInt64(RV);
                    GetVersiontStatus(ref v);
                    return v.Id;
                }
                else
                {
                    throw new TraceException("Parent content deleted", null, "Content manager");
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new TraceException("Parent content deleted", null, "Content manager");
            }
        }

        #endregion

        #region Get Child ID For Add Version

        public static int GetChildIDForAddVersion(long ContentId)
        {
            // Initialize work fields           
            int lastChildID = 0;

            try
            { 
                // Work fields
                var SB = new StringBuilder(string.Empty);
                SB.Clear();
                SB.Append("SELECT MAX(CV_ChildNumber) FROM ContentVersion WHERE CV_id_Content = " + ContentId);
                // Fetch the DataTable from the database
                Int16 max = -1;
                object objMax = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null);

                if (objMax == DBNull.Value)
                {
                    max = -1;
                }
                else
                {
                    max = Convert.ToInt16(objMax);
                }

                if (max >= 0)
                {
                    lastChildID = max;
                }
                else
                {
                    lastChildID = -1;
                }

            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return 1;
            }

            lastChildID++;
            return lastChildID;
        }

        public static int GetVersionsCountForAddVersion(long ContentId)
        {
            // Initialize work fields           
            int lastChildID = 0;

            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                SB.Clear();
                SB.Append("SELECT count(CV_ChildNumber) FROM ContentVersion WHERE CV_id_Content = " + ContentId);
                // Fetch the DataTable from the database
                Int16 max = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));

                if (max >= 0)
                {
                    lastChildID = max;
                }
                else
                {
                    lastChildID = -1;
                }

            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return 1;
            }
            return lastChildID;
        }

        #endregion

        #region Generate Version Name

        public static string GenerateVersionName(ref CMVersionModel version)
        {
            try
            {
                // Work fields
                int index = 0;
                var SB = new StringBuilder(string.Empty);
                SB.Clear();
                SB.Append("SELECT COUNT(CV_ID) FROM ContentVersion where CV_Name = '" + version.Name.ToString() + "' AND CV_id_Content = " + version.ParentID);
                // Fetch the DataTable from the database
                Int16 res = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));

                if (res == 0)
                {
                    return version.Name;
                }
                else
                {
                    do
                    {
                        index++;
                        SB.Clear();
                        SB.Append("SELECT COUNT(CV_ID) FROM ContentVersion where CV_Name = '" + version.Name.ToString() + index + "' AND CV_id_Content = " + version.ParentID);
                        res = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));

                    } while (res > 0);

                    version.Name = version.Name + index;
                }
                return version.Name;
            }
            catch (Exception e) //there is no child's for folder parent
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return null;
            }
        }

        #endregion

        #region Get Changed Linked Version

        public static List<CMTreeNode> GetChangedLinkedVersion(CMVersionModel contentVersionNew)
        {
            bool existVersion;
            List<CMTreeNode> contentVersionLinkedVersionChange = new List<CMTreeNode>();

            if (contentVersionNew == null)
                return contentVersionLinkedVersionChange;

            if (contentVersionNew == null)
            {
                foreach (KeyValuePair<int, CMContentVersionSubVersionModel> contentVersionSubVersion in contentVersionNew.BeforeUpdateLinkedVersions)
                    contentVersionLinkedVersionChange.Add(contentVersionSubVersion.Value.ContentSubVersion);

                return contentVersionLinkedVersionChange;
            }

            if (contentVersionNew == null)
            {
                foreach (KeyValuePair<int, CMContentVersionSubVersionModel> contentVersionSubVersion in contentVersionNew.ContentVersions)
                    contentVersionLinkedVersionChange.Add(contentVersionSubVersion.Value.ContentSubVersion);

                return contentVersionLinkedVersionChange;
            }

            foreach (KeyValuePair<int, CMContentVersionSubVersionModel> contentVersionSubVersionNew in contentVersionNew.ContentVersions)
            {
                existVersion = false;
                foreach (KeyValuePair<int, CMContentVersionSubVersionModel> contentVersionSubVersionOld in contentVersionNew.BeforeUpdateLinkedVersions)
                {
                    if (contentVersionSubVersionNew.Value.ContentSubVersion.ID == contentVersionSubVersionOld.Value.ContentSubVersion.ID)
                        existVersion = true;
                }

                if (!existVersion)
                    contentVersionLinkedVersionChange.Add(contentVersionSubVersionNew.Value.ContentSubVersion);
            }

            foreach (KeyValuePair<int, CMContentVersionSubVersionModel> contentVersionSubVersionOld in contentVersionNew.BeforeUpdateLinkedVersions)
            {
                existVersion = false;
                foreach (KeyValuePair<int, CMContentVersionSubVersionModel> contentVersionSubVersionNew in contentVersionNew.ContentVersions)
                {
                    if (contentVersionSubVersionNew.Value.ContentSubVersion.ID == contentVersionSubVersionOld.Value.ContentSubVersion.ID)
                        existVersion = true;
                }

                if (!existVersion)
                    contentVersionLinkedVersionChange.Add(contentVersionSubVersionOld.Value.ContentSubVersion);
            }

            return contentVersionLinkedVersionChange;
        }

        #endregion

        #region Add Content Version Files

        public static void AddContentVersionFiles(ref CMVersionModel v)
        {
            Dictionary<int, CMContentFileModel> filesWithUpdatedIndex = new Dictionary<int, CMContentFileModel>();

            foreach (KeyValuePair<int, CMContentFileModel> file in v.Files)
            {
                AddContentVersionFile(v, file.Value);
                filesWithUpdatedIndex.Add(file.Value.ID, file.Value);
            }

            v.Files = filesWithUpdatedIndex;
        }

        #endregion

        #region Add Content Version File

        public static void AddContentVersionFile(CMVersionModel v, CMContentFileModel f)
        {
            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                List<ParamStruct> CommandParams;
                Boolean DatabaseSupportsBatchQueries = Domain.PersistenceLayer.GetSupportsBatchQueries();

                UpdateControlFields(ref f);

                SB.Clear();
                // Build the Query
                SB.Append("INSERT INTO ContentVersionFile (CVF_LastUpdateUser, CVF_LastUpdateComputer, CVF_LastUpdateApplication, CVF_LastUpdateTime, CVF_Name, CVF_Path, CVF_Size, CVF_FileLastWriteTime, CVF_id_ContentVersion) ");
                SB.Append("VALUES (@CVF_LastUpdateUser, @CVF_LastUpdateComputer, @CVF_LastUpdateApplication, @CVF_LastUpdateTime, @CVF_Name, @CVF_Path, @CVF_Size, @CVF_FileLastWriteTime, @CVF_id_ContentVersion) ");
                if (DatabaseSupportsBatchQueries)
                {
                    SB.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                }

                // Set the parameters
                CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "CVF_ID", DataType = DbType.Int32, Value = f.ID },
                new ParamStruct { ParamName = "CVF_Name", DataType = DbType.String, Value = f.FileName },
                new ParamStruct { ParamName = "CVF_id_ContentVersion", DataType = DbType.String, Value = v.Id },
                new ParamStruct { ParamName = "CVF_Path", DataType = DbType.String, Value = f.FileRelativePath},
                new ParamStruct { ParamName = "CVF_Size", DataType = DbType.Int64, Value = f.FileSize },
                new ParamStruct { ParamName = "CVF_FileLastWriteTime", DataType = DbType.DateTime, Value = f.FileLastWriteTime},
                new ParamStruct { ParamName = "CVF_LastUpdateUser", DataType = DbType.String, Value = f.LastUpdateUser},   
                new ParamStruct { ParamName = "CVF_LastUpdateTime", DataType = DbType.DateTime, Value = f.LastUpdateTime },
                new ParamStruct { ParamName = "CVF_LastUpdateComputer", DataType = DbType.String, Value = f.LastUpdateComputer },
                new ParamStruct { ParamName = "CVF_LastUpdateApplication", DataType = DbType.String, Value = f.LastUpdateApplication } 
                };
                
                    
                //Execute the query
                object RV = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());


                if (RV != null)
                {
                    f.ID = Convert.ToInt32(RV);
                    return;
                }
                else
                {
                    return;
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return;
            }
        }   

        #endregion

        #region Add Content Version Version Link

        public static void AddContentVersionVersionLink(ref CMVersionModel v)
        {
            foreach (KeyValuePair<int, CMContentVersionSubVersionModel> versionLink in v.ContentVersions)
                AddContentVersionLink(v, versionLink.Value);
        }

        #endregion

        #region Add Content Version Link

        public static void AddContentVersionLink(CMVersionModel v, CMContentVersionSubVersionModel versionLink)
        {
            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                List<ParamStruct> CommandParams;
                Boolean DatabaseSupportsBatchQueries = Domain.PersistenceLayer.GetSupportsBatchQueries();

                UpdateControlFields(ref versionLink);

                SB.Clear();
                // Build the Query
                SB.Append("INSERT INTO ContentVersionVersionLink (CVVL_LastUpdateUser, CVVL_LastUpdateComputer, CVVL_LastUpdateApplication, CVVL_LastUpdateTime, CVVL_ChildNumber, CVVL_id_ContentVersion_Parent, CVVL_id_ContentVersion_Link) ");
                SB.Append("VALUES (@CVVL_LastUpdateUser, @CVVL_LastUpdateComputer, @CVVL_LastUpdateApplication, @CVVL_LastUpdateTime, @CVVL_ChildNumber, @CVVL_id_ContentVersion_Parent, @CVVL_id_ContentVersion_Link) ");
                if (DatabaseSupportsBatchQueries)
                {
                    SB.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                }

                // Set the parameters
                CommandParams = new List<ParamStruct>()
                {
                //new ParamStruct { ParamName = "CVF_ID", DataType = DbType.Int32, Value = versionLink.i },
                new ParamStruct { ParamName = "CVVL_id_ContentVersion_Parent", DataType = DbType.Int32, Value = v.Id },
                new ParamStruct { ParamName = "CVVL_ChildNumber", DataType = DbType.Int32, Value = versionLink.Order },
                new ParamStruct { ParamName = "CVVL_id_ContentVersion_Link", DataType = DbType.Int32, Value = versionLink.ContentSubVersion.Id},
                new ParamStruct { ParamName = "CVVL_LastUpdateUser", DataType = DbType.String, Value = versionLink.LastUpdateUser},   
                new ParamStruct { ParamName = "CVVL_LastUpdateTime", DataType = DbType.DateTime, Value = versionLink.LastUpdateTime },
                new ParamStruct { ParamName = "CVVL_LastUpdateComputer", DataType = DbType.String, Value = versionLink.LastUpdateComputer },
                new ParamStruct { ParamName = "CVVL_LastUpdateApplication", DataType = DbType.String, Value = versionLink.LastUpdateApplication } 
                };

                //Execute the query
                object RV = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());

                if (RV != null)
                {
                    //v.Id = Convert.ToInt64(RV);
                    return;
                }
                else
                {
                    return;
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return;
            }
        }

        #endregion

        #region Check Version Name

        public static string CheckVersionName(ref CMVersionModel v)
        {
            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                SB.Clear();
                SB.Append("SELECT COUNT(CV_ID) FROM ContentVersion WHERE CV_Name = '" + v.Name.ToString() + "' AND CV_id_Content = " + v.id_Content);

                // Fetch the DataTable from the database
                Int16 Count = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));

                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);

                if (Count > 0)
                {
                    return "Adding existing version";
                }
                else
                {
                    return "OK";
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

        #region Check If There Are Linked Vesrions To Version

        public static bool CheckIfThereAreLinkedVesrionsToVersion(long VersionId)
        {
            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                SB.Clear();
                SB.Append("SELECT COUNT(*) FROM ContentVersionVersionLink WHERE CVVL_id_ContentVersion_Link = '" + VersionId + " '");

                // Fetch the DataTable from the database
                Int16 Count = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));

                if (Count > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return false;
            }
        }

        #endregion 

        #region Check Version Linked Version

        public static bool CheckVersionLinkedVersion(CMVersionModel Version)
        {
            try
            {
                // Work Fields
                var SB = new StringBuilder(string.Empty);
                // Build the Query
                SB.Append("SELECT [Content].CO_Name AS Content, ContentVersion.CV_Name AS ContentVersion ");
                SB.Append("FROM ContentVersionVersionLink INNER JOIN ");
                SB.Append("ContentVersion ON ContentVersionVersionLink.CVVL_id_ContentVersion_Parent = ContentVersion.CV_ID INNER JOIN ");
                SB.Append("[Content] ON ContentVersion.CV_id_Content = [Content].CO_ID ");
                SB.Append("WHERE (ContentVersionVersionLink.CVVL_id_ContentVersion_Link = " + Version.Id + ") ");

                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null)
                {
                    if (ResTable.Rows.Count > 0)
                    {
                        throw new TraceException("Version linked deleted", true, new List<string>() { (String)ResTable.Rows[0]["Content"], (String)ResTable.Rows[0]["ContentVersion"] }, "Content manager");
                    }
                    else
                    {
                        return true;
                    }

                }
                return true;
            }
            catch (TraceException te)
            {
                throw te;
            }
        }
        #endregion

        #region Check Version Linked Project

        public static void CheckVersionLinkedProject(CMVersionModel Version)
        {
            Dictionary<int, HierarchyModel> contentVersionProjects =
                ApiBLL.GetProjectsByContentVersionID((int)Version.Id);

            if (contentVersionProjects.Count > 0)
                throw new TraceException("Version linked deleted", true, new List<string>() { contentVersionProjects.First().Value.Name, contentVersionProjects.First().Value.VM.VersionName }, "Content manager");
        }

        //CR 3692, error message "Versions linked in PE"
        public static Domain.ErrorHandling CheckLinkedVersionsInPE(CMVersionModel parentVersion, CMVersionModel Version2)
        {
            Domain.ErrorHandling Status = new Domain.ErrorHandling();
            Dictionary<int, HierarchyModel> parentVersionProjects = ApiBLL.GetProjectsByContentVersionID((int)parentVersion.Id);

            Dictionary<int, CMFolderModel> outFolders = new Dictionary<int, CMFolderModel>();
            Dictionary<int, CMContentModel> outContents = new Dictionary<int, CMContentModel>();
            Dictionary<int, CMVersionModel> outVersions = new Dictionary<int, CMVersionModel>();

            if (parentVersionProjects == null || parentVersionProjects.Count == 0)
            {
                return Status;
            }

            //Get all PE versions for relevant project
            List<int> prIds = new List<int>();
            foreach (int prId in parentVersionProjects.Keys)
            {
                prIds.Add(prId);
            }

            DataTable peVersions = VersionBLL.GetVersionsDataByProjectIds(prIds);
            //To do
            List<int> versionIdToList = new List<int>();
            versionIdToList.Add((int)Version2.ID);
            List<int> allLinkedVersions = GetVersionAllSubVersions(versionIdToList);
            allLinkedVersions.Add((int)Version2.ID);
            List<int> allLinkedContents = CMContentBLL.GetContentIds(allLinkedVersions);

            CMTreeNodeBLL.GetSubTreeObjects(allLinkedContents,
                                                        out outFolders, out outContents, out outVersions);

            foreach (KeyValuePair<int, HierarchyModel> project in parentVersionProjects)
            {
                foreach (VersionModel version in parentVersionProjects[project.Key].GetAllVersions)
                {
                    string selectCondition = "VersionId =" + version.VersionId + " and HierarchyId = " + project.Key;
                    DataRow[] cvRow = peVersions.Select(selectCondition);
                    foreach (DataRow dr in cvRow)
                    {
                        if (outVersions.ContainsKey(Convert.ToInt32(dr["ContentVersionId"])))
                        {
                            Status.messsageId = "Versions linked in PE";
                            Status.messageParams[0] = outContents[outVersions[Convert.ToInt32(dr["ContentVersionId"])].ParentID].Name;
                            Status.messageParams[1] = parentVersionProjects[project.Key].TreeHeader;
                            Status.messageParams[2] = version.VersionName;
                            return Status;
                        }
                    }
                }
            }
            return Status;
        }

        #endregion

        #region Select Content Version Linked

        public static void SelectContentVersionLinked(CMVersionModel contentVersionNode)
        {
            foreach (KeyValuePair<int, CMContentVersionSubVersionModel> contentVersionSubVersion in contentVersionNode.ContentVersions)
               SelectContentVersion(contentVersionSubVersion.Value.ContentSubVersion);
        }

        #endregion

        #region Select Content Version

        public static void SelectContentVersion(CMVersionModel contentVersion)
        {

            // Work fields
            String updateUser;
            var SB = new StringBuilder(string.Empty);
            SB.Clear();
            SB.Append("SELECT CV_LastUpdateTime, CV_LastUpdateUser, (SELECT COUNT(CVVL_id_ContentVersion_Parent) AS Expr1 FROM ContentVersionVersionLink WHERE (CVVL_id_ContentVersion_Parent = " +
            contentVersion.Id + ")) AS VersionLinksAmount, (SELECT COUNT(CVF_ID) AS Expr1 FROM ContentVersionFile WHERE (CVF_id_ContentVersion = " +
            contentVersion.Id + ")) AS FilesAmount FROM ContentVersion WITH(UPDLOCK) WHERE CV_ID = " + contentVersion.Id);

            // Fetch the DataTable from the database
            DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);

            if (ResTable.Rows.Count == 0)
                throw new TraceException("Version deleted", true, new List<string>() { contentVersion.Name }, "Content manager");

            updateUser = (String)ResTable.Rows[0]["CV_LastUpdateUser"];

            if(!CompareUpdateTime(contentVersion.LastUpdateTime, contentVersion.Id))
                throw new TraceException("Version changed", true, new List<string>() { contentVersion.Name, updateUser }, "Content manager");
  
            //if (((int)ResTable.Rows[0]["FilesAmount"]) != contentVersion.Files.Count)
            //    throw new TraceException("Version changed", true, new List<string>() { contentVersion.Name, updateUser }, "Content manager");

            //if (((int)ResTable.Rows[0]["VersionLinksAmount"]) != contentVersion.ContentVersions.Count)
            //    throw new TraceException("Version changed", true, new List<string>() { contentVersion.Name, updateUser }, "Content manager");

            foreach (KeyValuePair<int, CMContentFileModel> file in contentVersion.Files)
                SelectContentVersionFiles(contentVersion, file.Value, updateUser);

            foreach (KeyValuePair<int, CMContentVersionSubVersionModel> subVersion in contentVersion.ContentVersions)
                SelectContentVersionVersionLink(contentVersion, subVersion.Value, updateUser);            
        }

        #endregion

        #region Select Content Version Files

        public static void SelectContentVersionFiles(CMVersionModel contentVersion, CMContentFileModel contentVersionFile, String updateUser)
        {
            try
            {
                var SB = new StringBuilder(string.Empty);

                SB.Append("SELECT CVF_LastUpdateTime, CVF_LastUpdateUser FROM ContentVersionFile WITH(UPDLOCK) WHERE CVF_ID  = '" + contentVersionFile.ID + "' ");
                // Fetch the DataTable from the database
                DataTable ResTbl = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTbl != null)
                {
                    if (ResTbl.Rows.Count == 0)
                        throw new TraceException("Version changed", true, new List<string>() { contentVersion.Name, updateUser }, "Content manager");

                    //if (!CompareUpdateTime(((DateTime)dt.Rows[0]["UpdateTime"]), contentVersionFile.LastUpdateTime))
                    //    throw new TraceException("Version changed", true, new List<string>() { contentVersion.Name, (String)dt.Rows[0]["UpdateUser"] }, "Content manager");
                }
                else
                {
                    return;
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return;
            }
        }

        #endregion

        #region Select Content Version Version Link

        public static void SelectContentVersionVersionLink(CMVersionModel contentVersion, CMContentVersionSubVersionModel versionLink, String updateUser)
        {
            try
            {
                var SB = new StringBuilder(string.Empty);
         
                SB.Append("SELECT CVVL_LastUpdateTime, CVVL_LastUpdateUser FROM ContentVersionVersionLink WITH(UPDLOCK) WHERE CVVL_id_ContentVersion_Parent  = '" + contentVersion.Id + "' AND CVVL_id_ContentVersion_Link = " + versionLink.ContentSubVersion.Id);
                // Fetch the DataTable from the database
                DataTable ResTbl = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTbl != null)
                {
                    if (ResTbl.Rows.Count == 0)
                        throw new TraceException("Version changed", true, new List<string>() { contentVersion.Name, updateUser }, "Content manager");

                    //if (!CompareUpdateTime(((DateTime)ResTbl.Rows[0]["UpdateTime"]), versionLink.LastUpdateTime))
                    //    throw new TraceException("Version changed", true, new List<string>() { contentVersion.Name, (String)ResTbl.Rows[0]["UpdateUser"] }, "Content manager");
                }
                else
                {
                    return;
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return;
            }
        }

        #endregion

        #region Check Existing Version Name

        public static string CheckExistingVersionName(ref CMVersionModel v)
        {
            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                SB.Clear();
                SB.Append("SELECT COUNT(CV_ID) FROM ContentVersion WHERE CV_Name = '" + v.Name.ToString() + "' AND CV_ID <> " + v.Id + " AND CV_id_Content = " + v.id_Content);

                // Fetch the DataTable from the database
                Int16 Count = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));

                if (Count > 0)
                {
                    return "Update version name to existing";
                }
                else
                {
                    return "OK";
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

        #region UpdateContentVersionFilesOnFs

        public static void UpdateContentVersionFilesOnFs(CMVersionModel contentVersion, CMVersionModel contentVersionOriginal, out bool updateFs, CMICopyFilesProgressModel progressBar, CMImpersonationBLL imp)
        {
            updateFs = false;
            CMContentFileModel contentFileOriginal;

            if (contentVersion.Files.Count != contentVersion.BeforeUpdateFiles.Count)
                updateFs = true;
            else
            {
                foreach (KeyValuePair<int, CMContentFileModel> file in contentVersion.Files)
                {
                    if (contentVersion.BeforeUpdateFiles.ContainsKey(file.Value.ID))
                    {
                        contentFileOriginal = contentVersion.BeforeUpdateFiles[file.Value.ID];
                        if (file.Value.FileName != contentFileOriginal.FileName ||
                            file.Value.FileRelativePath != contentFileOriginal.FileRelativePath)
                        {
                            updateFs = true;
                            break;
                        }
                    }
                    else
                    {
                        updateFs = true;
                        break;
                    }
                }
            }

            if (!updateFs)
            {
                contentVersion.Path = new ATSBusinessObjects.ContentMgmtModels.CMVersionModel.PathFS() { Type = contentVersionOriginal.Path.Type, Name = contentVersionOriginal.Path.Name };
                return;
            }

            CMFileSystemUpdaterBLL.AddContentVersionFilesOnFs(contentVersion, progressBar, imp);
        }

        #endregion

        #region Update Existing Version

        public static long UpdateExistingVersion(ref CMVersionModel v)
        {        
            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                List<ParamStruct> CommandParams;

                UpdateControlFields(ref v); // Update Control fields

                // Build the Query
                SB.Clear();
                SB.Append("UPDATE ContentVersion SET CV_LastUpdateUser=@CV_LastUpdateUser, ");
                SB.Append("CV_LastUpdateComputer=@CV_LastUpdateComputer,CV_LastUpdateApplication=@CV_LastUpdateApplication, ");
                SB.Append("CV_LastUpdateTime=@CV_LastUpdateTime, CV_Description=@CV_Description, ");
                //removed ChildNumber
                SB.Append("CV_id_ContentVersionStatus=@CV_id_ContentVersionStatus, CV_Name=@CV_Name, ");
                SB.Append("CV_ECR=@CV_ECR, CV_id_Content=@CV_id_Content, CV_DocumentID=@CV_DocumentID, ");
                SB.Append("CV_PDMDocVersion=@CV_PDMDocVersion, CV_ConfManagementLink=@CV_ConfManagementLink, ");
                SB.Append("CV_Path=@CV_Path, CV_CommandLine=@CV_CommandLine, CV_LockWithDescription=@CV_LockWithDescription, ");
                SB.Append("CV_id_PathType=@CV_id_PathType ");
                SB.Append("WHERE CV_ID=@CV_ID");   

                // Set the parameters
                CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "CV_ID", DataType = DbType.Int32, Value = v.Id },
                new ParamStruct { ParamName = "CV_Name", DataType = DbType.String, Value = v.Name },
                new ParamStruct { ParamName = "CV_id_ContentVersionStatus", DataType = DbType.String, Value = v.id_ContentVersionStatus },                   
                new ParamStruct { ParamName = "CV_Description", DataType = DbType.String, Value = v.Description },
                new ParamStruct { ParamName = "CV_Path", DataType = DbType.String, Value = v.Path.Name },        
                //new ParamStruct { ParamName = "CV_ChildNumber", DataType = DbType.Int32, Value = v.ChildNumber},
                new ParamStruct { ParamName = "CV_ECR", DataType = DbType.String, Value = v.ECR },
                new ParamStruct { ParamName = "CV_id_Content", DataType = DbType.String, Value = v.id_Content },
                new ParamStruct { ParamName = "CV_DocumentID", DataType = DbType.String, Value = v.DocumentID },
                new ParamStruct { ParamName = "CV_PDMDocVersion", DataType = DbType.String, Value = v.PDMDocumentVersion },
                new ParamStruct { ParamName = "CV_ConfManagementLink", DataType = DbType.String, Value = v.ConfigurationManagementLink },
                new ParamStruct { ParamName = "CV_CommandLine", DataType = DbType.String, Value = v.CommandLine },
                new ParamStruct { ParamName = "CV_LockWithDescription", DataType = DbType.String, Value = v.LockWithDescription },
                new ParamStruct { ParamName = "CV_id_PathType", DataType = DbType.String, Value = v.id_PathType },
                new ParamStruct { ParamName = "CV_LastUpdateTime", DataType = DbType.DateTime, Value = v.LastUpdateTime },
                new ParamStruct { ParamName = "CV_LastUpdateUser", DataType = DbType.String, Value = v.LastUpdateUser },
                new ParamStruct { ParamName = "CV_LastUpdateComputer", DataType = DbType.String, Value = v.LastUpdateComputer },
                new ParamStruct { ParamName = "CV_LastUpdateApplication", DataType = DbType.String, Value = v.LastUpdateApplication }
                };
                //Execute the query
                object RV = Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());

                if (RV != null)
                {
                    if (Convert.ToInt32(RV) > 0)  //if content exists
                    {
                        GetVersiontStatus(ref v);
                        return v.Id;
                    }
                    else
                    {
                        throw new TraceException("Version deleted", true, new List<string>() { v.Name }, "Content manager");
                    }    
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new TraceException("Version deleted", true, new List<string>() { v.Name }, "Content manager");
            }
        }

        #endregion

        #region Post Update ContentVersion

        public static void PostUpdateContentVersion(ref CMVersionModel contentVersionUpdated, ref CMVersionModel contentVersionOriginal, CMImpersonationBLL imp)
        {
            imp.Logon();

            if (contentVersionUpdated.Path.Name == contentVersionOriginal.Path.Name &&
                    contentVersionUpdated.Path.Type == contentVersionOriginal.Path.Type)
                return;

            if (contentVersionOriginal.Path.Type == ATSBusinessObjects.ContentMgmtModels.CMVersionModel.PathType.Relative)
                CMFileSystemUpdaterBLL.DeleteFolder(CMTreeNodeBLL.getRootPath() + "\\" + contentVersionOriginal.Path.Name);

            imp.Dispose();
        }

        #endregion

        #region Update Content Version Files

        public static void UpdateContentVersionFiles(ref CMVersionModel contentVersion)
        {
            String filesNotToDelete = String.Empty;
            Dictionary<int, CMContentFileModel> contentFileUpdated = new Dictionary<int, CMContentFileModel>();

            foreach (KeyValuePair<int, CMContentFileModel> contentFile in contentVersion.Files)
            {
                if (contentFile.Value.ID != 0)
                    UpdateContentVersionFileUpdate(contentFile.Value);
                else
                    AddContentVersionFile(contentVersion, contentFile.Value);

                filesNotToDelete = filesNotToDelete == String.Empty
                                        ? contentFile.Value.ID.ToString()
                                        : filesNotToDelete + "," + contentFile.Value.ID;

                contentFileUpdated.Add(contentFile.Value.ID, contentFile.Value);
            }

            contentVersion.Files = contentFileUpdated;
      
            UpdateContentVersionFilesDelete(contentVersion, filesNotToDelete);
        }

        #endregion

        #region Update Content Version File Update

        public static void UpdateContentVersionFileUpdate(CMContentFileModel file)
        {

            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                List<ParamStruct> CommandParams;

                UpdateControlFields(ref file);

                // Build the Query
                SB.Clear();
                SB.Append("UPDATE ContentVersionFile SET CVF_LastUpdateUser=@CVF_LastUpdateUser, CVF_LastUpdateComputer=@CVF_LastUpdateComputer,CVF_LastUpdateApplication=@CVF_LastUpdateApplication, ");
                SB.Append("CVF_LastUpdateTime=@CVF_LastUpdateTime, CVF_Name=@CVF_Name, CVF_Path=@CVF_Path, CVF_Size=@CVF_Size, CVF_FileLastWriteTime=@CVF_FileLastWriteTime ");
                SB.Append("WHERE CVF_ID=@CVF_ID");
   
                // Set the parameters
                CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "CVF_ID", DataType = DbType.Int32, Value = file.ID },
                new ParamStruct { ParamName = "CVF_Name", DataType = DbType.String, Value = file.FileName },
                new ParamStruct { ParamName = "CVF_Path", DataType = DbType.String, Value = file.FileRelativePath },        
                new ParamStruct { ParamName = "CVF_Size", DataType = DbType.Int64, Value = file.FileSize },
                new ParamStruct { ParamName = "CVF_FileLastWriteTime", DataType = DbType.DateTime, Value = file.FileLastWriteTime},
                new ParamStruct { ParamName = "CVF_LastUpdateTime", DataType = DbType.DateTime, Value = file.LastUpdateTime },
                new ParamStruct { ParamName = "CVF_LastUpdateUser", DataType = DbType.String, Value = file.LastUpdateUser },
                new ParamStruct { ParamName = "CVF_LastUpdateComputer", DataType = DbType.String, Value = file.LastUpdateComputer },
                new ParamStruct { ParamName = "CVF_LastUpdateApplication", DataType = DbType.String, Value = file.LastUpdateApplication }
                };
                //Execute the query
                object RV = Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());

                return;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return;
            }
        }

        #endregion

        #region Update Content Version Version Links

        public static void UpdateContentVersionVersionLinks(ref CMVersionModel contentVersion, ref CMVersionModel contentVersionOriginal)
        {
            int versionLink;
            String strVersionLinksToDelete = String.Empty;
            List<int> versionLinksToLeave = new List<int>();
  
            DataTable contentVersionsSubVercionsDataTable = CMTreeNodeBLL.GetContentVersionSubVersionsByContentVersionID(contentVersion.Id);

            foreach (DataRow row in contentVersionsSubVercionsDataTable.Rows)
            {
                versionLink = (int)row["CVVL_id_ContentVersion_Link"];
   
                if (!contentVersion.ContentVersions.ContainsKey(versionLink) ||
                    contentVersion.ContentVersions[versionLink].Order != (int)row["CVVL_ChildNumber"])
                {
                    if (strVersionLinksToDelete == String.Empty)
                        strVersionLinksToDelete = versionLink.ToString();
                    else
                        strVersionLinksToDelete += "," + versionLink.ToString();
                }
                else
                {
                    contentVersion.ContentVersions[versionLink] = contentVersionOriginal.ContentVersions[versionLink];
                    versionLinksToLeave.Add(versionLink);
                }
            }

            UpdateContentVersionVersionLinksDelete(contentVersion, strVersionLinksToDelete);

            foreach (KeyValuePair<int, CMContentVersionSubVersionModel> contentVersionLink in contentVersion.ContentVersions)
            {
                if (!versionLinksToLeave.Contains(contentVersionLink.Key))
                     AddContentVersionLink(contentVersion, contentVersionLink.Value);
            }
        }

        #endregion

        #region Update Content Version Version Links Delete

        public static void UpdateContentVersionVersionLinksDelete(CMVersionModel contentVersion, String strVersionLinksToDelete)
        {
            if (strVersionLinksToDelete == String.Empty)
                return;

            try
            {
                // Work Fields
                var SB = new StringBuilder(string.Empty);
                // Build the Query
                SB.Append("DELETE FROM ContentVersionVersionLink WHERE CVVL_id_ContentVersion_Parent = " + contentVersion.Id + " AND CVVL_id_ContentVersion_Link IN (" + strVersionLinksToDelete + ")");

                long result = Convert.ToInt64(Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null));
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return;
            }
   
        }

        #endregion

        #region Update Content Version Files Delete

        public static void UpdateContentVersionFilesDelete(CMVersionModel contentVersion, String filesNotToDelete)
        {
            try
            {
                // Work Fields
                var SB = new StringBuilder(string.Empty);
                //List<ParamStruct> CommandParams;
                // Build the Query
                string addToQry;
                if (filesNotToDelete != String.Empty)
                    addToQry = " AND CVF_ID NOT IN (" + filesNotToDelete + ")";

                if (filesNotToDelete != String.Empty)
                {
                    SB.Append("DELETE FROM ContentVersionFile WHERE CVF_id_ContentVersion = " + contentVersion.Id + " AND CVF_ID NOT IN (" + filesNotToDelete + ")");
                }
                else
                {
                    SB.Append("DELETE FROM ContentVersionFile WHERE CVF_id_ContentVersion = " + contentVersion.Id);
                }
                long result = Convert.ToInt64(Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null));
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return;
            }
   
        }

        #endregion

        #region Delete Content Version Sub Versions

        public static long DeleteContentVersionSubVersions(CMVersionModel Version)
        {
            try
            {
                // Work Fields
                long RV = 0;
                var SB = new StringBuilder(string.Empty);
                List<ParamStruct> CommandParams;
                // Build the Query
                SB.Append("DELETE FROM ContentVersionVersionLink WHERE CVVL_id_ContentVersion_Parent=@CVVL_id_ContentVersion_Parent");
                // Set the parameters
                CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "CVVL_id_ContentVersion_Parent", DataType = DbType.Int32, Value = Version.Id }
                };
                // Execute the query
                RV = (long)Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                // Finalize
                if (RV < 0) // Something went wrong... No rows were affected
                {
                    return -1;
                }
                else // All OK
                {
                    return RV;
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return -1;
            }
        }

        #endregion

        #region Delete Content Version Files Db

        public static long DeleteContentVersionFilesDb(CMVersionModel Version)
        {
            try
            {
                // Work Fields
                long RV = 0;
                var SB = new StringBuilder(string.Empty);
                List<ParamStruct> CommandParams;
                // Build the Query
                SB.Append("DELETE FROM ContentVersionFile WHERE CVF_id_ContentVersion=@CVF_id_ContentVersion");
                // Set the parameters
                CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "CVF_id_ContentVersion", DataType = DbType.Int32, Value = Version.Id }
                };
                // Execute the query
                RV = (long)Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                // Finalize
                if (RV < 0) // Something went wrong... No rows were affected
                {
                    return -1;
                }
                else // All OK
                {
                    return RV;
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return -1;
            }
        }

        #endregion

        #region Delete Content Version Data

        public static long DeleteContentVersionData(CMVersionModel Version)
        {
            try
            {
                // Work Fields
                long RV = 0;
                var SB = new StringBuilder(string.Empty);
                List<ParamStruct> CommandParams;
                // Build the Query
                SB.Append("DELETE FROM ContentVersion WHERE CV_ID=@CV_ID");
                // Set the parameters
                CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "CV_ID", DataType = DbType.Int32, Value = Version.Id }
                };
                // Execute the query
                RV = (long)Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                // Finalize
                if (RV < 0) // Something went wrong... No rows were affected
                {
                    return -1;
                }
                else // All OK
                {
                    return RV;
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return -1;
            }
        }

        #endregion

        #region Delete Content Version Files Fs

        public static void DeleteContentVersionFilesFs(CMVersionModel Version)
        {
            string pathType = Version.id_PathType;
            try
            {
                switch (pathType)
                {
                    case "Rel":
                        CMFileSystemUpdaterBLL.DeleteFolder(CMTreeNodeBLL.getRootPath() + "\\" + Version.Path.Name);
                        break;
                    case "Full":
                        CMFileSystemUpdaterBLL.DeleteFolder(Version.Path.Name);
                        break;
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
            }
        }

        #endregion

        #region Delete Folder

        public static void DeleteFolder(string folderFullPath)
        {
            try
            {
                if (Directory.Exists(folderFullPath))
                {
                    DirectoryClearReadOnlyAttributes(folderFullPath);
                    Directory.Delete(folderFullPath, true);
                }
            }
            catch (Exception e)
            {
                throw new TraceException("Delete folder recursive", new List<string>() { folderFullPath, e.Message }, new StackFrame(1, true), e, "Content manager");
            }
        }

        #endregion

        #region Directory Clear Read Only Attributes

        public static void DirectoryClearReadOnlyAttributes(string currentDirectory)
        {
            string[] subFiles = Directory.GetFiles(currentDirectory);
            string[] subDirectorys = Directory.GetDirectories(currentDirectory);

            foreach (string file in subFiles)
            {
                FileAttributes attrib = File.GetAttributes(file);

                if ((attrib & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    File.SetAttributes(file, attrib & ~FileAttributes.ReadOnly);
            }

            foreach (string directory in subDirectorys)
                DirectoryClearReadOnlyAttributes(directory);
        }

        #endregion

        #region Update Version Control Fields

        public static void UpdateControlFields(ref CMVersionModel v)
        {
            v.LastUpdateApplication = "Content manager";
            v.LastUpdateComputer = Domain.Workstn;
            v.LastUpdateUser = Domain.User;
            v.LastUpdateTime = DateTime.Now;
        }

        #endregion

        #region Update sub Version Control Fields

        public static void UpdateControlFields(ref CMContentVersionSubVersionModel v)
        {
            v.LastUpdateApplication = "Content manager";
            v.LastUpdateComputer = Domain.Workstn;
            v.LastUpdateUser = Domain.User;
            v.LastUpdateTime = DateTime.Now;
        }

        #endregion

        #region Update file Control Fields

        public static void UpdateControlFields(ref CMContentFileModel f)
        {
            f.LastUpdateApplication = "Content manager";
            f.LastUpdateComputer = Domain.Workstn;
            f.LastUpdateUser = Domain.User;
            f.LastUpdateTime = DateTime.Now;
        }

        #endregion

        #region Compare Update Time

        public static bool CompareUpdateTime(DateTime LastUpdateTime, long currVersionId)
        {
            try
            {
                DateTime updateDate;

                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("SELECT CV_LastUpdateTime FROM ContentVersion WHERE CV_ID = " + currVersionId);
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null && ResTable.Rows.Count == 1)
                {
                    DataRow DataRow = ResTable.Rows[0];
                    updateDate = (DateTime)DataRow["CV_LastUpdateTime"];
                    if (LastUpdateTime.Year != updateDate.Year ||
                    LastUpdateTime.Month != updateDate.Month ||
                    LastUpdateTime.Day != updateDate.Day ||
                    LastUpdateTime.Hour != updateDate.Hour ||
                    LastUpdateTime.Minute != updateDate.Minute ||
                    LastUpdateTime.Second != updateDate.Second)
                        return false;
                    //  throw new TraceException("Version changed", null, "Content manager");
                }
                return true;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return false;
            }
        }

        #endregion

        #region Get all version sub-versions

        public static List<int> GetVersionAllSubVersions(List<int> versionIds)
        {
            List<int> linkedVersionIds = new List<int>();
            try
            {
                var SB = new StringBuilder(string.Empty);
                SB.Clear();
                string strVersions = string.Join(",", versionIds);

                SB.Append("with SubTree as ");
                SB.Append("( ");
                SB.Append("select CVVL_id_ContentVersion_Link ");
                SB.Append("from dbo.ContentVersionVersionLink  ");
                SB.Append("where CVVL_id_ContentVersion_Parent in (" + strVersions + ") ");
                SB.Append("union all  ");
                SB.Append("select R.CVVL_id_ContentVersion_Link ");
                SB.Append("from ContentVersionVersionLink R  ");
                SB.Append("inner join SubTree T ");
                SB.Append("on (R.CVVL_id_ContentVersion_Parent = T.CVVL_id_ContentVersion_Link) ");
                SB.Append(") ");
                SB.Append("select * from SubTree ");

                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);

                if (ResTable == null)
                {
                    throw new Exception("Failed to retrieve list of linked content versions. Verify there is no loop in liked versions ");
                }

                foreach (DataRow row in ResTable.Rows)
                {
                    linkedVersionIds.Add(Convert.ToInt32(row["CVVL_id_ContentVersion_Link"]));
                }

                return linkedVersionIds;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("Failed to retrieve list of linked content versions. Verify there is no loop in liked versions ");
            }

        }

        #endregion

        #region Performance#1

        public static void GetListOfUsedContentVersionsCM(ref List<int> listOfUsedContentVersionsCM)
        {
            try
            {
                string qry = "select distinct CVVL_id_ContentVersion_Link from ContentVersionVersionLink";
                DataTable contentVersions = Domain.PersistenceLayer.FetchDataTable(qry.ToString(), CommandType.Text, null);
                if (contentVersions != null)
                {
                    foreach (DataRow DataRow in contentVersions.Rows)
                    {
                        int cvID = Convert.ToInt32(DataRow.ItemArray[0]);
                        listOfUsedContentVersionsCM.Add(cvID);
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

        #endregion

        #region Performance 3474

        public static ObservableCollection<CMWhereUsedContentLinkItemModel> GetListOfWhereUsedByVersionId(int contentVersionId)
        {
            ObservableCollection<CMWhereUsedContentLinkItemModel>  listOfWhereUsedContentVersionsCM = new ObservableCollection<CMWhereUsedContentLinkItemModel>();
            try
            {
                var SB = new StringBuilder(string.Empty);
                SB.Append("select distinct co_Name, CV_Name from dbo.ContentVersionVersionLink "); 
                SB.Append("join ContentVersion on CVVL_id_ContentVersion_Parent = CV_ID ");
                SB.Append("join Content on CV_id_Content = CO_ID ");
                SB.Append("where CVVL_id_ContentVersion_Link = " + Convert.ToString(contentVersionId));

                DataTable contentVersions = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                if (contentVersions != null)
                {
                    foreach (DataRow DataRow in contentVersions.Rows)
                    {
                        CMWhereUsedContentLinkItemModel tempV = new CMWhereUsedContentLinkItemModel();
                        tempV.ContentName = (string)DataRow["co_Name"];
                        tempV.VersionName = (string)DataRow["CV_Name"];
                        listOfWhereUsedContentVersionsCM.Add(tempV);
                    }
                }
                return listOfWhereUsedContentVersionsCM;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("Failed to get list of content versions");
            }
        }

        #endregion

        #region Get DataTables

        public static DataTable GetContentVersionVersionLinksDataTable(List<int> versionIds)
        {
            try
            {
                // Build The Query String
                string strVersions = string.Join(",", versionIds);
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("Select * FROM ContentVersionVersionLink  ");
                QryStr.Append("WHERE CVVL_id_ContentVersion_Link in (" + strVersions + ") ");
                string Qry = QryStr.ToString();
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null)
                {
                    return ResTable;
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

        public static DataTable GetContentVersionFileDataTable(List<int> versionIds)
        {
            try
            {
                // Build The Query String
                string strVersions = string.Join(",", versionIds);
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("Select * FROM ContentVersionFile  ");
                QryStr.Append("WHERE CVF_id_ContentVersion in (" + strVersions + ") ");
                string Qry = QryStr.ToString();
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null)
                {
                    return ResTable;
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

        public static DataTable GetContentVersionDataTable(List<int> versionIds)
        {
            try
            {
                // Build The Query String
                string strVersions = string.Join(",", versionIds);
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("Select * FROM ContentVersion  ");
                QryStr.Append("WHERE CV_ID in (" + strVersions + ") ");
                string Qry = QryStr.ToString();
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null)
                {
                    return ResTable;
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

        #region Import

        public static bool VersionExists(long contentId, string versionName)
        {
            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                SB.Clear();
                SB.Append("SELECT COUNT(CV_ID) FROM ContentVersion WHERE CV_Name = '" + versionName + "' AND CV_id_Content = " + contentId);

                // Fetch the DataTable from the database
                Int16 Count = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));

                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);

                if (Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("Failed to retrieve version data."); ;
            }
        }

        public static CMVersionModel GetVersionRow(long contentId, string versionName)
        {
            // Initialize work fields
            CMVersionModel Node = new CMVersionModel();
            try
            {
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("Select * FROM ContentVersion WHERE CV_Name = '" + versionName + "' AND CV_id_Content = " + contentId);
                string Qry = QryStr.ToString();
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null && ResTable.Rows.Count == 1)
                {
                    DataRow DataRow = ResTable.Rows[0];

                    Node.Id = (int)DataRow["CV_ID"];
                    Node.Name = (string)DataRow["CV_Name"];
                    Node.Description = (string)DataRow["CV_Description"];
                    Node.ECR = (string)DataRow["CV_ECR"];
                    Node.id_ContentVersionStatus = (string)DataRow["CV_id_ContentVersionStatus"];
                    Node.DocumentID = (string)DataRow["CV_DocumentID"];

                    if (DataRow["CV_PDMDocVersion"] == DBNull.Value)
                    {
                        Node.PDMDocumentVersion = "";
                    }
                    else
                    {
                        Node.PDMDocumentVersion = (string)DataRow["CV_PDMDocVersion"];
                    }

                    if (DataRow["CV_ConfManagementLink"] == DBNull.Value)
                    {
                        Node.ConfigurationManagementLink = "";
                    }
                    else
                    {
                        Node.ConfigurationManagementLink = (string)DataRow["CV_ConfManagementLink"];
                    }

                    Node.id_Content = (int)DataRow["CV_id_Content"];
                    Node.Path.Name = (string)DataRow["CV_Path"];
                    Node.ChildNumber = (int)DataRow["CV_ChildNumber"];
                    Node.id_PathType = (string)DataRow["CV_id_PathType"];
                    Node.CommandLine = (string)DataRow["CV_CommandLine"];
                    Node.LockWithDescription = (string)DataRow["CV_LockWithDescription"];
                    Node.LastUpdateTime = (DateTime)DataRow["CV_LastUpdateTime"];
                    Node.LastUpdateUser = (string)DataRow["CV_LastUpdateUser"];
                    Node.LastUpdateComputer = (string)DataRow["CV_LastUpdateComputer"];
                    Node.LastUpdateApplication = (string)DataRow["CV_LastUpdateApplication"];

                    if (DataRow["CV_CreationDate"] == DBNull.Value)
                    {
                        Node.CreationDate = DateTime.MinValue;
                    }
                    else
                    {
                        Node.CreationDate = (DateTime)DataRow["CV_CreationDate"];
                    }

                    GetVersiontStatus(ref Node); //get version status
                }
                return Node;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return Node;
            }
        }

        public static Domain.ErrorHandling CheckVersionsLinkedProject(CMVersionModel Version1, CMVersionModel Version2)
        {
            Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try 
	        {	        
		        Dictionary<int, HierarchyModel> contentVersion1Projects = ApiBLL.GetProjectsByContentVersionID((int)Version1.Id);

                Dictionary<int, HierarchyModel> contentVersion2Projects = ApiBLL.GetProjectsByContentVersionID((int)Version2.Id);
                foreach (int pKey in contentVersion1Projects.Keys)
                {
                    if (contentVersion2Projects.ContainsKey(pKey))
                    {
                        foreach (var version in contentVersion2Projects[pKey].GetAllVersions)
                        {
                            if (contentVersion1Projects[pKey].GetAllVersions.Contains(version))
                            { 
                                Status.messsageId = "229";
                                Status.messageParams[0] = Version1.VersionName;
                                Status.messageParams[1] = Version1.id_Content;
                                Status.messageParams[2] = Version2.VersionName;
                                Status.messageParams[3] = Version2.id_Content;
                                Status.messageParams[4] = contentVersion1Projects[pKey].TreeHeader;
                                Status.messageParams[5] = version.VersionName;
                            }
                        }
                    }
                }
                return Status;
	        }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Status.messsageId = "105";
                return Status;
            }
        }

        public static int GetVersionIdByNames(string contentName, string versionName)
        {
            // Initialize work fields
            int id = 0;
            try
            {
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();               
                QryStr.Append("select CV_ID from dbo.ContentVersion ");
                QryStr.Append("join dbo.Content ");
                QryStr.Append("on CV_id_Content=CO_ID ");
                QryStr.Append("where CV_Name= '" + versionName + "' AND CO_Name = '" + contentName + "'");
                
                
                string Qry = QryStr.ToString();
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null && ResTable.Rows.Count >= 1)
                {
                    DataRow DataRow = ResTable.Rows[0];
                    id = (int)DataRow["CV_ID"];
                }
                return id;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return id;
            }
        }

        #endregion

        #region Generate default version name

        public static string GenerateDefaultVersionName(long parentId)
        {
            string defaultVersionName = string.Empty;
            decimal defaultVersionNameNumeric = 0; 

            string versionNamePrefix = string.Empty;
            string strFirstNewVersionName = string.Empty;
            string strVersionNameIncrement = string.Empty;
            string strVersionIncrementPrecision = string.Empty;

            decimal firstNewVersionName = 0;
            decimal versionNameIncrement = 0;

            try
            {
                string[] namingConvensionSettings = CMFolderBLL.GetNamingConvensionSettings(parentId);

                versionNamePrefix = namingConvensionSettings[0];

                try
                {
                    strFirstNewVersionName = namingConvensionSettings[1];
                    strFirstNewVersionName = strFirstNewVersionName.Replace(',', '.');
                    firstNewVersionName = Convert.ToDecimal(strFirstNewVersionName);
                }
                catch (Exception e)
                {
                    String logMessage = "\nFailed to generate default version name." +
                     "\nPlease check parent folder default version naming properties.";
                    Domain.SaveGeneralWarningLog(logMessage);
                    return defaultVersionName;
                }

                try
                {
                    strVersionNameIncrement = namingConvensionSettings[2];
                    strVersionNameIncrement = strVersionNameIncrement.Replace(',', '.');
                    versionNameIncrement = Convert.ToDecimal(strVersionNameIncrement);
                    if (strVersionNameIncrement.Split('.')[0].Length != strVersionNameIncrement.Length)//with precision
                    {
                        strVersionIncrementPrecision = strVersionNameIncrement.Split('.')[1];
                    }
                }
                catch (Exception e)
                {
                    String logMessage = "\nFailed to generate default version name." +
                     "\nPlease check parent folder default version naming properties.";
                    ATSDomain.Domain.SaveGeneralWarningLog(logMessage);
                    return defaultVersionName;
                }

                if (parentId > 0)
                {
                    string latestVersionName = GetLastVersionName((int)parentId);
                    if (latestVersionName != null)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(versionNamePrefix) && !string.IsNullOrWhiteSpace(versionNamePrefix))
                            {
                                string latestVersionNamePrefix = latestVersionName.Substring(0, versionNamePrefix.Length);
                                if (latestVersionNamePrefix == versionNamePrefix)
                                {
                                    decimal latestVersionNameNumeric = Convert.ToDecimal(latestVersionName.Substring(versionNamePrefix.Length));
                                    decimal div = latestVersionNameNumeric / versionNameIncrement;
                                    decimal rem = div - Convert.ToInt32(div);
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
                                decimal latestVersionNameNumeric = Convert.ToDecimal(latestVersionName);
                                decimal div = latestVersionNameNumeric / versionNameIncrement;
                                decimal rem = div - Convert.ToInt32(div);
                                if (rem == 0) //name is according to the policy
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

                    int versionsCount = GetVersionsCountForAddVersion(parentId);
                    defaultVersionNameNumeric = firstNewVersionName + versionsCount * versionNameIncrement;
                    defaultVersionName = versionNamePrefix + defaultVersionNameNumeric.ToString();
                }
                defaultVersionName = AddTrailingZeros(defaultVersionName, strVersionIncrementPrecision);
                return defaultVersionName;
            }
            catch (Exception e)
            {
                String logMessage = "\nFailed to generate default version name";
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

        public static int CountVersionFiles(List<int> versionIds)
        {
            int cntFiles = 0;
            if (versionIds != null && versionIds.Count > 0)
            {
                string strVersions = string.Join(",", versionIds);
                string query = "Select count(*) from ContentVersionFile where CVF_id_ContentVersion in (" + strVersions + ")";

                object cnt = Domain.PersistenceLayer.FetchDataValue(query.ToString(), System.Data.CommandType.Text, null);

                if (cnt != null)
                {
                    cntFiles = Convert.ToInt32(cnt);
                }
                return cntFiles;            
            }
            return 0;
        }

        //public static Dictionary<int, CMFolderModel> outFolders { get; set; }
    }
}
