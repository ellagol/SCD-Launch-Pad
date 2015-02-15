using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using ATSBusinessObjects.ContentMgmtModels;
using ATSDomain;
using Infra.DAL;
using TraceExceptionWrapper;


namespace ATSBusinessLogic.ContentMgmtBLL
{
    public class CMContentBLL
    {
        #region Retrieve Content Row from database and return as CMContentModel

        public static CMContentModel GetContentRow(long ContentId)
        {
            // Initialize work fields
            CMContentModel Node = new CMContentModel();
            string cerFree;

            try
            {
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("Select * FROM Content WHERE CO_ID = " + ContentId);
                string Qry = QryStr.ToString();
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null && ResTable.Rows.Count == 1)
                {
                    DataRow DataRow = ResTable.Rows[0];
                    //
                    Node.Id = (int)DataRow["CO_ID"];
                    Node.Name = (string)DataRow["CO_Name"];
                    Node.IconPath = (string)DataRow["CO_Icon"];
                    Node.Description = (string)DataRow["CO_Description"];
                    Node.Id_ContentTree = (int)DataRow["CO_id_ContentTree"];
                    Node.ChildNumber = (int)DataRow["CO_ChildNumber"];
                    Node.Id_ContentType = (string)DataRow["CO_id_ContentType"];
                    if (DataRow["CO_ExtATRInd"] == DBNull.Value)
                    {
                        Node.ATRInd = false;
                    }
                    else
                    {
                        Node.ATRInd = Convert.ToBoolean(DataRow["CO_ExtATRInd"]);
                    }

                    if (DataRow["CO_CreationDate"] == DBNull.Value)
                    {
                        Node.CreationDate = DateTime.MinValue;             
                    }
                    else
                    {
                        Node.CreationDate = (DateTime)DataRow["CO_CreationDate"];
                    }

                    cerFree = (string)DataRow["CO_CertificateFree"];
                    switch (cerFree)
                    {
                        case "YES":
                            Node.CertificateFree = true;
                            break;
                        case "NOT":
                            Node.CertificateFree = false;
                            break;
                    }

                    Node.LastUpdateTime = (DateTime)DataRow["CO_LastUpdateTime"];
                    Node.LastUpdateUser = (string)DataRow["CO_LastUpdateUser"];
                    Node.LastUpdateComputer = (string)DataRow["CO_LastUpdateComputer"];
                    Node.LastUpdateApplication = (string)DataRow["CO_LastUpdateApplication"];

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

        #region Retrieve Content Id

        public static int GetContentId(string ContentName)
        {
            // Initialize work fields
            CMContentModel Node = new CMContentModel();
            try
            {
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("SELECT CO_ID FROM Content WHERE CO_Name = '" + ContentName + "' ");
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null && ResTable.Rows.Count == 1)
                {
                    DataRow DataRow = ResTable.Rows[0];
                    return (int)DataRow["CO_ID"];
                }
                else
                {
                    throw new TraceException("Content deleted", null, "Content manager");
                }

            }
            catch (Exception)
            {
                throw new TraceException("Content deleted", null, "Content manager");
            }
        }

        #endregion

        #region Get Content Name

        public static String GetContentName(int contentID)
        {
            try
            {
                var SB = new StringBuilder(string.Empty);

                SB.Append("SELECT CO_Name FROM Content WHERE CO_ID = '" + contentID + "' ");

                // Fetch the DataTable from the database
                DataTable ResTbl = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTbl != null)
                {
                    DataRow DataRow = ResTbl.Rows[0];
                    return (string)DataRow["CO_Name"];
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

        #region Get Content Type List

        public static List<KeyValuePair<string, string>> GetContentTypeList()
        {
            try
            {
                List<KeyValuePair<string, string>> GetContentTypeList = new List<KeyValuePair<string, string>>();
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Clear();
                QryStr.Append("Select CTY_ID, CTY_Name FROM ContentType");
                string Qry = QryStr.ToString();
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null)
                {
                    foreach (DataRow DataRow in ResTable.Rows)
                    {
                        KeyValuePair<string, string> TypeKeyValue = new KeyValuePair<string, string>((string)DataRow["CTY_ID"], (string)DataRow["CTY_Name"]);
                        GetContentTypeList.Add(TypeKeyValue);
                    }
                }
                return GetContentTypeList;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return null;
            }
        }

        #endregion

        #region Delete Content

        public static long DeleteContent(long ContentId)
        {
            CMContentModel Content = GetContentRow(ContentId);
            // Work Fields
            long RV = 0;
            var SB = new StringBuilder(string.Empty);
            List<ParamStruct> CommandParams;
            // Build the Query
            SB.Append("DELETE FROM Content WHERE CO_ID=@CO_ID");
            // Set the parameters
            CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "CO_ID", DataType = DbType.Int32, Value = ContentId }
                };
            // Execute the query
            RV = (long)Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
            // Finalize
            if (RV < 1) // Something went wrong... No rows were affected
            {
                throw new TraceException("Content deleted", null, "Content manager");
            }
            else // All OK
            {
                DeleteContentFiles(Content);
                return RV;
            }
        }

