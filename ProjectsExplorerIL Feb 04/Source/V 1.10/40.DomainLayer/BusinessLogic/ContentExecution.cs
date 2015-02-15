using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using ATSBusinessLogic.ContentMgmtBLL;
using ATSBusinessObjects;
using ATSBusinessObjects.ContentMgmtModels;
using ATSDomain;
using Infra.DAL;
using ResourcesProvider;


namespace ATSBusinessLogic
{
    //Ella - ContentExecution class

    public class ContentExecutionBLL
    {

        #region Project CM data

        static ErrorHandling GetListOfAllProjectVersions(ObservableCollection<ContentModel> projectContents, out List<int> listOfContentIds)
        {
            listOfContentIds = new List<int>();

            ErrorHandling Status = new ErrorHandling();
            List<int> vIds = new List<int>();
            List<int> linkedVersionIds = new List<int>();
            List<int> allVIds = new List<int>();

            try
            {
                if (projectContents != null && projectContents.Count > 0)
                {
                    foreach (ContentModel v in projectContents)
                    {
                        vIds.Add(v.id);
                    }

                    allVIds = ContentBLL.GetVersionAllLinkedSubVersions(vIds);
                    allVIds.AddRange(vIds);
                    listOfContentIds = ContentBLL.GetAllContentIds(allVIds);
                    return Status;
                }
                Status.errorId = "105";
                
                return Status;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Status.errorId = "105";
                Status.errorParams[0] = logMessage;
                
                return Status;
            }
        }

        public static ErrorHandling GetCMSubTree(ObservableCollection<ContentModel> projectContents,
                                                    out Dictionary<int, CMFolderModel> outFolders,  
                                                    out Dictionary<int, CMContentModel> outContents, 
                                                    out Dictionary<int, CMVersionModel> outVersions)
        {
            //Get contents, folder and version tree from API
            outFolders = new Dictionary<int, CMFolderModel>();
            outContents = new Dictionary<int, CMContentModel>();
            outVersions = new Dictionary<int, CMVersionModel>();
            ErrorHandling Status = new ContentExecutionBLL.ErrorHandling();
            List<int> allCIds = new List<int>();
            int errorId = -1;

            try
            {
                //Performance09
                Status = ContentExecutionBLL.GetListOfAllProjectVersions((ObservableCollection<ContentModel>)projectContents, out allCIds);
                if (Status.errorId != string.Empty)
                {
                    return Status;
                }

                ContentBLL.CMApiReturnCode callCMResult1 = ContentBLL.GetContentsSubTree(allCIds, out outFolders, out outContents, out outVersions);
                if (callCMResult1 != ContentBLL.CMApiReturnCode.Success) //Failed to get Contents tree
                {
                    errorId = ExecutionErrorHandler("ContentBLL", (int)callCMResult1);
                    Status.errorId = Convert.ToString(errorId);
                    Status.errorParams[0] = "Failed to get Contents sub tree";
                    return Status;
                }
                return Status;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Status.errorId = "105";
                Status.errorParams[0] = logMessage;
                
                return Status;
            }
        }

        #endregion

        #region Prior validations

        static ErrorHandling ExecutionLastUpdateCheck(HierarchyModel hierarchy)
        {
            ErrorHandling Status = new ErrorHandling();
            try
            {
                string updateCheck = HierarchyBLL.LastUpadateCheck(ref hierarchy);
                if (!(String.IsNullOrEmpty(updateCheck)))
                {
                    Status.errorId = updateCheck;
                    return Status;
                }
                updateCheck = VersionBLL.LastUpadateVersionCheck(ref hierarchy);
                if (!(String.IsNullOrEmpty(updateCheck)))
                {
                    Status.errorId = updateCheck;
                    return Status;
                }
                return Status;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Status.errorId = "105";
                return Status;
            }
        }

