using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ATSBusinessObjects.ContentMgmtModels;
using ATSDomain;
using Infra.DAL;
using TraceExceptionWrapper;

namespace ATSBusinessLogic.ContentMgmtBLL
{
    public class CMFolderBLL
    {
        #region Retrieve Folder Row from database and return as CMFolderModel

        public static CMFolderModel GetFolderRow(long FolderId)
        {
            // Initialize work fields
            CMFolderModel Node = new CMFolderModel();
            try
            {
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("SELECT * FROM ContentTree WHERE CT_ID = " + FolderId);
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null && ResTable.Rows.Count == 1)
                {
                    DataRow DataRow = ResTable.Rows[0];
                    //
                    Node.Id = (int)DataRow["CT_ID"];
                    Node.Name = (string)DataRow["CT_Name"];
                    Node.FolderName = (string)DataRow["CT_Name"];
                    Node.Description = (string)DataRow["CT_Description"];
                    Node.ParentId = (int)DataRow["CT_ParentID"];
                    Node.ChildNumber = (int)DataRow["CT_ChildNumber"];
                    Node.LastUpdateTime = (DateTime)DataRow["CT_LastUpdateTime"];
                    Node.LastUpdateUser = (string)DataRow["CT_LastUpdateUser"];
                    Node.LastUpdateComputer = (string)DataRow["CT_LastUpdateComputer"];
                    Node.LastUpdateApplication = (string)DataRow["CT_LastUpdateApplication"];
                    if (DataRow["CT_DefaultVNIncrement"] != DBNull.Value)
                    {
                        Node.DefaultVNIncrement = (string)DataRow["CT_DefaultVNIncrement"];
                    }
                    else
                    {
                        Node.DefaultVNIncrement = string.Empty;
                    }

                    if (DataRow["CT_DefaultVNPrefix"] != DBNull.Value)
                    {
                        Node.DefaultVNPrefix = (string)DataRow["CT_DefaultVNPrefix"];
                    }
                    else
                    {
                        Node.DefaultVNPrefix = string.Empty;
                    }

                    if (DataRow["CT_DefaultVNStartValue"] != DBNull.Value)
                    {
                        Node.DefaultVNStartValue = (string)DataRow["CT_DefaultVNStartValue"];
                    }
                    else
                    {
                        Node.DefaultVNStartValue = string.Empty;
                    }
                    if (DataRow["CT_CreationDate"] == DBNull.Value)
                    {
                        Node.CreationDate = DateTime.MinValue;             
                    }
                    else
                    {
                        Node.CreationDate = (DateTime)DataRow["CT_CreationDate"];
                    }
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

        public static CMFolderModel GetFolderRowByChildContentId(long contentId)
        {
            // Initialize work fields
            CMFolderModel Node = new CMFolderModel();
            try
            {
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("select * from ContentTree ct ");
                QryStr.Append("join Content c ");
                QryStr.Append("on ct.CT_ID = c.CO_id_ContentTree ");
                QryStr.Append("where c.CO_ID = " + contentId);

                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null && ResTable.Rows.Count == 1)
                {
                    DataRow DataRow = ResTable.Rows[0];
                    //
                    Node.Id = (int)DataRow["CT_ID"];
                    Node.Name = (string)DataRow["CT_Name"];
                    Node.FolderName = (string)DataRow["CT_Name"];
                    Node.Description = (string)DataRow["CT_Description"];
                    Node.ParentId = (int)DataRow["CT_ParentID"];
                    Node.ChildNumber = (int)DataRow["CT_ChildNumber"];
                    Node.LastUpdateTime = (DateTime)DataRow["CT_LastUpdateTime"];
                    Node.LastUpdateUser = (string)DataRow["CT_LastUpdateUser"];
                    Node.LastUpdateComputer = (string)DataRow["CT_LastUpdateComputer"];
                    Node.LastUpdateApplication = (string)DataRow["CT_LastUpdateApplication"];
                    if (DataRow["CT_DefaultVNIncrement"] != DBNull.Value)
                    {
                        Node.DefaultVNIncrement = (string)DataRow["CT_DefaultVNIncrement"];
                    }
                    else
                    {
                        Node.DefaultVNIncrement = string.Empty;
                    }

                    if (DataRow["CT_DefaultVNPrefix"] != DBNull.Value)
                    {
                        Node.DefaultVNPrefix = (string)DataRow["CT_DefaultVNPrefix"];
                    }
                    else
                    {
                        Node.DefaultVNPrefix = string.Empty;
                    }

                    if (DataRow["CT_DefaultVNStartValue"] != DBNull.Value)
                    {
                        Node.DefaultVNStartValue = (string)DataRow["CT_DefaultVNStartValue"];
                    }
                    else
                    {
                        Node.DefaultVNStartValue = string.Empty;
                    }
                    if (DataRow["CT_CreationDate"] == DBNull.Value)
                    {
                        Node.CreationDate = DateTime.MinValue;
                    }
                    else
                    {
                        Node.CreationDate = (DateTime)DataRow["CT_CreationDate"];
                    }
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

        public static CMFolderModel GetFolderRowByChildFolderId(long childFolderId)
        {
            // Initialize work fields
            CMFolderModel Node = new CMFolderModel();
            try
            {
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("SELECT * FROM ContentTree WHERE CT_ID = " + childFolderId);
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null && ResTable.Rows.Count == 1)
                {
                    DataRow DataRow = ResTable.Rows[0];
                    //
                    Node.Id = (int)DataRow["CT_ID"];
                    Node.Name = (string)DataRow["CT_Name"];
                    Node.FolderName = (string)DataRow["CT_Name"];
                    Node.Description = (string)DataRow["CT_Description"];
                    Node.ParentId = (int)DataRow["CT_ParentID"];
                    Node.ChildNumber = (int)DataRow["CT_ChildNumber"];
                    Node.LastUpdateTime = (DateTime)DataRow["CT_LastUpdateTime"];
                    Node.LastUpdateUser = (string)DataRow["CT_LastUpdateUser"];
                    Node.LastUpdateComputer = (string)DataRow["CT_LastUpdateComputer"];
                    Node.LastUpdateApplication = (string)DataRow["CT_LastUpdateApplication"];
                    if (DataRow["CT_DefaultVNIncrement"] != DBNull.Value)
                    {
                        Node.DefaultVNIncrement = (string)DataRow["CT_DefaultVNIncrement"];
                    }
                    else
                    {
                        Node.DefaultVNIncrement = string.Empty;
                    }

                    if (DataRow["CT_DefaultVNPrefix"] != DBNull.Value)
                    {
                        Node.DefaultVNPrefix = (string)DataRow["CT_DefaultVNPrefix"];
                    }
                    else
                    {
                        Node.DefaultVNPrefix = string.Empty;
                    }

                    if (DataRow["CT_DefaultVNStartValue"] != DBNull.Value)
                    {
                        Node.DefaultVNStartValue = (string)DataRow["CT_DefaultVNStartValue"];
                    }
                    else
                    {
                        Node.DefaultVNStartValue = string.Empty;
                    }
                    if (DataRow["CT_CreationDate"] == DBNull.Value)
                    {
                        Node.CreationDate = DateTime.MinValue;
                    }
                    else
                    {
                        Node.CreationDate = (DateTime)DataRow["CT_CreationDate"];
                    }
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

        #region Retrieve Folder Id

        public static long GetFolderId(long FolderId)
        {
            // Initialize work fields
            CMFolderModel Node = new CMFolderModel();
            try
            {
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("SELECT CT_ID FROM ContentTree WHERE CT_ID = " + FolderId);
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null && ResTable.Rows.Count == 1)
                {
                    return 1;
                }
                else
                {
                    throw new TraceException("Folder deleted", null, "Content manager");
                }

            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new TraceException("Folder deleted", null, "Content manager");
            }
        }

        #endregion

        #region Delete folder

        public static long DeleteFolder(long FolderId)
        {
            // Work Fields
            long RV = 0;
            var SB = new StringBuilder(string.Empty);
            List<ParamStruct> CommandParams;
            //CMFolderModel FolderNode = GetFolderRow(FolderId); 
            // Build the Query
            SB.Append("DELETE FROM ContentTree WHERE CT_ID=@CT_ID");
            // Set the parameters
            CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "CT_ID", DataType = DbType.Int32, Value = FolderId }
                };
            // Execute the query
            RV = (long)Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
            // Finalize
            if (RV < 1) // Something went wrong... No rows were affected
            {
                throw new TraceException("Folder deleted", null, "Content manager");
            }
            else // All OK
            {
                return RV;
            }
        }

        public static long DeleteFolderContentTreeUserGroupType(long FolderId)
        {
            // Work Fields
            long RV = 0;
            var SB = new StringBuilder(string.Empty);
            List<ParamStruct> CommandParams;
            // Build the Query
            SB.Append("DELETE FROM ContentTreeUserGroupType WHERE CTUGT_id_ContentTree=@CT_ID");
            // Set the parameters
            CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "CT_ID", DataType = DbType.Int32, Value = FolderId }
                };
            // Execute the query
            RV = (long)Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
            // Finalize
            if (RV < 1) // Something went wrong... No rows were affected
            {
                return -1;
            }
            else // All OK
            {
                return RV;
            }
        }

        #endregion

        #region Get Folder Child Count

        public static long GetFolderChildCount(long folderId)
        {
            // Work Fields
            int subFolders;
            int subContents;
            var SB = new StringBuilder(string.Empty);

            // Build the Query
            SB.Append("SELECT COUNT(CT_ID) FROM ContentTree WHERE CT_ParentID ='" + folderId + "'");

            // Execute the query
            subFolders = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));