        #endregion

        #region Get Content Child Count

        public static long GetContentChildCount(long contentId)
        {
            // Work Fields
            int subVersions;
            var SB = new StringBuilder(string.Empty);

            // Build the Query
            SB.Append("SELECT COUNT(CV_ID) FROM ContentVersion WHERE CV_id_Content ='" + contentId + "'");

            // Execute the query
            subVersions = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));

            if (subVersions > 0)
                throw new TraceException("Content delete with subitems", null, "Content manager");

            return 1;
        }

        #endregion

        #region Clone Content

        public static CMContentModel CloneContent(CMContentModel CM)
        {
            try
            {
                // Work fields
                CMContentModel c = new CMContentModel();
                var SB = new StringBuilder(string.Empty);
                List<ParamStruct> CommandParams;
                Boolean DatabaseSupportsBatchQueries = Domain.PersistenceLayer.GetSupportsBatchQueries();

                c.Name = CM.Name;
                c.Id_ContentTree = CM.Id_ContentTree;
                c.IconPath = CM.IconPath;
                c.Id_ContentType = CM.Id_ContentType;
                c.Description = CM.Description;
                c.CertificateFree = CM.CertificateFree;
                c.ChildNumber = CM.ChildNumber;
                c.ATRInd = CM.ATRInd;
                c.CreationDate = DateTime.Now;

                string CertificateFree;
                if (c.CertificateFree == true)
                {
                    CertificateFree = "YES";
                }
                else
                {
                    CertificateFree = "NOT";
                }

                UpdateControlFields(ref c);

                SB.Clear();
                // Build the Query
                SB.Append("INSERT INTO Content (CO_Name, CO_id_ContentTree, CO_Icon, CO_id_ContentType, CO_CertificateFree, CO_ExtATRInd, CO_ChildNumber, CO_Description, CO_LastUpdateUser, CO_LastUpdateTime, CO_LastUpdateComputer, CO_LastUpdateApplication, CO_CreationDate) ");
                SB.Append("VALUES (@CO_Name, @CO_id_ContentTree, @CO_Icon, @CO_id_ContentType, @CO_CertificateFree, @CO_ExtATRInd, @CO_ChildNumber, @CO_Description, @CO_LastUpdateUser, @CO_LastUpdateTime, @CO_LastUpdateComputer, @CO_LastUpdateApplication, @CO_CreationDate) ");
                if (DatabaseSupportsBatchQueries)
                {
                    SB.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                }

                // Set the parameters
                CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "CO_ID", DataType = DbType.Int32, Value = c.Id },
                new ParamStruct { ParamName = "CO_Name", DataType = DbType.String, Value = c.Name },
                new ParamStruct { ParamName = "CO_id_ContentTree", DataType = DbType.Int32, Value = c.Id_ContentTree },
                new ParamStruct { ParamName = "CO_Icon", DataType = DbType.String, Value = c.IconPath },
                new ParamStruct { ParamName = "CO_id_ContentType", DataType = DbType.String, Value = c.Id_ContentType },
                new ParamStruct { ParamName = "CO_CertificateFree", DataType = DbType.String, Value = CertificateFree },
                new ParamStruct { ParamName = "CO_ExtATRInd", DataType = DbType.String, Value = c.ATRInd },
                new ParamStruct { ParamName = "CO_ChildNumber", DataType = DbType.Int32, Value = c.ChildNumber},
                new ParamStruct { ParamName = "CO_Description", DataType = DbType.String, Value = c.Description },
                new ParamStruct { ParamName = "CO_LastUpdateUser", DataType = DbType.String, Value = c.LastUpdateUser},   
                new ParamStruct { ParamName = "CO_LastUpdateTime", DataType = DbType.DateTime, Value = c.LastUpdateTime },
                new ParamStruct { ParamName = "CO_LastUpdateComputer", DataType = DbType.String, Value = c.LastUpdateComputer },
                new ParamStruct { ParamName = "CO_LastUpdateApplication", DataType = DbType.String, Value = c.LastUpdateApplication },
                new ParamStruct { ParamName = "CO_CreationDate", DataType = DbType.DateTime, Value = c.CreationDate }
                };

                //Execute the query
                object RV = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());

                if (RV != null)
                {
                    c.Id = Convert.ToInt64(RV);
                    return c;
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

        #region Delete Content Files

        public static void DeleteContentFiles(CMContentModel Content)
        {
            try
            {

                string folderFullPath = Path.GetDirectoryName(Content.IconPath);

                CMFileSystemUpdaterBLL.DeleteFile(Content.IconPath);
                CMFileSystemUpdaterBLL.DeleteFolder(folderFullPath);
            }
            catch(Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
            }
        }

        #endregion

        #region Add New Content

        public static long AddNewContent(ref CMContentModel c)
        {
            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                List<ParamStruct> CommandParams;
                Boolean DatabaseSupportsBatchQueries = Domain.PersistenceLayer.GetSupportsBatchQueries();

                UpdateControlFields(ref c);
                c.CreationDate = DateTime.Now;

                int tempChildNum = 0;
                c.ChildNumber = CMFolderBLL.GetChildIDForAddFolder(c.Id_ContentTree);
                tempChildNum = GetChildIDForAddContent(c.Id_ContentTree);

                if (tempChildNum > c.ChildNumber)  //take the largest child number
                    c.ChildNumber = tempChildNum;

                switch (c.Id_ContentType)
                {
                    case "AN":
                        c.Id_ContentType = "AN   ";
                        break;
                    case "FW":
                        c.Id_ContentType = "FW   ";
                        break;
                    case "GA":
                        c.Id_ContentType = "GA   ";
                        break;
                    case "ME":
                        c.Id_ContentType = "ME   ";
                        break;
                    case "SW":
                        c.Id_ContentType = "SW   ";
                        break;
                }
                
                string CertificateFree;
                if (c.CertificateFree == true)
                {
                    CertificateFree = "YES";
                }
                else
                {
                    CertificateFree = "NOT";
                }
     
                SB.Clear();
                // Build the Query
                SB.Append("INSERT INTO Content (CO_Name, CO_id_ContentTree, CO_Icon, CO_id_ContentType, CO_CertificateFree, CO_ExtATRInd, CO_ChildNumber, CO_Description, CO_LastUpdateUser, CO_LastUpdateTime, CO_LastUpdateComputer, CO_LastUpdateApplication, CO_CreationDate) ");
                SB.Append("VALUES (@CO_Name, @CO_id_ContentTree, @CO_Icon, @CO_id_ContentType, @CO_CertificateFree, @CO_ExtATRInd, @CO_ChildNumber, @CO_Description, @CO_LastUpdateUser, @CO_LastUpdateTime, @CO_LastUpdateComputer, @CO_LastUpdateApplication, @CO_CreationDate) ");
                if (DatabaseSupportsBatchQueries)
                {
                    SB.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                }

                // Set the parameters
                CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "CO_ID", DataType = DbType.Int32, Value = c.Id },
                new ParamStruct { ParamName = "CO_Name", DataType = DbType.String, Value = c.Name },
                new ParamStruct { ParamName = "CO_id_ContentTree", DataType = DbType.Int32, Value = c.Id_ContentTree },
                new ParamStruct { ParamName = "CO_Icon", DataType = DbType.String, Value = c.IconPath },
                new ParamStruct { ParamName = "CO_id_ContentType", DataType = DbType.String, Value = c.Id_ContentType },
                new ParamStruct { ParamName = "CO_CertificateFree", DataType = DbType.String, Value = CertificateFree },
                new ParamStruct { ParamName = "CO_ExtATRInd", DataType = DbType.String, Value = c.ATRInd },
                new ParamStruct { ParamName = "CO_ChildNumber", DataType = DbType.Int32, Value = c.ChildNumber},
                new ParamStruct { ParamName = "CO_Description", DataType = DbType.String, Value = c.Description },
                new ParamStruct { ParamName = "CO_LastUpdateUser", DataType = DbType.String, Value = c.LastUpdateUser},   
                new ParamStruct { ParamName = "CO_LastUpdateTime", DataType = DbType.DateTime, Value = c.LastUpdateTime },
                new ParamStruct { ParamName = "CO_LastUpdateComputer", DataType = DbType.String, Value = c.LastUpdateComputer },
                new ParamStruct { ParamName = "CO_LastUpdateApplication", DataType = DbType.String, Value = c.LastUpdateApplication },
                new ParamStruct { ParamName = "CO_CreationDate", DataType = DbType.DateTime, Value = c.CreationDate }
                };

                //Execute the query
                object RV = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());

                if (RV != null)
                {
                    c.Id = Convert.ToInt64(RV);
                    return c.Id;
                }
                else
                {
                    throw new TraceException("Parent folder deleted", null, "Content manager");
                }
            }
            catch (Exception e)
            {
                throw new TraceException("Parent folder deleted", null, "Content manager");
            }
        }

        #endregion

        #region Get Child ID For Add Content

        public static int GetChildIDForAddContent(long folderOrContentId)
        {
            // Initialize work fields           
            int lastChildID = 0;

            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                SB.Clear();
                SB.Append("SELECT MAX(CO_ChildNumber) FROM Content WHERE CO_id_ContentTree = " + folderOrContentId);
                // Fetch the DataTable from the database
                object objMax = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null);

                Int16 max = -1;
                if (objMax == DBNull.Value)
                {
                    max = -1;
                }
                else
                {
                    max = Convert.ToInt16(objMax);
                }

                if (max > 0)
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

        #endregion

        #region Check Content Name

        public static string CheckContentName(ref CMContentModel c)
        {
            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                SB.Clear();
                SB.Append("SELECT Count(CO_ID) FROM Content WHERE CO_Name = '" + c.Name.ToString().Trim() + "'");

                // Fetch the DataTable from the database
                Int16 Count = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));

                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);

                if (Count > 0)
                {
                    return "Adding existing content";
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

        #region Generate Content Name

        public static string GenerateContentName(ref CMContentModel content)
        {
            try
            {
                // Work fields
                int index = 0;
                var SB = new StringBuilder(string.Empty);
                SB.Clear();
                SB.Append("SELECT COUNT(CO_ID) FROM Content where CO_Name = '" + content.Name.ToString() + " ' ");
                // Fetch the DataTable from the database
                Int16 res = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));

                if (res == 0)
                {
                    return content.Name;
                }
                else
                {
                    do
                    {
                        index++;
                        SB.Clear();
                        SB.Append("SELECT COUNT(CO_ID) FROM Content where CO_Name = '" + content.Name.ToString() + index + " ' ");
                        res = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));

                    } while (res > 0);

                    content.Name = content.Name + index;
                }
                return content.Name;
            }
            catch (Exception e) //there is no child's for folder parent
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return null;
            }
        }

        #endregion

        #region Check Existing Content Name

        public static string CheckExistingContentName(ref CMContentModel c)
        {
            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                SB.Clear();
                SB.Append("SELECT COUNT(CO_ID) FROM Content WHERE CO_Name = '" + c.Name.ToString() + "' AND CO_ID <> " + c.Id);

                // Fetch the DataTable from the database
                Int16 Count = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));

                if (Count > 0)
                {
                    return "Update content name to existing";
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

        #region Update Existing Content

        public static long UpdateExistingContent(ref CMContentModel c)
        {
            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                List<ParamStruct> CommandParams;

                UpdateControlFields(ref c); // Update Control fields

                switch (c.Id_ContentType)
                {
                    case "AN":
                        c.Id_ContentType = "AN   ";
                        break;
                    case "FW":
                        c.Id_ContentType = "FW   ";
                        break;
                    case "GA":
                        c.Id_ContentType = "GA   ";
                        break;
                    case "ME":
                        c.Id_ContentType = "ME   ";
                        break;
                    case "SW":
                        c.Id_ContentType = "SW   ";
                        break;
                }

                string CertificateFree;
                if (c.CertificateFree == true)
                {
                    CertificateFree = "YES";
                }
                else
                {
                    CertificateFree = "NOT";
                }

                // Build the Query
                SB.Clear();
                SB.Append("UPDATE Content SET CO_LastUpdateUser=@CO_LastUpdateUser, ");
                SB.Append("CO_LastUpdateComputer=@CO_LastUpdateComputer,CO_LastUpdateApplication=@CO_LastUpdateApplication, ");
                SB.Append("CO_LastUpdateTime=@CO_LastUpdateTime, CO_Description=@CO_Description, ");
                SB.Append("CO_CertificateFree=@CO_CertificateFree, CO_ExtATRInd=@CO_ExtATRInd, CO_Name=@CO_Name, ");
                //removed co_childnumber
                SB.Append("CO_Icon=@CO_Icon, CO_id_ContentType=@CO_id_ContentType, CO_id_ContentTree=@CO_id_ContentTree ");
                SB.Append("WHERE CO_ID=@CO_ID");

                // Set the parameters
                CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "CO_ID", DataType = DbType.Int32, Value = c.Id },
                new ParamStruct { ParamName = "CO_Name", DataType = DbType.String, Value = c.Name },
                new ParamStruct { ParamName = "CO_CertificateFree", DataType = DbType.String, Value = CertificateFree },                   
                new ParamStruct { ParamName = "CO_ExtATRInd", DataType = DbType.String, Value = c.ATRInd },  
                new ParamStruct { ParamName = "CO_Description", DataType = DbType.String, Value = c.Description },
                //new ParamStruct { ParamName = "CO_ChildNumber", DataType = DbType.Int32, Value = c.ChildNumber},
                new ParamStruct { ParamName = "CO_Icon", DataType = DbType.String, Value = c.IconPath },
                new ParamStruct { ParamName = "CO_id_ContentType", DataType = DbType.String, Value = c.Id_ContentType },
                new ParamStruct { ParamName = "CO_id_ContentTree", DataType = DbType.Int32, Value = c.Id_ContentTree },
                new ParamStruct { ParamName = "CO_LastUpdateTime", DataType = DbType.DateTime, Value = c.LastUpdateTime },
                new ParamStruct { ParamName = "CO_LastUpdateUser", DataType = DbType.String, Value = c.LastUpdateUser },
                new ParamStruct { ParamName = "CO_LastUpdateComputer", DataType = DbType.String, Value = c.LastUpdateComputer },
                new ParamStruct { ParamName = "CO_LastUpdateApplication", DataType = DbType.String, Value = c.LastUpdateApplication }
                };
                //Execute the query
                object RV = Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());

                if (RV != null)
                {
                    if (Convert.ToInt32(RV) > 0)  //if content exists
                    {
                        return c.Id;
                    }
                    else
                    {
                        throw new TraceException("Content deleted", null, "Content manager");
                    }                  
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception)
            {
                throw new TraceException("Content deleted", null, "Content manager");
            }
        }

        #endregion

        #region Add Content Icon On Fs

        public static void AddContentIconOnFs(ref CMContentModel c, CMImpersonationBLL imp)
        {
            if (c.IconPath == "")
                return;

            string relativePath;
            CMDoubleCopyFileModel dbFile = new CMDoubleCopyFileModel();
            List<CMDoubleCopyFileModel> copyFiles = new List<CMDoubleCopyFileModel> { dbFile };

            CMFileSystemUpdaterBLL.DeleteFolder(CMFileSystemUpdaterBLL.getCmFolder());
            CMFileSystemUpdaterBLL.CreateDirectory(CMFileSystemUpdaterBLL.getCmFolder());

            dbFile.SourceFileFullPath = c.IconPath;
            dbFile.DestinationDirectoryPath = "%Root%";
            dbFile.DestinationFileFullPath = dbFile.DestinationDirectoryPath + "\\" + Path.GetFileName(c.IconPath);
            dbFile.LocalCopyDirectoryPath = CMFileSystemUpdaterBLL.getCmFolder();
            dbFile.LocalCopyFullPath = dbFile.LocalCopyDirectoryPath + "\\" + Path.GetFileName(c.IconPath);

            CMFileSystemUpdaterBLL.AddFilesOnFs(c, out relativePath, copyFiles, null, imp);
            c.IconPath = dbFile.DestinationFileFullPath;
        }

        #endregion

        #region Update Content Icon On Fs

        public static void UpdateContentIconOnFs(ref CMContentModel contentUpdated, ref CMContentModel contentOriginal, CMImpersonationBLL imp)
        {
            if (contentUpdated.IconPath == "" ||
               contentUpdated.IconPath == contentOriginal.IconPath)
                return;

            string relativePath;
            CMDoubleCopyFileModel dbFile = new CMDoubleCopyFileModel();
            List<CMDoubleCopyFileModel> copyFiles = new List<CMDoubleCopyFileModel> { dbFile };

            CMFileSystemUpdaterBLL.DeleteFolder(CMFileSystemUpdaterBLL.getCmFolder());
            CMFileSystemUpdaterBLL.CreateDirectory(CMFileSystemUpdaterBLL.getCmFolder());

            dbFile.SourceFileFullPath = contentUpdated.IconPath;
            dbFile.DestinationDirectoryPath = "%Root%";
            dbFile.DestinationFileFullPath = dbFile.DestinationDirectoryPath + "\\" + Path.GetFileName(contentUpdated.IconPath);
            dbFile.LocalCopyDirectoryPath = CMFileSystemUpdaterBLL.getCmFolder();
            dbFile.LocalCopyFullPath = dbFile.LocalCopyDirectoryPath + "\\" + Path.GetFileName(contentUpdated.IconPath);

            CMFileSystemUpdaterBLL.AddFilesOnFs(contentUpdated, out relativePath, copyFiles, null, imp);
            contentUpdated.IconPath = dbFile.DestinationFileFullPath;
        }

        #endregion

        #region Post Update Content

        public static void PostUpdateContent(ref CMContentModel contentUpdated, ref CMContentModel contentOriginal)
        {
            if (contentUpdated.IconPath == contentOriginal.IconPath)
                return;

            String folderName = CMFileSystemUpdaterBLL.GetFileFolder(contentOriginal.IconPath);
            CMFileSystemUpdaterBLL.DeleteFolder(folderName);
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

        #region Update Content Control Fields

        public static void UpdateControlFields(ref CMContentModel c)
        {
            c.LastUpdateApplication = "Content manager";
            c.LastUpdateComputer = Domain.Workstn;
            c.LastUpdateUser = Domain.User;
            c.LastUpdateTime = DateTime.Now;
        }

        #endregion

        #region Compare Update Time

        public static bool CompareUpdateTime(DateTime LastUpdateTime, long currContentId)
        {
            DateTime updateDate;

            // Build The Query String
            System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
            QryStr.Append("SELECT CO_LastUpdateTime FROM Content WHERE CO_ID = " + currContentId);
            // Fetch the DataTable from the database
            DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
            // Populate the collection
            if (ResTable != null && ResTable.Rows.Count == 1)
            {
                DataRow DataRow = ResTable.Rows[0];
                updateDate = (DateTime)DataRow["CO_LastUpdateTime"];
                if (LastUpdateTime.Year != updateDate.Year ||
                LastUpdateTime.Month != updateDate.Month ||
                LastUpdateTime.Day != updateDate.Day ||
                LastUpdateTime.Hour != updateDate.Hour ||
                LastUpdateTime.Minute != updateDate.Minute ||
                LastUpdateTime.Second != updateDate.Second)
                    throw new TraceException("Content changed", null, "Content manager");
            }
            return true;
        }

        #endregion

        #region Get Content Ids list by Version Ids list

        public static List<int> GetContentIds(List<int> versionIds)
        {
            List<int> contentIds = new List<int>();
            try
            {
                if (versionIds == null || versionIds.Count == 0)
                {
                    return contentIds;
                }

                string strVids = string.Join(",", versionIds);
                
                var SB = new StringBuilder(string.Empty);

                SB.Append("SELECT distinct CV_id_Content FROM ContentVersion WHERE CV_ID in (" + strVids + ") ");

                // Fetch the DataTable from the database
                DataTable ResTbl = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTbl != null)
                {
                    foreach (DataRow DataRow in ResTbl.Rows)
                    {
                        contentIds.Add(Convert.ToInt32(DataRow["CV_id_Content"]));
                    }
                    return contentIds;
                }
                else
                {
                    throw new Exception("Failed to retrieve contents.");
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("Failed to retrieve contents.");
            }
        }

        #endregion

        #region Get DataTables
        public static DataTable GetContentDataTable(List<int> contentIds)
        {
            try
            {
                // Build The Query String
                string strContents = string.Join(",", contentIds);
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("Select * FROM Content ");
                QryStr.Append("WHERE CO_ID in (" + strContents + ") ");
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

        public static bool ContentNameExists(string name)
        {
            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                SB.Clear();
                SB.Append("SELECT Count(CO_ID) FROM Content WHERE CO_Name = '" + name.Trim() + "'");

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
                return false;
            }

        }

        public static CMContentModel GetContentRowByName(string name)
        {
            // Initialize work fields
            CMContentModel Node = new CMContentModel();
            string cerFree;

            try
            {
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("Select * FROM Content WHERE CO_Name = '" + name.Trim() + "'");
                string Qry = QryStr.ToString();
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null && ResTable.Rows.Count == 1)
                {
                    DataRow DataRow = ResTable.Rows[0];
                    //
                    Node.Id = (int)DataRow["CO_ID"];
                    Node.Name = (string)DataRow["CO_Name"];
                    Node.IconPath = (string)DataRow["CO_Icon"];
                    Node.Description = (string)DataRow["CO_Description"];
                    Node.Id_ContentTree = (int)DataRow["CO_id_ContentTree"];
                    Node.ChildNumber = (int)DataRow["CO_ChildNumber"];
                    Node.Id_ContentType = (string)DataRow["CO_id_ContentType"];
                    if (DataRow["CO_ExtATRInd"] == DBNull.Value)
                    {
                        Node.ATRInd = false;
                    }
                    else
                    {
                        Node.ATRInd = Convert.ToBoolean(DataRow["CO_ExtATRInd"]);
                    }

                    if (DataRow["CO_CreationDate"] == DBNull.Value)
                    {
                        Node.CreationDate = DateTime.MinValue;
                    }
                    else
                    {
                        Node.CreationDate = (DateTime)DataRow["CO_CreationDate"];
                    }

                    cerFree = (string)DataRow["CO_CertificateFree"];
                    switch (cerFree)
                    {
                        case "YES":
                            Node.CertificateFree = true;
                            break;
                        case "NOT":
                            Node.CertificateFree = false;
                            break;
                    }

                    Node.LastUpdateTime = (DateTime)DataRow["CO_LastUpdateTime"];
                    Node.LastUpdateUser = (string)DataRow["CO_LastUpdateUser"];
                    Node.LastUpdateComputer = (string)DataRow["CO_LastUpdateComputer"];
                    Node.LastUpdateApplication = (string)DataRow["CO_LastUpdateApplication"];

                }
                else if (ResTable != null && ResTable.Rows.Count > 1)
                {
                    String logMessage = "Content '" + name + "' appears more than once in Target environment. Import failed.";
                    Domain.SaveGeneralErrorLog(logMessage);
                    throw new Exception("Invalid content.");
                }
                return Node;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("Invalid content.");
            }
        }

        public static List<string> GetContentTypeKeys()
        {
            try
            {
                List<string> ContentTypeKeys = new List<string>();
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Clear();
                QryStr.Append("Select CTY_ID FROM ContentType");
                string Qry = QryStr.ToString();
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null)
                {
                    foreach (DataRow DataRow in ResTable.Rows)
                    {
                        ContentTypeKeys.Add((string)DataRow["CTY_ID"]);
                    }
                }
                return ContentTypeKeys;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return null;
            }
        }

        #region Get Content Name

        public static String GetContentNameByVersionId(int versionID)
        {
            try
            {
                var SB = new StringBuilder(string.Empty);

                SB.Append("select co_name from Content ");
                SB.Append("join ContentVersion ");
                SB.Append("on CO_ID=CV_id_Content ");
                SB.Append("where CV_ID = '" + versionID + "' ");

                // Fetch the DataTable from the database
                DataTable ResTbl = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTbl != null)
                {
                    DataRow DataRow = ResTbl.Rows[0];
                    return (string)DataRow["CO_Name"];
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

        #endregion

    }
}