        public static ErrorHandling PriorValidations(HierarchyModel hierarchy, string permission)
        {
            ErrorHandling Status = new ErrorHandling();
            try
            {
                Status = ExecutionLastUpdateCheck(hierarchy);
                if (Status.errorId != string.Empty)
                    return Status;
                if (Domain.IsPermitted(permission))
                {
                    return Status;
                }
                else
                {
                    Status.errorId = "106";
                }
                return Status;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Status.errorId = "105";
                return Status;
            }
        }

        #endregion

        #region Validations

        public static ErrorHandling validations(Dictionary<int, CMContentModel> contents,
                                Dictionary<int, CMVersionModel> versions,
                                ContentModel contentToAction, HierarchyModel Hierarchy)
        {
            ErrorHandling Status = new ErrorHandling();
            try
            {
                //Check for content certificate free
                ContentBLL.CMApiReturnCode cerFree = ContentBLL.checkCertificateFree(contentToAction.id, contents, versions);
                if (cerFree == ContentBLL.CMApiReturnCode.Success)
                {
                    //Certificate is free, user certificate validations
                    //return userCertificatesValidations(Hierarchy);

                    //content is certificate free, skip validation
                    return Status;
                }
                if (cerFree == ContentBLL.CMApiReturnCode.ContentVersionNotFound)
                {
                    Status.errorId = Convert.ToString(ExecutionErrorHandler("ContentBLL", (int)cerFree));
                    return Status;
                }

                Status = stationCertificatesValidations(Hierarchy);
                if (Status.errorId != string.Empty)
                {
                    return Status;
                }

                //moved to VM, due to previous pop-up
                //Status = userCertificatesValidations(Hierarchy);
                return Status;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Status.errorId = "105";
                return Status;
            }
        }

        public static ErrorHandling userCertificatesValidations(HierarchyModel Hierarchy)
        {
            ErrorHandling Status = new ErrorHandling();

            try
            {
                //Get branch certificates
                List<String> HierarchyBranchCertificates = new List<string>();
                String status = UserCertificateBLL.GetHierarchyBranchCertificatesByProjectId(Hierarchy.Id, out HierarchyBranchCertificates);
                if (status != "0")
                {
                    Status.errorId = "105";
                    return Status;
                }

                //Get user certificates
                List<String> userCertificates = new List<string>();
                status = UserCertificateBLL.GetCertificatesByUserId(out userCertificates);
                if (status != "0")
                {
                    Status.errorId = "105";
                    return Status;
                }


                List<String> missingCertificates = new List<String>();

                missingCertificates = UserCertificateBLL.IsWorkstationCertified(HierarchyBranchCertificates, userCertificates);

                if (missingCertificates != null && missingCertificates.Count > 0)
                {
                    var SB = new StringBuilder(string.Empty);

                    string listDelimiter = System.Environment.NewLine + "\t\t";
                    string missingCertificatesToString = string.Join(listDelimiter, missingCertificates);
                    missingCertificatesToString = listDelimiter + missingCertificatesToString + System.Environment.NewLine;

                    object[] ArgMissingCertificates = { missingCertificatesToString }; //Argument for warning message

                    string warningMessage = Domain.GetMessageDescriptionById("170");

                    warningMessage = String.Format(warningMessage, ArgMissingCertificates); // Substitute message parameter - missing certificates list
                    Status.errorId = "170";
                    object[] temp = { warningMessage };
                    Status.errorParams = temp;
                    return Status;
                }
                return Status;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Status.errorId = "105";
                return Status;
            }
        }

