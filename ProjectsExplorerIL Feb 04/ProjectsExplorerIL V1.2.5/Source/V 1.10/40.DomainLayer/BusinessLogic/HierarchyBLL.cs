using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using ATSBusinessObjects;
using ATSBusinessObjects.ContentMgmtModels;
using ATSDomain;
using Infra.DAL;

namespace ATSBusinessLogic
{
    public class HierarchyBLL
    {

        #region Retrieve Hierarchy from database and return as ObservableCollection

        public static DataTable ProjectStepTable = new DataTable();

        public static List<int> listOfHierarchyIdsWithSpecialNotes = new List<int>();

        public static ObservableCollection<HierarchyModel> GetHierarchy(long ParentId)
        {
            // Initialize work fields
            ObservableCollection<HierarchyModel> Hierarchy = new ObservableCollection<HierarchyModel>();
            try
            {
                HierarchyModel EmptyNode = Domain.GetBusinessObject<HierarchyModel>();
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("Select * FROM PE_Hierarchy ");
                if (ParentId != 0)
                {
                    QryStr.Append("WHERE ParentId = " + ParentId + " ");
                }
                QryStr.Append("ORDER BY Name");
                string Qry = QryStr.ToString();
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null)
                {
                    foreach (DataRow DataRow in ResTable.Rows)
                    {
                        HierarchyModel Node = (HierarchyModel)Domain.DeepCopy(EmptyNode); // DeepCopy an empty instance to save on the Reflection work
                        //
                        Node.Id = (int)DataRow["Id"];
                        Node.IsNew = false;
                        Node.IsDirty = false;
                        if (DataRow["ParentId"] == DBNull.Value)
                        {
                            Node.ParentId = 0;
                        }
                        else
                        {
                            Node.ParentId = (int)DataRow["ParentId"];
                        }

                        if (!(DataRow["Sequence"] is System.DBNull))
                        {
                            Node.Sequence = Convert.ToInt32(DataRow["Sequence"]);
                        }


                        switch ((string)DataRow["NodeType"])
                        {
                            case "F":
                                Node.NodeType = NodeTypes.F;
                                break;
                            case "P":
                                Node.NodeType = NodeTypes.P;
                                break;
                            case "T":
                                Node.NodeType = NodeTypes.T;
                                break;
                            default:
                                Node.NodeType = NodeTypes.F;
                                break;
                        }
                        Node.Name = (string)DataRow["Name"];
                        Node.Description = (string)DataRow["Description"];
                        Node.CreationDate = (DateTime)DataRow["CreationDate"];
                        Node.LastUpdateTime = (DateTime)DataRow["LastUpdateTime"];
                        Node.LastUpdateUser = (string)DataRow["LastUpdateUser"];
                        if ((string)(DataRow["NodeType"]) == "P" || (string)(DataRow["NodeType"]) == "T")
                        {
                            if (!(DataRow["ProjectCode"] is System.DBNull))
                            {
                                Node.Code = (string)DataRow["ProjectCode"].ToString().Trim();
                            }
                            if (!(DataRow["ProjectStep"] is System.DBNull))
                            {
                                string stepName = GetStepNameByCode((string)DataRow["ProjectStep"]);
                                Node.SelectedStep = stepName.ToString();
                            }
                            if (!(DataRow["SCDUSASyncInd"] is System.DBNull))
                            {
                                Node.Synchronization = (Boolean)DataRow["SCDUSASyncInd"];
                            }
                            Node.IRISTechInd = (DataRow["IRISTechInd"] is System.DBNull) ? false : (Boolean)DataRow["IRISTechInd"];

                            if (!(DataRow["ProjectStatus"] is System.DBNull))
                            {
                                switch ((string)DataRow["ProjectStatus"].ToString().Trim())
                                {
                                    case "O":
                                        Node.ProjectStatus = "Open";
                                        break;
                                    case "D":
                                        Node.ProjectStatus = "Disabled";
                                        break;
                                    default:
                                        Node.ProjectStatus = "Open";
                                        break;
                                }

                            }

                            if (!(DataRow["GroupId"] is System.DBNull))
                            {
                                System.Text.StringBuilder groupQur = new System.Text.StringBuilder();
                                groupQur.Append("SELECT  h.Name as GroupName, h.Description as GroupDescription, h.ProjectStep as ProjectStep, hg.GroupId as GroupId , h.LastUpdateTime as GroupLastUpdateTime  " +
                                "FROM PE_Hierarchy as h inner join PE_Hierarchy as hg on h.Id = hg.GroupId " +
                                "WHERE hg.id='" + Node.Id.ToString().Trim() + "'");

                                DataTable GroupData = Domain.PersistenceLayer.FetchDataTable(groupQur.ToString(), CommandType.Text, null);
                                //var a = Domain.PersistenceLayer.FetchDataValue(groupQur.ToString(), System.Data.CommandType.Text, null);

                                if (!(GroupData.Rows[0]["GroupName"] is System.DBNull))
                                {
                                    Node.GroupName = (string)GroupData.Rows[0]["GroupName"];
                                    Node.GroupDescription = (string)GroupData.Rows[0]["GroupDescription"];
                                    Node.GroupId = (int)DataRow["GroupId"];
                                    Node.GroupLastUpdateTime = (DateTime)GroupData.Rows[0]["GroupLastUpdateTime"];
                                    if (!(GroupData.Rows[0]["ProjectStep"] is System.DBNull))
                                    {
                                        string stepName = GetStepNameByCode((string)GroupData.Rows[0]["ProjectStep"]);
                                        Node.SelectedStep = stepName.ToString();
                                    }

                                }
                            }

                            System.Text.StringBuilder ActiveVersionQry = new System.Text.StringBuilder();
                            ActiveVersionQry.Append("SELECT v.VersionName " +
                            "FROM PE_Version v inner join PE_Hierarchy h on v.HierarchyId = h.Id " +
                             " where v.VersionStatus = 'A' and h.id='" + Node.Id.ToString().Trim() + "'");
                            string activeVersion = (string)Domain.PersistenceLayer.FetchDataValue(ActiveVersionQry.ToString(), System.Data.CommandType.Text, null);
                            if (!(String.IsNullOrEmpty(activeVersion)))
                            {
                                Node.ActiveVersion = activeVersion;
                            }

                        }


                        // TODO: Continue populating all properties
                        // Add the row to the collection. For now, I am OMITTING 'G' and 'V' rows - we will modify this later (when we add Version nodes and decide what to do with Group nodes)
                        if (((string)DataRow["NodeType"] == "F") || ((string)DataRow["NodeType"] == "P") || ((string)DataRow["NodeType"] == "T") )
                        {
                            Hierarchy.Add(Node);
                        }
                    }
                }
              
                return Hierarchy;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return Hierarchy;
            }

        }

        #endregion

        #region Retrieve Hierarchy Row from database and return as HierarchyModel

