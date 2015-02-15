﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ATSBusinessObjects;
using ATSBusinessObjects.ContentMgmtModels;
using ATSDomain;
using Infra.DAL;
using System.Linq;


namespace ATSBusinessLogic
{
    public  class TemplateBLL
    {

        #region GetALLSteps //get all steps from projects family

        public static ObservableCollection<string> GetAllSteps(ObservableCollection<HierarchyModel> Children)
        {
            ObservableCollection<string> Steps = new ObservableCollection<string>();
            var SB = new StringBuilder(string.Empty);
            try
            {
                //flag to indicate if it is the first condition of the query.
                bool IsFirstCondition = false;
                //flg to Indicate if there is a project will step null.
                bool IsNullStep = false;
                SB.Append("SELECT PS.StepDescription From PE_ProjectStep PS ");
                foreach (var Hierarchy in Children)
                {
                    if (Hierarchy.ProjectStatus != "Disabled")
                    {
                        if (!String.IsNullOrEmpty(Hierarchy.SelectedStep.Trim()))
                        {
                            if (!IsFirstCondition)
                            {
                                SB.Append("WHERE StepDescription != '" + Hierarchy.SelectedStep + "' ");
                                IsFirstCondition = true;
                            }
                            else
                            {
                                SB.Append("  and StepDescription!= '" + Hierarchy.SelectedStep + "'");
                            }
                        }
                        else
                        {
                            IsNullStep = true;

                        }
                    }
                }

                //There is no project with step null.
                if (!IsNullStep)
                    Steps.Add("");

                // Fetch the DataTable from the database
                DataTable ResTable = ATSDomain.Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);

                if (ResTable != null)
                {
                    foreach (DataRow DataRow in ResTable.Rows)
                    {
                        //Populate steps with returning steps from DB.
                        Steps.Add(DataRow["StepDescription"].ToString());
                    }
                }


                return Steps;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                return Steps;
            }
        }

        #endregion GetALLSteps //get all steps from projects family.

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
            ProjectStatusEnum projectStatus = ProjectStatusEnum.O;
            if (string.IsNullOrEmpty(Hierarchy.ProjectStatus) || string.IsNullOrWhiteSpace(Hierarchy.ProjectStatus) || Hierarchy.ProjectStatus != "Disabled")
            {
                projectStatus = ProjectStatusEnum.O;
            }
            else
            {
                projectStatus = ProjectStatusEnum.D;
            }