        static ErrorHandling stationCertificatesValidations(HierarchyModel Hierarchy)
        {
            ErrorHandling Status = new ErrorHandling();
            try
            {
                //Get project certificates
                CertificateBLL.CertificateBLLReturnResult callCertificatesResult;
                List<String> outProjectCertificates = new List<String>();
                callCertificatesResult = CertificateBLL.GetProjectCertificatesByProjectId(Hierarchy.Id, out outProjectCertificates);

                if (callCertificatesResult != CertificateBLL.CertificateBLLReturnResult.Success)
                {
                    Status.errorId = Convert.ToString(ExecutionErrorHandler("CertificateBLL", (int)callCertificatesResult));
                    return Status;
                }

                List<String> missingCertificates = new List<String>();

                if (outProjectCertificates.Count > 0)
                {
                    //Get WorkStation certificates
                    SqlConnection connectionString = new SqlConnection(Domain.DbConnString);
                    connectionString.Open();
                    Dictionary<String, Certificate> outStationCertificates = new Dictionary<String, Certificate>();
                    ResourcesProviderApi resourceProvider = new ResourcesProviderApi(connectionString);
                    CertificateBLL.CertificateBLLReturnResult callWSCertificatesResult = CertificateBLL.GetCertificatesByWorkstationId(resourceProvider, out outStationCertificates);
                    connectionString.Close();
                    if (callWSCertificatesResult != CertificateBLL.CertificateBLLReturnResult.Success)
                    {
                        Status.errorId = Convert.ToString(ExecutionErrorHandler("CertificateBLL", (int)callWSCertificatesResult));
                        return Status;
                    }
                    missingCertificates = CertificateBLL.IsWorkstationCertified(outProjectCertificates, outStationCertificates);
                }

                if (missingCertificates != null && missingCertificates.Count > 0)
                {
                    var SB = new StringBuilder(string.Empty);

                    string listDelimiter = System.Environment.NewLine + "\t\t";
                    string missingCertificatesToString = string.Join(listDelimiter, missingCertificates);
                    missingCertificatesToString = listDelimiter + missingCertificatesToString + System.Environment.NewLine;

                    object[] ArgMissingCertificates = { missingCertificatesToString }; //Argument for warning message

                    string warningMessage = Domain.GetMessageDescriptionById("107");

                    warningMessage = String.Format(warningMessage, ArgMissingCertificates); // Substitute message parameter - missing certificates list
                    Status.errorId = "107";
                    object[] temp = { warningMessage };
                    Status.errorParams = temp;
                    return Status;
                }
                return Status;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Status.errorId = "105";
                return Status;
            }
        }
        #endregion Validations

        #region Files Logic

        #region Copy Content files to local directiory and delete obsolete files

        public static ErrorHandling prepareLocalDirectoryToExecution(Dictionary<int, CMVersionModel> versions,
                                                                        Dictionary<int, CMContentModel> contents,
                                                                        ThreadStart ts,
                                                                        ObservableCollection<ContentModel> activeContents,
                                                                        ContentModel contentToAction,
                                                                        HierarchyModel hierarchy)
        {
            ErrorHandling Status = new ErrorHandling();
            //Check available disk space
            string driveName = string.Empty;
            int reqSpace = -1;

            FileSystemBLL.FileSystemBLLReturnCode checkDriveFreeSpace = FileSystemBLL.checkDiskSpace(hierarchy.VM.TargetPath, out driveName, out reqSpace);
            if (checkDriveFreeSpace != FileSystemBLL.FileSystemBLLReturnCode.Success)
            {
                int errorId = ExecutionErrorHandler("FileSystemBLL", (int)checkDriveFreeSpace);
                if (errorId != 121 && errorId != 140)
                {
                    Status.errorId = Convert.ToString(errorId);
                    return Status;
                }
                else if (errorId == 140)
                {
                    object[] driveArgs = { driveName };
                    Status.errorId = "140";
                    Status.errorParams = driveArgs;
                    return Status;
                }
                else
                {
                    object[] reqspaceArg = { reqSpace }; 
                    Status.errorId = "121";
                    Status.errorParams = reqspaceArg;
                    return Status;
                }
            }

            //Execute file copy and delete
            Status = fileList(versions, contents, contentToAction, hierarchy, activeContents, ts);
            if (Status.errorId != string.Empty)
            {
                return Status;
            }
            return Status;
        }