            SB.Clear();
 
            // Build the Query
            SB.Append("SELECT COUNT(CO_ID) FROM Content WHERE CO_id_ContentTree='" + folderId + "'");

            // Execute the query
            subContents = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));

            if(subFolders + subContents > 0)
                throw new TraceException("Folder delete with subitems", null, "Content manager");

            return 1;
        }         

        #endregion

        #region Get Group Types List

        public static ObservableCollection<CMUserGroupTypeModel> GetGroupTypesList()
        {
            try
            {
                ObservableCollection<CMUserGroupTypeModel> GroupTypesList = new ObservableCollection<CMUserGroupTypeModel>();
                CMUserGroupTypeModel userGroup;
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Clear();
                QryStr.Append("Select GT_ID, GT_Name FROM GroupTypes");
                string Qry = QryStr.ToString();
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null)
                {
                    foreach (DataRow DataRow in ResTable.Rows)
                    {
                        userGroup = new CMUserGroupTypeModel();

                        userGroup.ID = (string)DataRow["GT_ID"];
                        userGroup.Name = (string)DataRow["GT_Name"];
                        userGroup.Checked = false;

                        GroupTypesList.Add(userGroup);
                    }
                }
                return GroupTypesList;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return null;
            }
        }

        #endregion

        #region Clone Folder

        public static CMFolderModel CloneFolder(CMFolderModel FM)
        {
            try
            {
                // Work fields
                CMFolderModel f = new CMFolderModel();
                var SB = new StringBuilder(string.Empty);
                List<ParamStruct> CommandParams;
                Boolean DatabaseSupportsBatchQueries = Domain.PersistenceLayer.GetSupportsBatchQueries();

                f.Name = FM.Name;
                f.Description = FM.Description;
                f.ChildNumber = FM.ChildNumber;
                f.ParentId = FM.ParentId;
                f.UserGroupTypePermission = FM.UserGroupTypePermission;
                f.DefaultVNIncrement = FM.DefaultVNIncrement;
                f.DefaultVNPrefix = FM.DefaultVNPrefix;
                f.DefaultVNStartValue = FM.DefaultVNStartValue;
                f.CreationDate = DateTime.Now;

                UpdateFolderControlFields(ref f);

                SB.Clear();
                // Build the Query
                SB.Append("INSERT INTO ContentTree ");
                SB.Append("(CT_Name, CT_ParentID, CT_ChildNumber, CT_Description, CT_LastUpdateUser, CT_LastUpdateTime, ");
                SB.Append("CT_LastUpdateComputer, CT_LastUpdateApplication, CT_CreationDate, CT_DefaultVNPrefix, ");
                SB.Append("CT_DefaultVNStartValue, CT_DefaultVNIncrement) ");
                SB.Append("VALUES (@CT_Name, @CT_ParentID, @CT_ChildNumber, @CT_Description, @CT_LastUpdateUser, ");
                SB.Append(" @CT_LastUpdateTime, @CT_LastUpdateComputer, @CT_LastUpdateApplication, ");
                SB.Append(" @CT_CreationDate, @CT_DefaultVNPrefix, @CT_DefaultVNStartValue, @CT_DefaultVNIncrement) ");

                if (DatabaseSupportsBatchQueries)
                {
                    SB.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                }

                // Set the parameters
                CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "CT_ID", DataType = DbType.Int32, Value = f.Id },
                new ParamStruct { ParamName = "CT_Name", DataType = DbType.String, Value = f.Name },
                new ParamStruct { ParamName = "CT_ParentID", DataType = DbType.Int32, Value = f.ParentId },
                new ParamStruct { ParamName = "CT_ChildNumber", DataType = DbType.Int32, Value = f.ChildNumber},
                new ParamStruct { ParamName = "CT_Description", DataType = DbType.String, Value = f.Description },
                new ParamStruct { ParamName = "CT_LastUpdateUser", DataType = DbType.String, Value = f.LastUpdateUser},   
                new ParamStruct { ParamName = "CT_LastUpdateTime", DataType = DbType.DateTime, Value = f.LastUpdateTime },
                new ParamStruct { ParamName = "CT_LastUpdateComputer", DataType = DbType.String, Value = f.LastUpdateComputer },
                new ParamStruct { ParamName = "CT_LastUpdateApplication", DataType = DbType.String, Value = f.LastUpdateApplication },
                new ParamStruct { ParamName = "CT_CreationDate", DataType = DbType.DateTime, Value = f.CreationDate },
                new ParamStruct { ParamName = "CT_DefaultVNPrefix", DataType = DbType.String, Value = f.DefaultVNPrefix },
                new ParamStruct { ParamName = "CT_DefaultVNStartValue", DataType = DbType.String, Value = f.DefaultVNStartValue },
                new ParamStruct { ParamName = "CT_DefaultVNIncrement", DataType = DbType.String, Value = f.DefaultVNIncrement } 
                };

                //Execute the query
                object RV = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());

                if (RV != null)
                {
                    f.Id = Convert.ToInt64(RV);
                    AddFolderUserGroupTypesData(ref f);
                    return f;
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

        #region Add New Folder

        public static long AddNewFolder(ref CMFolderModel f)
        {
            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                List<ParamStruct> CommandParams;
                Boolean DatabaseSupportsBatchQueries = Domain.PersistenceLayer.GetSupportsBatchQueries();

                UpdateFolderControlFields(ref f);
                f.CreationDate = DateTime.Now;

                int tempChildNum = 0;
                f.ChildNumber = GetChildIDForAddFolder(f.ParentId);
                tempChildNum = CMContentBLL.GetChildIDForAddContent(f.ParentId);

                if (tempChildNum > f.ChildNumber)  //take the largest child number
                    f.ChildNumber = tempChildNum;
  
                SB.Clear();
                // Build the Query
                SB.Append("INSERT INTO ContentTree ");
                SB.Append("(CT_Name, CT_ParentID, CT_ChildNumber, CT_Description, CT_LastUpdateUser, CT_LastUpdateTime, ");
                SB.Append("CT_LastUpdateComputer, CT_LastUpdateApplication, CT_CreationDate, CT_DefaultVNPrefix, CT_DefaultVNStartValue, CT_DefaultVNIncrement) ");
                SB.Append("VALUES (@CT_Name, @CT_ParentID, @CT_ChildNumber, @CT_Description, @CT_LastUpdateUser, ");
                SB.Append(" @CT_LastUpdateTime, @CT_LastUpdateComputer, @CT_LastUpdateApplication, @CT_CreationDate, @CT_DefaultVNPrefix, @CT_DefaultVNStartValue, @CT_DefaultVNIncrement) ");
                if (DatabaseSupportsBatchQueries)
                {
                    SB.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                }

                // Set the parameters
                CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "CT_ID", DataType = DbType.Int32, Value = f.Id },
                new ParamStruct { ParamName = "CT_Name", DataType = DbType.String, Value = f.Name },
                new ParamStruct { ParamName = "CT_ParentID", DataType = DbType.Int32, Value = f.ParentId },
                new ParamStruct { ParamName = "CT_ChildNumber", DataType = DbType.Int32, Value = f.ChildNumber},
                new ParamStruct { ParamName = "CT_Description", DataType = DbType.String, Value = f.Description },
                new ParamStruct { ParamName = "CT_LastUpdateUser", DataType = DbType.String, Value = f.LastUpdateUser},   
                new ParamStruct { ParamName = "CT_LastUpdateTime", DataType = DbType.DateTime, Value = f.LastUpdateTime },
                new ParamStruct { ParamName = "CT_LastUpdateComputer", DataType = DbType.String, Value = f.LastUpdateComputer },
                new ParamStruct { ParamName = "CT_LastUpdateApplication", DataType = DbType.String, Value = f.LastUpdateApplication }, 
                new ParamStruct { ParamName = "CT_CreationDate", DataType = DbType.DateTime, Value = f.CreationDate },
                new ParamStruct { ParamName = "CT_DefaultVNPrefix", DataType = DbType.String, Value = f.DefaultVNPrefix },
                new ParamStruct { ParamName = "CT_DefaultVNStartValue", DataType = DbType.String, Value = f.DefaultVNStartValue },
                new ParamStruct { ParamName = "CT_DefaultVNIncrement", DataType = DbType.String, Value = f.DefaultVNIncrement }
                };

                //Execute the query
                object RV = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());

                if (RV != null)
                {
                    f.Id = Convert.ToInt64(RV);
                    AddFolderUserGroupTypesData(ref f);
                    return f.Id;
                }
                else
                {
                    throw new TraceException("Parent folder deleted", null, "Content manager");
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new TraceException("Parent folder deleted", null, "Content manager");
            }
        }

        #endregion

        #region Get Child ID For Add Folder

        public static int GetChildIDForAddFolder(long folderOrContentId)
        {
            // Initialize work fields           
            int lastChildID = 0;

            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                SB.Clear();
                SB.Append("SELECT MAX(CT_ChildNumber) FROM ContentTree WHERE CT_ParentID = " + folderOrContentId);
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
            catch (Exception e) //there is no child's for folder parent
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return 1;
            }

            lastChildID++;
            return lastChildID;
        }

        #endregion

        #region Check Folder Name

        public static string CheckFolderName(ref CMFolderModel f)
        {

            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                SB.Clear();
                SB.Append("SELECT COUNT(CT_ID) FROM ContentTree WHERE CT_Name = '" + f.Name.ToString() + "' AND CT_ParentID = " + f.ParentId);

                // Fetch the DataTable from the database
                Int16 Count = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));

                if (Count > 0)
                {
                    return "Adding existing folder";
                }
                else
                {
                    return "OK";
                }

            }
            catch (Exception e)
            {

                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage); return null;
            }

        }

        #endregion

        #region Generate Folder Name

        public static string GenerateFolderName(ref CMFolderModel folder)
        {
            try
            {
                // Work fields
                int index = 0;
                var SB = new StringBuilder(string.Empty);
                SB.Clear();
                SB.Append("SELECT COUNT(CT_ID) FROM ContentTree where CT_Name = '" + folder.Name.ToString() + "' AND CT_ParentID = " + folder.ParentId);
                // Fetch the DataTable from the database
                Int16 res = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));

                if (res == 0)
                {
                    return folder.Name;
                }
                else
                {
                    do
                    {
                        index++;
                        SB.Clear();
                        SB.Append("SELECT COUNT(CT_ID) FROM ContentTree where CT_Name = '" + folder.Name.ToString() + index + "' AND CT_ParentID = " + folder.ParentId);
                        res = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));

                    }while (res > 0);

                    folder.Name = folder.Name + index;
                }
                return folder.Name;
            }
            catch (Exception e) //there is no child's for folder parent
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return null;
            }
        }

        #endregion

        #region Check Existing Folder Name

        public static string CheckExistingFolderName(ref CMFolderModel f)
        {
            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                SB.Clear();
                SB.Append("SELECT COUNT(CT_ID) FROM ContentTree WHERE CT_Name = '" + f.Name.ToString() + "' AND CT_ID <> " + f.Id + " AND CT_ParentID = " + f.ParentId);

                // Fetch the DataTable from the database
                Int16 Count = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));

                if (Count > 0)
                {
                    return "Update folder name to existing";
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

        #region Update Existing Folder

        public static long UpdateExistingFolder(ref CMFolderModel f)
        {
            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                List<ParamStruct> CommandParams;

                UpdateFolderControlFields(ref f);  // Update Control fields

                // Build the Query
                SB.Clear();
                SB.Append("UPDATE ContentTree SET CT_Name=@CT_Name, CT_ParentID=@CT_ParentID, ");
                SB.Append(" CT_ChildNumber=@CT_ChildNumber, CT_Description=@CT_Description, CT_LastUpdateTime=@CT_LastUpdateTime, ");
                SB.Append(" CT_LastUpdateUser=@CT_LastUpdateUser, CT_LastUpdateComputer=@CT_LastUpdateComputer, ");
                SB.Append(" CT_LastUpdateApplication=@CT_LastUpdateApplication, ");
                SB.Append(" CT_DefaultVNPrefix=@CT_DefaultVNPrefix, CT_DefaultVNStartValue = @CT_DefaultVNStartValue, CT_DefaultVNIncrement = @CT_DefaultVNIncrement  ");
                SB.Append("WHERE CT_ID=@CT_ID");

                // Set the parameters
                CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "CT_ID", DataType = DbType.Int32, Value = f.Id },
                new ParamStruct { ParamName = "CT_Name", DataType = DbType.String, Value = f.Name },
                new ParamStruct { ParamName = "CT_ParentID", DataType = DbType.Int32, Value = f.ParentId },                   
                new ParamStruct { ParamName = "CT_Description", DataType = DbType.String, Value = f.Description },
                new ParamStruct { ParamName = "CT_ChildNumber", DataType = DbType.Int32, Value = f.ChildNumber},
                new ParamStruct { ParamName = "CT_LastUpdateTime", DataType = DbType.DateTime, Value = f.LastUpdateTime },
                new ParamStruct { ParamName = "CT_LastUpdateUser", DataType = DbType.String, Value = f.LastUpdateUser },
                new ParamStruct { ParamName = "CT_LastUpdateComputer", DataType = DbType.String, Value = f.LastUpdateComputer },
                new ParamStruct { ParamName = "CT_LastUpdateApplication", DataType = DbType.String, Value = f.LastUpdateApplication },
                new ParamStruct { ParamName = "CT_DefaultVNPrefix", DataType = DbType.String, Value = f.DefaultVNPrefix },
                new ParamStruct { ParamName = "CT_DefaultVNStartValue", DataType = DbType.String, Value = f.DefaultVNStartValue },
                new ParamStruct { ParamName = "CT_DefaultVNIncrement", DataType = DbType.String, Value = f.DefaultVNIncrement }
                };
                //Execute the query
                object RV = Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());

                if (RV != null)
                {
                    if (Convert.ToInt32(RV) > 0)  //if folder exists
                    {
                        UpdateFolderUserGroupTypes(ref f);
                        return f.Id;
                    }
                    else
                    {
                        throw new TraceException("Folder deleted", null, "Content manager");
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
                throw new TraceException("Folder deleted", null, "Content manager");
            }
        }

        #endregion

        #region Add Folder User Group Types Data

        public static void AddFolderUserGroupTypesData(ref CMFolderModel f)
        {
            try
            {
                //get user group types as it is in the updated UI
                List<string> UserGroupTypeList = new List<string>();

                foreach (CMFolderUserGroupTypeModel ugt in f.UserGroupTypePermission.Values)
                {
                    if (ugt.UserGroupType.Checked == true)
                        UserGroupTypeList.Add(ugt.UserGroupType.ID);
                }

                foreach (string type in UserGroupTypeList)
                {
                    // Work fields
                    var SB = new StringBuilder(string.Empty);
                    List<ParamStruct> CommandParams;
                    Boolean DatabaseSupportsBatchQueries = Domain.PersistenceLayer.GetSupportsBatchQueries();
                    // Build the Query
                    SB.Append("INSERT INTO ContentTreeUserGroupType (CTUGT_id_ContentTree, CTUGT_id_UserGroupType, CTUGT_LastUpdateUser, CTUGT_LastUpdateComputer, CTUGT_LastUpdateApplication, CTUGT_LastUpdateTime) ");
                    SB.Append("VALUES (@CTUGT_id_ContentTree, @CTUGT_id_UserGroupType, @CTUGT_LastUpdateUser, @CTUGT_LastUpdateComputer, @CTUGT_LastUpdateApplication, @CTUGT_LastUpdateTime) ");
                    if (DatabaseSupportsBatchQueries)
                    {
                        SB.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                    }

                    // Set the parameters
                    CommandParams = new List<ParamStruct>()
                    {
                    new ParamStruct { ParamName = "CTUGT_id_ContentTree", DataType = DbType.Int32, Value = f.Id },
                    new ParamStruct { ParamName = "CTUGT_id_UserGroupType", DataType = DbType.String, Value = type},      
                    new ParamStruct { ParamName = "CTUGT_LastUpdateUser", DataType = DbType.String, Value = f.LastUpdateUser},   
                    new ParamStruct { ParamName = "CTUGT_LastUpdateTime", DataType = DbType.DateTime, Value = f.LastUpdateTime },
                    new ParamStruct { ParamName = "CTUGT_LastUpdateComputer", DataType = DbType.String, Value = f.LastUpdateComputer },
                    new ParamStruct { ParamName = "CTUGT_LastUpdateApplication", DataType = DbType.String, Value = f.LastUpdateApplication } 
                    };

                    //Execute the query
                    object RV = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());

                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
            }
        }

        #endregion

        #region Update Folder User Group Types Data

        public static void UpdateFolderUserGroupTypes(ref CMFolderModel f)
        {
            string tempUserGroupType = null;
            List<string> UserGroupTypeListInDataBase = new List<string>();
            DataTable UserGroupTypeDataTable = CMTreeNodeBLL.GetFolderUserGroupTypesDataTable((int)f.Id);

            foreach (DataRow row in UserGroupTypeDataTable.Rows) //get user group types cuurrently in the db for this folder
            {
                tempUserGroupType = (string)row["CTUGT_id_UserGroupType"];
                UserGroupTypeListInDataBase.Add(tempUserGroupType);
         
            }

            //get user group types as it is in the updated UI
            List<string> UserGroupTypeListToUpdate = new List<string>(); 

            foreach (CMFolderUserGroupTypeModel ugt in f.UserGroupTypePermission.Values) 
            {
                if(ugt.UserGroupType.Checked == true)
                    UserGroupTypeListToUpdate.Add(ugt.UserGroupType.ID);
            }

            foreach (string UserGroupType in UserGroupTypeListInDataBase)
            {
                if(!UserGroupTypeListToUpdate.Contains(UserGroupType) ) //if type is in the db but not in ui - delete from db
                {
                    CMFolderUserGroupTypeModel fugt = new CMFolderUserGroupTypeModel();
                    CMUserGroupTypeModel ugt = new CMUserGroupTypeModel();
                    ugt.ID = f.Id.ToString();
                    ugt.Name = UserGroupType;
                    fugt.UserGroupType = ugt;
                    UpdateFolderUserGroupTypesDelete(f.Id, fugt);
                   // f.UserGroupTypePermission.Remove(ugt.Name);
                }
            }

            foreach (string UserGroupType in UserGroupTypeListToUpdate)
            {
                if (!UserGroupTypeListInDataBase.Contains(UserGroupType)) //if type is in the updated ui but not in the db - add to db
                {
                    CMFolderUserGroupTypeModel fugt = new CMFolderUserGroupTypeModel();
                    CMUserGroupTypeModel ugt = new CMUserGroupTypeModel();
                    ugt.ID = f.Id.ToString();
                    ugt.Name = UserGroupType;
                    fugt.UserGroupType = ugt;
                    UpdateUserGroupTypeControlFields(ref ugt);
                    UpdateFolderUserGroupTypesAdd(f.Id, fugt);
                  //  f.UserGroupTypePermission.Add(ugt.Name, fugt);
                }
            }
        }

        #endregion

        #region UpdateFolderUserGroupTypesAdd

        public static void UpdateFolderUserGroupTypesAdd(long folderID, CMFolderUserGroupTypeModel folderUserGroupType)
        {
            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                List<ParamStruct> CommandParams;
                Boolean DatabaseSupportsBatchQueries = Domain.PersistenceLayer.GetSupportsBatchQueries();

                UpdatefolderUserGroupTypeControlFields(ref folderUserGroupType);
            
                SB.Clear();
                // Build the Query
                SB.Append("INSERT INTO ContentTreeUserGroupType (CTUGT_id_ContentTree, CTUGT_id_UserGroupType, CTUGT_LastUpdateUser, CTUGT_LastUpdateComputer, CTUGT_LastUpdateApplication, CTUGT_LastUpdateTime) ");
                SB.Append("VALUES (@CTUGT_id_ContentTree, @CTUGT_id_UserGroupType, @CTUGT_LastUpdateUser, @CTUGT_LastUpdateComputer, @CTUGT_LastUpdateApplication, @CTUGT_LastUpdateTime) ");
                if (DatabaseSupportsBatchQueries)
                {
                    SB.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                }

                // Set the parameters
                CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "CTUGT_id_ContentTree", DataType = DbType.Int32, Value = folderID },
                new ParamStruct { ParamName = "CTUGT_id_UserGroupType", DataType = DbType.String, Value = folderUserGroupType.UserGroupType.Name },     
                new ParamStruct { ParamName = "CTUGT_LastUpdateUser", DataType = DbType.String, Value = folderUserGroupType.UserGroupType.LastUpdateUser},   
                new ParamStruct { ParamName = "CTUGT_LastUpdateComputer", DataType = DbType.String, Value = folderUserGroupType.UserGroupType.LastUpdateComputer },
                new ParamStruct { ParamName = "CTUGT_LastUpdateApplication", DataType = DbType.String, Value = folderUserGroupType.UserGroupType.LastUpdateApplication },
                new ParamStruct { ParamName = "CTUGT_LastUpdateTime", DataType = DbType.DateTime, Value = folderUserGroupType.UserGroupType.LastUpdateTime } 
                };

                //Execute the query
                object RV = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());

            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return;
            }
        }

        #endregion

        #region Update Folder User GroupTypes Delete

        public static void UpdateFolderUserGroupTypesDelete(long folderID, CMFolderUserGroupTypeModel folderUserGroupType)
        {
            try
            {
                // Work Fields
                long RV = 0;
                var SB = new StringBuilder(string.Empty);
                List<ParamStruct> CommandParams;
                // Build the Query
                SB.Append("DELETE FROM ContentTreeUserGroupType WHERE CTUGT_id_ContentTree=@CTUGT_id_ContentTree AND CTUGT_id_UserGroupType=@CTUGT_id_UserGroupType");
                // Set the parameters
                CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "CTUGT_id_ContentTree", DataType = DbType.Int32, Value = folderID },
                new ParamStruct { ParamName = "CTUGT_id_UserGroupType", DataType = DbType.String, Value = folderUserGroupType.UserGroupType.Name }
                };
                // Execute the query
                RV = (long)Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return;
            }
         }

         #endregion

        #region Update Folder Control Fields

        public static void UpdateFolderControlFields(ref CMFolderModel f)
        {
            f.LastUpdateApplication = "Content manager";
            f.LastUpdateComputer = Domain.Workstn;
            f.LastUpdateUser = Domain.User;
            f.LastUpdateTime = DateTime.Now;
        }

        #endregion

        #region Update folderUserGroupType Control Fields

        public static void UpdatefolderUserGroupTypeControlFields(ref CMFolderUserGroupTypeModel f)
        {
            f.LastUpdateApplication = "Content manager";
            f.LastUpdateComputer = Domain.Workstn;
            f.LastUpdateUser = Domain.User;
            f.LastUpdateTime = DateTime.Now;
        }

        #endregion

        #region Update UserGroupType Control Fields

        public static void UpdateUserGroupTypeControlFields(ref CMUserGroupTypeModel f)
        {
            f.LastUpdateApplication = "Content manager";
            f.LastUpdateComputer = Domain.Workstn;
            f.LastUpdateUser = Domain.User;
            f.LastUpdateTime = DateTime.Now;
        }

        #endregion

        #region Compare Update Time

        public static bool CompareUpdateTime(DateTime LastUpdateTime, long currFolderId)
        {
            DateTime updateDate;

            // Build The Query String
            System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
            QryStr.Append("SELECT CT_LastUpdateTime FROM ContentTree WHERE CT_ID = " + currFolderId);
            // Fetch the DataTable from the database
            DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
            // Populate the collection
            if (ResTable != null && ResTable.Rows.Count == 1)
            {
                DataRow DataRow = ResTable.Rows[0];
                updateDate = (DateTime)DataRow["CT_LastUpdateTime"];
                if (LastUpdateTime.Year != updateDate.Year ||
                LastUpdateTime.Month != updateDate.Month ||
                LastUpdateTime.Day != updateDate.Day ||
                LastUpdateTime.Hour != updateDate.Hour ||
                LastUpdateTime.Minute != updateDate.Minute ||
                LastUpdateTime.Second != updateDate.Second)
                    throw new TraceException("Folder changed", null, "Content manager");
            }       
            return true;
        }

        #endregion

        #region Get Content folders branch

        public static List<int> GetAllFolderIds(int contentId)
        {
            List<int> branchFolderIds = new List<int>();
            try
            {
                var SB = new StringBuilder(string.Empty);

                SB.Append("with cte (NodeId,FullIdPath) as ");
	            SB.Append("(select CT_ID, CONVERT(varchar(50),CT_ID) from ContentTree ");
		        SB.Append("where (CT_ParentID = 0 and CT_ParentID != CT_ID) ");
		        SB.Append("union all select n.CT_ID, CONVERT(varchar(50),cte.FullIdPath  + ',' + CONVERT(varchar(50),n.CT_ID)) ");
                SB.Append("as name from ContentTree n  ");
                SB.Append("join cte on n.CT_ParentID = cte.NodeId )  ");
                SB.Append("select CONVERT(varchar(500),FullIdPath) as BranchIdList from cte  ");
                SB.Append("join Content on NodeId = CO_id_ContentTree ");
                SB.Append("where CO_id = " + contentId);


                // Fetch the DataTable from the database
                object objSelectResult = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null);
                // Populate the collection
                if (objSelectResult != null && objSelectResult.ToString().Length > 0)
                {
                    string[] fullPath = objSelectResult.ToString().Split(',');
                    foreach (string id in fullPath)
                    {
                        branchFolderIds.Add(Convert.ToInt32(id));
                    }
                    return branchFolderIds;
                }
                else
                {
                    throw new Exception("Failed to retrieve folders for content " + contentId);
                }

            }
            catch (Exception e)
            {

                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage); 
                return null;
            }
        }

        public static void GetListOfAllContentsParentFolders(List<int> listOfCMContentIds, out List<int> listOfCMFolderIds)
        {
            listOfCMFolderIds = new List<int>();
            try
            {
                if (listOfCMContentIds != null && listOfCMContentIds.Count > 0)
                {
                    foreach (int cId in listOfCMContentIds)
                    {
                        List<int> tempList = new List<int>();
                        tempList = CMFolderBLL.GetAllFolderIds(cId);
                        if (tempList != null && tempList.Count > 0)
                        {
                            listOfCMFolderIds.AddRange(tempList);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
            }
        }

        #endregion

        #region Get DataTables
        public static DataTable GetContentTreeUserGroupTypeDataTable(List<int> folderIds)
        {
            try
            {
                // Build The Query String
                string strFolders = string.Join(",", folderIds);
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("Select * FROM ContentTreeUserGroupType ");
                QryStr.Append("WHERE CTUGT_id_ContentTree in (" + strFolders + ") ");
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

        public static DataTable GetContentTreeDataTable(List<int> folderIds)
        {
            try
            {
                // Build The Query String
                string strFolders = string.Join(",", folderIds);
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("Select * FROM ContentTree ");
                QryStr.Append("WHERE CT_ID in (" + strFolders + ") ");
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

        public static bool FolderExists(long parentId, string name)
        {

            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                SB.Clear();
                SB.Append("SELECT COUNT(CT_ID) FROM ContentTree WHERE CT_Name = '" + name + "' AND CT_ParentID = " + parentId);

                // Fetch the DataTable from the database
                Int16 Count = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));

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
                throw new Exception("Failed to retrieve folder data.");
            }

        }

        public static CMFolderModel GetFolderRow(long parentId, string name)
        {
            // Initialize work fields
            CMFolderModel Node = new CMFolderModel();
            try
            {
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("SELECT * FROM ContentTree WHERE CT_Name = '" + name + "' AND CT_ParentID = " + parentId);
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null && ResTable.Rows.Count == 1)
                {
                    DataRow DataRow = ResTable.Rows[0];
                    //
                    Node.Id = (int)DataRow["CT_ID"];
                    Node.Name = (string)DataRow["CT_Name"];
                    Node.FolderName = (string)DataRow["CT_Name"];
                    Node.Description = (string)DataRow["CT_Description"];
                    Node.ParentId = (int)DataRow["CT_ParentID"];
                    Node.ChildNumber = (int)DataRow["CT_ChildNumber"];
                    Node.LastUpdateTime = (DateTime)DataRow["CT_LastUpdateTime"];
                    Node.LastUpdateUser = (string)DataRow["CT_LastUpdateUser"];
                    Node.LastUpdateComputer = (string)DataRow["CT_LastUpdateComputer"];
                    Node.LastUpdateApplication = (string)DataRow["CT_LastUpdateApplication"];

                    if (DataRow["CT_DefaultVNIncrement"] != DBNull.Value)
                    {
                        Node.DefaultVNIncrement = (string)DataRow["CT_DefaultVNIncrement"];
                    }
                    else
                    {
                        Node.DefaultVNIncrement = string.Empty;
                    }

                    if (DataRow["CT_DefaultVNPrefix"] != DBNull.Value)
                    {
                        Node.DefaultVNPrefix = (string)DataRow["CT_DefaultVNPrefix"]; 
                    }
                    else
                    {
                        Node.DefaultVNPrefix = string.Empty;
                    }

                    if (DataRow["CT_DefaultVNStartValue"] != DBNull.Value)
                    {
                        Node.DefaultVNStartValue = (string)DataRow["CT_DefaultVNStartValue"];
                    }
                    else
                    {
                        Node.DefaultVNStartValue = string.Empty;
                    }
                    
                    if (DataRow["CT_CreationDate"] == DBNull.Value)
                    {
                        Node.CreationDate = DateTime.MinValue;
                    }
                    else
                    {
                        Node.CreationDate = (DateTime)DataRow["CT_CreationDate"];
                    }
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

        public static ObservableCollection<string> GetGroupTypesKeys()
        {
            try
            {
                ObservableCollection<string> GroupTypesKeys = new ObservableCollection<string>();
                string userGroup;
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Clear();
                QryStr.Append("Select GT_ID FROM GroupTypes");
                string Qry = QryStr.ToString();
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null)
                {
                    foreach (DataRow DataRow in ResTable.Rows)
                    {
                        GroupTypesKeys.Add((string)DataRow["GT_ID"]);
                    }
                }
                return GroupTypesKeys;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return null;
            }
        }

        #endregion

        #region Naming convention

        public static string[] GetNamingConvensionSettings(long contentId)
        {
            string[] namingParameters = new String[3];
            CMFolderModel parentFolder = CMFolderBLL.GetFolderRowByChildContentId(contentId);

            if (!string.IsNullOrEmpty(parentFolder.DefaultVNStartValue) && !string.IsNullOrEmpty(parentFolder.DefaultVNIncrement))
            {
                namingParameters[0] = parentFolder.DefaultVNPrefix;
                namingParameters[1] = parentFolder.DefaultVNStartValue;
                namingParameters[2] = parentFolder.DefaultVNIncrement;
            }
            else
            {
                CMFolderModel relevantFolder = FindFolderWithNamingConvention(parentFolder);
                namingParameters[0] = relevantFolder.DefaultVNPrefix;
                namingParameters[1] = relevantFolder.DefaultVNStartValue;
                namingParameters[2] = relevantFolder.DefaultVNIncrement;
            }
            return namingParameters;
        }

        static CMFolderModel FindFolderWithNamingConvention(CMFolderModel Folder)
        {
            CMFolderModel relevantFolder = GetFolderRow(Folder.ParentId);
            if ((!string.IsNullOrEmpty(relevantFolder.DefaultVNStartValue) && !string.IsNullOrEmpty(relevantFolder.DefaultVNIncrement)) ||
                relevantFolder.ParentId == 0)
            {
                return relevantFolder;
            }
            relevantFolder = FindFolderWithNamingConvention(relevantFolder);
            return relevantFolder;
        }
        #endregion
    }
}