            // Do work
            try
            {
                SB.Append("SELECT COUNT(*) FROM PE_Hierarchy WHERE Name = '" + Hierarchy.Name + "' ");
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

                HierarchyBLL.UpdateControlFields(ref Hierarchy); // Update Control fields
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

                else  // Existing row; construct UPDATE
                {

                    // Build the Query
                    SB.Append("UPDATE PE_Hierarchy SET ParentId=@ParentId, Sequence=@Sequence, NodeType=@NodeType, Name=@Name, Description=@Description, ");
                    SB.Append("ProjectStep=@ProjectStep, ProjectCode=@ProjectCode, ProjectStatus=@ProjectStatus, ");
                    SB.Append("LastUpdateTime=@LastUpdateTime, LastUpdateUser=@LastUpdateUser, LastUpdateComputer=@LastUpdateComputer, LastUpdateApplication=@LastUpdateApplication, SCDUSASyncInd=@SCDUSASyncInd, IRISTechInd=@IRISTechInd ");
                    SB.Append("WHERE Id=@Id");
                }


                string step = HierarchyBLL.GetStepCodeByName(Hierarchy.SelectedStep.Trim());

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
                new ParamStruct { ParamName = "ProjectCode", DataType = DbType.String, Value = (object)DBNull.Value},
                new ParamStruct { ParamName = "ProjectStatus", DataType = DbType.String, Value = projectStatus.ToString()},
                new ParamStruct { ParamName = "CreationDate", DataType = DbType.DateTime, Value = Hierarchy.CreationDate },
                new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = Hierarchy.LastUpdateTime },
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
                new ParamStruct { ParamName = "HierarchyId", DataType = DbType.Int32, Value = Hierarchy.Id },
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
                                return "141";
                            }
                        }
                    }

                    //Fixed bug 3046 - added IsDirty condition
                    if ((Hierarchy.VM.Contents.Count > 0 && Hierarchy.VM.IsDirty == true))
                    {
                        //Boolean cert = CheckCertificateList(Hierarchy.Certificates);
                        int J = 1;
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
                        new ParamStruct { ParamName = "ContentSeqNo", DataType = DbType.Int32, Value = I.seq },
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
                            J++;
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

        #region CloneTemplate

        public static string PersistCloneTemplate(ref HierarchyModel Hierarchy)
        {
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
                SB.Append("SELECT COUNT(*) FROM PE_Hierarchy WHERE Name = '" + Hierarchy.Name + "' ");
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

                HierarchyBLL.UpdateControlFields(ref Hierarchy); // Update Control fields
                SB.Clear();

                //get step FK according to description


                    // Set Creation DateTime
                    Hierarchy.CreationDate = DateTime.Now;
                    // Build the Query
                    SB.Append("INSERT INTO PE_Hierarchy (ParentId, Sequence, NodeType, Name, Description, ProjectStep, ProjectCode, ProjectStatus, CreationDate, LastUpdateTime, LastUpdateUser, LastUpdateComputer, LastUpdateApplication, SCDUSASyncInd, IRISTechInd ) ");
                    SB.Append("VALUES (@ParentId, @Sequence, @NodeType, @Name, @Description, @ProjectStep, @ProjectCode, @ProjectStatus, @CreationDate, @LastUpdateTime, @LastUpdateUser, @LastUpdateComputer, @LastUpdateApplication, @SCDUSASyncInd, @IRISTechInd) ");
                    if (DatabaseSupportsBatchQueries)
                    {
                        SB.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                    }
              

                string step = HierarchyBLL.GetStepCodeByName(Hierarchy.SelectedStep.Trim());

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
                new ParamStruct { ParamName = "ProjectCode", DataType = DbType.String, Value = (object)DBNull.Value},
                new ParamStruct { ParamName = "ProjectStatus", DataType = DbType.String, Value = ProjectStatusEnum.O.ToString()},
                new ParamStruct { ParamName = "CreationDate", DataType = DbType.DateTime, Value = Hierarchy.CreationDate },
                new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = Hierarchy.LastUpdateTime },
                new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = Hierarchy.LastUpdateUser },
                new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = Hierarchy.LastUpdateComputer },
                new ParamStruct { ParamName = "LastUpdateApplication", DataType = DbType.String, Value = Hierarchy.LastUpdateApplication },
                new ParamStruct { ParamName = "SCDUSASyncInd", DataType = DbType.Boolean, Value = Hierarchy.Synchronization },
                new ParamStruct { ParamName = "IRISTechInd", DataType = DbType.Boolean, Value = Hierarchy.IRISTechInd },
                new ParamStruct { ParamName = "GroupId", DataType = DbType.Int32, Value = Hierarchy.GroupId }
                };

                long RV = 0;

                var VersionsQuery = new StringBuilder(string.Empty);

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
                else // All OK
                {
                    long RvVersions = 0;
                    bool IsVersionRequired = false;
                    // Update the object

                    if (DatabaseSupportsBatchQueries)
                    {
                        Hierarchy.Id = RV;

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
                new ParamStruct { ParamName = "HierarchyId", DataType = DbType.Int32, Value = Hierarchy.Id },
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

                        if (DatabaseSupportsBatchQueries )
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
                                return "141";
                            }
                        }
                    }

                    //Fixed bug 3046 - added IsDirty condition
                    if ((Hierarchy.VM.Contents.Count > 0 && Hierarchy.VM.IsDirty == true))
                    {
                        //Boolean cert = CheckCertificateList(Hierarchy.Certificates);
                        //int J = 1;
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
                        new ParamStruct { ParamName = "VersionId", DataType = DbType.Int32, Value = RvVersions},
                        new ParamStruct { ParamName = "ContentVersionId", DataType = DbType.Int32, Value = I.id},
                        new ParamStruct { ParamName = "ContentSeqNo", DataType = DbType.Int32, Value = I.seq },
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
                            //J++;
                        }
                    }
                    if (Hierarchy.UserCertificates.Count > 0)
                    {
                        foreach (var i in Hierarchy.UserCertificates)
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

        #region Get contents for projects (New Project)

        public static Dictionary<int, int> GetAllContents(string BranchIds , Dictionary<int, CMVersionModel> outVersions, bool ParentHirearchy)
        {
            var SB = new StringBuilder(string.Empty);
            Dictionary<int, int> Contents = new Dictionary<int, int>();
            try
            {
                string[] Ids = BranchIds.Split(',');
                //Bool Indicate if parent of hirearchy is sent or hirearchy id.
                //In cases which hirearchy is new and not exists.
                if(ParentHirearchy) //if true then parent id is sent. 
                 Contents = GetAllContentsForBranchParentID(Ids,  outVersions);
                else
                 Contents = GetAllContentsForBranch(Ids, outVersions);

               return Contents;
               
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                return Contents;
            }
          
        }



        public static Dictionary<int, int> GetAllContentsForBranch(string[] BranchIds, Dictionary<int, CMVersionModel> outVersions)
        {
            var SB = new StringBuilder(string.Empty);
            //Collection for existing contetns. (key - content version id, value content id)
            Dictionary<int, int> Contents = new Dictionary<int, int>();
            //collection for linked version contents of existing  contents (key - content version id, value content id)
            Dictionary<int, int> LinkedContents= new Dictionary<int, int>();
            try
            {
                //The returing order is bottom up - the last is project id. no need to check it.
                for (int i = BranchIds.Length - 2; i >= 0; i--)
                //for (int i = 0; i < BranchIds.Length - 1; i++)
                {
                    //Getting all contents from templates with this parent.
                    SB.Append("SELECT DISTINCT VC.ContentVersionId FROM PE_Hierarchy H INNER JOIN PE_Version V ON  H.Id = V.HierarchyId ");
                    SB.Append(" INNER JOIN PE_VersionContent VC ON V.VersionId = VC.VersionId WHERE H.ParentId = '" + BranchIds[i] + "' AND H.NodeType='T' AND H.ProjectStatus = 'O' AND H.ProjectStep IS NULL ");
                    DataTable ResTable = ATSDomain.Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                    if (ResTable != null)
                    {
                        ContentValidation(ResTable, outVersions, Contents, LinkedContents);
                    }
                }
                //FOR PARENT ID NULL:
                SB.Clear();
                SB.Append("SELECT DISTINCT VC.ContentVersionId FROM PE_Hierarchy H INNER JOIN PE_Version V ON  H.Id = V.HierarchyId ");
                SB.Append(" INNER JOIN PE_VersionContent VC ON V.VersionId = VC.VersionId WHERE H.ParentId IS NULL AND H.NodeType='T' AND H.ProjectStatus = 'O' AND H.ProjectStep IS NULL ");
                DataTable ResTableNULL = ATSDomain.Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                if (ResTableNULL != null)
                {
                    ContentValidation(ResTableNULL, outVersions, Contents, LinkedContents);
                }

                return Contents;
            }
          
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                return Contents;
            }

        }

        public static void  CheckSubLinkedVersions(Dictionary<int, CMContentVersionSubVersionModel> ContentVersions, Dictionary<int, int> LinkedCollection)
        {
            foreach (var i in ContentVersions)
            {
                LinkedCollection.Add(i.Key, i.Value.ContentSubVersion.ParentID);
                CheckSubLinkedVersions(i.Value.ContentSubVersion.ContentVersions, LinkedCollection);
            }
        }


        //Validation for parent ID IS NOT NULL
        public static void ContentValidation(DataTable ResTable , Dictionary<int, CMVersionModel> outVersions, Dictionary<int, int> Contents, Dictionary<int, int> LinkedContents)
        {
            foreach (DataRow DataRow in ResTable.Rows)
            {
                int ContentVersionID = (int)DataRow["ContentVersionId"];
                int ContentID = outVersions[ContentVersionID].ParentID;
                //the content is already exists
                if (Contents.ContainsKey(ContentVersionID) || Contents.ContainsValue(ContentID))
                    break;
                else
                {
                    //check if the linked contents of the current list (contents ) contain the new content.
                    if (LinkedContents.ContainsKey(ContentVersionID) || LinkedContents.ContainsValue(ContentID))
                        break;
                    else
                    {
                        Dictionary<int, int> LinkedCollection = new Dictionary<int, int>();
                        //Getting all linked contents of the current content.
                        CheckSubLinkedVersions(outVersions[ContentVersionID].ContentVersions, LinkedCollection);
                        bool IsContain = false;
                        foreach (var j in LinkedCollection)
                        {
                            if (Contents.ContainsKey(j.Key) || Contents.ContainsValue(j.Value))
                            {
                                IsContain = true;
                                break;
                            }
                            if (LinkedContents.ContainsKey(j.Key) || LinkedContents.ContainsValue(j.Value))
                            {
                                IsContain = true;
                                break;
                            }
                        }
                        if (!IsContain)
                        {
                            Contents.Add(ContentVersionID, ContentID);
                            foreach (var j in LinkedCollection)
                            {
                                LinkedContents.Add(j.Key, j.Value);
                            }
                        }
                        else
                            break;

                    }

                }
            }
        }

        public static Dictionary<int, int> GetAllContentsForBranchParentID(string[] BranchIds, Dictionary<int, CMVersionModel> outVersions)
        {
            var SB = new StringBuilder(string.Empty);
            //Collection for existing contetns. (key - content version id, value content id)
            Dictionary<int, int> Contents = new Dictionary<int, int>();
            //collection for linked version contents of existing  contents (key - content version id, value content id)
            Dictionary<int, int> LinkedContents = new Dictionary<int, int>();
            try
            {
                //Checking all branchs - the last one is hirerachy parent id. (hirerachy id is not saved yet in DB.)
                for (int i = BranchIds.Length - 1; i >= 0; i--)
                //for (int i = 0; i < BranchIds.Length - 1; i++)
                {
                    //Getting all contents from templates with this parent.
                    SB.Append("SELECT DISTINCT VC.ContentVersionId ");
                    SB.Append("FROM PE_Hierarchy H ");
                    SB.Append("INNER JOIN PE_Version V ON  H.Id = V.HierarchyId ");
                    SB.Append(" INNER JOIN PE_VersionContent VC ON V.VersionId = VC.VersionId ");
                    SB.Append("WHERE H.ParentId = '" + BranchIds[i] + "' AND H.NodeType='T' AND H.ProjectStatus = 'O' ");

                    DataTable ResTable = ATSDomain.Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                    if (ResTable != null)
                    {
                        ContentValidation(ResTable, outVersions, Contents, LinkedContents);
                    }
                }
                //FOR PARENT ID NULL:
                SB.Clear();
                SB.Append("SELECT DISTINCT VC.ContentVersionId ");
                SB.Append("FROM PE_Hierarchy H INNER ");
                SB.Append("JOIN PE_Version V ON  H.Id = V.HierarchyId ");
                SB.Append(" INNER JOIN PE_VersionContent VC ON V.VersionId = VC.VersionId ");
                SB.Append("WHERE H.ParentId IS NULL AND H.NodeType='T' AND H.ProjectStatus = 'O' ");

                DataTable ResTableNULL = ATSDomain.Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                if (ResTableNULL != null)
                {
                    ContentValidation(ResTableNULL, outVersions, Contents, LinkedContents);
                }

                return Contents;
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                return Contents;
            }

        }
        

        #endregion Get contents for projects

        #region Get contents for projects  (Changing step)

        public static Dictionary<int, int> GetAllContentsForStep(string BranchIds, Dictionary<int, CMVersionModel> outVersions, string Step, IEnumerable<ContentModel> ActiveContents)
        {
            var SB = new StringBuilder(string.Empty);
            Dictionary<int, int> Contents = new Dictionary<int, int>();
            try
            {
                string[] Ids = BranchIds.Split(',');

                //Getting step code from step description.
                string step = HierarchyBLL.GetStepCodeByName(Step.Trim());
                Contents = GetAllContentsForBranchForStep(Ids, outVersions, step, ActiveContents);
               

                return Contents;

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                return Contents;
            }

        }


        public static Dictionary<int, int> GetAllContentsForBranchForStep(string[] BranchIds, Dictionary<int, CMVersionModel> outVersions, string step, IEnumerable<ContentModel> ActiveContents)
        {
            var SB = new StringBuilder(string.Empty);
            //Collection for existing contetns. (key - content version id, value content id)
            Dictionary<int, int> Contents = new Dictionary<int, int>();
            //collection for linked version contents of existing  contents (key - content version id, value content id)
            Dictionary<int, int> LinkedContents = new Dictionary<int, int>();
            try
            {
                //The returing order is bottom up - the last is project id. no need to check it.
                for (int i = BranchIds.Length - 2; i >= 0; i--)
                {
                    //Getting all contents from templates with this parent.
                    SB.Append("SELECT DISTINCT VC.ContentVersionId FROM PE_Hierarchy H INNER JOIN PE_Version V ON  H.Id = V.HierarchyId ");
                    SB.Append(" INNER JOIN PE_VersionContent VC ON V.VersionId = VC.VersionId WHERE H.ParentId = '" + BranchIds[i] + "' AND H.NodeType='T' AND H.ProjectStatus = 'O' AND H.ProjectStep IS NULL ");
                    DataTable ResTable = ATSDomain.Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                    if (ResTable != null)
                    {
                        ContentValidation(ResTable, outVersions, Contents, LinkedContents);
                    }
                }
                //FOR PARENT ID NULL:
                SB.Clear();
                SB.Append("SELECT DISTINCT VC.ContentVersionId FROM PE_Hierarchy H INNER JOIN PE_Version V ON  H.Id = V.HierarchyId ");
                SB.Append(" INNER JOIN PE_VersionContent VC ON V.VersionId = VC.VersionId WHERE H.ParentId IS NULL AND H.NodeType='T' AND H.ProjectStatus = 'O' AND H.ProjectStep IS NULL ");
                DataTable ResTableNULL = ATSDomain.Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                if (ResTableNULL != null)
                {
                    ContentValidation(ResTableNULL, outVersions, Contents, LinkedContents);
                }

                //---Performing the same validation for new step(not null)--- //

                for (int i = BranchIds.Length - 2; i >= 0; i--)
                {
                    //Getting all contents from templates with this parent.
                    SB.Append("SELECT DISTINCT VC.ContentVersionId FROM PE_Hierarchy H INNER JOIN PE_Version V ON  H.Id = V.HierarchyId ");
                    SB.Append(" INNER JOIN PE_VersionContent VC ON V.VersionId = VC.VersionId WHERE H.ParentId = '" + BranchIds[i] + "' AND H.NodeType='T' AND H.ProjectStatus = 'O' AND H.ProjectStep ='"+ step +"'");
                    DataTable ResTable = ATSDomain.Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                    if (ResTable != null)
                    {
                        ContentValidation(ResTable, outVersions, Contents, LinkedContents);
                    }
                }

                //FOR PARENT ID NULL:
                SB.Clear();
                SB.Append("SELECT DISTINCT VC.ContentVersionId FROM PE_Hierarchy H INNER JOIN PE_Version V ON  H.Id = V.HierarchyId ");
                SB.Append(" INNER JOIN PE_VersionContent VC ON V.VersionId = VC.VersionId WHERE H.ParentId IS NULL AND H.NodeType='T' AND H.ProjectStatus = 'O' AND H.ProjectStep ='" + step + "'");
                DataTable ResTableNullParent = ATSDomain.Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                if (ResTableNULL != null)
                {
                    ContentValidation(ResTableNullParent, outVersions, Contents, LinkedContents);
                }
                

                //--Preformaing for project existing contents --//
                ContentValidationForProject(ActiveContents, outVersions, Contents, LinkedContents);


                return Contents;
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                return Contents;
            }

        }





        public static void ContentValidationForProject(IEnumerable<ContentModel> ActiveContents, Dictionary<int, CMVersionModel> outVersions, Dictionary<int, int> Contents, Dictionary<int, int> LinkedContents)
        {
            if (ActiveContents != null)
            {
                foreach (var i in ActiveContents)
                {
                    int ContentVersionID = i.id;
                    int ContentID = outVersions[ContentVersionID].ParentID;
                    //the content is already exists
                    if (Contents.ContainsKey(ContentVersionID) || Contents.ContainsValue(ContentID))
                        break;
                    else
                    {
                        //check if the linked contents of the current list (contents ) contain the new content.
                        if (LinkedContents.ContainsKey(ContentVersionID) || LinkedContents.ContainsValue(ContentID))
                            break;
                        else
                        {
                            Dictionary<int, int> LinkedCollection = new Dictionary<int, int>();
                            //Getting all linked contents of the current content.
                            CheckSubLinkedVersions(outVersions[ContentVersionID].ContentVersions, LinkedCollection);
                            bool IsContain = false;
                            foreach (var j in LinkedCollection)
                            {
                                if (Contents.ContainsKey(j.Key) || Contents.ContainsValue(j.Value))
                                {
                                    IsContain = true;
                                    break;
                                }
                                if (LinkedContents.ContainsKey(j.Key) || LinkedContents.ContainsValue(j.Value))
                                {
                                    IsContain = true;
                                    break;
                                }
                            }
                            if (!IsContain)
                            {
                                Contents.Add(ContentVersionID, ContentID);
                                foreach (var j in LinkedCollection)
                                {
                                    LinkedContents.Add(j.Key, j.Value);
                                }
                            }
                            else
                                break;

                        }

                    }
                }
            }
        }


        #endregion Get contents for projects

        #region Steps validations

        public static List<string> GetAvailableSteps(long folderId)
        {
            List<string> availableSteps = new List<string>();
            try
            {
                var SBstep = new StringBuilder(string.Empty);
                bool excludeNullStep = false;
                if (folderId > 0)
                {
                    SBstep.Append("(select distinct StepDescription from PE_ProjectStep ps ");
                    SBstep.Append("right outer join PE_Hierarchy h ");
                    SBstep.Append("on h.ProjectStep = ps.StepCode ");
                    SBstep.Append("where NodeType = 'T' ");
                    SBstep.Append("and ProjectStatus = 'O' ");
                    SBstep.Append("and ParentId = " + folderId);
                    SBstep.Append("union all ");
                    SBstep.Append("select StepDescription from PE_ProjectStep) ");
                    SBstep.Append("except ");
                    SBstep.Append("(select StepDescription from PE_ProjectStep ps  ");
                    SBstep.Append("right outer join PE_Hierarchy h ");
                    SBstep.Append("on h.ProjectStep = ps.StepCode ");
                    SBstep.Append("where NodeType = 'T' ");
                    SBstep.Append("and ProjectStatus = 'O' ");
                    SBstep.Append("and ParentId = " + folderId);
                    SBstep.Append(" intersect ");
                    SBstep.Append("select StepDescription from PE_ProjectStep ps)  ");
                }
                else
                {
                    SBstep.Append("(select distinct StepDescription from PE_ProjectStep ps ");
                    SBstep.Append("right outer join PE_Hierarchy h ");
                    SBstep.Append("on h.ProjectStep = ps.StepCode ");
                    SBstep.Append("where NodeType = 'T' ");
                    SBstep.Append("and ProjectStatus = 'O' ");
                    SBstep.Append("and ParentId is null ");
                    SBstep.Append("union all ");
                    SBstep.Append("select StepDescription from PE_ProjectStep) ");
                    SBstep.Append("except ");
                    SBstep.Append("(select StepDescription from PE_ProjectStep ps  ");
                    SBstep.Append("right outer join PE_Hierarchy h ");
                    SBstep.Append("on h.ProjectStep = ps.StepCode ");
                    SBstep.Append("where NodeType = 'T' ");
                    SBstep.Append("and ProjectStatus = 'O' ");
                    SBstep.Append("and ParentId is null ");
                    SBstep.Append(" intersect ");
                    SBstep.Append("select StepDescription from PE_ProjectStep ps)  ");
                }

                // Fetch the DataTable from the database
                DataTable ResTable = ATSDomain.Domain.PersistenceLayer.FetchDataTable(SBstep.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTable != null)
                {
                    foreach (DataRow DataRow in ResTable.Rows)
                    {
                        if (DataRow["StepDescription"] != DBNull.Value)
                        {
                            availableSteps.Add(DataRow["StepDescription"].ToString());
                        }
                        else
                        {
                            excludeNullStep = true;
                        }
                    }
                    if (!excludeNullStep)
                    {
                        availableSteps.Add(" ");
                    }
                }
                return availableSteps;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return availableSteps;
            }
        }

        public static Domain.ErrorHandling IsStepAvailable(HierarchyModel Project, long parentId)
        {
            Domain.ErrorHandling Status = new Domain.ErrorHandling();
            List<string> availableSteps = GetAvailableSteps(parentId);

            try
            {
                if (availableSteps == null || availableSteps.Count == 0) //Templates for all step exist
                {
                    Status.messsageId = "165";
                    return Status;
                }

                if ((string.IsNullOrWhiteSpace(Project.SelectedStep) || string.IsNullOrEmpty(Project.SelectedStep))
                                        && !availableSteps.Contains(" "))
                {
                    Status.messsageId = "166";
                    Status.messageParams[0] = "NULL";
                    return Status;
                }

                if (!availableSteps.Contains(Project.SelectedStep))
                {
                    object[] ArgsList = new object[] { 0 };
                    if (!(string.IsNullOrWhiteSpace(Project.SelectedStep) || string.IsNullOrEmpty(Project.SelectedStep)))
                    {
                        ArgsList = new object[] { Project.SelectedStep };
                    }
                    else
                    {
                        ArgsList = new object[] { "NULL" };
                    }

                    Status.messsageId = "166";
                    Status.messageParams = ArgsList;
                }

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

        #region updateActiveContentSequence

        public static IEnumerable<ContentModel> updateActiveContentSequence(ContentModel contentToAction, IEnumerable<ContentModel> activeContents, int seq)
        {
            List<ContentModel> cm = activeContents.ToList();
            
            cm.Remove(contentToAction);
            contentToAction.seq = seq;
            cm.Add(contentToAction);

            activeContents = cm;
            activeContents = activeContents.OrderBy(con => con.seq);

            return activeContents;
        }

        #endregion
    }
}