        #endregion

        #region Create list of files to be copied

        static ErrorHandling fileList(Dictionary<int, CMVersionModel> versions, 
                                        Dictionary<int, CMContentModel> contents,
                                        ContentModel contentToAction,
                                        HierarchyModel Hierarchy,
                                        ObservableCollection<ContentModel> activeContents,
                                        ThreadStart t)
        {
            ErrorHandling Status = new ErrorHandling();
            try
            {
                ContentBLL bll = new ContentBLL(Hierarchy.VM.VersionId);
                Hashtable filesToCopyList = bll.filesToCopyList(contentToAction, versions, activeContents);
                Dictionary<long, int> activeVersionContentsSorted = new Dictionary<long, int>();

                Status = GetAllProjectContentsListSorted(versions, contents, activeContents, out activeVersionContentsSorted);

                #region Moved to method
                //List<ContentModel> sortedActiveContents = activeContents.OrderBy(activeContent => activeContent.seq).ToList();

                ////Generate active version content dictionary
                //foreach (ContentModel cm in sortedActiveContents)
                //{
                //    List<int> linkedContent = new List<int>();
                //    Status = getAllLinkedContent(cm.id, versions, ref linkedContent);
                //    if (Status.errorId != string.Empty)
                //    {
                //        return Status;
                //    }
                //    //linkedContent.Reverse(); //1931
                //    linkedContent.Add(cm.id);
                //    foreach (int x in linkedContent)
                //    {
                //        try
                //        {
                //            if (activeVersionContents.ContainsKey(x))
                //            {
                //                Status.errorId = "158";
                //                long cId = versions[x].ParentID;
                //                object[] temp = {contents[Convert.ToInt32(cId)].Name, versions[x].Name};
                //                Status.errorParams = temp;
                //                return Status;
                //            }
                //            activeVersionContents.Add(x, cm.seq);
                //        }
                //        catch (Exception ex)
                //        {
                //            String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                //            Domain.SaveGeneralErrorLog(logMessage);
                //            Status.errorId = "105";
                //            return Status;
                //        }
                //    }
                //    linkedContent.Clear();
                //}
                #endregion

                //Generate version target path string
                String targetPath = Hierarchy.VM.TargetPath;
                string failedFile = string.Empty;
                FileSystemBLL.FileSystemBLLReturnCode status = FileSystemBLL.CopyContentVersionsFilesToLocalWithFileList(versions, activeVersionContentsSorted, 
                                                                                                                            targetPath, out failedFile, t);
                if (status != FileSystemBLL.FileSystemBLLReturnCode.Success)
                {
                    int errorId = ExecutionErrorHandler("FileSystemBLL", (int)status);
                    Status.errorId = Convert.ToString(errorId);
                    object[] temp = { failedFile };
                    Status.errorParams = temp;
                    return Status;
                }
                return Status;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Status.errorId = "105";
                return Status;
            }
        }

        public static ErrorHandling GetAllProjectContentsListSorted(Dictionary<int, CMVersionModel> versions,
                                                                    Dictionary<int, CMContentModel> contents,
                                                                    ObservableCollection<ContentModel> activeContents,
                                                                    out Dictionary<long, int> activeVersionContents)
        {
            ErrorHandling Status = new ErrorHandling();
            activeVersionContents = new Dictionary<long, int>();

            try
            {
                List<ContentModel> sortedActiveContents = activeContents.OrderBy(activeContent => activeContent.seq).ToList();

                //Generate active version content dictionary
                foreach (ContentModel cm in sortedActiveContents)
                {
                    List<int> linkedContent = new List<int>();
                    Status = getAllLinkedContent(cm.id, versions, ref linkedContent);
                    if (Status.errorId != string.Empty)
                    {
                        return Status;
                    }
                    //linkedContent.Reverse(); //1931
                    linkedContent.Add(cm.id);
                    foreach (int x in linkedContent)
                    {
                        try
                        {
                            if (activeVersionContents.ContainsKey(x))
                            {
                                Status.errorId = "158";
                                long cId = versions[x].ParentID;
                                object[] temp = { contents[Convert.ToInt32(cId)].Name, versions[x].Name };
                                Status.errorParams = temp;
                                return Status;
                            }
                            activeVersionContents.Add(x, cm.seq);
                        }
                        catch (Exception ex)
                        {
                            String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                            Domain.SaveGeneralErrorLog(logMessage);
                            Status.errorId = "105";
                            Status.errorParams[0] = logMessage;
                            return Status;
                        }
                    }
                    linkedContent.Clear();
                }
                return Status;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Status.errorId = "105";
                return Status;
            }
        }