        public static HierarchyModel GetHierarchyRow(long Id)
        {
            // Initialize work fields
            HierarchyModel Node = Domain.GetBusinessObject<HierarchyModel>();
            try
            {
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("Select * FROM PE_Hierarchy WHERE Id = " + Id);
                string Qry = QryStr.ToString();
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null && ResTable.Rows.Count == 1)
                {
                    DataRow DataRow = ResTable.Rows[0];
                    //
                    Node.Id = (int)DataRow["Id"];
                    Node.IsNew = false;
                    Node.IsDirty = false;
                    if (DataRow["ParentId"] == DBNull.Value)
                    {
                        Node.ParentId = 0;
                    }
                    else
                    {
                        Node.ParentId = (int)DataRow["ParentId"];
                    }
                    if (!(DataRow["Sequence"] is System.DBNull))
                    {
                    Node.Sequence = Convert.ToInt32(DataRow["Sequence"]);
                    }

                    switch ((string)DataRow["NodeType"])
                    {
                        case "F":
                            Node.NodeType = NodeTypes.F;
                            break;
                        case "P":
                            Node.NodeType = NodeTypes.P;
                            break;
                        case "G":
                            Node.NodeType = NodeTypes.G;
                            break;
                        case "T":
                            Node.NodeType = NodeTypes.T;
                            break;
                        default:
                            Node.NodeType = NodeTypes.F;
                            break;
                    }
                    Node.Name = (string)DataRow["Name"];
                    Node.Description = (string)DataRow["Description"];
                    Node.CreationDate = (DateTime)DataRow["CreationDate"];
                    Node.LastUpdateTime = (DateTime)DataRow["LastUpdateTime"];
                    Node.LastUpdateUser = (string)DataRow["LastUpdateUser"];

                    if ((string)(DataRow["NodeType"]) == "P" ||(string)(DataRow["NodeType"]) == "T")
                    {
                        if (!(DataRow["ProjectCode"] is System.DBNull))
                        {
                            Node.Code = (string)DataRow["ProjectCode"].ToString().Trim();
                        }
                        if (!(DataRow["ProjectStep"] is System.DBNull))
                        {
                            string stepName = GetStepNameByCode((string)DataRow["ProjectStep"]);
                            Node.SelectedStep = stepName.ToString();
                        }
                        if (!(DataRow["SCDUSASyncInd"] is System.DBNull))
                        {
                            Node.Synchronization = (Boolean)DataRow["SCDUSASyncInd"];
                        }
                        Node.IRISTechInd = (DataRow["IRISTechInd"] is System.DBNull) ? false : (Boolean)DataRow["IRISTechInd"];

                        if (!(DataRow["ProjectStatus"] is System.DBNull))
                        {
                            switch ((string)DataRow["ProjectStatus"])
                            {
                                case "O":
                                    Node.ProjectStatus = "Open";
                                    break;
                                case "D":
                                    Node.ProjectStatus = "Disabled";
                                    break;
                                default:
                                    Node.ProjectStatus = "Open";
                                    break;
                            }

                        }


                        if (!(DataRow["GroupId"] is System.DBNull))
                        {
                            System.Text.StringBuilder groupQur = new System.Text.StringBuilder(); 
                            groupQur.Append("SELECT  h.Name as GroupName, h.Description as GroupDescription, h.ProjectStep as ProjectStep, hg.GroupId as  GroupId , h.LastUpdateTime as GroupLastUpdateTime " +
                            "FROM PE_Hierarchy as h inner join PE_Hierarchy as hg on h.Id = hg.GroupId " +
                            "WHERE hg.id='" + Node.Id.ToString().Trim() + "'");

                            DataTable GroupData = Domain.PersistenceLayer.FetchDataTable(groupQur.ToString(), CommandType.Text, null);
                            //var a = Domain.PersistenceLayer.FetchDataValue(groupQur.ToString(), System.Data.CommandType.Text, null);

                            if (!(GroupData.Rows[0]["GroupName"] is System.DBNull))
                            {
                                Node.GroupName = (string)GroupData.Rows[0]["GroupName"];
                                Node.GroupDescription = (string)GroupData.Rows[0]["GroupDescription"];
                                Node.GroupId = (int)DataRow["GroupId"];
                                Node.GroupLastUpdateTime = (DateTime)GroupData.Rows[0]["GroupLastUpdateTime"];
                                if (!(GroupData.Rows[0]["ProjectStep"] is System.DBNull))
                                {
                                    string stepName = GetStepNameByCode((string)GroupData.Rows[0]["ProjectStep"]);
                                    Node.SelectedStep = stepName.ToString();
                                }

                            }
                        }
                    }

                    // TODO: Continue populating all properties
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

        #region Persist Hierarchy Row from database 

        public static string PersistHierarchyRow(ref HierarchyModel Hierarchy)
        {
            try
            {
                switch (Hierarchy.NodeType)
                {
                    case NodeTypes.F:
                        return PersistFolder(ref Hierarchy);
                    case NodeTypes.P:
                        return PersistProject(ref Hierarchy);
                    case NodeTypes.T:
                        return TemplateBLL.PersistProject(ref Hierarchy);
                }
                return string.Empty;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("DB Error");
            }
        }

        #endregion

        #region Persist (Add/Update) Folder

        public static string PersistFolder(ref HierarchyModel Hierarchy)
        {
            // Work fields
            var SB = new StringBuilder(string.Empty);
            List<ParamStruct> CommandParams;
            Boolean DatabaseSupportsBatchQueries = Domain.PersistenceLayer.GetSupportsBatchQueries();
            // Handle Null Parent
            object ParentObj = (object)DBNull.Value;
            if (Hierarchy.ParentId > 0)
            {
                ParentObj = Hierarchy.ParentId;
            }

            // Do work
            try
            {
                // Prior to updating, check Name is unique within the same parent
                SB.Clear();
                SB.Append("SELECT COUNT(*) FROM PE_Hierarchy WHERE Name = '" + Hierarchy.Name + "' AND Id <> '" + Hierarchy.Id + "' AND ParentId ");
                if (ParentObj == (object)DBNull.Value)
                {
                    SB.Append("IS NULL");
                }
                else
                {
                    SB.Append("= " + ParentObj);
                }
                object CountObj = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null);
                if (CountObj != null)
                {
                    Int16 Count = Convert.ToInt16(CountObj);
                    if (Count > 0)
                    {
                        return "100";
                    }
                }

                UpdateControlFields(ref Hierarchy); // Update Control fields
                SB.Clear();
                if (Hierarchy.IsNew) // New row; construct INSERT
                {
                    // Set Creation DateTime
                    Hierarchy.CreationDate = DateTime.Now;
                    // Build the Query
                    SB.Append("INSERT INTO PE_Hierarchy (ParentId, Sequence, NodeType, Name, Description, CreationDate, LastUpdateTime, LastUpdateUser, LastUpdateComputer, LastUpdateApplication) ");
                    SB.Append("VALUES (@ParentId, @Sequence, @NodeType, @Name, @Description, @CreationDate, @LastUpdateTime, @LastUpdateUser, @LastUpdateComputer, @LastUpdateApplication) ");
                    if (DatabaseSupportsBatchQueries)
                    {
                        SB.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                    }
                }
                else // Existing row; construct UPDATE
                {

                    // Build the Query
                    SB.Append("UPDATE PE_Hierarchy SET ParentId=@ParentId, Sequence=@Sequence, NodeType=@NodeType, Name=@Name, Description=@Description, ");
                    SB.Append("LastUpdateTime=@LastUpdateTime, LastUpdateUser=@LastUpdateUser, LastUpdateComputer=@LastUpdateComputer, LastUpdateApplication=@LastUpdateApplication ");
                    SB.Append("WHERE Id=@Id");
                }
                // Set the parameters
                CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "Id", DataType = DbType.Int32, Value = Hierarchy.Id },
                new ParamStruct { ParamName = "ParentId", DataType = DbType.Int32, Value = ParentObj },
                new ParamStruct { ParamName = "Sequence", DataType = DbType.Int16, Value = Hierarchy.Sequence },
                new ParamStruct { ParamName = "NodeType", DataType = DbType.String, Value = Hierarchy.NodeType.ToString() },
                new ParamStruct { ParamName = "Name", DataType = DbType.String, Value = Hierarchy.Name },
                new ParamStruct { ParamName = "Description", DataType = DbType.String, Value = Hierarchy.Description },
                new ParamStruct { ParamName = "CreationDate", DataType = DbType.DateTime, Value = Hierarchy.CreationDate },
                new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = Hierarchy.LastUpdateTime },
                new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = Hierarchy.LastUpdateUser },
                new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = Hierarchy.LastUpdateComputer },
                new ParamStruct { ParamName = "LastUpdateApplication", DataType = DbType.String, Value = Hierarchy.LastUpdateApplication }
                };
                // Execute the query
                long RV = 0;
                if (Hierarchy.IsNew & DatabaseSupportsBatchQueries)
                {
                    Object RVobj = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                    if (RVobj != null)
                    {
                        RV = Convert.ToInt64(RVobj);
                    }
                 
                }
                else
                {
                    RV = (long)Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                }
                // Finalize
                if (RV < 1) // Something went wrong... No rows were affected
                {
                    return "141";
                }
                else // All OK
                {
                    // Update the object
                    if (Hierarchy.IsNew)
                    {
                        if (DatabaseSupportsBatchQueries)
                        {
                            Hierarchy.Id = RV;
                        }
                        else // Mainly for SqlServerCE, which does not support ScopeIdentity
                        {
                            Hierarchy.Id = (int)Domain.PersistenceLayer.FetchDataValue("SELECT MAX(Id) FROM PE_Hierarchy", System.Data.CommandType.Text, null);
                        }
                    }
                    //Add Certificates
                    if (Hierarchy.Certificates.Count > 0)
                    {
                        foreach (String I in Hierarchy.Certificates)
                        {
                            var InsertCert = new StringBuilder(string.Empty);
                            InsertCert.Append("INSERT INTO PE_FolderCertificate (HierarchyId, CertificateId, EffectiveDate, ExpirationDate, LastUpdateTime, LastUpdateUser, LastUpdateComputer, LastUpdateAppliation) ");
                            InsertCert.Append("VALUES (@HierarchyId, @CertificateId, @EffectiveDate, @ExpirationDate, @LastUpdateTime, @LastUpdateUser, @LastUpdateComputer, @LastUpdateAppliation) ");
                            if (DatabaseSupportsBatchQueries)
                            {
                                InsertCert.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                            }

                            CommandParams = new List<ParamStruct>()
                        {
                        new ParamStruct { ParamName = "HierarchyId", DataType = DbType.Int32, Value = Hierarchy.Id },
                        new ParamStruct { ParamName = "CertificateId", DataType = DbType.String, Value =I.Trim()},
                        new ParamStruct { ParamName = "EffectiveDate", DataType = DbType.DateTime, Value = DateTime.Now },
                        new ParamStruct { ParamName = "ExpirationDate", DataType = DbType.DateTime, Value = (object)DBNull.Value },
                        new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = DateTime.Now },
                        new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = Domain.User },
                        new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = Domain.Workstn },
                        new ParamStruct { ParamName = "LastUpdateAppliation", DataType = DbType.String, Value = Domain.AppName },
                        
                        };
                            long CertID = 0;
                            if (DatabaseSupportsBatchQueries)
                            {
                                object CertIDObj = Domain.PersistenceLayer.FetchDataValue(InsertCert.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                                if (CertIDObj != null)
                                {
                                    CertID = Convert.ToInt64(CertIDObj);
                                }
                                else
                                {
                                    CertID = (long)Domain.PersistenceLayer.ExecuteDbCommand(InsertCert.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                                }
                            }
                            //else
                            //{
                            //    CertID = (long)Domain.PersistenceLayer.ExecuteDbCommand(InsertCert.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                            //}
                            if (CertID < 1) // Something went wrong... No rows were affected
                            {
                                return "141";
                            }
                        }
                    }
                    if (Hierarchy.UserCertificates.Count > 0)
                    {
                        foreach (var i in Hierarchy.UserCertificates)
                        {
                            if (i.IsNew)
                            {
                                
                                var InsertCert = new StringBuilder(string.Empty);
                                InsertCert.Append("INSERT INTO PE_FolderUserCertificate (HierarchyId, UserCertificateId, EffectiveDate, ExpirationDate, LastUpdateTime, LastUpdateUser, LastUpdateComputer, LastUpdateAppliation) ");
                                InsertCert.Append("VALUES (@HierarchyId, @UserCertificateId, @EffectiveDate, @ExpirationDate, @LastUpdateTime, @LastUpdateUser, @LastUpdateComputer, @LastUpdateAppliation) ");
                                if (DatabaseSupportsBatchQueries)
                                {
                                    InsertCert.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                                }

                                i.LastUpdateTime = DateTime.Now;
                                CommandParams.Clear();
                                CommandParams = new List<ParamStruct>()
                                    {
                                    new ParamStruct { ParamName = "HierarchyId", DataType = DbType.Int32, Value = Hierarchy.Id },
                                    new ParamStruct { ParamName = "UserCertificateId", DataType = DbType.String, Value = i.UserCertificateId},
                                    new ParamStruct { ParamName = "EffectiveDate", DataType = DbType.DateTime, Value = DateTime.Now },
                                    new ParamStruct { ParamName = "ExpirationDate", DataType = DbType.DateTime, Value = (object)DBNull.Value },
                                    new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = i.LastUpdateTime },
                                    new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = Domain.User },
                                    new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = Domain.Workstn },
                                    new ParamStruct { ParamName = "LastUpdateAppliation", DataType = DbType.String, Value = Domain.AppName },
                        
                                    };
                                long CertID = 0;
                                if (DatabaseSupportsBatchQueries)
                                {
                                    object CertIDObj = Domain.PersistenceLayer.FetchDataValue(InsertCert.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                                    if (CertIDObj != null)
                                    {
                                        CertID = Convert.ToInt64(CertIDObj);
                                    }

                                }
                                else
                                {
                                    CertID = (long)Domain.PersistenceLayer.ExecuteDbCommand(InsertCert.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                                }
                                if (CertID < 1) // Something went wrong... No rows were affected
                                {
                                    //return "Failed to update database";
                                    return "141";
                                }
                                i.IsNew = false;
                            }
                        }
                    }



                    Hierarchy.Certificates.Clear();
                    Hierarchy.IsDirty = false;
                    Hierarchy.IsNew = false;
                    Hierarchy.IsLoading = false;
                    return string.Empty;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("DB Error");
            }
        }

        #endregion

        #region Delete Folder

        public static long DeleteFolder(HierarchyModel Hierarchy)
        {
            try
            {
                // Work Fields
                long RV = 0;
                var SB = new StringBuilder(string.Empty);
                List<ParamStruct> CommandParams;
                // Build the Query
                SB.Append("DELETE FROM PE_Hierarchy WHERE Id=@Id");
                // Set the parameters
                CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "Id", DataType = DbType.Int32, Value = Hierarchy.Id }
                };
                // Execute the query
                RV = (long)Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                // Finalize
                if (RV < 1) // Something went wrong... No rows were affected
                {
                    throw new Exception("DB Error");
                }
                else // All OK
                {
                    return RV;
                }
            }
            catch(Exception e)
            {
                throw new Exception("DB Error");
            }
        }

        #endregion

        #region Persist (Add/Update) Project

        public static string PersistProject(ref HierarchyModel Hierarchy)
        {
            Boolean flgUpate = false;
            // Work fields
            var SB = new StringBuilder(string.Empty);
            List<ParamStruct> CommandParams;
            List<ParamStruct> CommandParamsVersion;
            Boolean DatabaseSupportsBatchQueries = Domain.PersistenceLayer.GetSupportsBatchQueries();
            // Handle Null Parent
            object ParentObj = (object)DBNull.Value;
            if (Hierarchy.ParentId > 0)
            {
                ParentObj = Hierarchy.ParentId;
            }

            // Do work
            try
            {
                //Update All Related Project
                if (Hierarchy.IsClonedRelatedUpdate == true)
                {
                    if (!(String.IsNullOrEmpty(Hierarchy.SelectedStep)) && (!String.IsNullOrEmpty(Hierarchy.Code)))
                    {
                        var Qry = new StringBuilder(string.Empty);
                        Qry.Append("SELECT ProjectCode FROM PE_Hierarchy where GroupId = '" + Hierarchy.GroupId + "' ");
                        DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(Qry.ToString(), CommandType.Text, null);
                        if (ResTable != null)
                        {
                            foreach (DataRow DataRow in ResTable.Rows)
                            {
                                if (!(DataRow["ProjectCode"] is System.DBNull))
                                {
                                    string Code = (string)DataRow["ProjectCode"];
                                    string CheckCodeAndStepStringAllRelated = null;
                                    CheckCodeAndStepStringAllRelated = CheckCodeAndStepForAllRelated(Code, Hierarchy.SelectedStep, Hierarchy.GroupId);
                                    if (!(String.IsNullOrEmpty(CheckCodeAndStepStringAllRelated)))
                                        return "110";
                                }
                                else if (DataRow["ProjectCode"] is System.DBNull)
                                {
                                    return "155";
                                }
                            }
                        }
                  }

                   
                }
              
                else
                {
                    string CheckCodeAndStepString = null;
                    CheckCodeAndStepString = CheckCodeAndStep(ref Hierarchy);
                    if (!(String.IsNullOrEmpty(CheckCodeAndStepString)))
                        return "110";
                } 

                // Prior to updating, check Name is unique within the same parent
                SB.Clear();
                SB.Append("SELECT COUNT(*) FROM PE_Hierarchy WHERE Name = '" + Hierarchy.Name + "' " );
                if (Hierarchy.IsCloned == false)
                {
                    SB.Append(" AND Id<> '" + Hierarchy.Id + "'  AND ParentId ");
                }
                else
                {
                    SB.Append(" AND ParentId ");
                }
                if (ParentObj == (object)DBNull.Value)
                {
                    SB.Append("IS NULL");
                }
                else
                {
                    SB.Append("= " + ParentObj);
                }
                object CountObj = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null);
                if (CountObj != null)
                {
                    Int16 Count = Convert.ToInt16(CountObj);
                    if (Count > 0)
                    {
                        return "100";
                    }
                }
                else
                    return "105";

                UpdateControlFields(ref Hierarchy); // Update Control fields
                SB.Clear();

                //get step FK according to description

                if (Hierarchy.IsNew) // New row; construct INSERT
                {

                    // Set Creation DateTime
                    Hierarchy.CreationDate = DateTime.Now;
                    // Build the Query
                    SB.Append("INSERT INTO PE_Hierarchy (ParentId, Sequence, NodeType, Name, Description, ProjectStep, ProjectCode, ProjectStatus, CreationDate, LastUpdateTime, LastUpdateUser, LastUpdateComputer, LastUpdateApplication, SCDUSASyncInd, IRISTechInd ) ");
                    SB.Append("VALUES (@ParentId, @Sequence, @NodeType, @Name, @Description, @ProjectStep, @ProjectCode, @ProjectStatus, @CreationDate, @LastUpdateTime, @LastUpdateUser, @LastUpdateComputer, @LastUpdateApplication, @SCDUSASyncInd, @IRISTechInd) ");
                    if (DatabaseSupportsBatchQueries)
                    {
                        SB.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                    }
                }

                else if (Hierarchy.IsNew == false && Hierarchy.IsClonedRelatedUpdate == false) // Existing row; construct UPDATE
                {

                    // Build the Query
                    SB.Append("UPDATE PE_Hierarchy SET ParentId=@ParentId, Sequence=@Sequence, NodeType=@NodeType, Name=@Name, Description=@Description, ");
                    SB.Append("ProjectStep=@ProjectStep, ProjectCode=@ProjectCode, ProjectStatus=@ProjectStatus, ");
                    SB.Append("LastUpdateTime=@LastUpdateTime, LastUpdateUser=@LastUpdateUser, LastUpdateComputer=@LastUpdateComputer, LastUpdateApplication=@LastUpdateApplication, SCDUSASyncInd=@SCDUSASyncInd, IRISTechInd=@IRISTechInd ");
                    SB.Append("WHERE Id=@Id");
                }

                else if (Hierarchy.IsNew == false && Hierarchy.IsClonedRelatedUpdate == true) // Existing row; construct UPDATE
                {
                    Hierarchy.GroupLastUpdateTime = DateTime.Now;
                    // Build the Query
                    SB.Append("UPDATE PE_Hierarchy SET ");
                    SB.Append("ProjectStep=@ProjectStep, ");
                    SB.Append("LastUpdateTime=@LastUpdateTime, LastUpdateUser=@LastUpdateUser, LastUpdateComputer=@LastUpdateComputer, LastUpdateApplication=@LastUpdateApplication ");
                    SB.Append("WHERE Id=@GroupId");
                }
                string step = GetStepCodeByName(Hierarchy.SelectedStep.Trim());
                if (Hierarchy.IsClonedRelatedUpdate == false && Hierarchy.GroupId != -1)
                {
                    //Update related project. The step is updating on group record.
                    step = string.Empty;
                }
                ProjectStatusEnum projectStatus = ProjectStatusEnum.O;
                if (string.IsNullOrEmpty(Hierarchy.ProjectStatus) || string.IsNullOrWhiteSpace(Hierarchy.ProjectStatus) || Hierarchy.ProjectStatus != "Disabled")
                {
                    projectStatus = ProjectStatusEnum.O;
                }
                else
                {
                    projectStatus = ProjectStatusEnum.D;
                }
                // Set the parameters
                CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "Id", DataType = DbType.Int32, Value = Hierarchy.Id },
                new ParamStruct { ParamName = "ParentId", DataType = DbType.Int32, Value = ParentObj },
                new ParamStruct { ParamName = "Sequence", DataType = DbType.Int16, Value = Hierarchy.Sequence },
                new ParamStruct { ParamName = "NodeType", DataType = DbType.String, Value = Hierarchy.NodeType.ToString() },
                new ParamStruct { ParamName = "Name", DataType = DbType.String, Value = Hierarchy.Name },
                new ParamStruct { ParamName = "Description", DataType = DbType.String, Value = Hierarchy.Description },
                new ParamStruct { ParamName = "ProjectStep", DataType = DbType.String, Value = (String.IsNullOrEmpty(step))? (object)DBNull.Value : step.Trim()},
                new ParamStruct { ParamName = "ProjectCode", DataType = DbType.String, Value = (String.IsNullOrEmpty(Hierarchy.Code))?(object)DBNull.Value :Hierarchy.Code},
                new ParamStruct { ParamName = "ProjectStatus", DataType = DbType.String, Value = projectStatus.ToString()},
                new ParamStruct { ParamName = "CreationDate", DataType = DbType.DateTime, Value = Hierarchy.CreationDate },
                new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = (Hierarchy.IsClonedRelatedUpdate)? Hierarchy.GroupLastUpdateTime : Hierarchy.LastUpdateTime },
                new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = Hierarchy.LastUpdateUser },
                new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = Hierarchy.LastUpdateComputer },
                new ParamStruct { ParamName = "LastUpdateApplication", DataType = DbType.String, Value = Hierarchy.LastUpdateApplication },
                new ParamStruct { ParamName = "SCDUSASyncInd", DataType = DbType.Boolean, Value = Hierarchy.Synchronization },
                new ParamStruct { ParamName = "IRISTechInd", DataType = DbType.Boolean, Value = Hierarchy.IRISTechInd },
                new ParamStruct { ParamName = "GroupId", DataType = DbType.Int32, Value = Hierarchy.GroupId }
                };

                long RV = 0;

                var VersionsQuery = new StringBuilder(string.Empty);

                if (Hierarchy.IsNew & DatabaseSupportsBatchQueries)
                {
                    object RVObj = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                    if (RVObj != null)
                    {
                        RV = Convert.ToInt64(RVObj);
                    }
                    else
                        return "105";
                }
                else
                {

                    RV = (long)Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                }
                // Finalize
                if (RV < 1) // Something went wrong... No rows were affected
                {
                    return "141";
                }
                else // All OK
                {
                    long RvVersions = 0;
                    bool IsVersionRequired = false;
                    // Update the object
                    if (Hierarchy.IsNew || Hierarchy.VM.IsClosed == true)
                    {
                        if (DatabaseSupportsBatchQueries)
                        {
                            if (Hierarchy.IsNew)
                            {
                                Hierarchy.Id = RV;
                            }
                            Hierarchy.VM.CreationDate = DateTime.Now;
                            // Build the Query
                            SB.Clear();
                            SB.Append("INSERT INTO PE_Version (HierarchyId, VersionName, VersionSeqNo, VersionStatus, Description, TargetPath, CreationDate, LastUpdateTime, LastUpdateUser, LastUpdateComputer, LastUpdateapplication, DefaultTargetPathInd, ECRId) ");
                            SB.Append("VALUES (@HierarchyId, @VersionName, @VersionSeqNo, @VersionStatus, @Description, @TargetPath, @CreationDate, @LastUpdateTime, @LastUpdateUser, @LastUpdateComputer, @LastUpdateapplication, @DefaultTargetPathInd , @ECRId) ");
                            if (DatabaseSupportsBatchQueries)
                            {
                                SB.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                            }

                        }
                        else // Mainly for SqlServerCE, which does not support ScopeIdentity
                        {

                            Hierarchy.Id = (int)Domain.PersistenceLayer.FetchDataValue("SELECT MAX(Id) FROM PE_Hierarchy", System.Data.CommandType.Text, null);
                            Hierarchy.VM.CreationDate = DateTime.Now;
                            // Build the Query
                            SB.Clear();
                            SB.Append("INSERT INTO PE_Version (HierarchyId, VersionName, VersionSeqNo, VersionStatus, Description, TargetPath, CreationDate, LastUpdateTime, LastUpdateUser, LastUpdateComputer, LastUpdateapplication, DefaultTargetPathInd, ECRId) ");
                            SB.Append("VALUES (@HierarchyId, @VersionName, @VersionSeqNo, @VersionStatus, @Description, @TargetPath, @CreationDate, @LastUpdateTime, @LastUpdateUser, @LastUpdateComputer, @LastUpdateapplication, @DefaultTargetPathInd , @ECRId) ");
                            if (DatabaseSupportsBatchQueries)
                            {
                                SB.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                            }
                        }
                        IsVersionRequired = true;
                    }
                    if (!Hierarchy.IsNew && Hierarchy.VM.IsClosed == false && Hierarchy.VM.IsDirty)
                    {
                        flgUpate = true;
                        SB.Clear();
                        SB.Append("UPDATE PE_Version SET VersionName=@VersionName, VersionSeqNo=@VersionSeqNo, VersionStatus=@VersionStatus, Description=@Description, TargetPath=@TargetPath, ");
                        SB.Append("CreationDate=@CreationDate, LastUpdateTime=@LastUpdateTime, LastUpdateUser=@LastUpdateUser, ");
                        SB.Append("LastUpdateComputer=@LastUpdateComputer, LastUpdateapplication=@LastUpdateapplication, DefaultTargetPathInd=@DefaultTargetPathInd , ECRId=@ECRId ");
                        SB.Append("WHERE VersionId=@VersionId");

                        IsVersionRequired = true;

                    }

                    if (IsVersionRequired)
                    {
                        
                        var VB = new StringBuilder(string.Empty);
                        VB.Append("SELECT VersionId FROM PE_Version WHERE VersionName = '" + Hierarchy.VM.VersionName + "' AND  HierarchyId ='" + Hierarchy.Id + "' AND VersionId <> '" + Hierarchy.VM.VersionId + "'");

                        long VBtemp = 0;
                        if (DatabaseSupportsBatchQueries)
                        {
                            object VBtempObj = Domain.PersistenceLayer.FetchDataValue(VB.ToString(), System.Data.CommandType.Text, null);
                            if (VBtempObj != null)
                            {
                                VBtemp = Convert.ToInt64(VBtempObj);
                            }
                        }
                        else
                        {
                            VBtemp = (long)Domain.PersistenceLayer.ExecuteDbCommand(VB.ToString(), System.Data.CommandType.Text, null);
                        }
                        if (VBtemp != 0)
                        {
                            var Error = new StringBuilder(string.Empty);
                            Error.Append("SELECT Description FROM PE_Messages where id = '124'");
                            return (Domain.PersistenceLayer.FetchDataValue(Error.ToString(), CommandType.Text, null)).ToString();
                        }


                        CommandParamsVersion = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "VersionId", DataType = DbType.Int32, Value = Hierarchy.VM.VersionId },
                new ParamStruct { ParamName = "HierarchyId", DataType = DbType.Int32, Value = (Hierarchy.IsClonedRelatedUpdate) ? Hierarchy.GroupId : Hierarchy.Id },
                new ParamStruct { ParamName = "VersionName", DataType = DbType.String, Value = Hierarchy.VM.VersionName.Trim() },
                new ParamStruct { ParamName = "VersionSeqNo", DataType = DbType.Int32, Value = Hierarchy.VM.Sequence },
                new ParamStruct { ParamName = "VersionStatus", DataType = DbType.String, Value = 'A' },
                new ParamStruct { ParamName = "Description", DataType = DbType.String, Value = Hierarchy.VM.Description.Trim() },
                new ParamStruct { ParamName = "TargetPath", DataType = DbType.String, Value = Hierarchy.VM.TargetPath.Trim()},
                new ParamStruct { ParamName = "CreationDate", DataType = DbType.DateTime, Value = Hierarchy.VM.CreationDate },
                new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = Hierarchy.LastUpdateTime },
                new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = Hierarchy.LastUpdateUser },
                new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = Hierarchy.LastUpdateComputer },
                new ParamStruct { ParamName = "LastUpdateApplication", DataType = DbType.String, Value = Hierarchy.LastUpdateApplication },
                new ParamStruct { ParamName = "DefaultTargetPathInd", DataType = DbType.Boolean, Value = Hierarchy.VM.DefaultTargetPathInd },
                new ParamStruct { ParamName = "ECRId", DataType = DbType.String, Value = Hierarchy.VM.EcrId }
                };
                        
                        if (DatabaseSupportsBatchQueries && flgUpate == false)
                        {
                            object RvVersionsObj = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, CommandParamsVersion.ToArray());
                            if (RvVersionsObj != null)
                            {
                                RvVersions = Convert.ToInt64(RvVersionsObj);
                                Hierarchy.VM.VersionId = RvVersions;
                            }
                            else
                                return "105";

                        }
                        if (flgUpate == true)
                        {
                            RvVersions = (long)Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParamsVersion.ToArray());
                        }

                      

                        if (RvVersions < 1) // Something went wrong... No rows were affected
                        {
                            //return "Failed to update database";
                            return "141";
                        }
                    }
                    if (Hierarchy.Certificates.Count > 0)
                    {

                        List<string> newCertificateToInsert = new List<string>();

                        // Build The Query String
                        System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                        string status = NoteStatusTypes.D.ToString();

                        QryStr.Append("SELECT HierarchyId, CertificateId FROM PE_FolderCertificate WHERE HierarchyId = " + Hierarchy.Id + " ");

                        // Fetch the DataTable from the database
                        DataTable ResTableNote = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                        // Populate the collection
                        if (ResTableNote != null)
                        {
                            if (ResTableNote.Rows.Count > 0)
                            {
                                List<string> tempCertificateList = new List<string>();
                                foreach (DataRow DataRow in ResTableNote.Rows)
                                {
                                    string tempCer = (string)DataRow["CertificateId"];
                                    tempCertificateList.Add(tempCer);
                                }
                                foreach (string cer in Hierarchy.Certificates)
                                {
                                    if (!tempCertificateList.Contains(cer))
                                    {
                                        newCertificateToInsert.Add(cer);
                                    }
                                }
                            }
                            else
                            {
                                newCertificateToInsert = Hierarchy.Certificates;
                            }
                        }

                        foreach (String I in newCertificateToInsert)
                        {
                            var InsertCert = new StringBuilder(string.Empty);
                            InsertCert.Append("INSERT INTO PE_FolderCertificate (HierarchyId, CertificateId, EffectiveDate, ExpirationDate, LastUpdateTime, LastUpdateUser, LastUpdateComputer, LastUpdateAppliation) ");
                            InsertCert.Append("VALUES (@HierarchyId, @CertificateId, @EffectiveDate, @ExpirationDate, @LastUpdateTime, @LastUpdateUser, @LastUpdateComputer, @LastUpdateAppliation) ");
                            if (DatabaseSupportsBatchQueries)
                            {
                                InsertCert.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                            }

                            CommandParams = new List<ParamStruct>()
                        {
                        new ParamStruct { ParamName = "HierarchyId", DataType = DbType.Int32, Value = Hierarchy.Id },
                        new ParamStruct { ParamName = "CertificateId", DataType = DbType.String, Value = I.ToString().Trim()},
                        new ParamStruct { ParamName = "EffectiveDate", DataType = DbType.DateTime, Value = DateTime.Now },
                        new ParamStruct { ParamName = "ExpirationDate", DataType = DbType.DateTime, Value = (object)DBNull.Value },
                        new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = DateTime.Now },
                        new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = Domain.User },
                        new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = Domain.Workstn },
                        new ParamStruct { ParamName = "LastUpdateAppliation", DataType = DbType.String, Value = Domain.AppName },
                        
                        };
                            long CertID = 0;
                            if (DatabaseSupportsBatchQueries)
                            {
                                object CertIDObj = Domain.PersistenceLayer.FetchDataValue(InsertCert.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                                if (CertIDObj != null)
                                {
                                    CertID = Convert.ToInt64(CertIDObj);
                                }

                            }
                            else
                            {
                                CertID = (long)Domain.PersistenceLayer.ExecuteDbCommand(InsertCert.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                            }
                            if (CertID < 1) // Something went wrong... No rows were affected
                            {
                                //return "Failed to update database";
                                return "141";
                            }
                        }
                    }

                    //Fixed bug 3046 - added IsDirty condition
                    if ((Hierarchy.VM.Contents.Count > 0 && Hierarchy.VM.IsDirty == true) || (Hierarchy.VM.Contents.Count > 0 && Hierarchy.IsCloned)) 
                    {
                        //Boolean cert = CheckCertificateList(Hierarchy.Certificates);
                        int contentIndex = 0;
                        foreach (var I in Hierarchy.VM.Contents)
                        {

                            var InsertCert = new StringBuilder(string.Empty);
                            InsertCert.Append("INSERT INTO PE_VersionContent (VersionId, ContentVersionId, ContentSeqNo, CreationDate, LastUpdateTime, LastUpdateUser, LastUpdateComputer, LastUpdateApplication) ");
                            InsertCert.Append("VALUES (@VersionId, @ContentVersionId, @ContentSeqNo, @CreationDate, @LastUpdateTime, @LastUpdateUser, @LastUpdateComputer, @LastUpdateApplication) ");
                            if (DatabaseSupportsBatchQueries)
                            {
                                InsertCert.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                            }

                            CommandParams = new List<ParamStruct>()
                        {
                        new ParamStruct { ParamName = "VersionId", DataType = DbType.Int32, Value =  flgUpate ? Hierarchy.VM.VersionId : RvVersions},
                        new ParamStruct { ParamName = "ContentVersionId", DataType = DbType.Int32, Value = I.id},
                        new ParamStruct { ParamName = "ContentSeqNo", DataType = DbType.Int32, Value = Hierarchy.VM.Contents[contentIndex].seq },
                        new ParamStruct { ParamName = "CreationDate", DataType = DbType.DateTime, Value =  DateTime.Now },
                        new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = DateTime.Now },
                        new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = Domain.User },
                        new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = Domain.Workstn },
                        new ParamStruct { ParamName = "LastUpdateApplication", DataType = DbType.String, Value = Domain.AppName },
                        
                        };
                            long ContID = 0;
                            if (DatabaseSupportsBatchQueries)
                            {
                                object ContIDObj = Domain.PersistenceLayer.FetchDataValue(InsertCert.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                                if (ContIDObj != null)
                                {
                                    ContID = Convert.ToInt64(ContIDObj);
                                }
                               
                            }
                            else
                            {
                                ContID = (long)Domain.PersistenceLayer.ExecuteDbCommand(InsertCert.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                            }
                            if (ContID < 1) // Something went wrong... No rows were affected
                            {
                                //return "Failed to update database";
                                return "141";
                            }
                            contentIndex++;
                        }
                    }
                    if (Hierarchy.UserCertificates.Count > 0)
                    {
                        foreach (var i in Hierarchy.UserCertificates)
                        {
                            if (i.IsNew || Hierarchy.IsCloned)
                            {

                                var InsertCert = new StringBuilder(string.Empty);
                                InsertCert.Append("INSERT INTO PE_FolderUserCertificate (HierarchyId, UserCertificateId, EffectiveDate, ExpirationDate, LastUpdateTime, LastUpdateUser, LastUpdateComputer, LastUpdateAppliation) ");
                                InsertCert.Append("VALUES (@HierarchyId, @UserCertificateId, @EffectiveDate, @ExpirationDate, @LastUpdateTime, @LastUpdateUser, @LastUpdateComputer, @LastUpdateAppliation) ");
                                if (DatabaseSupportsBatchQueries)
                                {
                                    InsertCert.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                                }

                                i.LastUpdateTime = DateTime.Now;
                                CommandParams.Clear();
                                CommandParams = new List<ParamStruct>()
                                    {
                                    new ParamStruct { ParamName = "HierarchyId", DataType = DbType.Int32, Value = Hierarchy.Id },
                                    new ParamStruct { ParamName = "UserCertificateId", DataType = DbType.String, Value = i.UserCertificateId},
                                    new ParamStruct { ParamName = "EffectiveDate", DataType = DbType.DateTime, Value = DateTime.Now },
                                    new ParamStruct { ParamName = "ExpirationDate", DataType = DbType.DateTime, Value = (object)DBNull.Value },
                                    new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = i.LastUpdateTime },
                                    new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = Domain.User },
                                    new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = Domain.Workstn },
                                    new ParamStruct { ParamName = "LastUpdateAppliation", DataType = DbType.String, Value = Domain.AppName },
                        
                                    };
                                long CertID = 0;
                                if (DatabaseSupportsBatchQueries)
                                {
                                    object CertIDObj = Domain.PersistenceLayer.FetchDataValue(InsertCert.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                                    if (CertIDObj != null)
                                    {
                                        CertID = Convert.ToInt64(CertIDObj);
                                    }

                                }
                                else
                                {
                                    CertID = (long)Domain.PersistenceLayer.ExecuteDbCommand(InsertCert.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                                }
                                if (CertID < 1) // Something went wrong... No rows were affected
                                {
                                    //return "Failed to update database";
                                    return "141";
                                }
                                i.IsNew = false;
                            }
                        }
                    }
                    Hierarchy.VM.IsNew = false;
                    Hierarchy.VM.IsClosed = false;
                    Hierarchy.IsDirty = false;
                    Hierarchy.IsNew = false;
                    Hierarchy.IsLoading = false;
                    Hierarchy.VM.IsDirty = false;
                    return string.Empty;
                }
            }
           
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("DB Error");
            }
        }

        #endregion               
        
        #region Update Project Status

        public static long UpdateProjectStatus(long HierarchyId, string projectStatus)
        {
            try
            {
                string Qry = "Update PE_Hierarchy set PE_Hierarchy.ProjectStatus='" + projectStatus + "' where PE_Hierarchy.Id='" + HierarchyId.ToString().Trim() + "';";
                return (long)Domain.PersistenceLayer.ExecuteDbCommand(Qry, CommandType.Text, null);
            }
            catch (Exception e)
            {

                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                throw new Exception("DB Error");
            }

        }

        #endregion

        #region GetStepCodeByName

        public static string GetStepCodeByName(string Name)
        {
                if (String.IsNullOrEmpty(Name.Trim()))
                {
                    return null;
                }
                else
                {
                    string step = null;
                    var StepQry = new StringBuilder(string.Empty);
                    StepQry.Append("select StepCode from PE_ProjectStep where StepDescription = '" + Name.Trim() + "'");
                    object stepObj = Domain.PersistenceLayer.FetchDataValue(StepQry.ToString(), System.Data.CommandType.Text, null);
                    if (stepObj != null)
                    {
                        step = (string)stepObj;
                    }
                    return step;
                }   
        }
        #endregion GetStepNameByCode

        #region GetStepNameByCode
        public static string GetStepNameByCode(string Code)
        {
            string stepDescription = null;

            //Performance#8
            //var StepQry = new StringBuilder(string.Empty);
            //StepQry.Append("select StepDescription from PE_ProjectStep where StepCode = '" + Code.Trim() + "'");
            //object stepDescriptionObj = Domain.PersistenceLayer.FetchDataValue(StepQry.ToString(), System.Data.CommandType.Text, null);
            //if (stepDescriptionObj != null)
            //{
            //     stepDescription = (string)stepDescriptionObj;
            //}

            string selectCondition = "StepCode ='" + Code.Trim() + "'";

            if (ProjectStepTable == null || ProjectStepTable.Rows.Count == 0)
            {
                ProjectStepTable = GetProjectStepDataTable();
            }

            DataRow[] stepRow = ProjectStepTable.Select(selectCondition);
            if (stepRow != null && stepRow.Length > 0)
            {
                stepDescription = Convert.ToString(stepRow[0]["StepDescription"]);
            }
            return stepDescription;
        }
        #endregion GetStepNameByCode                               

        #region Update Note Control Fields

        public static void UpdateControlFields(ref HierarchyModel Hierarchy)
        {
            if (!Hierarchy.IsClonedRelatedUpdate)
            {
                Hierarchy.LastUpdateTime = DateTime.Now;
            }
            Hierarchy.LastUpdateApplication = Domain.AppName;
            Hierarchy.LastUpdateComputer = Domain.Workstn;
            Hierarchy.LastUpdateUser = Domain.User;
            Hierarchy.VM.LastUpdateApplication = Domain.AppName;
            Hierarchy.VM.LastUpdateComputer = Domain.Workstn;
            Hierarchy.VM.LastUpdateUser = Domain.User;
            Hierarchy.VM.LastUpdateTime = DateTime.Now;
        }

        #endregion

        #region PersistClosedVersion
        public static String PersistClosedVersion(ref HierarchyModel Hierarchy)
        {
            try
            {
                var VB = new StringBuilder(string.Empty);

                VB.Append("SELECT VersionId FROM PE_Version WHERE VersionName = '" + Hierarchy.VM.VersionName.Trim() + "' ");
                if (Hierarchy.GroupId == -1)
                {
                    VB.Append(" AND  HierarchyId ='" + Hierarchy.Id + "' ");
                }
                else
                {
                    VB.Append(" AND  HierarchyId ='" + Hierarchy.GroupId + "' ");
                }
                long VBtemp = 0;
                object VBtempOBj = Domain.PersistenceLayer.FetchDataValue(VB.ToString(), CommandType.Text, null);
                if (VBtempOBj != null)
                {
                    VBtemp = Convert.ToInt64(VBtempOBj);
                }

                if (VBtemp > 0)
                {
                    return "124";
                }
                if (Hierarchy.VM.VersionId != -1)
                {
                    long updateVersion = 0;
                    string Qry = "Update PE_Version set PE_Version.VersionStatus='C' where PE_Version.VersionId='" + Hierarchy.VM.VersionId.ToString().Trim() + "';";
                    //updateVersion = Convert.ToInt64(Domain.PersistenceLayer.FetchDataValue(Qry.ToString(), CommandType.Text, null));
                    updateVersion = (long)Domain.PersistenceLayer.ExecuteDbCommand(Qry, CommandType.Text, null);
                    if (updateVersion != 0)
                    {
                        return string.Empty;
                    }
                    else
                        return "105";
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
        #endregion PersistClosedVersion

        #region Related project Managment

        #region GetGroupIDFromHierarchy
        public static HierarchyModel GetGroupIDFromHierarchy(HierarchyModel Hierarchy)
        {

            System.Text.StringBuilder groupQur = new System.Text.StringBuilder();
            groupQur.Append("select  hg.GroupId as GroupId, h.Name as GroupName, h.Description as GroupDescription, h.ProjectStep as ProjectStep " +
            "from PE_Hierarchy as h inner join PE_Hierarchy as hg on h.Id = hg.GroupId " +
              "where hg.id='" + Hierarchy.Id.ToString().Trim() + "' and hg.GroupId is not null");

            DataTable GroupData = Domain.PersistenceLayer.FetchDataTable(groupQur.ToString(), CommandType.Text, null);
            //var a = Domain.PersistenceLayer.FetchDataValue(groupQur.ToString(), System.Data.CommandType.Text, null);

            if (GroupData != null)
            {
                if (GroupData.Rows.Count > 0)
                {
                    if (!(GroupData.Rows[0]["GroupId"] is System.DBNull))
                    {
                        Hierarchy.GroupId = (int)GroupData.Rows[0]["GroupId"];
                        Hierarchy.GroupName = (string)GroupData.Rows[0]["GroupName"];
                        Hierarchy.GroupDescription = (string)GroupData.Rows[0]["GroupDescription"];
                        if (!(GroupData.Rows[0]["ProjectStep"] is System.DBNull))
                        {
                            string stepName = GetStepNameByCode((string)GroupData.Rows[0]["ProjectStep"]);
                            Hierarchy.SelectedStep = stepName.ToString();
                        }

                    }
                }
            }

            return Hierarchy;
        }
        #endregion GetGroupIDFromHierarchy

        public static Boolean getGroupName(string GroupName)
        {
            try
            {
                Int16 Count = -1;
                System.Text.StringBuilder groupNameSB = new System.Text.StringBuilder();
                groupNameSB.Append("select COUNT(*) FROM PE_Hierarchy where NodeType ='G' and Name ='" + GroupName.Trim() + "' ");

                object CountObj = Domain.PersistenceLayer.FetchDataValue(groupNameSB.ToString(), System.Data.CommandType.Text, null);
                if (CountObj != null)
                {
                    Count = Convert.ToInt16(CountObj);
                }
                if (Count > 0)
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error

                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("DB Error");
            }
        }


        public static string LastUpadateCheck(ref HierarchyModel HM)
        {
            try
            {
                if (!HM.IsNew)
                {
                    System.Text.StringBuilder LastUpdateSB = new System.Text.StringBuilder();
                    LastUpdateSB.Append("SELECT LastUpdateTime FROM PE_Hierarchy WHERE Id = ");
                    LastUpdateSB.Append(HM.Id);
                    object LastDateObj = Domain.PersistenceLayer.FetchDataValue(LastUpdateSB.ToString(), System.Data.CommandType.Text, null);
                    if (LastDateObj != null)
                    {
                        string LastDate = Convert.ToDateTime(LastDateObj).ToString();
                        string ObjDate = HM.LastUpdateTime.ToString();
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

        public static string LastUpadateGroupCheck(ref HierarchyModel HM)
        {
            try
            {
                if (!HM.IsNew)
                {
                    System.Text.StringBuilder LastUpdateSB = new System.Text.StringBuilder();
                    LastUpdateSB.Append("SELECT LastUpdateTime FROM PE_Hierarchy WHERE Id = ");
                    LastUpdateSB.Append(HM.GroupId);
                    object LastDateObj = Domain.PersistenceLayer.FetchDataValue(LastUpdateSB.ToString(), System.Data.CommandType.Text, null);
                    if (LastDateObj != null)
                    {
                        string LastDate = Convert.ToDateTime(LastDateObj).ToString();
                        string ObjDate = HM.GroupLastUpdateTime.ToString();
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

        public static string PresistRelatedProject(ref HierarchyModel HM)
        {
            var SB = new StringBuilder(string.Empty);
            List<ParamStruct> CommandParams;
            Boolean DatabaseSupportsBatchQueries = Domain.PersistenceLayer.GetSupportsBatchQueries();
            // Handle Null Parent
          
            Object ParentObj = (object)DBNull.Value;


            // Do work
            try
            {
                long RV = -1;

                long RVH = -1;
                string CheckCodeAndStepString = null;
                CheckCodeAndStepString = CheckCodeAndStep(ref HM);
                if (!(String.IsNullOrEmpty(CheckCodeAndStepString)))
                    return "110";


                
                SB.Append("SELECT COUNT(*) FROM PE_Hierarchy WHERE Name = '" + HM.Name + "'  AND ParentId ");
                if (ParentObj == (object)DBNull.Value)
                {
                    SB.Append("IS NULL");
                }
                else
                {
                    SB.Append("= " + ParentObj);
                }
                object CountObj = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null);
                if (CountObj != null)
                {
                    Int16 Count = Convert.ToInt16(CountObj);
                    if (Count > 0)
                    {
                        return "100";
                    }
                }

                if (HM.ParentId > 0)
                {
                    ParentObj = HM.ParentId;
                }
                // Prior to updating, check if object has changed since it was loaded and alert the user if it has
                UpdateControlFields(ref HM); // Update Control fields
                SB.Clear();

                // Set Creation DateTime
                HM.CreationDate = DateTime.Now;
                // Build the Query
                SB.Append("INSERT INTO PE_Hierarchy (NodeType, Name, Description, ProjectStep, CreationDate, LastUpdateTime, LastUpdateUser, LastUpdateComputer, LastUpdateApplication) ");
                SB.Append("VALUES (@NodeType, @Name, @Description, @ProjectStep, @CreationDate, @LastUpdateTime, @LastUpdateUser, @LastUpdateComputer, @LastUpdateApplication) ");
                if (DatabaseSupportsBatchQueries)
                {
                    SB.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                }
                // Set the parameters

                string step = GetStepCodeByName(HM.SelectedStep.Trim());
                HM.GroupLastUpdateTime = DateTime.Now;

                ;
                CommandParams = new List<ParamStruct>()
                    {
                    new ParamStruct { ParamName = "NodeType", DataType = DbType.String, Value = 'G' },
                    new ParamStruct { ParamName = "Name", DataType = DbType.String, Value = HM.GroupName },
                    new ParamStruct { ParamName = "Description", DataType = DbType.String, Value = HM.GroupDescription },
                    new ParamStruct { ParamName = "ProjectStep", DataType = DbType.String, Value = (String.IsNullOrEmpty(step))? (object)DBNull.Value : step.Trim()},
                    new ParamStruct { ParamName = "CreationDate", DataType = DbType.DateTime, Value = HM.CreationDate },
                    new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = HM.GroupLastUpdateTime },
                    new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = HM.LastUpdateUser },
                    new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = HM.LastUpdateComputer },
                    new ParamStruct { ParamName = "LastUpdateApplication", DataType = DbType.String, Value = HM.LastUpdateApplication },
          
                    };



                if (DatabaseSupportsBatchQueries)
                {
                    object RVObj = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                    if (RVObj != null)
                    {
                        RV = Convert.ToInt64(RVObj);
                    }
                    else
                        return "105";
                }
                else
                {

                    RV = (long)Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                }
                // Finalize
                if (RV < 1) // Something went wrong... No rows were affected
                {
                    return "141";
                }
                //All Ok

                else
                {
                   

                    SB.Clear();
                    HM.CreationDate = DateTime.Now;


                    // Build the Query
                    SB.Append("INSERT INTO PE_Hierarchy (ParentId, Sequence, NodeType, Name, Description, ProjectCode, ProjectStatus, GroupId, CreationDate, LastUpdateTime, LastUpdateUser, LastUpdateComputer, LastUpdateApplication, SCDUSASyncInd, IRISTechInd) ");
                    SB.Append("VALUES (@ParentId, @Sequence, @NodeType, @Name, @Description, @ProjectCode, @ProjectStatus, @GroupId, @CreationDate, @LastUpdateTime, @LastUpdateUser, @LastUpdateComputer, @LastUpdateApplication, @SCDUSASyncInd, @IRISTechInd) ");
                    if (DatabaseSupportsBatchQueries)
                    {
                        SB.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                    }
                    CommandParams.Clear();
                    CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "ParentId", DataType = DbType.Int32, Value = ParentObj },
                new ParamStruct { ParamName = "Sequence", DataType = DbType.Int16, Value = HM.Sequence },
                new ParamStruct { ParamName = "NodeType", DataType = DbType.String, Value = HM.NodeType.ToString() },
                new ParamStruct { ParamName = "Name", DataType = DbType.String, Value = HM.Name },
                new ParamStruct { ParamName = "Description", DataType = DbType.String, Value = HM.Description },
                new ParamStruct { ParamName = "ProjectCode", DataType = DbType.String, Value = (String.IsNullOrEmpty(HM.Code))?(object)DBNull.Value :HM.Code},
                new ParamStruct { ParamName = "ProjectStatus", DataType = DbType.String, Value = ProjectStatusEnum.O.ToString()},
                new ParamStruct { ParamName = "GroupId", DataType = DbType.Int32, Value = RV },
                new ParamStruct { ParamName = "CreationDate", DataType = DbType.DateTime, Value = HM.CreationDate },
                new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = HM.LastUpdateTime },
                new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = HM.LastUpdateUser },
                new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = HM.LastUpdateComputer },
                new ParamStruct { ParamName = "LastUpdateApplication", DataType = DbType.String, Value = HM.LastUpdateApplication },
                new ParamStruct { ParamName = "SCDUSASyncInd", DataType = DbType.Boolean, Value = HM.Synchronization },
                new ParamStruct { ParamName = "IRISTechInd", DataType = DbType.Boolean, Value = HM.IRISTechInd }
                };



                    if (DatabaseSupportsBatchQueries)
                    {
                        object RVHObj;
                        try
                        {
                            RVHObj = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                        }
                        catch(Exception e) {
                            return "100"; //name alredy exists
                        }
                        if (RVHObj != null)
                        {
                            RVH = Convert.ToInt64(RVHObj);
                        }
                        else
                            return "105";
                    }
                    else
                    {

                        RVH = (long)Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                    }
                    // Finalize
                    if (RVH < 1) // Something went wrong... No rows were affected
                    {
                        return "141";
                    }
                    //All OK
                    else
                    {
                        
                        SB.Clear();
                        //string Qry = "Update PE_Hierarchy set PE_Hierarchy.GroupId ='" + RV + "' and ProjectStep =" + StepH + " where PE_Hierarchy.Id='" + HM.ToString().Trim() + "';";
                        SB.Append("UPDATE PE_Hierarchy SET GroupId=@GroupId,  ");
                        SB.Append("ProjectStep=@ProjectStep,  ");
                        SB.Append("LastUpdateTime=@LastUpdateTime, LastUpdateUser=@LastUpdateUser, LastUpdateComputer=@LastUpdateComputer, LastUpdateApplication=@LastUpdateApplication ");
                        SB.Append("WHERE Id=@Id");

                        CommandParams.Clear();
                        CommandParams = new List<ParamStruct>()
                {
                    new ParamStruct { ParamName = "Id", DataType = DbType.Int32, Value = HM.Id },
                    new ParamStruct { ParamName = "GroupId", DataType = DbType.Int32, Value = RV },
                    new ParamStruct { ParamName = "ProjectStep", DataType = DbType.String, Value = (object)DBNull.Value},
                    new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = HM.LastUpdateTime },
                    new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = HM.LastUpdateUser },
                    new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = HM.LastUpdateComputer },
                    new ParamStruct { ParamName = "LastUpdateApplication", DataType = DbType.String, Value = HM.LastUpdateApplication },
             
                };
                        long UP = 0;


                        UP = (long)Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());

                        if (UP < 1) // Something went wrong... No rows were affected
                        {
                            return "141";
                        }
                        //All Ok
                        else
                        {
                            long updateVersion = 0;
                            SB.Clear();
                            SB.Append("UPDATE PE_Version SET TargetPath=@TargetPath, HierarchyId=@HierarchyId, ");
                            SB.Append(" LastUpdateTime=@LastUpdateTime, LastUpdateUser=@LastUpdateUser, ");
                            SB.Append("LastUpdateComputer=@LastUpdateComputer, LastUpdateapplication=@LastUpdateapplication, DefaultTargetPathInd=@DefaultTargetPathInd ");
                            SB.Append("WHERE VersionId=@VersionId");

                            CommandParams.Clear();
                            CommandParams = new List<ParamStruct>()
                            {
                            new ParamStruct { ParamName = "VersionId", DataType = DbType.Int32, Value = HM.VM.VersionId },
                            new ParamStruct { ParamName = "HierarchyId", DataType = DbType.Int32, Value = RV },
                            new ParamStruct { ParamName = "TargetPath", DataType = DbType.String, Value = HM.VM.TargetPath.Trim()},
                            new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = HM.VM.LastUpdateTime },
                            new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = HM.VM.LastUpdateUser },
                            new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = HM.VM.LastUpdateComputer },
                            new ParamStruct { ParamName = "LastUpdateapplication", DataType = DbType.String, Value = HM.VM.LastUpdateApplication },
                            new ParamStruct { ParamName = "DefaultTargetPathInd", DataType = DbType.Boolean, Value = HM.VM.DefaultTargetPathInd }
                            };

                            updateVersion = (long)Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());

                            if (updateVersion == 0)
                            {
                                return "141";
                            }

                            //ALL OK
                            else
                            {

                                SB.Clear();
                                SB.Append("SELECT COUNT(*) FROM PE_Note where HierarchyId ='" + HM.Id + "' ");

                                Int16 CountNote = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));
                                if (CountNote > 0)
                                {
                                    long updateNote = 0;

                                    string QryNote = "Update PE_Note set PE_Note.HierarchyId = '" + RV + "' where HierarchyId = '" + HM.Id + "';";
                                    //updateVersion = Convert.ToInt64(Domain.PersistenceLayer.FetchDataValue(Qry.ToString(), CommandType.Text, null));
                                    updateNote = (long)Domain.PersistenceLayer.ExecuteDbCommand(QryNote, CommandType.Text, null);
                                    if (updateNote == 0)
                                    {
                                        return "141";
                                    }
                                }
                                else
                                {
                                    SB.Clear();
                                    SB.Append("SELECT COUNT(*) FROM PE_Version where HierarchyId ='" + HM.Id + "' ");

                                    Int16 CountVersionNum = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));
                                    if (CountVersionNum > 0)
                                    {
                                        long updateClosV = 0;

                                        string QryCloseVer = "Update PE_Version set PE_Version.HierarchyId = '" + RV + "' where HierarchyId = '" + HM.Id + "';";
                                        //updateVersion = Convert.ToInt64(Domain.PersistenceLayer.FetchDataValue(Qry.ToString(), CommandType.Text, null));
                                        updateClosV = (long)Domain.PersistenceLayer.ExecuteDbCommand(QryCloseVer, CommandType.Text, null);
                                        if (updateClosV == 0)
                                        {
                                            return "141";
                                        }
                                    }
                                }

                                if (HM.Certificates.Count > 0)
                                {
                                    foreach (String I in HM.Certificates)
                                    {
                                        var InsertCert = new StringBuilder(string.Empty);
                                        InsertCert.Append("INSERT INTO PE_FolderCertificate (HierarchyId, CertificateId, EffectiveDate, ExpirationDate, LastUpdateTime, LastUpdateUser, LastUpdateComputer, LastUpdateAppliation) ");
                                        InsertCert.Append("VALUES (@HierarchyId, @CertificateId, @EffectiveDate, @ExpirationDate, @LastUpdateTime, @LastUpdateUser, @LastUpdateComputer, @LastUpdateAppliation) ");
                                        if (DatabaseSupportsBatchQueries)
                                        {
                                            InsertCert.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                                        }

                                        CommandParams = new List<ParamStruct>()
                                        {
                                        new ParamStruct { ParamName = "HierarchyId", DataType = DbType.Int32, Value = RVH },
                                        new ParamStruct { ParamName = "CertificateId", DataType = DbType.String, Value = I.ToString().Trim()},
                                        new ParamStruct { ParamName = "EffectiveDate", DataType = DbType.DateTime, Value = DateTime.Now },
                                        new ParamStruct { ParamName = "ExpirationDate", DataType = DbType.DateTime, Value = (object)DBNull.Value },
                                        new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = DateTime.Now },
                                        new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = Domain.User },
                                        new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = Domain.Workstn },
                                        new ParamStruct { ParamName = "LastUpdateAppliation", DataType = DbType.String, Value = Domain.AppName },
                        
                                        };
                                        long CertID = 0;
                                        if (DatabaseSupportsBatchQueries)
                                        {
                                            object CertIDObj = Domain.PersistenceLayer.FetchDataValue(InsertCert.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                                            if (CertIDObj != null)
                                            {
                                                CertID = Convert.ToInt64(CertIDObj);
                                            }
                                        }
                                        else
                                        {
                                            CertID = (long)Domain.PersistenceLayer.ExecuteDbCommand(InsertCert.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                                        }
                                        if (CertID < 1) // Something went wrong... No rows were affected
                                        {
                                            //return "Failed to update database";
                                            return "141";
                                        }
                                    }
                                }
                                if (HM.UserCertificates.Count > 0)
                                {
                                    foreach (UserCertificateApiModel I in HM.UserCertificates)
                                    {
                                        var InsertUserCert = new StringBuilder(string.Empty);
                                        InsertUserCert.Append("INSERT INTO PE_FolderUserCertificate (HierarchyId, UserCertificateId, EffectiveDate, ExpirationDate, LastUpdateTime, LastUpdateUser, LastUpdateComputer, LastUpdateAppliation) ");
                                        InsertUserCert.Append("VALUES (@HierarchyId, @UserCertificateId, @EffectiveDate, @ExpirationDate, @LastUpdateTime, @LastUpdateUser, @LastUpdateComputer, @LastUpdateAppliation) ");
                                        if (DatabaseSupportsBatchQueries)
                                        {
                                            InsertUserCert.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                                        }

                                        // I.LastUpdateTime = DateTime.Now;
                                        CommandParams.Clear();
                                        CommandParams = new List<ParamStruct>()
                                        {
                                            new ParamStruct { ParamName = "HierarchyId", DataType = DbType.Int32, Value = RVH },
                                            new ParamStruct { ParamName = "UserCertificateId", DataType = DbType.String, Value = I.UserCertificateId},
                                            new ParamStruct { ParamName = "EffectiveDate", DataType = DbType.DateTime, Value = DateTime.Now },
                                            new ParamStruct { ParamName = "ExpirationDate", DataType = DbType.DateTime, Value = (object)DBNull.Value },
                                            new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = DateTime.Now },
                                            new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = Domain.User },
                                            new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = Domain.Workstn },
                                            new ParamStruct { ParamName = "LastUpdateAppliation", DataType = DbType.String, Value = Domain.AppName },
                        

                                        };
                                        long UserCertID = 0;
                                        if (DatabaseSupportsBatchQueries)
                                        {
                                            object UserCertIDObj = Domain.PersistenceLayer.FetchDataValue(InsertUserCert.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                                            if (UserCertIDObj != null)
                                            {
                                                UserCertID = Convert.ToInt64(UserCertIDObj);
                                            }
                                        }
                                        else
                                        {
                                            UserCertID = (long)Domain.PersistenceLayer.ExecuteDbCommand(InsertUserCert.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                                        }
                                        if (UserCertID < 1) // Something went wrong... No rows were affected
                                        {
                                            //return "Failed to update database";
                                            return "141";
                                        }


                                    }//end of foreach user certificates
                                }

                            }

                        }
                    }

                }
                HM.VM.IsDirty = false;
                HM.VM.IsNew = false;
                HM.Id = RVH;
                HM.GroupId = RV;
                HM.IsDirty = false;
                HM.IsClonedRelated = false;
                return string.Empty;
            }

            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("DB Error");
             

            }
        }



        public static string UpdateRelatedProject(ref HierarchyModel HM)
        {
            var SB = new StringBuilder(string.Empty);
            List<ParamStruct> CommandParams;
            Boolean DatabaseSupportsBatchQueries = Domain.PersistenceLayer.GetSupportsBatchQueries();
          


            // Do work
            try
            {
              string CheckCodeAndStepString = null;
              long HierarchyId = HM.Id;
              //initialize Id for code and step check.
              HM.Id = -1;
                CheckCodeAndStepString = CheckCodeAndStep(ref HM);
                if (!(String.IsNullOrEmpty(CheckCodeAndStepString)))
                    return "110";

                //returning correct value id. 
                HM.Id = HierarchyId;
                UpdateControlFields(ref HM);
                HM.CreationDate = DateTime.Now;
                object ParentObj = (object)DBNull.Value;
                if (HM.ParentId > 0)
                {
                    ParentObj = HM.ParentId;
                }

                SB.Clear();
                SB.Append("SELECT COUNT(*) FROM PE_Hierarchy WHERE Name = '" + HM.Name + "' AND Id<> '" + HM.Id + "'  AND ParentId ");
                if (ParentObj == (object)DBNull.Value)
                {
                    SB.Append("IS NULL");
                }
                else
                {
                    SB.Append("= " + ParentObj);
                }
                object CountObj = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null);
                if (CountObj != null)
                {
                    Int16 Count = Convert.ToInt16(CountObj);
                    if (Count > 0)
                    {
                        return "100";
                    }
                }
                SB.Clear();
                // Build the Query
                SB.Append("INSERT INTO PE_Hierarchy (ParentId, Sequence, NodeType, Name, Description, ProjectCode, ProjectStatus, GroupId, CreationDate, LastUpdateTime, LastUpdateUser, LastUpdateComputer, LastUpdateApplication, SCDUSASyncInd, IRISTechInd) ");
                SB.Append("VALUES (@ParentId, @Sequence, @NodeType, @Name, @Description, @ProjectCode, @ProjectStatus, @GroupId, @CreationDate, @LastUpdateTime, @LastUpdateUser, @LastUpdateComputer, @LastUpdateApplication, @SCDUSASyncInd, @IRISTechInd) ");
                if (DatabaseSupportsBatchQueries)
                {
                    SB.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                }
                CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "ParentId", DataType = DbType.Int32, Value = ParentObj },
                new ParamStruct { ParamName = "Sequence", DataType = DbType.Int16, Value = HM.Sequence },
                new ParamStruct { ParamName = "NodeType", DataType = DbType.String, Value = HM.NodeType.ToString() },
                new ParamStruct { ParamName = "Name", DataType = DbType.String, Value = HM.Name },
                new ParamStruct { ParamName = "Description", DataType = DbType.String, Value = HM.Description },
                new ParamStruct { ParamName = "ProjectCode", DataType = DbType.String, Value = (String.IsNullOrEmpty(HM.Code))?(object)DBNull.Value :HM.Code},
                new ParamStruct { ParamName = "ProjectStatus", DataType = DbType.String, Value = ProjectStatusEnum.O.ToString()},
                new ParamStruct { ParamName = "GroupId", DataType = DbType.Int32, Value = HM.GroupId },
                new ParamStruct { ParamName = "CreationDate", DataType = DbType.DateTime, Value = HM.CreationDate },
                new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = HM.LastUpdateTime },
                new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = HM.LastUpdateUser },
                new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = HM.LastUpdateComputer },
                new ParamStruct { ParamName = "LastUpdateApplication", DataType = DbType.String, Value = HM.LastUpdateApplication },
                new ParamStruct { ParamName = "SCDUSASyncInd", DataType = DbType.Boolean, Value = HM.Synchronization },
                new ParamStruct { ParamName = "IRISTechInd", DataType = DbType.Boolean, Value = HM.IRISTechInd }
                };


                long RVH = 0;

                if (DatabaseSupportsBatchQueries)
                {
                    object RVHObj = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                    if (RVHObj != null)
                    {
                        RVH = Convert.ToInt64(RVHObj);
                    }
                }
                else
                {

                    RVH = (long)Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                }
                // Finalize
                if (RVH < 1) // Something went wrong... No rows were affected
                {
                    return "105";
                }

                if (HM.Certificates.Count > 0)
                {
                    foreach (String I in HM.Certificates)
                    {
                        var InsertCert = new StringBuilder(string.Empty);
                        InsertCert.Append("INSERT INTO PE_FolderCertificate (HierarchyId, CertificateId, EffectiveDate, ExpirationDate, LastUpdateTime, LastUpdateUser, LastUpdateComputer, LastUpdateAppliation) ");
                        InsertCert.Append("VALUES (@HierarchyId, @CertificateId, @EffectiveDate, @ExpirationDate, @LastUpdateTime, @LastUpdateUser, @LastUpdateComputer, @LastUpdateAppliation) ");
                        if (DatabaseSupportsBatchQueries)
                        {
                            InsertCert.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                        }

                        CommandParams = new List<ParamStruct>()
                                        {
                                        new ParamStruct { ParamName = "HierarchyId", DataType = DbType.Int32, Value = RVH },
                                        new ParamStruct { ParamName = "CertificateId", DataType = DbType.String, Value = I.ToString().Trim()},
                                        new ParamStruct { ParamName = "EffectiveDate", DataType = DbType.DateTime, Value = DateTime.Now },
                                        new ParamStruct { ParamName = "ExpirationDate", DataType = DbType.DateTime, Value = (object)DBNull.Value },
                                        new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = DateTime.Now },
                                        new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = Domain.User },
                                        new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = Domain.Workstn },
                                        new ParamStruct { ParamName = "LastUpdateAppliation", DataType = DbType.String, Value = Domain.AppName },
                        
                                        };
                        long CertID = 0;
                        if (DatabaseSupportsBatchQueries)
                        {
                            object CertIDObj = Domain.PersistenceLayer.FetchDataValue(InsertCert.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                            if (CertIDObj != null)
                            {
                                CertID = Convert.ToInt64(CertIDObj);
                            }
                        }
                        else
                        {
                            CertID = (long)Domain.PersistenceLayer.ExecuteDbCommand(InsertCert.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                        }
                        if (CertID < 1) // Something went wrong... No rows were affected
                        {
                            //return "Failed to update database";
                            return "141";
                        }
                    }
                }
                if (HM.UserCertificates.Count > 0)
                {
                    foreach (UserCertificateApiModel I in HM.UserCertificates)
                    {
                        var InsertUserCert = new StringBuilder(string.Empty);
                        InsertUserCert.Append("INSERT INTO PE_FolderUserCertificate (HierarchyId, UserCertificateId, EffectiveDate, ExpirationDate, LastUpdateTime, LastUpdateUser, LastUpdateComputer, LastUpdateAppliation) ");
                        InsertUserCert.Append("VALUES (@HierarchyId, @UserCertificateId, @EffectiveDate, @ExpirationDate, @LastUpdateTime, @LastUpdateUser, @LastUpdateComputer, @LastUpdateAppliation) ");
                        if (DatabaseSupportsBatchQueries)
                        {
                            InsertUserCert.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                        }

                        // I.LastUpdateTime = DateTime.Now;
                        CommandParams.Clear();
                        CommandParams = new List<ParamStruct>()
                                        {
                                            new ParamStruct { ParamName = "HierarchyId", DataType = DbType.Int32, Value = RVH },
                                            new ParamStruct { ParamName = "UserCertificateId", DataType = DbType.String, Value = I.UserCertificateId},
                                            new ParamStruct { ParamName = "EffectiveDate", DataType = DbType.DateTime, Value = DateTime.Now },
                                            new ParamStruct { ParamName = "ExpirationDate", DataType = DbType.DateTime, Value = (object)DBNull.Value },
                                            new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = DateTime.Now },
                                            new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = Domain.User },
                                            new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = Domain.Workstn },
                                            new ParamStruct { ParamName = "LastUpdateAppliation", DataType = DbType.String, Value = Domain.AppName },
                        

                                        };
                        long UserCertID = 0;
                        if (DatabaseSupportsBatchQueries)
                        {
                            object UserCertIDObj = Domain.PersistenceLayer.FetchDataValue(InsertUserCert.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                            if (UserCertIDObj != null)
                            {
                                UserCertID = Convert.ToInt64(UserCertIDObj);
                            }
                        }
                        else
                        {
                            UserCertID = (long)Domain.PersistenceLayer.ExecuteDbCommand(InsertUserCert.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                        }
                        if (UserCertID < 1) // Something went wrong... No rows were affected
                        {
                            //return "Failed to update database";
                            return "141";
                        }


                    }//end of foreach user certificates
                }

                //All OK
                HM.VM.IsDirty = false;
                HM.IsClonedRelated = false;
                HM.VM.IsNew = false;
                HM.Id = RVH;
                HM.IsDirty = false;
                return string.Empty;
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
           
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("DB Error");
            }
        }


        #endregion  Related project Managment

        #region split Related Project


            public static string SplitRelatedProject(ref HierarchyModel HM)
        {
            var SB = new StringBuilder(string.Empty);
            List<ParamStruct> CommandParams;
            Boolean DatabaseSupportsBatchQueries = Domain.PersistenceLayer.GetSupportsBatchQueries();
            // Handle Null Parent
            
            Object ParentObj = (object)DBNull.Value;


            // Do work
            try
            {
                string CheckCodeAndStepString = CheckCodeAndStep(ref HM);
                if (!(String.IsNullOrEmpty(CheckCodeAndStepString)))
                    return "110";

                long RV = 0;
                SB.Clear();
                SB.Append("SELECT v.VersionId from PE_Version v where v.VersionName ='" + HM.VM.VersionName + "'");
                SB.Append("AND v.HierarchyId ='" + HM.GroupId + "' and v.VersionStatus != 'A'");
                if (DatabaseSupportsBatchQueries)
                {
                    object VBtempObj = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null);
                    if (VBtempObj != null)
                    {
                        RV = Convert.ToInt64(VBtempObj);
                    }
                }
                else
                {
                    RV = (long)Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, null);
                }
                if (RV != 0)
                {
                    //var Error = new StringBuilder(string.Empty);
                    //Error.Append("SELECT Description FROM PE_Messages where id = '124'");
                    return "124";
                }


                UpdateControlFields(ref HM); // Update Control fields
                HM.CreationDate = DateTime.Now;
                string step = GetStepCodeByName(HM.SelectedStep.Trim());
                SB.Clear();
                //string Qry = "Update PE_Hierarchy set PE_Hierarchy.GroupId ='" + RV + "' and ProjectStep =" + StepH + " where PE_Hierarchy.Id='" + HM.ToString().Trim() + "';";
                SB.Append("UPDATE PE_Hierarchy SET GroupId=@GroupId,  ");
                SB.Append("ProjectStep=@ProjectStep,  ");
                SB.Append("LastUpdateTime=@LastUpdateTime, LastUpdateUser=@LastUpdateUser, LastUpdateComputer=@LastUpdateComputer, LastUpdateApplication=@LastUpdateApplication ");
                SB.Append("WHERE Id=@Id");

                CommandParams = new List<ParamStruct>()
                {
                    new ParamStruct { ParamName = "Id", DataType = DbType.Int32, Value = HM.Id },
                    new ParamStruct { ParamName = "GroupId", DataType = DbType.Int32, Value = (object)DBNull.Value },
                    new ParamStruct { ParamName = "ProjectStep", DataType = DbType.String, Value = (String.IsNullOrEmpty(step))? (object)DBNull.Value : step.Trim()},
                    new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = HM.LastUpdateTime },
                    new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = HM.LastUpdateUser },
                    new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = HM.LastUpdateComputer },
                    new ParamStruct { ParamName = "LastUpdateApplication", DataType = DbType.String, Value = HM.LastUpdateApplication },
             
                };
                long UP = 0;


                UP = (long)Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());

                if (UP < 1) // Something went wrong... No rows were affected
                {
                    return "141";
                }
                //ALL OK
                else
                {
                    //Copy All notes of the group to the split project
                    SB.Clear();
                    SB.Append("SELECT * FROM PE_Note where HierarchyId ='" + HM.GroupId + "' ");
                    DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                    if (ResTable != null)
                    {
                        foreach (DataRow DataRow in ResTable.Rows)
                        {

                            SB.Clear();
                            SB.Append("INSERT INTO PE_Note (HierarchyId, NoteType, NoteStatus, NoteTitle, NoteText, SpecialInd, CreatedByUser, CreationDate, LastUpdateTime, LastUpdateUser, LastUpdateComputer, LastUpdateapplication ) ");
                            SB.Append("VALUES (@HierarchyId, @NoteType, @NoteStatus, @NoteTitle, @NoteText, @SpecialInd, @CreatedByUser, @CreationDate, @LastUpdateTime, @LastUpdateUser, @LastUpdateComputer, @LastUpdateapplication ) ");

                            //(String.IsNullOrEmpty(step))? (object)DBNull.Value : step.Trim()
                            //DataRow["Sequence"] is System.DBNull
                            CommandParams.Clear();
                            CommandParams = new List<ParamStruct>()
                    {
                        new ParamStruct { ParamName = "HierarchyId", DataType = DbType.Int32, Value =  HM.Id },
                        new ParamStruct { ParamName = "NoteType", DataType = DbType.String, Value = (string)DataRow["NoteType"].ToString().Trim()},
                        new ParamStruct { ParamName = "NoteStatus", DataType = DbType.String, Value = (string)DataRow["NoteStatus"].ToString().Trim()},
                        new ParamStruct { ParamName = "NoteTitle", DataType = DbType.String, Value = (string)DataRow["NoteTitle"]},
                        new ParamStruct { ParamName = "NoteText", DataType = DbType.String, Value = (string)DataRow["NoteText"]},
                        new ParamStruct { ParamName = "SpecialInd", DataType = DbType.Boolean, Value = (Boolean)DataRow["SpecialInd"]},
                        new ParamStruct { ParamName = "CreatedByUser", DataType = DbType.String, Value = (string)DataRow["CreatedByUser"]},
                        new ParamStruct { ParamName = "CreationDate", DataType = DbType.DateTime, Value = (DateTime)DataRow["CreationDate"] },
                        new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = HM.LastUpdateTime },
                        new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = HM.LastUpdateUser },
                        new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = HM.LastUpdateComputer },
                        new ParamStruct { ParamName = "LastUpdateapplication", DataType = DbType.String, Value = HM.LastUpdateApplication },
             
                    };
                            long NoteCopy = 0;


                            NoteCopy = (long)Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());

                            if (NoteCopy < 1) // Something went wrong... No rows were affected
                            {
                                return "141";
                            }
                        }
                    }
                    //ALL OK. Create new version.
                   
                        //Insert new version based on previous version.
                        if (DatabaseSupportsBatchQueries)
                        {
                            // Build the Query
                            SB.Clear();
                            SB.Append("INSERT INTO PE_Version (HierarchyId, VersionName, VersionSeqNo, VersionStatus, Description, TargetPath, CreationDate, LastUpdateTime, LastUpdateUser, LastUpdateComputer, LastUpdateapplication, DefaultTargetPathInd, ECRId) ");
                            SB.Append("VALUES (@HierarchyId, @VersionName, @VersionSeqNo, @VersionStatus, @Description, @TargetPath, @CreationDate, @LastUpdateTime, @LastUpdateUser, @LastUpdateComputer, @LastUpdateapplication, @DefaultTargetPathInd, @ECRId) ");
                            if (DatabaseSupportsBatchQueries)
                            {
                                SB.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                            }

                        }
                        else // Mainly for SqlServerCE, which does not support ScopeIdentity
                        {

                            // Build the Query
                            SB.Clear();
                            SB.Append("INSERT INTO PE_Version (HierarchyId, VersionName, VersionSeqNo, VersionStatus, Description, TargetPath, CreationDate, LastUpdateTime, LastUpdateUser, LastUpdateComputer, LastUpdateapplication, DefaultTargetPathInd, ECRId) ");
                            SB.Append("VALUES (@HierarchyId, @VersionName, @VersionSeqNo, @VersionStatus, @Description, @TargetPath, @CreationDate, @LastUpdateTime, @LastUpdateUser, @LastUpdateComputer, @LastUpdateapplication, @DefaultTargetPathInd, @ECRId) ");
                            if (DatabaseSupportsBatchQueries)
                            {
                                SB.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                            }
                        }

                        //Check if there is version with the same name.
                        var VB = new StringBuilder(string.Empty);
                        HM.VM.VersionId = -1;
                        VB.Append("SELECT VersionId FROM PE_Version WHERE VersionName = '" + HM.VM.VersionName + "' AND  HierarchyId ='" + HM.Id + "' AND VersionId <> '" + HM.VM.VersionId + "'");

                        long VBtemp = 0;
                        if (DatabaseSupportsBatchQueries)
                        {
                            object VBtempObj = Domain.PersistenceLayer.FetchDataValue(VB.ToString(), System.Data.CommandType.Text, null);
                            if (VBtempObj != null)
                            {
                                VBtemp = Convert.ToInt64(VBtempObj);
                            }
                        }
                        else
                        {
                            VBtemp = (long)Domain.PersistenceLayer.ExecuteDbCommand(VB.ToString(), System.Data.CommandType.Text, null);
                        }
                        if (VBtemp != 0)
                        {
                            //var Error = new StringBuilder(string.Empty);
                            //Error.Append("SELECT Description FROM PE_Messages where id = '124'");
                            return "124";
                        }

                        CommandParams.Clear();
                        CommandParams = new List<ParamStruct>()
                        {
                        new ParamStruct { ParamName = "VersionId", DataType = DbType.Int32, Value = HM.VM.VersionId },
                        new ParamStruct { ParamName = "HierarchyId", DataType = DbType.Int32, Value = HM.Id },
                        new ParamStruct { ParamName = "VersionName", DataType = DbType.String, Value = HM.VM.VersionName.Trim() },
                        new ParamStruct { ParamName = "VersionSeqNo", DataType = DbType.Int32, Value = HM.VM.Sequence },
                        new ParamStruct { ParamName = "VersionStatus", DataType = DbType.String, Value = 'A' },
                        new ParamStruct { ParamName = "Description", DataType = DbType.String, Value = HM.VM.Description.Trim() },
                        new ParamStruct { ParamName = "TargetPath", DataType = DbType.String, Value = HM.VM.TargetPath.Trim()},
                        new ParamStruct { ParamName = "CreationDate", DataType = DbType.DateTime, Value = HM.VM.CreationDate },
                        new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = HM.LastUpdateTime },
                        new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = HM.LastUpdateUser },
                        new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = HM.LastUpdateComputer },
                        new ParamStruct { ParamName = "LastUpdateApplication", DataType = DbType.String, Value = HM.LastUpdateApplication },
                        new ParamStruct { ParamName = "ECRId", DataType = DbType.String, Value = HM.VM.EcrId },
                        new ParamStruct { ParamName = "DefaultTargetPathInd", DataType = DbType.Boolean, Value = HM.VM.DefaultTargetPathInd }
                        };

                        long RvVersions = 0;
                        if (DatabaseSupportsBatchQueries)
                        {
                            object RvVersionsObj = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                           if(RvVersionsObj != null)
                           {
                            RvVersions = Convert.ToInt64(RvVersionsObj);
                            HM.VM.VersionId = RvVersions;
                           }
                        }
                        else
                        {
                            RvVersions = (long)Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                        }

                        if (RvVersions < 1) // Something went wrong... No rows were affected
                        {
                            //return "Failed to update database";
                            return "141";
                        }


                        //Copy of all contents for the created version.
                        if (HM.VM.Contents.Count > 0)
                        {

                            int J = 1;
                            foreach (var I in HM.VM.Contents)
                            {

                                var InsertContent = new StringBuilder(string.Empty);
                                InsertContent.Append("INSERT INTO PE_VersionContent (VersionId, ContentVersionId, ContentSeqNo, CreationDate, LastUpdateTime, LastUpdateUser, LastUpdateComputer, LastUpdateApplication) ");
                                InsertContent.Append("VALUES (@VersionId, @ContentVersionId, @ContentSeqNo, @CreationDate, @LastUpdateTime, @LastUpdateUser, @LastUpdateComputer, @LastUpdateApplication) ");
                                if (DatabaseSupportsBatchQueries)
                                {
                                    InsertContent.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                                }

                                CommandParams = new List<ParamStruct>()
                        {
                        new ParamStruct { ParamName = "VersionId", DataType = DbType.Int32, Value =  HM.VM.VersionId},
                        new ParamStruct { ParamName = "ContentVersionId", DataType = DbType.Int32, Value = I.id},
                        new ParamStruct { ParamName = "ContentSeqNo", DataType = DbType.Int32, Value = J },
                        new ParamStruct { ParamName = "CreationDate", DataType = DbType.DateTime, Value =  DateTime.Now },
                        new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = DateTime.Now },
                        new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = Domain.User },
                        new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = Domain.Workstn },
                        new ParamStruct { ParamName = "LastUpdateApplication", DataType = DbType.String, Value = Domain.AppName },
                        
                        };
                                long ContID = 0;
                                if (DatabaseSupportsBatchQueries)
                                {
                                    object ContIDObj = Domain.PersistenceLayer.FetchDataValue(InsertContent.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                                   if(ContIDObj != null)
                                   {
                                    ContID = Convert.ToInt64(ContIDObj);
                                   }
                                }
                                else
                                {
                                    ContID = (long)Domain.PersistenceLayer.ExecuteDbCommand(InsertContent.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                                }
                                if (ContID < 1) // Something went wrong... No rows were affected
                                {
                                    //return "Failed to update database";
                                    return "141";
                                }
                                J++;
                            }
                        } //END OF ALL COPY ALL ACTIVE VERSION CONTENTS.
             


                    //Copy All versions of the group to the split project
                    SB.Clear();
                    
                  
                     SB.Append(" SELECT * FROM PE_Version where HierarchyId ='" + HM.GroupId + "' and VersionStatus != 'A'");
                
                    DataTable ResTableVersions = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                    if (ResTableVersions != null && ResTableVersions.Rows.Count > 0)
                    {
                        //Create new version.
                        foreach (DataRow DataRow in ResTableVersions.Rows)
                        {
                            SB.Clear();
                            SB.Append("INSERT INTO PE_Version (HierarchyId, VersionName, VersionSeqNo, VersionStatus, Description, TargetPath, CreationDate, LastUpdateTime, LastUpdateUser, LastUpdateComputer, LastUpdateapplication, DefaultTargetPathInd ,ECRId) ");
                            SB.Append("VALUES (@HierarchyId, @VersionName, @VersionSeqNo, @VersionStatus, @Description, @TargetPath, @CreationDate, @LastUpdateTime, @LastUpdateUser, @LastUpdateComputer, @LastUpdateapplication, @DefaultTargetPathInd ,@ECRId) ");
                            if (DatabaseSupportsBatchQueries)
                            {
                                SB.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                            }

                            CommandParams.Clear();
                            CommandParams = new List<ParamStruct>()
                                {
                                new ParamStruct { ParamName = "HierarchyId", DataType = DbType.Int32, Value =  HM.Id },
                                new ParamStruct { ParamName = "VersionName", DataType = DbType.String, Value = (string)DataRow["VersionName"]},
                                new ParamStruct { ParamName = "VersionSeqNo", DataType = DbType.Int32, Value = (int)DataRow["VersionSeqNo"]},
                                new ParamStruct { ParamName = "VersionStatus", DataType = DbType.String, Value = 'C'},
                                new ParamStruct { ParamName = "Description", DataType = DbType.String, Value = (string)DataRow["Description"]},
                                new ParamStruct { ParamName = "TargetPath", DataType = DbType.String, Value = (string)DataRow["TargetPath"]},
                                new ParamStruct { ParamName = "DefaultTargetPathInd", DataType = DbType.Boolean, Value = (Boolean)DataRow["DefaultTargetPathInd"]},
                                new ParamStruct { ParamName = "CreationDate", DataType = DbType.DateTime, Value = (DateTime)DataRow["CreationDate"] },
                                new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = HM.VM.LastUpdateTime },
                                new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = HM.VM.LastUpdateUser },
                                new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = HM.VM.LastUpdateComputer },
                                new ParamStruct { ParamName = "ECRId", DataType = DbType.String, Value = HM.VM.EcrId },
                                new ParamStruct { ParamName = "LastUpdateapplication", DataType = DbType.String, Value = HM.VM.LastUpdateApplication },
             
                                };
                            long VersionCopy = 0;

                            if (DatabaseSupportsBatchQueries)
                            {
                                object VersionCopyObj = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                               if(VersionCopyObj != null)
                               {
                                VersionCopy = Convert.ToInt64(VersionCopyObj);
                               }
                          
                            }
                            else
                            {
                                VersionCopy = (long)Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                            }


                            //VersionCopy = (long)Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());

                            if (VersionCopy < 1) // Something went wrong... No rows were affected
                            {
                                return "141";
                            }


                            //Copy of ALL CONTENTS.

                            SB.Clear();
                            SB.Append(" SELECT * FROM PE_VersionContent where VersionId  ='" + (int)DataRow["VersionId"] + "' ");
                            DataTable ResTableContents = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                            if (ResTableContents != null && ResTableVersions.Rows.Count > 0)
                            {
                                //Create new version.
                                foreach (DataRow DataRowContent in ResTableContents.Rows)
                                {
                                    var InsertContent = new StringBuilder(string.Empty);
                                    InsertContent.Append("INSERT INTO PE_VersionContent (VersionId, ContentVersionId, ContentSeqNo, CreationDate, LastUpdateTime, LastUpdateUser, LastUpdateComputer, LastUpdateApplication) ");
                                    InsertContent.Append("VALUES (@VersionId, @ContentVersionId, @ContentSeqNo, @CreationDate, @LastUpdateTime, @LastUpdateUser, @LastUpdateComputer, @LastUpdateApplication) ");
                                    if (DatabaseSupportsBatchQueries)
                                    {
                                        InsertContent.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                                    }
                                    CommandParams.Clear();
                                    CommandParams = new List<ParamStruct>()
                                {
                                new ParamStruct { ParamName = "VersionId", DataType = DbType.Int32, Value =  Convert.ToInt32(VersionCopy) },
                                new ParamStruct { ParamName = "ContentVersionId", DataType = DbType.Int32, Value = (int)DataRowContent["ContentVersionId"]},
                                new ParamStruct { ParamName = "ContentSeqNo", DataType = DbType.Int32, Value = (int)DataRowContent["ContentSeqNo"]},
                                new ParamStruct { ParamName = "CreationDate", DataType = DbType.DateTime, Value = (DateTime)DataRowContent["CreationDate"] },
                                new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = DateTime.Now},
                                new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = HM.VM.LastUpdateUser },
                                new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = HM.VM.LastUpdateComputer },
                                new ParamStruct { ParamName = "LastUpdateApplication", DataType = DbType.String, Value = HM.VM.LastUpdateApplication },
             
                                };
                                    long ContentCopy = 0;
                                    if (DatabaseSupportsBatchQueries)
                                    {
                                        Object ContentCopyObj = Domain.PersistenceLayer.FetchDataValue(InsertContent.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                                        if(ContentCopyObj != null)
                                        {
                                        ContentCopy = Convert.ToInt64(ContentCopyObj);
                                        }
                                    }
                                    else
                                    {
                                        ContentCopy = (long)Domain.PersistenceLayer.ExecuteDbCommand(InsertContent.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());
                                    }

                                    if (ContentCopy < 1) // Something went wrong... No rows were affected
                                    {
                                        return "141";
                                    }

                                }

                            }//End of foreach content versions. 



                        }//End of foreach closed versions. 

                        //Copy of all closed versions and  contents.



                    }// End of copy  old versions.
                }
                HM.VM.IsNew = false;
                HM.GroupId = -1;
                HM.VM.IsDirty = false;
                HM.IsDirty = false;
                HM.IsClonedRelatedSplit = false;
                return String.Empty;

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error

                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("DB Error");
            }
        }


        #endregion split Related Project

        #region HierarchyBLL return status enumeration - Ella

        public enum HierarchyBLLReturnCode : int
        {
            DBException,
            NoProjectId,
            CommonException,
            Success
        }

        #endregion

        #region Retrieve ProjectId by full project path - for Bootstrap API - Ella

        // Retrieve ProjectId from PE_Hierarchy receiving full project path as input parameter
        public static HierarchyBLLReturnCode GetNodeIdByFullPath(string HierarchyPath, out long projectId)
        {
            HierarchyBLLReturnCode returnStatus = HierarchyBLLReturnCode.NoProjectId;
            projectId = Convert.ToInt32(HierarchyBLLReturnCode.NoProjectId);

            try
            {
                if (HierarchyPath == string.Empty)
                {
                    projectId = 0;
                    return HierarchyBLLReturnCode.Success;
                }

                var SBForJoin = new StringBuilder(string.Empty); //string builder for "JOIN" part of the query
                var SBForWhere = new StringBuilder(string.Empty); //string builder for "WHERE part of the query
                char[] pathDelimiter = { '/' };

                //Split the path and get list of Project folders
                string[] listOfProjectFolders = HierarchyPath.Split(pathDelimiter);

                //Build a query. Number of self joins as number of folders in listOfProjectFolders, starting from root folder (ParentId is null)
                int i = 0;
                foreach (string s in listOfProjectFolders)
                {
                    if (i == 0)
                    {
                        SBForJoin.Append("Select ph" + (listOfProjectFolders.Length - 1) + ".Id from PE_Hierarchy ph0 ");
                        SBForWhere.Append(" WHERE ph0.name = " + "'" + s + "'" + " and ph0.ParentId is null ");
                    }
                    else
                    {
                        SBForJoin.Append(" JOIN PE_Hierarchy " + " ph" + i + " ON ph" + (i - 1) + ".Id = " + "ph" + i + ".ParentId ");
                        SBForWhere.Append(" AND ph" + i + ".Name = " + "'" + s + "'");
                    }
                    i++;
                }

                // Concatenate the "Join" section and "Where" clause
                string FullQuery = string.Concat(SBForJoin, SBForWhere) + ";";


                //select ProjectId from DB

                object objSelectResult = Domain.PersistenceLayer.FetchDataValue(FullQuery, CommandType.Text, null);

                if (objSelectResult != null)
                {
                    projectId = Convert.ToInt32(objSelectResult); //Success
                    returnStatus = HierarchyBLLReturnCode.Success;
                }
                else
                {
                    returnStatus = HierarchyBLLReturnCode.NoProjectId;//no Project found
                }
                return returnStatus;
            }
            catch (Exception e)
            {
                ApiBLL.TraceExceptionParameterValue.Add(e.Message);
                return HierarchyBLLReturnCode.CommonException;
            }
        }
        #endregion

        #region retrieve Project full Path by Project Id

        public static HierarchyBLL.HierarchyBLLReturnCode GetProjectFullPathByProjectId(long projId, out string fullPath)
        {
            fullPath = null;
            if (projId <= 0)
            {
                return HierarchyBLLReturnCode.NoProjectId;
            }
            try
            {
                var SB = new StringBuilder(string.Empty);

                SB.Append("with cte (NodeId,NodeName,FullPath) as ");
                SB.Append("(");
                SB.Append("select id,cast(name as varchar(1000)), cast(name as varchar(1000)) ");
                SB.Append("from PE_Hierarchy ");
                SB.Append("where ParentId is null ");
                SB.Append("union all ");
                SB.Append("select n.Id, cast(n.Name as varchar(1000)), CONVERT(varchar(1000),cte.FullPath + '/' + n.Name) as name ");
                SB.Append("from PE_Hierarchy n ");
                SB.Append("join cte ");
                SB.Append("on n.ParentId = cte.NodeId ");
                SB.Append(") ");
                SB.Append("select FullPath ");
                SB.Append("from cte ");
                SB.Append("where NodeId = " + projId);

                object objSelectResult =  Domain.PersistenceLayer.FetchDataValue(Convert.ToString(SB), CommandType.Text, null);
                if (objSelectResult != null)
                {
                    fullPath = Convert.ToString(objSelectResult); //Success
                    return HierarchyBLLReturnCode.Success;
                }
                else
                {
                    return HierarchyBLLReturnCode.DBException;//no path found
                }
            }
            catch (Exception)
            {
                return HierarchyBLLReturnCode.CommonException;
            }
        
        }



        #endregion


        public static string CheckCodeAndStep(ref HierarchyModel HM)
        {
            try
            {
                //Step and code are null - valid state.
                if ((!(String.IsNullOrEmpty(HM.Code.Trim())) && (!(String.IsNullOrEmpty(HM.SelectedStep.Trim())))))
                {
                    string step = GetStepCodeByName(HM.SelectedStep.Trim());
                    var SB = new StringBuilder(string.Empty);
                    SB.Append("SELECT COUNT(*) FROM PE_Hierarchy WHERE ProjectCode ");
                    SB.Append("= '" + HM.Code + "' ");
                    SB.Append(" AND ProjectStep");
                    SB.Append("= '" + step.Trim() + "' ");
                    if (HM.GroupId == -1 && HM.IsCloned == false && HM.IsClonedRelated == false)
                    {
                        SB.Append(" and Id <> '" + HM.Id + "'");
                    }
                    object CountObj = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null);
                    if (CountObj != null)
                    {
                        Int16 Count = Convert.ToInt16(CountObj);
                        if (Count > 0)
                        {
                            return "110";
                        }
                        SB.Clear();
                        SB.Append("SELECT COUNT(*)  FROM PE_Hierarchy P1 inner join PE_Hierarchy p2 ");
                        SB.Append(" on P1.Id = p2.GroupId where p2.ProjectCode  ");
                        SB.Append("= '" + HM.Code + "' ");
                        SB.Append(" AND p1.ProjectStep");
                        SB.Append("= '" + step.Trim() + "' ");
                        if (HM.GroupId != -1)
                        {
                            SB.Append(" and p2.Id <> '" + HM.Id + "'");
                        }
                        object CountGroupObj = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null);
                        if (CountGroupObj != null)
                        {
                            Int16 CountGroup = Convert.ToInt16(CountGroupObj);
                            if (CountGroup > 0)
                            {
                                return "110";
                            }
                            return string.Empty;
                        }
                        else
                            return "135";
                    }
                    else
                        return "135";
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return "105";
            }

        }

        public static string CheckCodeAndStepForAllRelated(string Code, string SelectedStep, long GroupId)
        {
            try
            {

                //Step and code are null - valid state.
                if ((!(String.IsNullOrEmpty(Code.Trim())) && (!(String.IsNullOrEmpty(SelectedStep.Trim())))))
                {
                    string step = GetStepCodeByName(SelectedStep.Trim());
                    var SB = new StringBuilder(string.Empty);
                    SB.Append("SELECT COUNT(*) FROM PE_Hierarchy WHERE ProjectCode ");
                    SB.Append("= '" + Code + "' ");
                    SB.Append(" AND ProjectStep");
                    SB.Append("= '" + step.Trim() + "' ");

                    object CountObj = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null);
                    if (CountObj != null)
                    {
                        Int16 Count = Convert.ToInt16(CountObj);
                        if (Count > 0)
                        {
                            return "110";
                        }
                        SB.Clear();
                        SB.Append("SELECT COUNT(*)  FROM PE_Hierarchy P1 inner join PE_Hierarchy p2 ");
                        SB.Append(" on P1.Id = p2.GroupId where p2.ProjectCode  ");
                        SB.Append("= '" + Code + "' ");
                        SB.Append(" AND p1.ProjectStep");
                        SB.Append("= '" + step.Trim() + "' ");
                        SB.Append(" and p1.Id <> '" + GroupId + "'");

                        Int16 CountGroup = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));
                        if (CountGroup > 0)
                        {
                            return "110";
                        }


                        return string.Empty;

                    }
                    else
                        return "135";
                }

                else
                {
                    return string.Empty;
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return "105";
            }

        }//end of CheckCodeAndStepForAllRelated


        #region list of Hierarchy branch Ids by project id

        public static HierarchyBLLReturnCode GetHierarchyBranchIdsProjectId(long nodeId, out string listOfBranchIds)
        {
            listOfBranchIds = "";
            try
            {
                var qry = new StringBuilder(string.Empty);

                qry.Append("with cte (NodeId,FullIdPath) as ");
                qry.Append("(select id, CONVERT(varchar(200),id) from PE_Hierarchy ");
                qry.Append("where ParentId is null union all select n.Id, ");
                qry.Append("CONVERT(varchar(200),cte.FullIdPath  + ',' + CONVERT(varchar(200),n.Id)) ");
                qry.Append("as name from PE_Hierarchy n join cte on n.ParentId = cte.NodeId ) ");
                qry.Append("select CONVERT(varchar(200),FullIdPath) ");
                qry.Append("as BranchIdList from cte where NodeId = ' " + nodeId + "';");

                object idsListObj = Domain.PersistenceLayer.FetchDataValue(qry.ToString(), System.Data.CommandType.Text, null);
                if (idsListObj != null)
                {
                    listOfBranchIds = (string)idsListObj;
                }

                return HierarchyBLLReturnCode.Success;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e);
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                return HierarchyBLLReturnCode.DBException;
            }

        }

        #endregion list of Hierarchy branch Ids by project id
        
        #region getAllSteps


        public static StringCollection _ProjectsCollection { get; set; }
        public static StringCollection GetAllSteps(string code, long id)
        {
            try
            {
                var SBstep = new StringBuilder(string.Empty);
                _ProjectsCollection = new StringCollection();

                //ObservableCollection<String> steps = new ObservableCollection<String>();
                SBstep.Append("SELECT ps.StepDescription " +
                  " FROM PE_ProjectStep ps " +
                  " WHERE NOT EXISTS " +
                   " (SELECT 1 " +
                    " FROM PE_Hierarchy h " +
                    " WHERE h.ProjectStep=ps.StepCode " +
                    "  AND h.ProjectCode='" + code.ToString() + "' " +
                       "AND h.Id<> '" + id.ToString().Trim() + "')");


                // Fetch the DataTable from the database
                DataTable ResTable = ATSDomain.Domain.PersistenceLayer.FetchDataTable(SBstep.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null)
                {
                    _ProjectsCollection.Add(" ");
                    foreach (DataRow DataRow in ResTable.Rows)
                    {
                        //steps.Add(DataRow["StepDescription"].ToString());
                        _ProjectsCollection.Add(DataRow["StepDescription"].ToString());
                    }
                }
                return _ProjectsCollection;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return _ProjectsCollection;
            }
        }

        public static ObservableCollection<String> GetAllStepsForTemplate()
        {
            ObservableCollection<String> steps = new ObservableCollection<String>();
            try
            {
                var SBstep = new StringBuilder(string.Empty);
                //ObservableCollection<String> steps = new ObservableCollection<String>();
                SBstep.Append("SELECT ps.StepDescription FROM PE_ProjectStep ps ");


                // Fetch the DataTable from the database
                DataTable ResTable = ATSDomain.Domain.PersistenceLayer.FetchDataTable(SBstep.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null)
                {
                    steps.Add(" ");
                    foreach (DataRow DataRow in ResTable.Rows)
                    {
                        steps.Add(DataRow["StepDescription"].ToString());
                    }
                }
                return steps;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return steps;
            }
        }
        #endregion

        #region get Message 128
        public static string GetMessage()
        {
            try
            {
                string Message = string.Empty;
                var SB = new StringBuilder(string.Empty);
                SB.Append("select Description from PE_Messages where Id ='128'");
                object StrtObj = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null);
                if (StrtObj != null)
                {
                    Message = StrtObj.ToString();

                }
                return Message;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                return "105";
            }
        }
        #endregion get Message 201

        #region Get HierarchyIds by Content Name - Search by Content Name

        public static List<string> ListOfHierarchyIdBycontentName = new List<string>();

        public static void GetHierarchyIdsByContentName(string searchPattern)
        {
            try
            {
                if (!string.IsNullOrEmpty(searchPattern) && !string.IsNullOrWhiteSpace(searchPattern))
                {
                    List<int> ContentVerIds = new List<int>();
                    ContentVerIds = ContentBLL.GetContVerIdByContentName(searchPattern);
                    if (ContentVerIds.Count > 0)
                    {
                        ListOfHierarchyIdBycontentName.Clear();
                        string commaSeparatedCVIdList = string.Join(",", ContentVerIds);

                        System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                        QryStr.Append("SELECT distinct pv.HierarchyId FROM PE_Version pv ");
                        QryStr.Append("JOIN PE_VersionContent pvc ");
                        QryStr.Append("on pv.VersionId = pvc.VersionId ");
                        QryStr.Append("WHERE pvc.ContentVersionId in " + "(" + commaSeparatedCVIdList + ") ");
                        QryStr.Append("AND pv.VersionStatus = 'A' ");
                        QryStr.Append("ORDER BY pv.HierarchyId");

                        // Fetch the DataTable from the database
                        DataTable ResTableVersion = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                        // Populate list
                        if (ResTableVersion != null)
                        {
                            foreach (DataRow DataRow in ResTableVersion.Rows)
                            {
                                ListOfHierarchyIdBycontentName.Add(Convert.ToString(DataRow.ItemArray[0]));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
            }
        }
        #endregion

        #region Check If Hirearchy has child
        public static int CountHierarchyChild(long HierarchyId )
        {
            try
            {
                int ChildNum = 0;
                System.Text.StringBuilder CheckChildSB = new System.Text.StringBuilder();
                CheckChildSB.Append("SELECT DISTINCT 1 FROM pe_hierarchy WHERE EXISTS  ");
                CheckChildSB.Append("(SELECT 1 FROM pe_hierarchy WHERE ParentId ='" + HierarchyId + "')");

                object CheckChildObj = Domain.PersistenceLayer.FetchDataValue(CheckChildSB.ToString(), System.Data.CommandType.Text, null);
                if (CheckChildObj != null)
                {
                    ChildNum = Convert.ToInt32(CheckChildObj);
                }
                return ChildNum;
            }
            catch (Exception e)
            {
                throw new Exception("DB Error");
            }
        }
        #endregion 

        public static DataTable GetProjectStepDataTable()
        {
            try
            {
                StringBuilder SBstep = new StringBuilder(string.Empty);

                SBstep.Append("SELECT * FROM PE_ProjectStep ");


                // Fetch the DataTable from the database
                ProjectStepTable = ATSDomain.Domain.PersistenceLayer.FetchDataTable(SBstep.ToString(), CommandType.Text, null);

                return ProjectStepTable;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return null;
            }
        }

        public static DataTable GetNodeDataTable(string commaSeparatedIds)
        {
            try
            {
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("Select * FROM PE_Hierarchy WHERE Id in (" + commaSeparatedIds + ")");
                string Qry = QryStr.ToString();
                // Fetch the DataTable from the database
                DataTable NodeDataTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (NodeDataTable != null && NodeDataTable.Rows.Count >= 1)
                {
                    return NodeDataTable;
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

        public static Boolean NameExists(int parentId, string name, out int id)
        {
            id = 0;
            try
            {
                StringBuilder SB = new StringBuilder();
                SB.Clear();
                SB.Append("SELECT id FROM PE_Hierarchy WHERE Name = '" + name + "' ");
                SB.Append(" AND ParentId ");

                if (parentId == 0)
                {
                    SB.Append("IS NULL");
                }
                else
                {
                    SB.Append("= " + parentId);
                }
                object objId = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null);
                if (objId != null)
                {
                    id = Convert.ToInt32(objId);
                    if (id > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("Failed to get node data.");
            }
        }

        public static Boolean StepExists(string step)
        {
            try
            {
                StringBuilder SBstep = new StringBuilder(string.Empty);
                
                SBstep.Append("SELECT * FROM PE_ProjectStep ");
                SBstep.Append("WHERE StepCode = '" + step + "'");


                // Fetch the DataTable from the database
                DataTable ProjectStepTable = ATSDomain.Domain.PersistenceLayer.FetchDataTable(SBstep.ToString(), CommandType.Text, null);

                if (ProjectStepTable != null && ProjectStepTable.Rows.Count == 1)
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
                throw new Exception("Failed to access PE_ProjectStep table.");
            }
        }

        public static HierarchyModel GetNodeModel(DataRow projectDataRow)
        {
            // Initialize work fields
            HierarchyModel Node = Domain.GetBusinessObject<HierarchyModel>();
            try
            {
                if (projectDataRow != null)
                {
                    DataRow DataRow = projectDataRow;
                    //
                    Node.Id = (int)DataRow["Id"];
                    Node.IsNew = false;
                    Node.IsDirty = false;
                    if (DataRow["ParentId"] == DBNull.Value)
                    {
                        Node.ParentId = 0;
                    }
                    else
                    {
                        Node.ParentId = (int)DataRow["ParentId"];
                    }
                    if (!(DataRow["Sequence"] is System.DBNull))
                    {
                        Node.Sequence = Convert.ToInt32(DataRow["Sequence"]);
                    }

                    switch ((string)DataRow["NodeType"])
                    {
                        case "F":
                            Node.NodeType = NodeTypes.F;
                            break;
                        case "P":
                            Node.NodeType = NodeTypes.P;
                            break;
                        case "G":
                            Node.NodeType = NodeTypes.G;
                            break;
                        case "T":
                            Node.NodeType = NodeTypes.T;
                            break;
                        default:
                            Node.NodeType = NodeTypes.F;
                            break;
                    }
                    Node.Name = (string)DataRow["Name"];
                    Node.Description = (string)DataRow["Description"];
                    Node.CreationDate = (DateTime)DataRow["CreationDate"];
                    Node.LastUpdateTime = (DateTime)DataRow["LastUpdateTime"];
                    Node.LastUpdateUser = (string)DataRow["LastUpdateUser"];

                    if ((string)(DataRow["NodeType"]) == "P" || (string)(DataRow["NodeType"]) == "T")
                    {
                        if (!(DataRow["ProjectCode"] is System.DBNull))
                        {
                            Node.Code = (string)DataRow["ProjectCode"].ToString().Trim();
                        }
                        if (!(DataRow["ProjectStep"] is System.DBNull))
                        {
                            string stepName = GetStepNameByCode((string)DataRow["ProjectStep"]);
                            Node.SelectedStep = stepName.ToString();
                        }
                        if (!(DataRow["SCDUSASyncInd"] is System.DBNull))
                        {
                            Node.Synchronization = (Boolean)DataRow["SCDUSASyncInd"];
                        }
                        Node.IRISTechInd = (DataRow["IRISTechInd"] is System.DBNull) ? false : (Boolean)DataRow["IRISTechInd"];

                        if (!(DataRow["ProjectStatus"] is System.DBNull))
                        {
                            switch ((string)DataRow["ProjectStatus"])
                            {
                                case "O":
                                    Node.ProjectStatus = "Open";
                                    break;
                                case "D":
                                    Node.ProjectStatus = "Disabled";
                                    break;
                                default:
                                    Node.ProjectStatus = "Open";
                                    break;
                            }

                        }


                        if (!(DataRow["GroupId"] is System.DBNull))
                        {
                            System.Text.StringBuilder groupQur = new System.Text.StringBuilder();
                            groupQur.Append("SELECT  h.Name as GroupName, h.Description as GroupDescription, h.ProjectStep as ProjectStep, hg.GroupId as  GroupId , h.LastUpdateTime as GroupLastUpdateTime " +
                            "FROM PE_Hierarchy as h inner join PE_Hierarchy as hg on h.Id = hg.GroupId " +
                            "WHERE hg.id='" + Node.Id.ToString().Trim() + "'");

                            DataTable GroupData = Domain.PersistenceLayer.FetchDataTable(groupQur.ToString(), CommandType.Text, null);
                            //var a = Domain.PersistenceLayer.FetchDataValue(groupQur.ToString(), System.Data.CommandType.Text, null);

                            if (!(GroupData.Rows[0]["GroupName"] is System.DBNull))
                            {
                                Node.GroupName = (string)GroupData.Rows[0]["GroupName"];
                                Node.GroupDescription = (string)GroupData.Rows[0]["GroupDescription"];
                                Node.GroupId = (int)DataRow["GroupId"];
                                Node.GroupLastUpdateTime = (DateTime)GroupData.Rows[0]["GroupLastUpdateTime"];
                                if (!(GroupData.Rows[0]["ProjectStep"] is System.DBNull))
                                {
                                    string stepName = GetStepNameByCode((string)GroupData.Rows[0]["ProjectStep"]);
                                    Node.SelectedStep = stepName.ToString();
                                }

                            }
                        }
                    }

                    // TODO: Continue populating all properties
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

        #region Populate contents from Templates

        public static Domain.ErrorHandling PopulateProjectContents(HierarchyModel Project, 
                                                        ObservableCollection<ContentModel> ProjectContents,
                                                        string selectedStep, 
                                                        Dictionary<int, CMContentModel> contents,
                                                        Dictionary<int, CMVersionModel> versions,
                                        out Dictionary<int, InheritedContentModel> inheritedContents)
        {
            Domain.ErrorHandling Status = new Domain.ErrorHandling();
            inheritedContents = new Dictionary<int, InheritedContentModel>();
            Dictionary<int, int> branchFolders = new Dictionary<int, int>(); //foder id and depth level in hierarchy

            try
            {

                #region get inheritedContents Data table

                branchFolders.Add(0, 0); // templates under root folder
                //if (Project.ParentId == 0)
                //                return Status; //if project resides under root node

                string listOfBranchIds;
                HierarchyBLLReturnCode status = GetHierarchyBranchIdsProjectId(Project.ParentId, out listOfBranchIds);
                if (status != HierarchyBLLReturnCode.Success)
                {
                    Domain.SaveGeneralErrorLog("Failed to get hierarchy branch Ids");
                    Status.messsageId = "105";
                }

                if (!string.IsNullOrEmpty(listOfBranchIds) && !string.IsNullOrWhiteSpace(listOfBranchIds))
                {
                    string[] folderIds = listOfBranchIds.Split(',');
                    int level = 1;
                    foreach (string fId in folderIds)
                    {
                        branchFolders.Add(Convert.ToInt32(fId), level);
                        level++;
                    }
                }
                else
                {
                    listOfBranchIds = "0";
                }

                StringBuilder qry = new StringBuilder();
                if (selectedStep == string.Empty)
                {
                    selectedStep = "no step";
                }

                qry.Append("select ID, parentId, projectStep, vc.ContentSeqNo, ContentVersionId from PE_Hierarchy h ");
                qry.Append("join PE_Version v on HierarchyId = Id ");
                qry.Append("join PE_VersionContent vc on v.VersionId = vc.VersionId ");
                qry.Append("left outer join PE_ProjectStep on h.ProjectStep = StepCode ");
                qry.Append("where (ParentId in ( " + listOfBranchIds + ") or ParentId is null)");
                qry.Append("and VersionStatus = 'A' ");
                qry.Append("and (ProjectStep is null or StepDescription = '" + selectedStep + "') ");
                qry.Append("and NodeType = 'T' ");
                qry.Append("and ProjectStatus = 'O'; ");


                DataTable allInheritedContents = Domain.PersistenceLayer.FetchDataTable(qry.ToString(), CommandType.Text, null);
                #endregion get inheritedContents Data table

                #region populate inheritedContents
                if (allInheritedContents != null && allInheritedContents.Rows.Count > 0)
                {
                    foreach (DataRow ic in allInheritedContents.Rows)
                    {
                        #region inherited content from datarow
                        InheritedContentModel inhContent = new InheritedContentModel();

                        if (ic["parentId"] == DBNull.Value)
                        {
                            inhContent.parentFolderId = 0;
                        }
                        else
                        {
                            inhContent.parentFolderId = (int)ic["parentId"];
                        }

                        if (ic["projectStep"] == DBNull.Value)
                        {
                            inhContent.stepDescription = string.Empty;
                        }
                        else
                        {
                            inhContent.stepDescription = ic["projectStep"].ToString();
                        }

                        inhContent.templateProjectId = (int)ic["ID"];
                        inhContent.contentVersionId = (int)ic["ContentVersionId"];
                        inhContent.cvPriority = (int)ic["ContentSeqNo"];
                        inhContent.parentFolderLevel = branchFolders[inhContent.parentFolderId];
                        #endregion

                        if (!inheritedContents.ContainsKey(inhContent.contentVersionId))
                        {
                            inheritedContents.Add(inhContent.contentVersionId, inhContent);
                        }
                        else
                        {
                            if (inheritedContents[inhContent.contentVersionId].stepDescription == string.Empty &&
                                inhContent.stepDescription != string.Empty)
                            {
                                inheritedContents[inhContent.contentVersionId] = inhContent;
                            }
                            else if (inheritedContents[inhContent.contentVersionId].parentFolderLevel <
                                inhContent.parentFolderLevel)
                            {
                                inheritedContents[inhContent.contentVersionId] = inhContent;
                            }
                        }
                    }
                }
                else
                {
                    return Status;
                }
                #endregion populate inheritedContents

                #region Add Project contents

                if (ProjectContents != null && ProjectContents.Count > 0)
                {
                    foreach (var cv in ProjectContents)
                    {
                        InheritedContentModel projectContent = new InheritedContentModel();
                        projectContent.contentVersionId = cv.id;
                        projectContent.cvPriority = cv.seq;
                        projectContent.parentFolderLevel = -1;
                        projectContent.parentFolderId = 0;
                        projectContent.stepDescription = string.Empty;
                        projectContent.templateProjectId = 0;

                        if (!inheritedContents.ContainsKey(projectContent.contentVersionId))
                        {
                            inheritedContents.Add(projectContent.contentVersionId, projectContent);
                        }
                    }
                }

                #endregion Add Project contents

                #region Remove duplicate priority

                List<int> keysToRemove = new List<int>();
                for (int idx1 = 0; idx1 < inheritedContents.Count; idx1++)
                {
                    for (int idx2 = idx1 + 1; idx2 < inheritedContents.Count; idx2++)
                    {
                        int key1 = inheritedContents.Keys.ElementAt(idx1);
                        int key2 = inheritedContents.Keys.ElementAt(idx2);
                        if (inheritedContents[key2].cvPriority == inheritedContents[key1].cvPriority)
                        {
                            if (inheritedContents[key1].stepDescription == string.Empty &&
                                     inheritedContents[key2].stepDescription != string.Empty)
                            {
                                keysToRemove.Add(key1);
                            }
                            else if (inheritedContents[key1].parentFolderLevel <
                                inheritedContents[key2].parentFolderLevel)
                            {
                                keysToRemove.Add(key1);
                            }
                            else
                            {
                                keysToRemove.Add(key2);
                            }
                        }
                    }
                }
                if (keysToRemove != null && keysToRemove.Count > 0)
                {
                    foreach (int key in keysToRemove)
                    {
                        inheritedContents.Remove(key);
                    }
                }

                #endregion

                #region removeLinksConflicts

                #region get cvLinkedContents
                Dictionary<int, List<int>> cvLinkedContents = new Dictionary<int, List<int>>();
                foreach (var cv in inheritedContents)
                {
                    List<int> cvList = new List<int>();
                    cvList.Add(cv.Key);
                    List<int> outTempListVersions = ContentBLL.GetVersionAllLinkedSubVersions(cvList);
                    outTempListVersions.Add(versions[cv.Key].ID);
                    List<int> outTempListContents = new List<int>();
                    outTempListContents = ContentBLL.GetAllContentIds(outTempListVersions);
                    cvLinkedContents.Add(cv.Key, outTempListContents);
                }
                #endregion get cvLinkedContents

                #region Compare Linked and remove

                for (int idx1 = 0; idx1 < cvLinkedContents.Count; idx1++)
                {
                    for (int idx2 = idx1 + 1; idx2 < cvLinkedContents.Count; idx2++)
                    {
                        int key1 = inheritedContents.Keys.ElementAt(idx1);
                        int key2 = inheritedContents.Keys.ElementAt(idx2);
                        IEnumerable<int> intersection = cvLinkedContents[key2].Intersect(cvLinkedContents[key1]);
                        if (intersection != null & intersection.Count() > 0)
                        {
                            if (inheritedContents[key1].stepDescription == string.Empty &&
                                 inheritedContents[key2].stepDescription != string.Empty)
                            {
                                inheritedContents.Remove(key1);
                            }
                            else if (inheritedContents[key1].parentFolderLevel <
                                inheritedContents[key2].parentFolderLevel)
                            {
                                inheritedContents.Remove(key1);
                            }
                            else
                            {
                                inheritedContents.Remove(key2);
                            }
                        }
                    }
                }

                #endregion Compare Linked and remove

                #endregion removeLinksConflicts

                return Status;
            }
           catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Status.messsageId = "105";
                return Status;
            }
        }


        #endregion

        #region Filtered CM tree
        //Get CM tree containing only contents and versions used in projects within PE subtree
        public static void GetAllUsedCM(int FolderId, out List <int> contentIds, out List<int> contentVersionIds)
        {
            contentIds = new List<int>();
            contentVersionIds = new List<int>();
            try
            {
                StringBuilder SB = new StringBuilder();
                SB.Clear();

                SB.Append(";WITH Children as( ");
                SB.Append("SELECT id FROM pe_hierarchy ");
                SB.Append("WHERE ID = " + FolderId.ToString());
                SB.Append(" UNION ALL ");
                SB.Append("SELECT h.id FROM pe_hierarchy h ");
                SB.Append("INNER JOIN Children x ON h.ParentId = x.Id ");
                SB.Append(") ");
                SB.Append("select distinct cv.ContentVersionId, ct.CO_ID from Children c ");
                SB.Append("join PE_Hierarchy h on h.Id=c.Id ");
                SB.Append("join PE_Version v on h.Id=v.HierarchyId ");
                SB.Append("join PE_VersionContent cv on v.VersionId = cv.VersionId ");
                SB.Append("join ContentVersion ccv on cv.ContentVersionId = ccv.CV_ID ");
                SB.Append("join Content ct on ccv.CV_id_Content=ct.CO_ID ");
                SB.Append("where NodeType='P' ");
                SB.Append("and v.VersionStatus = 'A' ");

                DataTable CMData = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                if (CMData != null)
                {
                    foreach (DataRow dr in CMData.Rows)
                    {
                        if (!contentVersionIds.Contains((int)dr["ContentVersionId"]))
                        {
                            contentVersionIds.Add((int)dr["ContentVersionId"]);
                        }

                        if (!contentIds.Contains((int)dr["CO_ID"]))
                        {
                            contentIds.Add((int)dr["CO_ID"]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
            }
        }

        //Get CM tree containing only contents and versions used in templates within PE subtree
        public static void GetAllTemplatesCM(int FolderId, out List<int> contentIds, out List<int> contentVersionIds)
        {
            contentIds = new List<int>();
            contentVersionIds = new List<int>();
            try
            {
                StringBuilder SB = new StringBuilder();
                SB.Clear();

                #region Within selected folder
                SB.Append(";WITH Children as( ");
                SB.Append("SELECT id FROM pe_hierarchy ");
                SB.Append("WHERE ID = " + FolderId.ToString());
                SB.Append(" UNION ALL ");
                SB.Append("SELECT h.id FROM pe_hierarchy h ");
                SB.Append("INNER JOIN Children x ON h.ParentId = x.Id ");
                SB.Append(") ");
                SB.Append("select distinct cv.ContentVersionId, ct.CO_ID from Children c ");
                SB.Append("join PE_Hierarchy h on h.Id=c.Id ");
                SB.Append("join PE_Version v on h.Id=v.HierarchyId ");
                SB.Append("join PE_VersionContent cv on v.VersionId = cv.VersionId ");
                SB.Append("join ContentVersion ccv on cv.ContentVersionId = ccv.CV_ID ");
                SB.Append("join Content ct on ccv.CV_id_Content=ct.CO_ID ");
                SB.Append("where NodeType='T' ");
                SB.Append("and v.VersionStatus = 'A' ");
                SB.Append("and projectStatus = 'O' ");

                DataTable CMData = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                if (CMData != null)
                {
                    foreach (DataRow dr in CMData.Rows)
                    {
                        if (!contentVersionIds.Contains((int)dr["ContentVersionId"]))
                        {
                            contentVersionIds.Add((int)dr["ContentVersionId"]);
                        }

                        if (!contentIds.Contains((int)dr["CO_ID"]))
                        {
                            contentIds.Add((int)dr["CO_ID"]);
                        }
                    }
                }
                #endregion Within selected folder

                #region Parallel and above selected folder

                SB.Clear();
                SB.Append(";WITH Parent AS  ");
                SB.Append(" (SELECT id, ParentId ");
                SB.Append(" FROM PE_Hierarchy HE ");
                SB.Append(" WHERE HE.Id = " + FolderId.ToString());
                SB.Append(" UNION ALL ");
                SB.Append(" SELECT HE.id , HE.ParentId ");
                SB.Append(" FROM PE_Hierarchy HE INNER JOIN Parent  ");
                SB.Append(" On HE.id = Parent.ParentId ");
                SB.Append(" WHERE  ");
                SB.Append(" HE.id != Parent.id)  ");
                SB.Append(" select distinct cv.ContentVersionId, ct.CO_ID,ct.CO_Name, ccv.CV_Name from Parent c ");
                SB.Append(" join PE_Hierarchy h on h.ParentId=c.id ");
                SB.Append(" join PE_Version v on h.Id=v.HierarchyId ");
                SB.Append(" join PE_VersionContent cv on v.VersionId = cv.VersionId ");
                SB.Append(" join ContentVersion ccv on cv.ContentVersionId = ccv.CV_ID ");
                SB.Append(" join Content ct on ccv.CV_id_Content=ct.CO_ID ");
                SB.Append(" where NodeType='T' ");
                SB.Append(" and v.VersionStatus = 'A' ");
                SB.Append(" and projectStatus = 'O' ");
                SB.Append(" union ");
                SB.Append(" select distinct cv.ContentVersionId, ct.CO_ID ,ct.CO_Name, ccv.CV_Name ");
                SB.Append(" from PE_Hierarchy h  ");
                SB.Append(" join PE_Version v on h.Id=v.HierarchyId ");
                SB.Append(" join PE_VersionContent cv on v.VersionId = cv.VersionId ");
                SB.Append(" join ContentVersion ccv on cv.ContentVersionId = ccv.CV_ID ");
                SB.Append(" join Content ct on ccv.CV_id_Content=ct.CO_ID ");
                SB.Append(" where NodeType='T' ");
                SB.Append(" and v.VersionStatus = 'A' ");
                SB.Append(" and projectStatus = 'O' ");
                SB.Append(" and h.ParentId is null ");

                CMData.Clear();
                CMData = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                if (CMData != null)
                {
                    foreach (DataRow dr in CMData.Rows)
                    {
                        if (!contentVersionIds.Contains((int)dr["ContentVersionId"]))
                        {
                            contentVersionIds.Add((int)dr["ContentVersionId"]);
                        }

                        if (!contentIds.Contains((int)dr["CO_ID"]))
                        {
                            contentIds.Add((int)dr["CO_ID"]);
                        }
                    }
                }
                #endregion Parallel and above selected folder
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
            }
        }

        //Get CM tree containing only contents and versions used in templates AND projects within PE subtree
        public static void GetAllTemplatesUsedCM(int FolderId, out List<int> contentIds, out List<int> contentVersionIds)
        {
            contentIds = new List<int>();
            contentVersionIds = new List<int>();

            List<int> contentIdsUsed = new List<int>();
            List<int> contentVersionIdsUsed = new List<int>();
            List<int> contentIdsTemplates = new List<int>();
            List<int> contentVersionIdsTemplates = new List<int>();
            try
            {
                GetAllTemplatesCM(FolderId, out contentIdsTemplates, out contentVersionIdsTemplates);
                GetAllUsedCM(FolderId, out contentIdsUsed, out contentVersionIdsUsed);

                if (contentIdsTemplates != null)
                {
                    contentIds = contentIdsTemplates.Intersect(contentIdsUsed).ToList();
                    contentVersionIds = contentVersionIdsTemplates.Intersect(contentVersionIdsUsed).ToList();
                }
                else
                {
                    contentIds = contentIdsUsed;
                    contentVersionIds = contentVersionIdsUsed;
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
            }
        }

        #endregion

        public static Domain.ErrorHandling GetProjectActiveContents(int projectId, out ObservableCollection<ContentModel> projectActiveContents
            )
        {
            Domain.ErrorHandling Status = new Domain.ErrorHandling();

            projectActiveContents = new ObservableCollection<ContentModel>();

            try
            {
                HierarchyModel Hierarchy = HierarchyBLL.GetHierarchyRow((long)projectId);
                if (Hierarchy == null || Hierarchy.Id <= 0)
                {
                    Status.messsageId = "dummy";
                    Status.messageParams[0] = "Project Id: " + projectId.ToString();
                    Status.messageParams[1] = "Project does not exist in this environment.";
                    return Status;
                }

                Hierarchy.VM = VersionBLL.GetVersionRow(Hierarchy.Id);
                projectActiveContents = ContentBLL.getActiveContentsPartialModel(Hierarchy.VM.VersionName, Hierarchy.Id);

                return Status;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Status.messsageId = "105";
                Status.messageParams[0] = logMessage;
                return Status;
            }
        }

        #region Retrieve Hierarchy Row by project Code and Step and return as HierarchyModel

        public static HierarchyModel GetHierarchyModelByCodeAndStep(string projectCode, string projectStep)
        {
            // Initialize work fields
            HierarchyModel Node = Domain.GetBusinessObject<HierarchyModel>();
            try
            {
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("Select * FROM PE_Hierarchy WHERE ProjectCode = '" + projectCode + "'");
                QryStr.Append(" and ProjectStep = '" + projectStep + "'");
                // Fetch the DataTable from the database
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null && ResTable.Rows.Count == 1)
                {
                    DataRow DataRow = ResTable.Rows[0];
                    //
                    Node.Id = (int)DataRow["Id"];
                    Node.IsNew = false;
                    Node.IsDirty = false;
                    if (DataRow["ParentId"] == DBNull.Value)
                    {
                        Node.ParentId = 0;
                    }
                    else
                    {
                        Node.ParentId = (int)DataRow["ParentId"];
                    }
                    if (!(DataRow["Sequence"] is System.DBNull))
                    {
                        Node.Sequence = Convert.ToInt32(DataRow["Sequence"]);
                    }

                    switch ((string)DataRow["NodeType"])
                    {
                        case "P":
                            Node.NodeType = NodeTypes.P;
                            break;
                        default:
                            Node.NodeType = NodeTypes.P;
                            break;
                    }
                    Node.Name = (string)DataRow["Name"];
                    Node.Description = (string)DataRow["Description"];
                    Node.CreationDate = (DateTime)DataRow["CreationDate"];
                    Node.LastUpdateTime = (DateTime)DataRow["LastUpdateTime"];
                    Node.LastUpdateUser = (string)DataRow["LastUpdateUser"];

                    if ((string)(DataRow["NodeType"]) == "P")
                    {
                        if (!(DataRow["ProjectCode"] is System.DBNull))
                        {
                            Node.Code = (string)DataRow["ProjectCode"].ToString().Trim();
                        }
                        if (!(DataRow["ProjectStep"] is System.DBNull))
                        {
                            string stepName = GetStepNameByCode((string)DataRow["ProjectStep"]);
                            Node.SelectedStep = stepName.ToString();
                        }
                        if (!(DataRow["SCDUSASyncInd"] is System.DBNull))
                        {
                            Node.Synchronization = (Boolean)DataRow["SCDUSASyncInd"];
                        }
                        Node.IRISTechInd = (DataRow["IRISTechInd"] is System.DBNull) ? false : (Boolean)DataRow["IRISTechInd"];

                        if (!(DataRow["ProjectStatus"] is System.DBNull))
                        {
                            switch ((string)DataRow["ProjectStatus"])
                            {
                                case "O":
                                    Node.ProjectStatus = "Open";
                                    break;
                                case "D":
                                    Node.ProjectStatus = "Disabled";
                                    break;
                                default:
                                    Node.ProjectStatus = "Open";
                                    break;
                            }
                        }
                    }
                    string treeHeader = string.Empty;
                    GetProjectFullPathByProjectId(Node.Id, out treeHeader);
                    Node.TreeHeader = treeHeader;
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

    }
}