        #endregion

        #region getContentVersionsID

        static ErrorHandling getAllLinkedContent(int contentVersionId, Dictionary<int, CMVersionModel> versions, ref List<int> VersionsContent)
        {
            ErrorHandling Status = new ErrorHandling();
            try
            {
                //1931
                int parentIndex = VersionsContent.IndexOf(contentVersionId);
                if (parentIndex >= 0)
                {
                    VersionsContent.InsertRange(parentIndex, versions[contentVersionId].ContentVersions.Keys);
                }
                else
                {
                    VersionsContent.AddRange(versions[contentVersionId].ContentVersions.Keys);
                }

                foreach (var i in versions[contentVersionId].ContentVersions)
                {
                    //VersionsConent.Add(contentVersionId);//1931              
                    Status = getAllLinkedContent(i.Key, versions, ref VersionsContent);
                    if (Status.errorId != string.Empty)
                    {
                        return Status;
                    }
                }
                return Status;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Status.errorId = "105";
                return Status;
            }
        }

        #endregion getContentVersionsID

        #endregion

        #region Execution logic

        public static ErrorHandling ExecuteContentVersionAndSaveInfo(HierarchyModel Hierarchy, Dictionary<int, CMVersionModel> cmVersions, Dictionary<int, CMContentModel> cmContents, int executedContentId)
        {
            int errorId = -1;
            ErrorHandling Status = new ContentExecutionBLL.ErrorHandling();

            try
            {
                //Get full project path
                string projectFullPath = string.Empty;
                HierarchyBLL.HierarchyBLLReturnCode getProjPathStatus = HierarchyBLL.GetProjectFullPathByProjectId(Hierarchy.Id, out projectFullPath);
                if (getProjPathStatus != HierarchyBLL.HierarchyBLLReturnCode.Success ||
                        string.IsNullOrWhiteSpace(projectFullPath) ||
                        string.IsNullOrEmpty(projectFullPath))
                {
                    errorId = ExecutionErrorHandler("HierarchyBLL", (int)getProjPathStatus);
                    Status.errorId = Convert.ToString(errorId);
                    return Status;
                }

                FileSystemBLL.FileSystemBLLReturnCode status = FileSystemBLL.ExecuteRunningStringContentExecution(Hierarchy.VM.VersionId,
                                                                                                                    projectFullPath,
                                                                                                                    Hierarchy.Code,
                                                                                                                    Hierarchy.VM.VersionName,
                                                                                                                    Hierarchy.VM.TargetPath,
                                                                                                                    cmVersions[executedContentId].RunningString);
                if (!status.Equals(FileSystemBLL.FileSystemBLLReturnCode.Success))
                {
                    errorId = ExecutionErrorHandler("FileSystemBLL", (int)status);
                    Status.errorId = Convert.ToString(errorId);
                    return Status;
                }

                Status = SaveExecutionInfo(Hierarchy, cmVersions, cmContents);
                if (Status.errorId != string.Empty)
                {
                return Status;
                }
                return Status;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Status.errorId = "105";
                
                return Status;
            }
        }

        #endregion Execution logic

        #region RecordExecutionHistory and Info

        public static ErrorHandling recordExecutionHistory(long versionID)
        {
            ErrorHandling Status = new ContentExecutionBLL.ErrorHandling();
            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                List<ParamStruct> CommandParams;
                Boolean DatabaseSupportsBatchQueries = Domain.PersistenceLayer.GetSupportsBatchQueries();

                // Build the Query
                SB.Append("INSERT INTO PE_ProjectExecutionHistory (VersionId, ExecutionTime, ExecutedByUser, ExecutedFromStationId) ");
                SB.Append("VALUES (@VersionId, @ExecutionTime, @ExecutedByUser, @ExecutedFromStationId) ");
                if (DatabaseSupportsBatchQueries)
                {
                    SB.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                }

                // Set the parameters
                CommandParams = new List<ParamStruct>()
                {
                //new ParamStruct { ParamName = "ExecutionId", DataType = DbType.Int32, Value = 10 },
                new ParamStruct { ParamName = "VersionId", DataType = DbType.Int32, Value = versionID },
                new ParamStruct { ParamName = "ExecutionTime", DataType = DbType.DateTime, Value = DateTime.Now },
                new ParamStruct { ParamName = "ExecutedByUser", DataType = DbType.String, Value = Domain.User },
                new ParamStruct { ParamName = "ExecutedFromStationId", DataType = DbType.String, Value = Domain.Workstn},
                };

                //Execute the query
                object RV = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());

                if (RV != null)
                {
                    return Status;
                }
                else
                {
                    String logMessage = "Failed to save record in ExecutionHistory table";
                    Domain.SaveGeneralErrorLog(logMessage);
                    Status.errorId = "105";
                    return Status;
                }
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Status.errorId = "105";               
                return Status;
            }
        }

        static ErrorHandling SaveExecutionInfo(HierarchyModel Hierarchy, Dictionary<int, CMVersionModel> cmVersions, Dictionary<int, CMContentModel> cmContents)
        {
            ErrorHandling Status = new ContentExecutionBLL.ErrorHandling();
            ExecutionInfoModel executionInfo = new ExecutionInfoModel();
            
            try
            {
                executionInfo = GetExecutionInfo(Hierarchy, cmVersions, cmContents);
                Status = CreateExecutionInfoFile(executionInfo, Hierarchy.VM.TargetPath);

                return Status;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Status.errorId = "105";
                return Status;
            }
        }

        static List<int> GetAllVersionsAndSubVersions(ObservableCollection<ContentModel> projectContents)
        {
            List<int> allLinkedVersions = new List<int>();
            List<int> allVersions = new List<int>();
            try
            {
                foreach (ContentModel vId in projectContents)
                {
                    allVersions.Add(vId.id);
                }
                allLinkedVersions = CMVersionBLL.GetVersionAllSubVersions(allVersions);

                allVersions.AddRange(allLinkedVersions);

                return allVersions;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception(logMessage);
            }
        }

        static ExecutionInfoContentModel PopulateContentsInfo(int cmVersionId, Dictionary<int, CMVersionModel> cmVersions, Dictionary<int, CMContentModel> cmContents)
        {
            ExecutionInfoContentModel content = new ExecutionInfoContentModel();
            ObservableCollection<CMWhereUsedContentLinkItemModel> whereUsed = new ObservableCollection<CMWhereUsedContentLinkItemModel>();
            try
            {
                int parentId = cmVersions[cmVersionId].ParentID;

                content.category = cmContents[parentId].ContentType.Name;
                content.description = cmContents[parentId].Description;
                content.name = cmContents[parentId].Name;
                content.versionDescription = cmVersions[cmVersionId].Description;
                content.versionName = cmVersions[cmVersionId].Name;
                content.versionStatus = cmVersions[cmVersionId].Status.Name;
                content.ATRIndicator = cmContents[parentId].ATRInd;

                whereUsed = CMVersionBLL.GetListOfWhereUsedByVersionId(cmVersionId);
                foreach (CMWhereUsedContentLinkItemModel item in whereUsed)
                {
                    ExecutionInfoWhereUsedModel eiItem = new ExecutionInfoWhereUsedModel();
                    eiItem.contentName = item.ContentName;
                    eiItem.versionName = item.VersionName;
                    content.whereUsed.Add(eiItem);
                }

                return content;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception(logMessage);
            }
        }

        static ExecutionInfoModel PopulateProjectInfo(HierarchyModel Project)
        {
            ExecutionInfoModel executionInfo = new ExecutionInfoModel();
            try
            {
                executionInfo.environment = Domain.Environment;
                if (string.IsNullOrEmpty(Project.Code) || string.IsNullOrWhiteSpace(Project.Code))
                {
                    executionInfo.projectCode = string.Empty;
                }
                else
                {
                    executionInfo.projectCode = Project.Code;
                }

                executionInfo.projectName = Project.Name;

                if (string.IsNullOrEmpty(Project.SelectedStep) || string.IsNullOrWhiteSpace(Project.SelectedStep))
                {
                    executionInfo.projectStep = string.Empty;
                }
                else
                {
                    executionInfo.projectStep = Project.SelectedStep;
                }

                executionInfo.station = Domain.Workstn;

                DateTime now = System.DateTime.Now;
                string ts = string.Format("{0:yyyyMMddHHmmss}", now);
                executionInfo.timestamp = ts;

                executionInfo.user = Domain.User;
                executionInfo.versionId = (int)Project.VM.VersionId;

                return executionInfo;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception(logMessage);
            }
        }

        static ExecutionInfoModel GetExecutionInfo(HierarchyModel Hierarchy, Dictionary<int, CMVersionModel> cmVersions, Dictionary<int, CMContentModel> cmContents)
        {
            ObservableCollection<ExecutionInfoContentModel> projectContents = new ObservableCollection<ExecutionInfoContentModel>();
            ExecutionInfoModel executionInfo = new ExecutionInfoModel();
            List<int> allLinkedVersions = new List<int>();

            try
            {
                executionInfo = PopulateProjectInfo(Hierarchy);

                allLinkedVersions = GetAllVersionsAndSubVersions(Hierarchy.VM.Contents);
                foreach (int vId in allLinkedVersions)
                {
                    ExecutionInfoContentModel content = new ExecutionInfoContentModel();
                    content = PopulateContentsInfo(vId, cmVersions, cmContents);
                    projectContents.Add(content);
                }

                executionInfo.contents = projectContents;

                return executionInfo;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception(logMessage);
            }
        }

        static ErrorHandling CreateExecutionInfoFile(ExecutionInfoModel executionInfo, string versionTargetPath)
        {
            ErrorHandling Status = new ErrorHandling();

            try
            {
                string executionInfoFolderFullPath = GetExecutionInfoFolderFullPath(versionTargetPath);
                string executionInfoFileName = GetExecutionInfoFileName(executionInfo);

                object objExecutionInfo = executionInfo;
                Domain.ErrorHandling status = FileSystemBLL.SaveClassInstanceToXmlFile(objExecutionInfo, executionInfoFolderFullPath, executionInfoFileName);

                Status.errorId = status.messsageId;
                Status.errorParams = status.messageParams;
                return Status;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception(logMessage);
            }
        }

        static string GetExecutionInfoFolderFullPath(string versionTargetPath)
        {
            string executionInfoFolderFullPath = string.Empty;
            try
            {
                executionInfoFolderFullPath = versionTargetPath + "\\" + "LaunchPadVersionInfo";

                return executionInfoFolderFullPath;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception(logMessage);
            }
        }

        static string GetExecutionInfoFileName(ExecutionInfoModel executionInfo)
        {
            string executionInfoFileName = string.Empty;
            try
            {
                executionInfoFileName = "LaunchPadVersionInfo.xml";

                return executionInfoFileName;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception(logMessage);
            }
        }

        #endregion RecordExecutionHistory

        #region ExecutionErrorHandler

        public class ErrorHandling
        {
            public string errorId = string.Empty;
            public object[] errorParams = { 0 };
        }

        static int ExecutionErrorHandler(string BLLName, int enumInt)
        {
            int errorId = 105;
            if (BLLName == "ContentBLL")
            {
                ContentBLL.CMApiReturnCode StatusEnum = (ContentBLL.CMApiReturnCode)enumInt;
                switch (StatusEnum)
                {
                    case ContentBLL.CMApiReturnCode.GetTreeObjectsException:
                        errorId = 144;
                        break;
                    case ContentBLL.CMApiReturnCode.EmptyCMTree:
                        errorId = 144;
                        break;
                    case ContentBLL.CMApiReturnCode.ContentNotFree:
                        errorId = 0;
                        break;
                    case ContentBLL.CMApiReturnCode.ContentVersionNotFound:
                        errorId = 144;
                        break;
                    default:
                        errorId = 144;
                        break;
                }
            }
            else if (BLLName == "CertificateBLL")
            {
                CertificateBLL.CertificateBLLReturnResult StatusEnum = (CertificateBLL.CertificateBLLReturnResult)enumInt;
                switch (StatusEnum)
                {
                    case CertificateBLL.CertificateBLLReturnResult.RMCommonException:
                        errorId = 137;
                        break;

                    case CertificateBLL.CertificateBLLReturnResult.DBConnectionError:
                        errorId = 137;
                        break;
                    case CertificateBLL.CertificateBLLReturnResult.AllCertListEmpty:
                        errorId = 137;
                        break;
                    case CertificateBLL.CertificateBLLReturnResult.RMException:
                        errorId = 137;
                        break;
                    default:
                        errorId = 137;
                        break;
                }
            }
            else if (BLLName == "FileSystemBLL")
            {
                // ClearViewModel(); Ella need to check
                FileSystemBLL.FileSystemBLLReturnCode StatusEnum = (FileSystemBLL.FileSystemBLLReturnCode)enumInt;
                switch (StatusEnum)
                {
                    case FileSystemBLL.FileSystemBLLReturnCode.UnauthorizedAccessException:
                        {
                            return 147;                           
                        }
                    case FileSystemBLL.FileSystemBLLReturnCode.FailedToCopyContentfiles:
                        {
                            return 145;
                        }
                    case FileSystemBLL.FileSystemBLLReturnCode.FailedToDeleteObsoleteFiles:
                        {
                            return 146;
                        }
                    case FileSystemBLL.FileSystemBLLReturnCode.CommonException:
                        {
                            return 105;
                        }
                    case FileSystemBLL.FileSystemBLLReturnCode.DriveNotFound:
                        {
                            return 140;
                        }
                    case FileSystemBLL.FileSystemBLLReturnCode.exeNotSpecified:
                        {
                            return 132;
                        }
                    case FileSystemBLL.FileSystemBLLReturnCode.ExecutableNotFound:
                        {
                            return 132;
                        }
                    case FileSystemBLL.FileSystemBLLReturnCode.FileNotFound:
                        {
                            return 142;
                        }
                    case FileSystemBLL.FileSystemBLLReturnCode.InvalidCommandLine:
                        {
                            return 132;
                        }
                    case FileSystemBLL.FileSystemBLLReturnCode.NotEnoughSpace:
                        {
                            return 121;
                        }
                    case FileSystemBLL.FileSystemBLLReturnCode.TargetDirectoryNotFound:
                        {
                            return 147;
                        }
                    case FileSystemBLL.FileSystemBLLReturnCode.UnableToExecute:
                        {
                            return 132;
                        }
                    default:
                        return 105;
                }
            }
            else
            {
                errorId = 105;
            }
            return errorId;
        }

        #endregion
    }
}
