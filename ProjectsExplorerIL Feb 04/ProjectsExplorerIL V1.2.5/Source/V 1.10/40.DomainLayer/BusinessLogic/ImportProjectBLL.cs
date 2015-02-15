using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using ATSBusinessLogic.ContentMgmtBLL;
using ATSBusinessObjects;
using ATSBusinessObjects.ContentMgmtModels;
using ATSDomain;
using System.Threading;

namespace ATSBusinessLogic
{
    public class ImportProjectBLL
    {

        #region Data

        //Folders names
        public static string archivePEFolderName = ExportProjectBLL.archivePEFolderName;
        public static string archiveCMFolderName = ExportProjectBLL.archiveCMFolderName;
        public static string archiveCMFilesFolderName = ExportProjectBLL.archiveCMFilesFolderName;
        public static string archiveSPFolderName = ExportProjectBLL.archiveSPFolderName;
        public static string archiveIconsFolderName = ExportProjectBLL.archiveIconsFolderName;

        //Project tables
        static string projectDetailsXmlFileName = ExportProjectBLL.projectDetailsXmlFileName;
        static string versionDetailsXmlFileName = ExportProjectBLL.versionDetailsXmlFileName;
        static string versionContentsXmlFileName = ExportProjectBLL.versionContentsXmlFileName;

        //CM tables
        public static string cmContentTreeXmlFileName = ExportProjectBLL.cmContentTreeXmlFileName;
        public static string cmContentDetailsXmlFileName = ExportProjectBLL.cmContentDetailsXmlFileName;
        public static string cmContentVersionXmlFileName = ExportProjectBLL.cmContentVersionXmlFileName;
        public static string cmContentTreeUserGroupTypeXmlFileName = ExportProjectBLL.cmContentTreeUserGroupTypeXmlFileName;
        public static string cmContentVersionFileXmlFileName = ExportProjectBLL.cmContentVersionFileXmlFileName;
        public static string cmContentVersionVersionLinkXmlFileName = ExportProjectBLL.cmContentVersionVersionLinkXmlFileName;

        //System Parameters tables
        public static string CMSystemParametersXmlFileName = ExportProjectBLL.CMSystemParametersXmlFileName;
        public static string PESystemParametersXmlFileName = ExportProjectBLL.PESystemParametersXmlFileName;

        //App
        //public static CMImpersonationBLL imp = new CMImpersonationBLL();
        public static CMVersionModel initCmVersion = new CMVersionModel();

        #endregion

        #region Main

        public static Domain.ErrorHandling ImportProject(string archiveFileFullPath, out HierarchyModel project)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            string importFolderFullPath = string.Empty;

            Dictionary<int, ImportCMContentModel> contentsImport = new Dictionary<int, ImportCMContentModel>();
            Dictionary<int, ImportCMVersionModel> versionsImport = new Dictionary<int, ImportCMVersionModel>();
            Dictionary<int, ImportCMFolderModel> cmFoldersImport = new Dictionary<int, ImportCMFolderModel>();
            Dictionary<int, ImportHierarchyModel> hierarchyImport = new Dictionary<int, ImportHierarchyModel>();

            project = new HierarchyModel();
            try
            {
                importFolderFullPath = archiveFileFullPath;

                Status = FileSystemBLL.ValidateInputFolder(importFolderFullPath);
                if (Status.messsageId != string.Empty)
                {
                    return Status;
                }

                Status = ImportProjectBLL.ImportProjectData(importFolderFullPath, out project, null);
                if (Status.messsageId != string.Empty)
                {
                    return Status;
                }

                return Status;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Status.messsageId = "105";

                //Rollback
                if (Domain.PersistenceLayer.IsInTransaction())
                {
                    Domain.PersistenceLayer.AbortTrans();
                }

                return Status;
            }
        }

        public static Domain.ErrorHandling ImportProjectData(string importFolderFullPath, out HierarchyModel project, ThreadStart t)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();

            Dictionary<int, ImportCMContentModel> contentsImport = new Dictionary<int, ImportCMContentModel>();
            Dictionary<int, ImportCMVersionModel> versionsImport = new Dictionary<int, ImportCMVersionModel>();
            Dictionary<int, ImportCMFolderModel> cmFoldersImport = new Dictionary<int, ImportCMFolderModel>();
            Dictionary<int, ImportHierarchyModel> hierarchyImport = new Dictionary<int, ImportHierarchyModel>();

            project = new HierarchyModel();
            try
            {
                ExportProjectToEnvBLL.progressBarTitle = "Validating data on target environment ...";
                if (t != null)
                {
                    FileSystemBLL.validationsCopmpleted = 0;
                    Thread thread = new Thread(t);
                    thread.Start();
                    thread.Join();
                }

                Status = ValidateRefData(importFolderFullPath);
                if (Status.messsageId != string.Empty) //failed
                {
                    //Housekeeping - delete archive folder if any
                    //FileSystemBLL.DeleteDirectoryRecursive(importFolderFullPath);
                    return Status;
                }

                //Status = GetProjectHierarchyModelFromArchive(ref project, Hierarchy, importFolderFullPath);
                Status = GetHierarchyModelFromArchive(ref hierarchyImport, ref project, importFolderFullPath);
                if (Status.messsageId != string.Empty) //failed
                {
                    //Housekeeping - delete archive folder if any
                    //FileSystemBLL.DeleteDirectoryRecursive(importFolderFullPath);
                    return Status;
                }

                Status = PopulatePEFoldersCollection(ref hierarchyImport);
                if (Status.messsageId != string.Empty) //failed
                {
                    return Status;
                }

                Status = ValidatePEDataPriorToImport(ref hierarchyImport, ref project, importFolderFullPath);
                if (Status.messsageId != string.Empty) //Project already exists
                {
                    return Status;
                }
                string cmFilesPath = importFolderFullPath + "\\" + archiveCMFolderName;
                if (Directory.Exists(cmFilesPath))
                {
                    Status = GetCMObjectsCollections(importFolderFullPath, out cmFoldersImport, out contentsImport, out versionsImport);
                    if (Status.messsageId != string.Empty) //Failed 
                    {
                        return Status;
                    }

                    Status = ValidateCMDataPriorToImport(importFolderFullPath, project, cmFoldersImport, contentsImport, versionsImport);
                    if (Status.messsageId != string.Empty) //Conflicts in CM
                    {
                        return Status;
                    }
                }
                //All validations passed - go
                Domain.PersistenceLayer.BeginTransWithIsolation(IsolationLevel.Serializable);

                ExportProjectToEnvBLL.progressBarTitle = "Importing project data to target environment ...";
                if (t != null)
                {
                    FileSystemBLL.importFilesCompleted = 0;
                    Thread thread = new Thread(t);
                    thread.Start();
                    thread.Join();
                }
                //import CM
                if (Directory.Exists(cmFilesPath))
                {
                    Status = CreateCMObjects(cmFoldersImport, contentsImport, versionsImport, importFolderFullPath, t);
                    if (Status.messsageId != string.Empty) //Conflicts in CM
                    {
                        //Rollback
                        if (Domain.PersistenceLayer.IsInTransaction())
                        {
                            Domain.PersistenceLayer.AbortTrans();
                        }
                        return Status;
                    }
                }

                //import PE
                Status = CreatePEObjects(ref project, ref hierarchyImport);
                if (Status.messsageId != string.Empty) //Failed
                {
                    //Rollback
                    if (Domain.PersistenceLayer.IsInTransaction())
                    {
                        Domain.PersistenceLayer.AbortTrans();
                    }
                    return Status;
                }

                Domain.PersistenceLayer.CommitTrans();

                return Status;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Status.messsageId = "105";

                //Rollback
                if (Domain.PersistenceLayer.IsInTransaction())
                {
                    Domain.PersistenceLayer.AbortTrans();
                }

                return Status;
            }
        }

        public static Domain.ErrorHandling ImportProjectDataToEnv(string importFolderFullPath, out HierarchyModel project, ThreadStart t)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();

            Dictionary<int, ImportCMContentModel> contentsImport = new Dictionary<int, ImportCMContentModel>();
            Dictionary<int, ImportCMVersionModel> versionsImport = new Dictionary<int, ImportCMVersionModel>();
            Dictionary<int, ImportCMFolderModel> cmFoldersImport = new Dictionary<int, ImportCMFolderModel>();
            Dictionary<int, ImportHierarchyModel> hierarchyImport = new Dictionary<int, ImportHierarchyModel>();

            project = new HierarchyModel();
            try
            {
                ExportProjectToEnvBLL.progressBarTitle = "Validating data on target environment ...";
                if (t != null)
                {
                    FileSystemBLL.validationsCopmpleted = 0;
                    Thread thread = new Thread(t);
                    thread.Start();
                    thread.Join();
                }

                Status = ValidateRefData(importFolderFullPath);
                if (Status.messsageId != string.Empty) //failed
                {
                    //Housekeeping - delete archive folder if any
                    //FileSystemBLL.DeleteDirectoryRecursive(importFolderFullPath);
                    return Status;
                }

                //Status = GetProjectHierarchyModelFromArchive(ref project, Hierarchy, importFolderFullPath);
                Status = GetHierarchyModelFromArchive(ref hierarchyImport, ref project, importFolderFullPath);
                if (Status.messsageId != string.Empty) //failed
                {
                    //Housekeeping - delete archive folder if any
                    //FileSystemBLL.DeleteDirectoryRecursive(importFolderFullPath);
                    return Status;
                }

                Status = PopulatePEFoldersCollectionEnvImport(ref hierarchyImport);
                if (Status.messsageId != string.Empty) //failed
                {
                    return Status;
                }

                Domain.ErrorHandling renameStatus = new Domain.ErrorHandling();
                Domain.ErrorHandling renameAndCreateStatus = new Domain.ErrorHandling();
                Domain.ErrorHandling moveStatus = new Domain.ErrorHandling();
                Domain.ErrorHandling moveAndCreateStatus = new Domain.ErrorHandling();

                Status = ValidatePEDataPriorToImportToEnv(ref hierarchyImport, ref project, importFolderFullPath);
                if (Status.messsageId != string.Empty) //Project already exists
                {
                    switch (Status.messsageId)
                    {
                        case "227" :
                            return Status; //Project and version exist, same name, same location
                        case "241" :
                            renameStatus = Status; //Project and version exist, different project name
                            break;
                        case "242" :
                            renameAndCreateStatus = Status; //Project exists, version does not exist, different project name
                            break;
                        case "240":
                            moveStatus = Status; //Project and version exist, different location
                            break;
                        case "245":
                            moveAndCreateStatus = Status;
                            break;
                        default:
                            return Status;
                    }
                }
                string cmFilesPath = importFolderFullPath + "\\" + archiveCMFolderName;
                if (Directory.Exists(cmFilesPath))
                {
                    Status = GetCMObjectsCollections(importFolderFullPath, out cmFoldersImport, out contentsImport, out versionsImport);
                    if (Status.messsageId != string.Empty) //Failed 
                    {
                        return Status;
                    }

                    Status = ValidateCMDataPriorToImport(importFolderFullPath, project, cmFoldersImport, contentsImport, versionsImport);
                    if (Status.messsageId != string.Empty) //Conflicts in CM
                    {
                        return Status;
                    }
                }
                //All validations passed - go
                Domain.PersistenceLayer.BeginTransWithIsolation(IsolationLevel.Serializable);

                ExportProjectToEnvBLL.progressBarTitle = "Importing project data to target environment ...";
                if (t != null)
                {
                    FileSystemBLL.importFilesCompleted = 0;
                    Thread thread = new Thread(t);
                    thread.Start();
                    thread.Join();
                }
                //import CM
                if (Directory.Exists(cmFilesPath))
                {
                    Status = CreateCMObjects(cmFoldersImport, contentsImport, versionsImport, importFolderFullPath, t);
                    if (Status.messsageId != string.Empty) //Conflicts in CM
                    {
                        //Rollback
                        if (Domain.PersistenceLayer.IsInTransaction())
                        {
                            Domain.PersistenceLayer.AbortTrans();
                        }
                        return Status;
                    }
                }

                //import PE
                bool allExist = true;
                foreach (ImportHierarchyModel ih in hierarchyImport.Values)
                {
                    if (!ih.ExistsInTargetEnv)
                    {
                        allExist = false;
                        break;
                    }
                }

                bool createNewVersion = false;
                if (renameStatus.messsageId == string.Empty && moveStatus.messsageId == string.Empty)
                {
                    createNewVersion = true;
                }

                if (createNewVersion || !allExist)
                {
                    Status = CreatePEObjectsEnvImport(ref project, ref hierarchyImport, createNewVersion);
                }
               
                if (Status.messsageId != string.Empty) //Failed
                {
                    //Rollback
                    if (Domain.PersistenceLayer.IsInTransaction())
                    {
                        Domain.PersistenceLayer.AbortTrans();
                    }
                    return Status;
                }

                foreach (ImportHierarchyModel hmtarget in hierarchyImport.Values)
                {
                    if (hmtarget.SourceFolder.NodeType == NodeTypes.P)
                    {
                        if (hmtarget.ExistsInTargetEnv)
                        {
                            project = hmtarget.TargetFolder;
                        }
                        else
                        {
                            hmtarget.TargetFolder = project;
                        }
                        if (hmtarget.ExistsInTargetEnv && (hmtarget.TargetFolder.Name != hmtarget.SourceFolder.Name
                                                                       || hmtarget.TargetFolder.TreeHeader != hmtarget.SourceFolder.TreeHeader))
                        {
                            int sourceTreePathLength = hmtarget.SourceFolder.TreeHeader.Length - hmtarget.SourceFolder.Name.Length - 1;
                            string sourceTreePath = string.Empty;
                            if (sourceTreePathLength > 0)
                            {
                                sourceTreePath = hmtarget.SourceFolder.TreeHeader.Substring(0, sourceTreePathLength);
                            }
                            long newParentId = -1;
                            HierarchyBLL.GetNodeIdByFullPath(sourceTreePath, out newParentId);
                            string newName = hmtarget.SourceFolder.Name;
                            Status = MoveAndRenameTargetProject(ref project, newParentId, newName);
                            if (Status.messsageId != string.Empty) //Failed
                            {
                                //Rollback
                                if (Domain.PersistenceLayer.IsInTransaction())
                                {
                                    Domain.PersistenceLayer.AbortTrans();
                                }
                                return Status;
                            }
                            break;
                        } 
                    }
                }

                Domain.PersistenceLayer.CommitTrans();

                if (renameStatus.messsageId != string.Empty)
                {
                    Status = renameStatus;
                }

                if (renameAndCreateStatus.messsageId != string.Empty)
                {
                    Status = renameAndCreateStatus;
                }

                if (moveStatus.messsageId != string.Empty)
                {
                    Status = moveStatus;
                }

                if (moveAndCreateStatus.messsageId != string.Empty)
                {
                    Status = moveAndCreateStatus;
                }

                return Status;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Status.messsageId = "105";

                //Rollback
                if (Domain.PersistenceLayer.IsInTransaction())
                {
                    Domain.PersistenceLayer.AbortTrans();
                }

                return Status;
            }
        }
 
        #endregion

        #region Import to Env

        public static Domain.ErrorHandling ImportProjectToTargetEnv(string archiveFileFullPath, string ConnectionString, string envName, long sourceProjectId,
                                                                    ThreadStart t, 
                                                                    out HierarchyModel targetProject)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            ATSDomain.Domain.ErrorHandling GetNoteTextStatus = new ATSDomain.Domain.ErrorHandling();
            targetProject = new HierarchyModel();
            try
            {
                string memoNoteText = string.Empty;
                GetNoteTextStatus = GetMemoNoteText(236, Domain.Environment, out memoNoteText);

                Domain.DomainInitForAPI(ConnectionString);

                Domain.ErrorHandling tmpStatus = new Domain.ErrorHandling();

                Status = ImportProjectDataToEnv(archiveFileFullPath, out targetProject, t);
                if (Status.messsageId != string.Empty && Status.messsageId != "241" 
                    && Status.messsageId != "242" && Status.messsageId != "240"
                    && Status.messsageId != "245") //Failed 
                {
                    Domain.DomainInitForAPI(Domain.DbConnString);
                    return Status;
                }
                else if (Status.messsageId != string.Empty)
                {
                    tmpStatus = Status;
                }

                ExportProjectToEnvBLL.progressBarTitle = "Final updates ...";
                if (t != null)
                {
                    Thread thread = new Thread(t);
                    thread.Start();
                    thread.Join();
                }
                //create memo on target
                if (memoNoteText != string.Empty && tmpStatus.messsageId != "240" && tmpStatus.messsageId != "241") //not only move or rename
                {
                    memoNoteText = memoNoteText.Replace("dummyProjectVersion", targetProject.VM.VersionName);
                    Status = ExportProjectToEnvBLL.CreateMemoNote(memoNoteText, "I", true, targetProject.Id);
                    //if (Status.messsageId != string.Empty)
                    //{
                    //    Domain.DomainInitForAPI(Domain.DbConnString);
                    //    return Status;
                    //}
                }

                ExportProjectToEnvBLL.targetEnvEmailDistributionList = Domain.getPESystemParameters("ExportEmailDistributionList");

                Domain.DomainInitForAPI(Domain.DbConnString);

                memoNoteText = string.Empty;
                GetNoteTextStatus = GetMemoNoteText(235, envName, out memoNoteText);
                if (memoNoteText != string.Empty && tmpStatus.messsageId != "240" && tmpStatus.messsageId != "241") //not only move or rename
                {
                    memoNoteText = memoNoteText.Replace("dummyProjectVersion", targetProject.VM.VersionName);
                    Status = ExportProjectToEnvBLL.CreateMemoNote(memoNoteText, "E", false, sourceProjectId);
                    NoteBLL.GetNotes(sourceProjectId);
                }
                //if (Status.messsageId != string.Empty)
                //{
                //    return Status;
                //}
                //else
                //{
                //    return tmpStatus;
                //}
                return tmpStatus;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Status.messsageId = "105";

                //Rollback
                if (Domain.PersistenceLayer.IsInTransaction())
                {
                    Domain.PersistenceLayer.AbortTrans();
                }
                Domain.DomainInitForAPI(Domain.DbConnString);
                return Status;
            }
        }

        #endregion

        #region PE Validate and Import

        static Domain.ErrorHandling PopulatePEFoldersCollection(ref Dictionary<int, ImportHierarchyModel> hierarchyImport)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                foreach (KeyValuePair<int, ImportHierarchyModel> folder in hierarchyImport)
                {
                    if (folder.Value.SourceFolder.ParentId == 0)
                    {
                        int folderId = 0;
                        Boolean folderExists = HierarchyBLL.NameExists(0, folder.Value.SourceFolder.Name, out folderId);
                        if (folderExists)
                        {
                            folder.Value.ExistsInTargetEnv = true;
                            folder.Value.TargetFolder = HierarchyBLL.GetHierarchyRow(Convert.ToInt64(folderId));
                            UpdateHierarchyCollectionParentExists((int)folder.Value.SourceFolder.Id, folderId, ref hierarchyImport);
                        }
                        else
                        {
                            folder.Value.ExistsInTargetEnv = false;
                            UpdateHierarchyCollectionParentNotExists(0, ref hierarchyImport); 
                        }
                        break;
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

        static Domain.ErrorHandling PopulatePEFoldersCollectionEnvImport(ref Dictionary<int, ImportHierarchyModel> hierarchyImport)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                foreach (KeyValuePair<int, ImportHierarchyModel> node in hierarchyImport)
                {
                    if (node.Value.SourceFolder.ParentId == 0 && node.Value.SourceFolder.NodeType == NodeTypes.F)
                    {
                        int folderId = 0;
                        Boolean folderExists = HierarchyBLL.NameExists(0, node.Value.SourceFolder.Name, out folderId);
                        if (folderExists)
                        {
                            node.Value.ExistsInTargetEnv = true;
                            node.Value.TargetFolder = HierarchyBLL.GetHierarchyRow(Convert.ToInt64(folderId));
                            UpdateHierarchyCollectionParentExistsEnvImport((int)node.Value.SourceFolder.Id, folderId, ref hierarchyImport);
                        }
                        else
                        {
                            node.Value.ExistsInTargetEnv = false;
                            UpdateHierarchyCollectionParentNotExistsEnvImport(0, ref hierarchyImport);
                        }
                        break;
                    }
                    else if (node.Value.SourceFolder.ParentId == 0 && node.Value.SourceFolder.NodeType == NodeTypes.P)
                    {
                        string projectStep = HierarchyBLL.GetStepCodeByName(node.Value.SourceFolder.SelectedStep);
                        node.Value.TargetFolder = HierarchyBLL.GetHierarchyModelByCodeAndStep(node.Value.SourceFolder.Code, projectStep.Trim());
                        if (node.Value.TargetFolder.Id > 1)
                        {
                            node.Value.ExistsInTargetEnv = true;
                            string treeHeader = string.Empty;
                            HierarchyBLL.GetProjectFullPathByProjectId(node.Value.TargetFolder.Id, out treeHeader);
                            node.Value.TargetFolder.TreeHeader = treeHeader;
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

        static void UpdateHierarchyCollectionParentExists(int sourceParentFolderId, int targetParentFolderId, ref Dictionary<int, ImportHierarchyModel> hierarchyImport)
        {
            foreach (KeyValuePair<int, ImportHierarchyModel> folder in hierarchyImport)
            {
                if (folder.Value.SourceFolder.ParentId == sourceParentFolderId)
                {
                    int folderId = 0;
                    Boolean folderExists = HierarchyBLL.NameExists(targetParentFolderId, folder.Value.SourceFolder.Name, out folderId);
                    if (folderExists)
                    {
                        folder.Value.ExistsInTargetEnv = true;
                        folder.Value.TargetFolder = HierarchyBLL.GetHierarchyRow(Convert.ToInt64(folderId));
                        UpdateHierarchyCollectionParentExists((int)folder.Value.SourceFolder.Id, folderId, ref hierarchyImport);
                    }
                    else
                    {
                        folder.Value.ExistsInTargetEnv = false;
                        UpdateHierarchyCollectionParentNotExists((int)folder.Value.SourceFolder.Id, ref hierarchyImport); 
                    }
                    break;
                }
            }
        }

        static void UpdateHierarchyCollectionParentExistsEnvImport(int sourceParentFolderId, int targetParentFolderId, ref Dictionary<int, ImportHierarchyModel> hierarchyImport)
        {
            foreach (KeyValuePair<int, ImportHierarchyModel> node in hierarchyImport)
            {
                if (node.Value.SourceFolder.ParentId == sourceParentFolderId && node.Value.SourceFolder.NodeType == NodeTypes.F)
                {
                    int folderId = 0;
                    Boolean folderExists = HierarchyBLL.NameExists(targetParentFolderId, node.Value.SourceFolder.Name, out folderId);
                    if (folderExists)
                    {
                        node.Value.ExistsInTargetEnv = true;
                        node.Value.TargetFolder = HierarchyBLL.GetHierarchyRow(Convert.ToInt64(folderId));
                        UpdateHierarchyCollectionParentExistsEnvImport((int)node.Value.SourceFolder.Id, folderId, ref hierarchyImport);
                    }
                    else
                    {
                        node.Value.ExistsInTargetEnv = false;
                        UpdateHierarchyCollectionParentNotExistsEnvImport((int)node.Value.SourceFolder.Id, ref hierarchyImport);
                    }
                    break;
                }
                else if (node.Value.SourceFolder.ParentId == sourceParentFolderId && node.Value.SourceFolder.NodeType == NodeTypes.P)
                {
                    string projectStep = HierarchyBLL.GetStepCodeByName(node.Value.SourceFolder.SelectedStep);
                    node.Value.TargetFolder = HierarchyBLL.GetHierarchyModelByCodeAndStep(node.Value.SourceFolder.Code, projectStep.Trim());
                    if (node.Value.TargetFolder.Id > 1)
                    {
                        node.Value.ExistsInTargetEnv = true;
                        string treeHeader = string.Empty;
                        HierarchyBLL.GetProjectFullPathByProjectId(node.Value.TargetFolder.Id, out treeHeader);
                        node.Value.TargetFolder.TreeHeader = treeHeader;
                    }
                }
            }
        }

        static void UpdateHierarchyCollectionParentNotExists(int sourceParentFolderId, ref Dictionary<int, ImportHierarchyModel> hierarchyImport)
        {
            foreach (KeyValuePair<int, ImportHierarchyModel> folder in hierarchyImport)
            {
                if (folder.Value.SourceFolder.ParentId == sourceParentFolderId)
                {
                    folder.Value.ExistsInTargetEnv = false;
                    UpdateHierarchyCollectionParentNotExists((int)folder.Value.SourceFolder.Id, ref hierarchyImport);
                    break;
                }
            }
        }

        static void UpdateHierarchyCollectionParentNotExistsEnvImport(int sourceParentFolderId, ref Dictionary<int, ImportHierarchyModel> hierarchyImport)
        {
            foreach (KeyValuePair<int, ImportHierarchyModel> node in hierarchyImport)
            {
                if (node.Value.SourceFolder.ParentId == sourceParentFolderId && node.Value.SourceFolder.NodeType == NodeTypes.F)
                {
                    node.Value.ExistsInTargetEnv = false;
                    UpdateHierarchyCollectionParentNotExistsEnvImport((int)node.Value.SourceFolder.Id, ref hierarchyImport);
                    break;
                }
                else if (node.Value.SourceFolder.ParentId == sourceParentFolderId && node.Value.SourceFolder.NodeType == NodeTypes.P)
                {
                    string projectStep = HierarchyBLL.GetStepCodeByName(node.Value.SourceFolder.SelectedStep);
                    node.Value.TargetFolder = HierarchyBLL.GetHierarchyModelByCodeAndStep(node.Value.SourceFolder.Code, projectStep.Trim());
                    if (node.Value.TargetFolder.Id > 1)
                    {
                        node.Value.ExistsInTargetEnv = true;
                        string treeHeader = string.Empty;
                        HierarchyBLL.GetProjectFullPathByProjectId(node.Value.TargetFolder.Id, out treeHeader);
                        node.Value.TargetFolder.TreeHeader = treeHeader;
                        node.Value.TargetFolder.VM = VersionBLL.GetVersionRow(node.Value.TargetFolder.Id);
                    }
                }
            }
        }

        static Domain.ErrorHandling ValidatePEDataPriorToImport(ref Dictionary<int, ImportHierarchyModel> hierarchyImport,
                                                                            ref HierarchyModel project, 
                                                                            string importFolderFullPath)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                foreach (int hId in hierarchyImport.Keys)
                {
                    if (!hierarchyImport[hId].ExistsInTargetEnv)
                    {
                        break;
                    }
                    else
                    {
                        if (hierarchyImport[hId].SourceFolder.NodeType == NodeTypes.P && hierarchyImport[hId].ExistsInTargetEnv)
                        {
                            List<string> projectVersions = VersionBLL.GetListOfVersionNamesByProjectId((int)hierarchyImport[hId].TargetFolder.Id);                            
                            if (projectVersions != null && projectVersions.Contains(hierarchyImport[hId].SourceFolder.VM.VersionName))
                            {
                                Status.messsageId = "227";
                                Status.messageParams[0] = hierarchyImport[hId].SourceFolder.Name +
                                        ", Version " + hierarchyImport[hId].SourceFolder.VM.VersionName;
                                if (hierarchyImport[hId].SourceFolder.ParentId == 0)
                                {
                                    Status.messageParams[1] = "Root";
                                }
                                else
                                {
                                    Status.messageParams[1] = hierarchyImport[(int)hierarchyImport[hId].SourceFolder.ParentId].SourceFolder.Name;
                                }
                            }
                            return Status;
                        }
                    }
                }
                //Boolean stepExists = true;

                //if (!string.IsNullOrEmpty(project.SelectedStep) && !string.IsNullOrWhiteSpace(project.SelectedStep))
                //{
                //    stepExists = HierarchyBLL.StepExists(project.SelectedStep);
                //    if (!stepExists)
                //    {
                //        Status.messsageId = "228";
                //        Status.messageParams[0] = project.SelectedStep;
                //        Status.messageParams[1] = "PE_ProjectStep";
                //    }
                //}

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

        static Domain.ErrorHandling ValidatePEDataPriorToImportToEnv(ref Dictionary<int, ImportHierarchyModel> hierarchyImport,
                                                                    ref HierarchyModel project,
                                                                    string importFolderFullPath)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                        if (hierarchyImport[(int)project.Id].ExistsInTargetEnv)
                        {
                            List<string> projectVersions = VersionBLL.GetListOfVersionNamesByProjectId((int)hierarchyImport[(int)project.Id].TargetFolder.Id);
                            //Version exists under the same path, same project name
                            if (projectVersions != null && projectVersions.Contains(hierarchyImport[(int)project.Id].SourceFolder.VM.VersionName)
                                && hierarchyImport[(int)project.Id].SourceFolder.TreeHeader == hierarchyImport[(int)project.Id].TargetFolder.TreeHeader)
                            {
                                Status.messsageId = "227";
                                Status.messageParams[0] = hierarchyImport[(int)project.Id].SourceFolder.Name +
                                        ", Version " + hierarchyImport[(int)project.Id].SourceFolder.VM.VersionName;
                                if (hierarchyImport[(int)project.Id].SourceFolder.ParentId == 0)
                                {
                                    Status.messageParams[1] = "Root";
                                }
                                else
                                {
                                    Status.messageParams[1] = hierarchyImport[(int)project.Id].TargetFolder.TreeHeader;
                                }
                            }
                                //Version exists, different project name
                            else if (projectVersions != null && projectVersions.Contains(hierarchyImport[(int)project.Id].SourceFolder.VM.VersionName)
                                && hierarchyImport[(int)project.Id].SourceFolder.Name != hierarchyImport[(int)project.Id].TargetFolder.Name)
                            {
                                Status.messsageId = "241";
                                Status.messageParams[0] = hierarchyImport[(int)project.Id].SourceFolder.Code;
                                Status.messageParams[1] = hierarchyImport[(int)project.Id].SourceFolder.SelectedStep;
                                Status.messageParams[2] = hierarchyImport[(int)project.Id].TargetFolder.Name;
                                Status.messageParams[3] = hierarchyImport[(int)project.Id].SourceFolder.Name;
                            }
                             //Version does not exist, different project name
                            else if (projectVersions != null && !projectVersions.Contains(hierarchyImport[(int)project.Id].SourceFolder.VM.VersionName)
                                     && hierarchyImport[(int)project.Id].SourceFolder.Name != hierarchyImport[(int)project.Id].TargetFolder.Name)
                            {
                                Status.messsageId = "242";
                                Status.messageParams[0] = hierarchyImport[(int)project.Id].SourceFolder.Code;
                                Status.messageParams[1] = hierarchyImport[(int)project.Id].SourceFolder.SelectedStep;
                                Status.messageParams[2] = hierarchyImport[(int)project.Id].TargetFolder.Name;
                                Status.messageParams[3] = hierarchyImport[(int)project.Id].SourceFolder.Name;
                                Status.messageParams[4] = hierarchyImport[(int)project.Id].SourceFolder.VM.VersionName;
                            }
                            //Version exists, same project name, different path
                            else if (projectVersions != null && projectVersions.Contains(hierarchyImport[(int)project.Id].SourceFolder.VM.VersionName)
                                     && hierarchyImport[(int)project.Id].SourceFolder.Name == hierarchyImport[(int)project.Id].TargetFolder.Name
                                     && hierarchyImport[(int)project.Id].SourceFolder.TreeHeader != hierarchyImport[(int)project.Id].TargetFolder.TreeHeader)
                            {
                                Status.messsageId = "240";
                                Status.messageParams[0] = hierarchyImport[(int)project.Id].SourceFolder.Code;
                                Status.messageParams[1] = hierarchyImport[(int)project.Id].SourceFolder.SelectedStep;

                                int targetTreePathLength = hierarchyImport[(int)project.Id].TargetFolder.TreeHeader.Length
                                                        - hierarchyImport[(int)project.Id].TargetFolder.Name.Length - 1;
                                string targetTreePath = string.Empty;
                                if (targetTreePathLength > 0)
                                {
                                    targetTreePath = hierarchyImport[(int)project.Id].TargetFolder.TreeHeader.Substring(0, targetTreePathLength);
                                }
                                if (targetTreePath == string.Empty)
                                {
                                    Status.messageParams[2] = "Root";
                                }
                                else
                                {
                                    Status.messageParams[2] = targetTreePath;
                                }
                                int sourceTreePathLength = hierarchyImport[(int)project.Id].SourceFolder.TreeHeader.Length 
                                                            - hierarchyImport[(int)project.Id].SourceFolder.Name.Length - 1;
                                string sourceTreePath = string.Empty;
                                if (sourceTreePathLength > 0)
                                {
                                    sourceTreePath = hierarchyImport[(int)project.Id].SourceFolder.TreeHeader.Substring(0, sourceTreePathLength);
                                }
                                if (sourceTreePath == string.Empty)
                                {
                                    Status.messageParams[3] = "Root";
                                }
                                else
                                {
                                    Status.messageParams[3] = sourceTreePath;
                                }
                            }
                            //Version does not exist, same project name, different path
                            else if (projectVersions != null && !projectVersions.Contains(hierarchyImport[(int)project.Id].SourceFolder.VM.VersionName)
                                     && hierarchyImport[(int)project.Id].SourceFolder.Name == hierarchyImport[(int)project.Id].TargetFolder.Name
                                     && hierarchyImport[(int)project.Id].SourceFolder.TreeHeader != hierarchyImport[(int)project.Id].TargetFolder.TreeHeader)
                            {
                                Status.messsageId = "245";
                                Status.messageParams[0] = hierarchyImport[(int)project.Id].SourceFolder.Code;
                                Status.messageParams[1] = hierarchyImport[(int)project.Id].SourceFolder.SelectedStep;

                                int targetTreePathLength = hierarchyImport[(int)project.Id].TargetFolder.TreeHeader.Length
                                                        - hierarchyImport[(int)project.Id].TargetFolder.Name.Length - 1;
                                string targetTreePath = string.Empty;
                                if (targetTreePathLength > 0)
                                {
                                    targetTreePath = hierarchyImport[(int)project.Id].TargetFolder.TreeHeader.Substring(0, targetTreePathLength);
                                }
                                if (targetTreePath == string.Empty)
                                {
                                    Status.messageParams[2] = "Root";
                                }
                                else
                                {
                                    Status.messageParams[2] = targetTreePath;
                                }
                                int sourceTreePathLength = hierarchyImport[(int)project.Id].SourceFolder.TreeHeader.Length
                                                            - hierarchyImport[(int)project.Id].SourceFolder.Name.Length - 1;
                                string sourceTreePath = string.Empty;
                                if (sourceTreePathLength > 0)
                                {
                                    sourceTreePath = hierarchyImport[(int)project.Id].SourceFolder.TreeHeader.Substring(0, sourceTreePathLength);
                                }
                                if (sourceTreePath == string.Empty)
                                {
                                    Status.messageParams[3] = "Root";
                                }
                                else
                                {
                                    Status.messageParams[3] = sourceTreePath;
                                }
                            }
                            return Status;
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
        
        
        static Domain.ErrorHandling CreatePEObjects(ref HierarchyModel importProject, ref Dictionary<int, ImportHierarchyModel> hierarchyImport)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();

            try
            {
                foreach (KeyValuePair<int, ImportHierarchyModel> node in hierarchyImport)
                {
                    if (node.Value.SourceFolder.ParentId == 0 && node.Value.SourceFolder.NodeType == NodeTypes.F)
                    {
                        if (node.Value.ExistsInTargetEnv == false) //not exists = create and continue recursively with the branch
                        {
                            HierarchyModel f = new HierarchyModel();
                            Status = CreateImportFolder(node.Value.SourceFolder, ref f, 0);
                            if (Status.messsageId != string.Empty)
                            {
                                return Status;
                            }
                            Status = CreateImportNodeRecursive(ref importProject, ref hierarchyImport, (int)f.Id, (int)node.Value.SourceFolder.Id);
                            if (Status.messsageId != string.Empty)
                            {
                                return Status;
                            }
                            break;
                        }
                        else //Exists - continue recursively with the branch
                        {
                            Status = CreateImportNodeRecursive(ref importProject, ref hierarchyImport, (int)node.Value.TargetFolder.Id, (int)node.Value.SourceFolder.Id);
                            if (Status.messsageId != string.Empty)
                            {
                                return Status;
                            }
                        }

                    }
                    else if (node.Value.SourceFolder.ParentId == 0 && node.Value.SourceFolder.NodeType == NodeTypes.P)
                    {
                        if (node.Value.ExistsInTargetEnv == false)
                        {
                            Status = CreateImportProject(ref importProject, 0);
                            if (Status.messsageId != string.Empty)
                            {
                                return Status;
                            }
                            break;
                        }
                        else
                        {
                            Status = CreateImportProjectVersion(ref importProject, node.Value.TargetFolder);
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

        static Domain.ErrorHandling CreatePEObjectsEnvImport(ref HierarchyModel importProject, ref Dictionary<int, ImportHierarchyModel> hierarchyImport, 
                                                                bool isNewVersion)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();

            try
            {
                foreach (KeyValuePair<int, ImportHierarchyModel> node in hierarchyImport)
                {
                    if (node.Value.SourceFolder.ParentId == 0 && node.Value.SourceFolder.NodeType == NodeTypes.F)
                    {
                        if (node.Value.ExistsInTargetEnv == false) //not exists = create and continue recursively with the branch
                        {
                            HierarchyModel f = new HierarchyModel();
                            Status = CreateImportFolder(node.Value.SourceFolder, ref f, 0);
                            if (Status.messsageId != string.Empty)
                            {
                                return Status;
                            }
                            Status = CreateImportNodeRecursiveEnvImport(ref importProject, ref hierarchyImport, (int)f.Id, 
                                                                            (int)node.Value.SourceFolder.Id, isNewVersion);
                            if (Status.messsageId != string.Empty)
                            {
                                return Status;
                            }
                            break;
                        }
                        else //Exists - continue recursively with the branch
                        {
                            Status = CreateImportNodeRecursiveEnvImport(ref importProject, ref hierarchyImport, (int)node.Value.TargetFolder.Id,
                                                                            (int)node.Value.SourceFolder.Id, isNewVersion);
                            if (Status.messsageId != string.Empty)
                            {
                                return Status;
                            }
                        }

                    }
                    else if (node.Value.SourceFolder.ParentId == 0 && node.Value.SourceFolder.NodeType == NodeTypes.P)
                    {
                        if (node.Value.ExistsInTargetEnv == false)
                        {
                            Status = CreateImportProject(ref importProject, 0);
                            if (Status.messsageId != string.Empty)
                            {
                                return Status;
                            }
                            break;
                        }
                        else if (isNewVersion)
                        {
                            Status = CreateImportProjectVersion(ref importProject, node.Value.TargetFolder);
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


        static Domain.ErrorHandling CreateImportFolder(HierarchyModel sourceFolder, ref HierarchyModel folder, int parentId)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();

            try
            {
                folder.Name = sourceFolder.Name;
                folder.Description = sourceFolder.Description;
                folder.IsNew = true;
                folder.CreationDate = DateTime.Now;
                folder.NodeType = NodeTypes.F;
                folder.ParentId = parentId;
                string status = HierarchyBLL.PersistFolder(ref folder);
                Status.messsageId = status;

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

        static Domain.ErrorHandling CreateImportNodeRecursive(ref HierarchyModel importProject,
                                                        ref Dictionary<int, ImportHierarchyModel> hierarchyImport,
                                                        int targetParentId, int sourceParentId)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();

            try
            {
                foreach (KeyValuePair<int, ImportHierarchyModel> node in hierarchyImport)
                {
                    if (node.Value.SourceFolder.ParentId == sourceParentId && node.Value.SourceFolder.NodeType == NodeTypes.F)
                    {
                        if (node.Value.ExistsInTargetEnv == false)
                        {
                            HierarchyModel f = new HierarchyModel();
                            Status = CreateImportFolder(node.Value.SourceFolder, ref f, targetParentId);
                            if (Status.messsageId != string.Empty)
                            {
                                return Status;
                            }
                            Status = CreateImportNodeRecursive(ref importProject, ref hierarchyImport, (int)f.Id, (int)node.Value.SourceFolder.Id);
                            if (Status.messsageId != string.Empty)
                            {
                                return Status;
                            }
                            break;
                        }
                        else
                        {
                            Status = CreateImportNodeRecursive(ref importProject, ref hierarchyImport, (int)node.Value.TargetFolder.Id, (int)node.Value.SourceFolder.Id);
                            if (Status.messsageId != string.Empty)
                            {
                                return Status;
                            }
                        }

                    }
                    else if (node.Value.SourceFolder.ParentId == sourceParentId && node.Value.SourceFolder.NodeType == NodeTypes.P)
                    {
                        if (node.Value.ExistsInTargetEnv == false)
                        {
                            Status = CreateImportProject(ref importProject, targetParentId);
                            if (Status.messsageId != string.Empty)
                            {
                                return Status;
                            }
                            break;
                        }
                        else
                        {
                            Status = CreateImportProjectVersion(ref importProject, node.Value.TargetFolder);
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

        static Domain.ErrorHandling CreateImportNodeRecursiveEnvImport(ref HierarchyModel importProject, 
                                                                ref Dictionary<int, ImportHierarchyModel> hierarchyImport,
                                                                int targetParentId, int sourceParentId, bool isNewVersion)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();

            try
            {
                foreach (KeyValuePair<int, ImportHierarchyModel> node in hierarchyImport)
                {
                    if (node.Value.SourceFolder.ParentId == sourceParentId && node.Value.SourceFolder.NodeType == NodeTypes.F)
                    {
                        if (node.Value.ExistsInTargetEnv == false)
                        {
                            HierarchyModel f = new HierarchyModel();
                            Status = CreateImportFolder(node.Value.SourceFolder, ref f, targetParentId);
                            if (Status.messsageId != string.Empty)
                            {
                                return Status;
                            }
                            Status = CreateImportNodeRecursiveEnvImport(ref importProject, ref hierarchyImport, (int)f.Id, 
                                                                        (int)node.Value.SourceFolder.Id, isNewVersion);
                            if (Status.messsageId != string.Empty)
                            {
                                return Status;
                            }
                            break;
                        }
                        else
                        {
                            Status = CreateImportNodeRecursiveEnvImport(ref importProject, ref hierarchyImport, (int)node.Value.TargetFolder.Id,
                                                                        (int)node.Value.SourceFolder.Id, isNewVersion);
                            if (Status.messsageId != string.Empty)
                            {
                                return Status;
                            }
                        }

                    }
                    else if (node.Value.SourceFolder.ParentId == sourceParentId && node.Value.SourceFolder.NodeType == NodeTypes.P)
                    {
                        if (node.Value.ExistsInTargetEnv == false)
                        {
                            Status = CreateImportProject(ref importProject, targetParentId);
                            if (Status.messsageId != string.Empty)
                            {
                                return Status;
                            }
                            break;
                        }
                        else if (isNewVersion)
                        {
                            Status = CreateImportProjectVersion(ref importProject, node.Value.TargetFolder);
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

        static Domain.ErrorHandling CreateImportProject(ref HierarchyModel importProject, int parentId)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();

            try
            {
                foreach (var content in importProject.VM.Contents)
                {
                    content.id = CMVersionBLL.GetVersionIdByNames(content.name, content.version);
                }

                importProject.ParentId = parentId;
                importProject.VM.TargetPath = getTargetPath(importProject);
                string status = HierarchyBLL.PersistProject(ref importProject);

                importProject.IsCloned = false;
                importProject.IsDirty = false;
                importProject.IsNew = false;

                if (status == "110") //Code and Step not unique
                {
                    Status.messsageId = "232";
                    Status.messageParams[0] = importProject.Code;
                    Status.messageParams[1] = importProject.SelectedStep;
                }
                else
                {
                    Status.messsageId = status;
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

        static Domain.ErrorHandling CreateImportProjectVersion(ref HierarchyModel importProject, HierarchyModel targetProject)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();

            try
            {
                foreach (var content in importProject.VM.Contents)
                {
                    content.id = CMVersionBLL.GetVersionIdByNames(content.name, content.version);
                }

                importProject.ParentId = targetProject.ParentId;
                importProject.Id = targetProject.Id;
                importProject.Code = targetProject.Code;
                importProject.CreationDate = targetProject.CreationDate;
                importProject.Description = targetProject.Description;
                importProject.IRISTechInd = targetProject.IRISTechInd;
                importProject.ProjectStatus = targetProject.ProjectStatus;
                importProject.SelectedStep = targetProject.SelectedStep;
                importProject.Sequence = targetProject.Sequence;
                importProject.Status = targetProject.Status;
                importProject.Synchronization = targetProject.Synchronization;
                importProject.IsNew = false;
                importProject.IsDirty = true;
                importProject.IsCloned = false;

                importProject.VM.TargetPath = getTargetPath(importProject);
                importProject.VM.IsNew = true;
                importProject.VM.IsDirty = true;
                importProject.VM.IsClosed = true;

                VersionModel vmTemp = VersionBLL.GetVersionRow(targetProject.Id);
                targetProject.VM.VersionId = vmTemp.VersionId;
                //patch 4199
                targetProject.VM.VersionName = importProject.VM.VersionName;

                string status = HierarchyBLL.PersistClosedVersion(ref targetProject);

                status = HierarchyBLL.PersistProject(ref importProject);


                importProject.IsDirty = false;
                importProject.IsNew = false;

                importProject.VM.IsNew = false;
                importProject.VM.IsDirty = false;
                importProject.VM.IsClosed = false;

                if (status == "110") //Code and Step not unique
                {
                    Status.messsageId = "232";
                    Status.messageParams[0] = importProject.Code;
                    Status.messageParams[1] = importProject.SelectedStep;
                }
                else
                {
                    Status.messsageId = status;
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

        static Domain.ErrorHandling MoveAndRenameTargetProject(ref HierarchyModel importProject, long newParentId, string newName)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();

            try
            {
                if (newParentId >= 0)
                {
                    importProject.ParentId = newParentId;
                }
                importProject.Name = newName;
                importProject.IsNew = false;
                importProject.IsDirty = true;
                importProject.IsCloned = false;
                string status = HierarchyBLL.PersistProject(ref importProject);


                importProject.IsDirty = false;
                importProject.IsNew = false;

                if (status == "110") //Code and Step not unique
                {
                    Status.messsageId = "232";
                    Status.messageParams[0] = importProject.Code;
                    Status.messageParams[1] = importProject.SelectedStep;
                }
                else
                {
                    Status.messsageId = status;
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


        static Domain.ErrorHandling GetHierarchyModelFromArchive(ref Dictionary<int, ImportHierarchyModel> hierarchyBranch, ref HierarchyModel Project,
                                                                                                string importFolderFullPath)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            Dictionary<int, CMFolderModel> folders = new Dictionary<int, CMFolderModel>();
            Dictionary<int, CMContentModel> contents = new Dictionary<int, CMContentModel>();
            Dictionary<int, CMVersionModel> versions = new Dictionary<int, CMVersionModel>();


            try
            {
                //GetTreeObjectsFromXmlFiles
                string cmFilesPath = importFolderFullPath + "\\" + archiveCMFolderName;
                if (Directory.Exists(cmFilesPath))
                {
                    Status = GetTreeObjectsFromXml(importFolderFullPath, out folders, out contents, out versions);
                    if (Status.messsageId != string.Empty) //Failed 
                    {
                        return Status;
                    }
                }
                DataTable projectDetailsDataTable = FileSystemBLL.ImportDataFromXml(importFolderFullPath, archivePEFolderName, projectDetailsXmlFileName);
                if (projectDetailsDataTable == null || projectDetailsDataTable.Rows.Count < 1) //Failed to get project details
                {
                    throw new Exception("ProjectDetails xml file contains invalid data. Import failed.");
                }

                #region get folders
                foreach (DataRow dr in projectDetailsDataTable.Rows)
                {
                    ImportHierarchyModel tempNode = new ImportHierarchyModel();
                    tempNode.SourceFolder = HierarchyBLL.GetNodeModel(dr);
                    if (tempNode.SourceFolder == null || tempNode.SourceFolder.Id <= 0) //Failed to get folder details
                    {
                        throw new Exception("ProjectDetails xml file contains invalid data. Import failed.");
                    }
                    hierarchyBranch.Add((int)tempNode.SourceFolder.Id, tempNode);
                }
                #endregion get folders

                #region get project
                string selectCondition = "NodeType='P'";

                DataRow[] projectRow = projectDetailsDataTable.Select(selectCondition);
                Project = HierarchyBLL.GetNodeModel(projectRow[0]);
                if (Project == null || Project.Id <= 0) //Failed to get project details
                {
                    throw new Exception("ProjectDetails xml file contains invalid data. Import failed.");
                }

                GetProjectFullPathFromDictionary(ref Project, ref hierarchyBranch);

                DataTable versionDetailsDataTable = FileSystemBLL.ImportDataFromXml(importFolderFullPath, archivePEFolderName, versionDetailsXmlFileName);
                if (versionDetailsDataTable == null || versionDetailsDataTable.Rows.Count != 1) //Failed or invalid
                {
                    throw new Exception("VersionDetails xml file contains invalid data. Import failed.");
                }

                VersionModel version = VersionBLL.GetActiveVersionModel(versionDetailsDataTable.Rows[0]);
                if (version == null || version.VersionId <= 0 || version.HierarchyId != Project.Id) //Failed or invalid
                {
                    throw new Exception("VersionDetails xml file contains invalid data. Import failed.");
                }


                DataTable versionContentsDataTable = FileSystemBLL.ImportDataFromXml(importFolderFullPath, archivePEFolderName, versionContentsXmlFileName);
                if (versionContentsDataTable == null) //Failed or invalid
                {
                    throw new Exception("versionContents xml file contains invalid data. Import failed.");
                }

                ObservableCollection<ContentModel> versionContents = ContentBLL.getActiveContents(versionContentsDataTable, version.VersionId, versions, contents);

                version.Contents = versionContents;
                Project.VM = version;
                Project.IsNew = true;
                Project.IsCloned = true;

                #endregion get project

                #region update Hierarchy Branch with version details

                foreach (KeyValuePair<int, ImportHierarchyModel> node in hierarchyBranch)
                {
                    if (node.Value.SourceFolder.NodeType == NodeTypes.P)
                    {
                        node.Value.SourceFolder.VM = version;
                        break;
                    }
                }

                #endregion

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

        static void GetProjectFullPathFromDictionary(ref HierarchyModel project, ref Dictionary<int, ImportHierarchyModel> hierarchySubTree)
        {
            string treeHeader = project.Name;

            GetTreeHeaderRecursive(ref treeHeader, hierarchySubTree, project.Id);

            project.TreeHeader = treeHeader;
            hierarchySubTree[(int)project.Id].SourceFolder.TreeHeader = treeHeader;           
        }

        static void GetTreeHeaderRecursive(ref string treeHeader, Dictionary<int, ImportHierarchyModel> hierarchySubTree, long nodeId)
        {
            int parentId = (int)hierarchySubTree[(int)nodeId].SourceFolder.ParentId;
            if (parentId != 0)
            {
                treeHeader = hierarchySubTree[parentId].SourceFolder.Name + "/" + treeHeader;
                GetTreeHeaderRecursive(ref treeHeader, hierarchySubTree, hierarchySubTree[(int)nodeId].SourceFolder.ParentId);
            }
        }

        public static string getTargetPath(HierarchyModel project)
        {
            var Target = new StringBuilder(string.Empty);
            try
            {

                var SysPathQry = new StringBuilder(string.Empty);
                SysPathQry.Append("select Value from PE_SystemParameters where Variable='ProjectLocalPath'");

                string SysParm = (string)Domain.PersistenceLayer.FetchDataValue(SysPathQry.ToString(), System.Data.CommandType.Text, null);

                Target = new StringBuilder(string.Empty);
                Target.Append(SysParm.ToString().Trim());
                if (project.ParentId >= 0)
                {
                    Target.Append(VersionBLL.getParentName(project.ParentId.ToString().Trim()));
                }

            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return ex.Message;
            }
            Target.Append("/" + project.Name.ToString().Trim() + "/" + project.VM.VersionName.ToString().Trim());
            return Target.ToString();

        }

        #endregion

        #region CM Validate and Import

        static Domain.ErrorHandling ValidateCMDataPriorToImport(string importFolderFullPath, HierarchyModel Project,
                                                                    Dictionary<int, ImportCMFolderModel> foldersImport, 
                                                                    Dictionary<int, ImportCMContentModel> contentsImport,
                                                                    Dictionary<int, ImportCMVersionModel> versionsImport)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                Status = ValidateContentLinksOnTarget(Project, contentsImport, versionsImport);
                if (Status.messsageId != string.Empty) //Failed 
                {
                    return Status;
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

        static Domain.ErrorHandling GetTreeObjectsFromXml(string parentFolderPath, out Dictionary<int, CMFolderModel> folders, out Dictionary<int, CMContentModel> contents, out Dictionary<int, CMVersionModel> versions)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            folders = new Dictionary<int, CMFolderModel>();
            contents = new Dictionary<int, CMContentModel>();
            versions = new Dictionary<int, CMVersionModel>();
            try
            {
                //GetTreeObjectsFromXmlFiles
                CMTreeNodeBLL.GetTreeObjectsFromXml(parentFolderPath, out folders, out contents, out versions);

                return Status;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Status.messsageId = "226";
                Status.messageParams[0] = parentFolderPath;

                return Status;
            }
        }

        static Domain.ErrorHandling ValidateContentLinksOnTarget(HierarchyModel project, Dictionary<int, ImportCMContentModel> contents, Dictionary<int, ImportCMVersionModel> versions)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                Status = ValidateCMLoops(contents, versions);
                if (Status.messsageId != string.Empty)
                {
                    return Status;
                }

                Status = ValidatePELoops(project, contents, versions);
                if (Status.messsageId != string.Empty)
                {
                    return Status;
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

        static Domain.ErrorHandling ValidateCMLoops(Dictionary<int, ImportCMContentModel> contents, Dictionary<int, ImportCMVersionModel> versions)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                foreach (var version in versions)
                {
                    List<int> linkedIds = new List<int>();
                    if (!version.Value.ExistsInTargetEnv)
                    {
                        foreach (var linked in version.Value.SourceVersion.ContentVersions.Values)
                        {
                            if (versions[linked.ContentSubVersion.ID].ExistsInTargetEnv)
                            {
                                linkedIds.Add((int)versions[linked.ContentSubVersion.ID].TargetVersion.Id);
                            }
                        }
                        if (linkedIds != null && linkedIds.Count > 1)
                        {
                            Status = CheckLoops(linkedIds);
                            if (Status.messsageId != string.Empty && Status.messageParams[3] != string.Empty)
                            {
                                Status.messsageId = "230";
                                Status.messageParams[4] = version.Value.SourceVersion.Name;
                                int parentId = version.Value.SourceVersion.ParentID;
                                Status.messageParams[5] = contents[parentId].SourceContent.Name;
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

        static Domain.ErrorHandling ValidatePELoops(HierarchyModel project, Dictionary<int, ImportCMContentModel> contents, Dictionary<int, ImportCMVersionModel> versions)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                List<int> linkedIds = new List<int>();
                foreach (ContentModel prContent in project.VM.Contents)
                {
                    if (versions[prContent.id].ExistsInTargetEnv)
                    {
                        int intId = Convert.ToInt32(versions[prContent.id].TargetVersion.Id);
                        linkedIds.Add(intId);
                    }
                }

                if (linkedIds != null && linkedIds.Count > 1)
                {
                    Status = CheckLoops(linkedIds);
                    if (Status.messsageId != string.Empty && Convert.ToString(Status.messageParams[3]) != string.Empty)
                    {
                        Status.messsageId = "229";
                        Status.messageParams[4] = project.Name;
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

        static Domain.ErrorHandling CheckLoops(List<int> linkedVersionsIds)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                Dictionary<int, List<int>> allLinkedContents = new Dictionary<int, List<int>>();
                foreach (int vId in linkedVersionsIds)
                {
                    List<int> inTempList = new List<int>();
                    inTempList.Add(vId);

                    List<int> outTempListVersions = new List<int>();
                    outTempListVersions = ContentBLL.GetVersionAllLinkedSubVersions(inTempList);
                    outTempListVersions.Add(vId);
                    List<int> outTempListContents = new List<int>();
                    outTempListContents = ContentBLL.GetAllContentIds(outTempListVersions);
                    allLinkedContents.Add(vId, outTempListContents);
                }
                //check conflicts
                for (int i = 0; i < allLinkedContents.Count; i++)
                {
                    for (int j = i + 1; j < allLinkedContents.Count; j++)
                    {
                        List<int> list1 = allLinkedContents.ElementAt(i).Value;
                        List<int> list2 = allLinkedContents.ElementAt(j).Value;

                        //contents in the intersection
                        IEnumerable<int> loops = list1.Intersect(list2);
                        if (loops != null && loops.Count() > 0)
                        {
                            Status.messsageId = "loop";
                            Status.messageParams[0] = CMVersionBLL.GetVersionName(allLinkedContents.ElementAt(i).Key);
                            Status.messageParams[1] = CMContentBLL.GetContentNameByVersionId(allLinkedContents.ElementAt(i).Key);
                            Status.messageParams[2] = CMVersionBLL.GetVersionName(allLinkedContents.ElementAt(j).Key);
                            Status.messageParams[3] = CMContentBLL.GetContentNameByVersionId(allLinkedContents.ElementAt(j).Key);

                            return Status;
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

        static Domain.ErrorHandling ValidateCMTargetRefData(string archivePath)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                Status = ValidateGroupTypes(archivePath);
                if (Status.messsageId != string.Empty) //Failed 
                {
                    return Status;
                }

                Status = ValidateContentTypes(archivePath);
                if (Status.messsageId != string.Empty) //Failed 
                {
                    return Status;
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

        static Domain.ErrorHandling GetCMObjectsCollections(string archivePath, out Dictionary<int, ImportCMFolderModel> importFolders, out Dictionary<int, ImportCMContentModel> importContents, out Dictionary<int, ImportCMVersionModel> importVersions)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();

            importFolders = new Dictionary<int, ImportCMFolderModel>();
            importContents = new Dictionary<int, ImportCMContentModel>();
            importVersions = new Dictionary<int, ImportCMVersionModel>();

            Dictionary<int, CMFolderModel> folders = new Dictionary<int, CMFolderModel>();
            Dictionary<int, CMContentModel> contents = new Dictionary<int, CMContentModel>();
            Dictionary<int, CMVersionModel> versions = new Dictionary<int, CMVersionModel>();


            try
            {
                //GetTreeObjectsFromXmlFiles
                Status = GetTreeObjectsFromXml(archivePath, out folders, out contents, out versions);
                if (Status.messsageId != string.Empty) //Failed 
                {
                    return Status;
                }

                Status = PopulateCMFoldersCollection(folders, out importFolders);
                if (Status.messsageId != string.Empty) //Failed 
                {
                    return Status;
                }

                Status = PopulateContentsAndVersionsCollections(contents, versions, out importContents, out importVersions);
                if (Status.messsageId != string.Empty) //Failed 
                {
                    return Status;
                }

                Status = UpdateCMFoldersCollections(importContents, ref importFolders);
                if (Status.messsageId != string.Empty) //Failed 
                {
                    return Status;
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

        static Domain.ErrorHandling PopulateCMFoldersCollection(Dictionary<int, CMFolderModel> folders, out Dictionary<int, ImportCMFolderModel> importFolders)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();

            importFolders = new Dictionary<int, ImportCMFolderModel>();
            try
            {
                foreach (CMFolderModel f in folders.Values)
                {
                    if (f.ParentID == 0)
                    {
                        ImportCMFolderModel importItem = new ImportCMFolderModel();
                        importItem.SourceFolder = f;
                        importItem.TargetFolder.ParentID = 0;
                        bool folderExists = CMFolderBLL.FolderExists(f.ParentID, f.Name);
                        if (!folderExists)
                        {
                            importItem.ExistsInTargetEnv = false;
                            importItem.TargetFolder = new CMFolderModel();
                            importFolders.Add(importItem.SourceFolder.ID, importItem);
                            UpdateImportFoldersParentNotExists(ref importFolders, f);
                        }
                        else
                        {
                            importItem.ExistsInTargetEnv = true;
                            importItem.TargetFolder = CMFolderBLL.GetFolderRow(f.ParentID, f.Name);
                            importFolders.Add(importItem.SourceFolder.ID, importItem);
                            UpdateImportFoldersParentExists(ref importFolders, importItem.SourceFolder, importItem.TargetFolder);
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

        static Domain.ErrorHandling PopulateContentsAndVersionsCollections(Dictionary<int, CMContentModel> contents, 
                                                                            Dictionary<int, CMVersionModel> versions, 
                                                       out Dictionary<int, ImportCMContentModel> importContents,
                                                       out Dictionary<int, ImportCMVersionModel> importVersions)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();

            importContents = new Dictionary<int, ImportCMContentModel>();
            importVersions = new Dictionary<int, ImportCMVersionModel>();
            try
            {
                foreach (CMContentModel cn in contents.Values)
                {
                    ImportCMContentModel importItem = new ImportCMContentModel();
                    importItem.SourceContent = cn;
                    bool contentExists = CMContentBLL.ContentNameExists(cn.Name);
                    if (!contentExists)
                    {
                        importItem.ExistsInTargetEnv = false;
                        importItem.TargetContent = new CMContentModel();
                        importContents.Add(importItem.SourceContent.ID, importItem);
                        UpdateImportVersionsParentNotExists(ref importVersions, importItem.SourceContent);
                    }
                    else
                    {
                        importItem.TargetContent = CMContentBLL.GetContentRowByName(cn.Name);
                        importItem.ExistsInTargetEnv = true;
                        importContents.Add(importItem.SourceContent.ID, importItem);
                        UpdateImportVersionsParentExists(ref importVersions, importItem.SourceContent, importItem.TargetContent);
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

        static Domain.ErrorHandling UpdateCMFoldersCollections(Dictionary<int, ImportCMContentModel> importContents,
                                                       ref Dictionary<int, ImportCMFolderModel> importFolders)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();

            try
            {
                foreach (ImportCMContentModel cn in importContents.Values)
                {
                    bool contentExists = cn.ExistsInTargetEnv;
                    if (!contentExists)
                    {
                        UpdateImportFoldersChildContentNotExists(ref importFolders, cn.SourceContent.ParentID);
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
 
        static void UpdateImportFoldersParentNotExists(ref Dictionary<int, ImportCMFolderModel> importFolders, CMFolderModel sourceFolder)
        {
            try
            {
                foreach (var node in sourceFolder.Nodes)
                {
                    if (node.TreeNodeType.Equals(TreeNodeObjectType.Folder))
                    {
                        ImportCMFolderModel importItem = new ImportCMFolderModel();
                        importItem.SourceFolder = (CMFolderModel)node;
                        importItem.ExistsInTargetEnv = false;
                        importItem.TargetFolder = new CMFolderModel();
                        importFolders.Add(importItem.SourceFolder.ID, importItem);
                        UpdateImportFoldersParentNotExists(ref importFolders, (CMFolderModel)node);
                    }
                }

                return;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("Failed to populated folders collection");
            }
        }

        static void UpdateImportFoldersParentExists(ref Dictionary<int, ImportCMFolderModel> importFolders, CMFolderModel sourceFolder, CMFolderModel targetFolder)
        {
            try
            {
                foreach (var node in sourceFolder.Nodes)
                {
                    if (node.TreeNodeType.Equals(TreeNodeObjectType.Folder))
                    {
                        ImportCMFolderModel importItem = new ImportCMFolderModel();
                        importItem.SourceFolder = (CMFolderModel)node;
                        bool folderExists = CMFolderBLL.FolderExists(targetFolder.Id, node.Name);
                        if (!folderExists)
                        {
                            importItem.ExistsInTargetEnv = false;
                            importItem.TargetFolder = new CMFolderModel();
                            importFolders.Add(importItem.SourceFolder.ID, importItem);
                            UpdateImportFoldersParentNotExists(ref importFolders, importItem.SourceFolder);
                        }
                        else
                        {
                            importItem.ExistsInTargetEnv = true;
                            importItem.TargetFolder = CMFolderBLL.GetFolderRow(targetFolder.Id, node.Name);
                            importFolders.Add(importItem.SourceFolder.ID, importItem);
                            UpdateImportFoldersParentExists(ref importFolders, importItem.SourceFolder, importItem.TargetFolder);
                        }
                    }
                }

                return;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("Failed to populated folders collection");
            }
        }

        static void UpdateImportVersionsParentNotExists(ref Dictionary<int, ImportCMVersionModel> importVersions, CMContentModel sourceContent)
        {
            try
            {
                foreach (var node in sourceContent.Versions)
                {

                    ImportCMVersionModel importItem = new ImportCMVersionModel();
                    importItem.SourceVersion = node.Value;
                    importItem.ExistsInTargetEnv = false;
                    importItem.TargetVersion = new CMVersionModel();
                    importVersions.Add(importItem.SourceVersion.ID, importItem);
                }

                return;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("Failed to populated versions collection");
            }
        }

        static void UpdateImportFoldersChildContentNotExists(ref Dictionary<int, ImportCMFolderModel> importFolders, int folderId)
        {
            try
            {
                if (importFolders.ContainsKey(folderId))
                    {
                        importFolders[folderId].ChildContentsExistInTargetEnv = false;
                        if (importFolders[folderId].SourceFolder.ParentID > 0)
                        {
                            UpdateImportFoldersChildContentNotExists(ref importFolders, importFolders[folderId].SourceFolder.ParentID);
                        }
                    }

                return;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("Failed to populated versions collection");
            }
        }

        static void UpdateImportVersionsParentExists(ref Dictionary<int, ImportCMVersionModel> importVersions, CMContentModel sourceContent, CMContentModel targetContent)
        {
            try
            {
                foreach (var version in sourceContent.Versions.Values)
                {
                    ImportCMVersionModel importItem = new ImportCMVersionModel();
                    importItem.SourceVersion = version;
                    bool versionExists = CMVersionBLL.VersionExists(targetContent.Id, version.Name);
                        if (!versionExists)
                        {
                            importItem.ExistsInTargetEnv = false;
                            importItem.TargetVersion = new CMVersionModel();
                            importItem.TargetVersion.id_Content = targetContent.Id;
                            importVersions.Add(importItem.SourceVersion.ID, importItem);
                        }
                        else
                        {
                            importItem.ExistsInTargetEnv = true;
                            importItem.TargetVersion = CMVersionBLL.GetVersionRow(targetContent.Id, version.Name);
                            importVersions.Add(importItem.SourceVersion.ID, importItem);
                        }
                }

                return;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("Failed to populated folders collection");
            }
        }

        static Domain.ErrorHandling ValidateGroupTypes(string archivePath)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                ObservableCollection<string> groupTypes = CMFolderBLL.GetGroupTypesKeys();
                DataTable foldersTable = FileSystemBLL.ImportDataFromXml(archivePath, archiveCMFolderName, cmContentTreeUserGroupTypeXmlFileName);

                if (foldersTable != null)
                {
                    foreach (DataRow row in foldersTable.Rows)
                    {
                        if (!groupTypes.Contains(row["CTUGT_id_UserGroupType"]))
                        {
                            Status.messsageId = "228";
                            Status.messageParams[0] = row["CTUGT_id_UserGroupType"];
                            Status.messageParams[1] = "GroupTypes";

                            return Status;
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

        static Domain.ErrorHandling ValidateContentTypes(string archivePath)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                List<string> contentTypes = CMContentBLL.GetContentTypeKeys();
                DataTable contentsTable = FileSystemBLL.ImportDataFromXml(archivePath, archiveCMFolderName, cmContentDetailsXmlFileName);
                foreach (DataRow row in contentsTable.Rows)
                {
                    if (!contentTypes.Contains(row["CO_id_ContentType"]))
                    {
                        Status.messsageId = "228";
                        Status.messageParams[0] = row["CO_id_ContentType"];
                        Status.messageParams[1] = "ContentType";

                        return Status;
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

        static Domain.ErrorHandling CreateCMObjects(Dictionary<int, ImportCMFolderModel> folders, 
                                                    Dictionary<int, ImportCMContentModel> contents,
                                                    Dictionary<int, ImportCMVersionModel> versions, 
                                                    string archivePath, ThreadStart t)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                Status = AddNewCMObjects(ref folders, ref contents, ref versions, archivePath, t);
                if (Status.messsageId != string.Empty) //Failed 
                {
                    return Status;
                }

                Status = LinkContentVersions(ref versions, contents);
                if (Status.messsageId != string.Empty) //Failed 
                {
                    return Status;
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

        static Domain.ErrorHandling CreateContentFoldersSubTree(ref ImportCMFolderModel startFolder, ref Dictionary<int, ImportCMFolderModel> folders)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                int folderId = -1;
                if (!startFolder.ExistsInTargetEnv)
                {
                    CMFolderModel folderToCreate = new CMFolderModel();
                    folderToCreate = startFolder.SourceFolder;
                    //folderToCreate.ParentID = startFolder.TargetFolder.ParentID;
                    folderToCreate.ParentId = startFolder.TargetFolder.ParentID;
                    folderId = (int)CMFolderBLL.AddNewFolder(ref folderToCreate);

                    startFolder.TargetFolder = folderToCreate;
                }
                else
                {
                    folderId = (int)startFolder.TargetFolder.Id;
                }

                foreach (CMTreeNode node in folders[startFolder.SourceFolder.ID].SourceFolder.Nodes)
                {
                    if (node.TreeNodeType == TreeNodeObjectType.Folder)
                    {
                        ImportCMFolderModel tempFolder = folders[node.ID];
                        tempFolder.TargetFolder.ParentID = folderId;
                        Status = CreateContentFoldersSubTree(ref tempFolder, ref folders);
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

        static Domain.ErrorHandling CreateContent(ref ImportCMContentModel content, string archivePath)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                CMContentModel newContent = new CMContentModel();
                newContent = content.SourceContent;
                newContent.ParentID = content.TargetContent.ParentID;
                newContent.Id_ContentTree = content.TargetContent.ParentID;
                newContent.IconPath = GetImportIconPath(newContent.ID, archivePath);
                CMImpersonationBLL imp = new CMImpersonationBLL();
                CMContentBLL.AddContentIconOnFs(ref newContent, imp);
                int contentId = (int)CMContentBLL.AddNewContent(ref newContent);

                content.TargetContent = newContent;
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

        static Domain.ErrorHandling CreateVersion(ref ImportCMVersionModel version, string archivePath, ThreadStart t)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                CMVersionModel newVersion = new CMVersionModel();
                newVersion = version.SourceVersion;
                newVersion.ParentID = version.TargetVersion.ParentID;
                newVersion.id_Content = version.TargetVersion.ParentID;
                newVersion.CommandLine = version.SourceVersion.RunningString;

                string filesPath = GetVersionFilesPath(newVersion.ID, archivePath);
                DirectoryInfo filesParenFolder = new DirectoryInfo(filesPath);
                long versionId = -1;
                newVersion.Files.Clear();
                //Populate Files
                ProceedFiles(filesParenFolder, newVersion);

               string parentContentName = CMContentBLL.GetContentName(newVersion.ParentID);
               newVersion.ChildNumber = CMVersionBLL.GetChildIDForAddVersion(newVersion.ParentID); ;
               newVersion.Path.Name = parentContentName + newVersion.ChildNumber;
               CMImpersonationBLL imp = new CMImpersonationBLL();
               CMFileSystemUpdaterBLL.AddContentVersionFilesOnFsBg(newVersion, null, imp, t);
               versionId = CMVersionBLL.AddNewVersion(ref newVersion);
               CMVersionBLL.AddContentVersionFiles(ref newVersion);

               version.TargetVersion = newVersion;
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

        static Domain.ErrorHandling AddNewCMObjects(ref Dictionary<int, ImportCMFolderModel> folders, 
                                                    ref Dictionary<int, ImportCMContentModel> contents, 
                                                    ref Dictionary<int, ImportCMVersionModel> versions,
                                                    string archivePath, ThreadStart t)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                Status = CreateFolders(ref folders);
                if (Status.messsageId != string.Empty) //Failed 
                {
                    return Status;
                }

                Status = CreateContents(ref contents, folders, archivePath);
                if (Status.messsageId != string.Empty) //Failed 
                {
                    return Status;
                }

                Status = CreateContentVersions(ref versions, contents, archivePath, t);
                if (Status.messsageId != string.Empty) //Failed 
                {
                    return Status;
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

        static Domain.ErrorHandling LinkContentVersions(ref Dictionary<int, ImportCMVersionModel> versions, Dictionary<int, ImportCMContentModel> contents)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                foreach (int vKey in versions.Keys)
                {
                    ImportCMVersionModel tempVersion = versions[vKey];
                    if (!tempVersion.ExistsInTargetEnv)
                    {
                        Status = AddLinkedToVersion(ref tempVersion, ref versions, contents);
                        if (Status.messsageId != string.Empty) //Failed 
                        {
                            return Status;
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

        static Domain.ErrorHandling AddLinkedToVersion(ref ImportCMVersionModel version, ref Dictionary<int, ImportCMVersionModel> versions, Dictionary<int, ImportCMContentModel> contents)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                foreach (CMContentVersionSubVersionModel lv in version.SourceVersion.ContentVersions.Values)
                {
                    lv.ContentSubVersion.Id = versions[(int)lv.ContentSubVersion.ID].TargetVersion.Id;
                    CMVersionBLL.AddContentVersionLink(version.TargetVersion, lv);
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

        static Domain.ErrorHandling CreateFolders(ref Dictionary<int, ImportCMFolderModel> folders)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                foreach (ImportCMFolderModel folder in folders.Values)
                {
                    if (!folder.ChildContentsExistInTargetEnv)
                    {
                        if (!folder.ExistsInTargetEnv && folder.SourceFolder.ParentID == 0)
                        {
                            ImportCMFolderModel tempFolder = folder;
                            tempFolder.TargetFolder.ParentID = 0;
                            Status = CreateContentFoldersSubTree(ref tempFolder, ref folders);
                            if (Status.messsageId != string.Empty) //Failed 
                            {
                                return Status;
                            }
                        }
                        else if (folder.ExistsInTargetEnv && folder.SourceFolder.ParentID == 0)
                        {
                            foreach (CMTreeNode node in folders[folder.SourceFolder.ID].SourceFolder.Nodes)
                            {
                                if (node.TreeNodeType == TreeNodeObjectType.Folder)
                                {
                                    ImportCMFolderModel tempFolder = folders[node.ID];
                                    tempFolder.TargetFolder.ParentID = (int)folder.TargetFolder.Id;
                                    Status = CreateContentFoldersSubTree(ref tempFolder, ref folders);
                                    if (Status.messsageId != string.Empty) //Failed 
                                    {
                                        return Status;
                                    }
                                }
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

        static Domain.ErrorHandling CreateContents(ref Dictionary<int, ImportCMContentModel> contents, Dictionary<int, ImportCMFolderModel> folders, string archivePath)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                foreach (int contentKey in contents.Keys)
                {
                    if (!contents[contentKey].ExistsInTargetEnv)
                    {
                        ImportCMContentModel tempContent = contents[contentKey];
                        int sourceParent = tempContent.SourceContent.ParentID;
                        tempContent.TargetContent.ParentID = (int)folders[sourceParent].TargetFolder.Id;
                        Status = CreateContent(ref tempContent, archivePath);
                        if (Status.messsageId != string.Empty) //Failed 
                        {
                            return Status;
                        }
                        contents[contentKey].TargetContent = tempContent.TargetContent;
                        contents[contentKey].ExistsInTargetEnv = true;
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

        static Domain.ErrorHandling CreateContentVersions(ref Dictionary<int, ImportCMVersionModel> versions, 
                                                            Dictionary<int, ImportCMContentModel> contents, 
                                                            string archivePath, ThreadStart t)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                //Count files
                int cntFilesToImport = 0;
                foreach (int versionKey in versions.Keys)
                {
                    if (!versions[versionKey].ExistsInTargetEnv)
                    {
                        cntFilesToImport = cntFilesToImport + versions[versionKey].SourceVersion.Files.Count;
                    }
                }

                foreach (int versionKey in versions.Keys)
                {
                    if (!versions[versionKey].ExistsInTargetEnv)
                    {
                        ImportCMVersionModel tempVersion = versions[versionKey];
                        int sourceParent = tempVersion.SourceVersion.ParentID;
                        tempVersion.TargetVersion.ParentID = (int)contents[sourceParent].TargetContent.Id;
                        Status = CreateVersion(ref tempVersion, archivePath, t);
                        if (Status.messsageId != string.Empty) //Failed 
                        {
                            return Status;
                        }
                        versions[versionKey].TargetVersion = tempVersion.TargetVersion;
                    }
                    //Update Progress Bar if content version exist on target
                    else if (t != null)
                    {
                        FileSystemBLL.importFilesCompleted = FileSystemBLL.importFilesCompleted + versions[versionKey].SourceVersion.Files.Count*2;
                        Thread thread = new Thread(t);
                        thread.Start();
                        thread.Join();
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

        static string GetImportIconPath(int contentId, string archiveParentFolderPath)
        {
            string iconPath = string.Empty;
            try
            {
                string folderName = ExportProjectBLL.IconFileFolderName(contentId);
                string iconFolderFullPath = ExportProjectBLL.GetContentIconFolderFullPath(archiveParentFolderPath, contentId);
                DirectoryInfo di = new DirectoryInfo(iconFolderFullPath);
                FileInfo[] files = di.GetFiles();
                if (files != null && files.Length > 0)
                {
                    iconPath = files[0].FullName;
                }

                return iconPath;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return string.Empty;
            }
        }

        static string GetVersionFilesPath(int versionId, string archiveParentFolderPath)
        {
            string filesFullPath = string.Empty;
            try
            {
                filesFullPath = ExportProjectBLL.GetVersionFilesFolderFullPath(archiveParentFolderPath, versionId);

                return filesFullPath;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return string.Empty;
            }
        }

        static void ProceedFiles(DirectoryInfo filesFolder, CMVersionModel addToVersion)
        {
            ObservableCollection<CMItemFileNode> SubItemNode = new ObservableCollection<CMItemFileNode>();
            List<string> allfiles = new List<string>();
            string[] itemToReplace = new string[1];

            FileInfo[] versionFiles = filesFolder.GetFiles();
            DirectoryInfo[] versionDirs = filesFolder.GetDirectories();

            foreach (System.IO.FileInfo f in versionFiles)
            {
                allfiles.Add(f.FullName);
            }
            foreach (System.IO.DirectoryInfo d in versionDirs)
            {
                allfiles.Add(d.FullName);
            }

            string[] FileNames = allfiles.ToArray();//get all files names  - full paths
            foreach (string FileName in FileNames)
            {

                itemToReplace[0] = FileName;
                AddSubItems(null, SubItemNode, itemToReplace, true);
            }

            foreach (CMItemFileNode fileNode in SubItemNode)
                AddFilesToContentVersionFiles(addToVersion, String.Empty, fileNode, initCmVersion);
        }

        static void AddSubItems(CMItemFileNode parent, ObservableCollection<CMItemFileNode> parentItemsCollection, string[] items, bool addFolderRecursive)
        {

            if (parent != null && parent.Type != ItemFileNodeType.Folder)
                return;

            foreach (string item in items)
            {
                CMItemFileNode newItem = new CMItemFileNode
                {
                    ID = 0,
                    Path = item,
                    Status = ItemFileStatus.New,
                    SubItemNode = new ObservableCollection<CMItemFileNode>()
                };

                if (File.Exists(item))
                {
                    newItem.Type = ItemFileNodeType.File;
                    newItem.Name = System.IO.Path.GetFileName(item);
                }
                else
                {
                    newItem.Type = ItemFileNodeType.Folder;
                    newItem.Name = new DirectoryInfo(item).Name;
                }

                if (newItem.Type == ItemFileNodeType.Folder && addFolderRecursive)
                {
                    AddSubItems(newItem, newItem.SubItemNode, Directory.GetDirectories(item), true);
                    AddSubItems(newItem, newItem.SubItemNode, Directory.GetFiles(item), true);
                }

                newItem.Parent = parent;
                parentItemsCollection.Add(newItem);
            }
        }

        static void AddFilesToContentVersionFiles(CMVersionModel contentVersion, String relativePath, CMItemFileNode file, CMVersionModel contentVersionOriginal)
        {
            if (file.Type == ItemFileNodeType.File)
            {
                int newIndex;

                CMContentFileModel newFile = new CMContentFileModel
                {
                    FileName = file.Name,
                    FileRelativePath = relativePath,
                    ID = file.ID
                };

                if (file.ID != 0)
                {
                    newIndex = file.ID;
                }
                else
                {
                    if (contentVersion.Files.Count == 0)
                        newIndex = -1;
                    else
                    {
                        if (contentVersion.Files.Keys.Min() < 0)
                            newIndex = contentVersion.Files.Keys.Min() - 1;
                        else
                            newIndex = -1;
                    }
                }

                if (String.IsNullOrEmpty(file.Path))
                    newFile.FileFullPath = contentVersion.Path.Name + "\\" + file.Name;
                else
                {
                    if (newFile.ID == 0) //New file
                        newFile.FileFullPath = file.Path;
                    else
                        newFile.FileFullPath = contentVersion.Path.Name + "\\" + file.Path + "\\" + file.Name;
                }
                try
                {
                    FileInfo fi = new FileInfo(newFile.FileFullPath);
                    newFile.FileSize = fi.Length.ToString(); //File Size in Bytes
                    newFile.FileLastWriteTime = fi.LastWriteTime;
                }
                catch (Exception ex)
                {
                    String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                    Domain.SaveGeneralErrorLog(logMessage);
                    throw new Exception("Failed to add files to content version files.");
                }
                contentVersion.Files.Add(newIndex, newFile);
            }
            else
            {
                foreach (CMItemFileNode fileNode in file.SubItemNode)
                    AddFilesToContentVersionFiles(contentVersion, relativePath == String.Empty ? file.Name : relativePath + "\\" + file.Name, fileNode, contentVersionOriginal);
            }
        }

        
        #endregion CM Validate and Import

        #region Utilities

        static Domain.ErrorHandling GetImportFolderFullPath(string archiveFolderFullPath, out string importTempFolderFullPath)
        {
            Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            importTempFolderFullPath = string.Empty;
            try
            {
                string importFolderName = Path.GetFileNameWithoutExtension(archiveFolderFullPath);
                if (Domain.PE_SystemParameters.ContainsKey("ImportFolder"))
                {
                    importTempFolderFullPath = Domain.PE_SystemParameters["ImportFolder"] + "\\" + importFolderName;
                }
                else
                {
                    Status.messsageId = "228";
                    Status.messageParams[0] = "ImportFolder";
                    Status.messageParams[1] = "PE_SystemParameters";
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

        static Domain.ErrorHandling ValidateRefData(string importFolderFullPath)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                Boolean stepExists = true;
                string projectStep = string.Empty;
                stepExists = StepExists(importFolderFullPath, out projectStep);
                if (!stepExists)
                {
                    Status.messsageId = "228";
                    Status.messageParams[0] = projectStep;
                    Status.messageParams[1] = "PE_ProjectStep";
                    return Status;
                }
                string cmFilesPath = importFolderFullPath + "\\" + archiveCMFolderName;
                if (Directory.Exists(cmFilesPath))
                {
                    Status = ValidateCMTargetRefData(importFolderFullPath);
                    if (Status.messsageId != string.Empty) //Failed 
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
                Status.messsageId = "105";

                return Status;
            }
        }

        static bool StepExists(string importFolderFullPath, out string projectStep)
        {
            Boolean stepExists = true;
            try
            {
                projectStep = string.Empty;
                DataTable projectTable = FileSystemBLL.ImportDataFromXml(importFolderFullPath, archivePEFolderName, projectDetailsXmlFileName);
                foreach (DataRow dr in projectTable.Rows)
                {
                    if ((string)(dr["NodeType"]) == "P")
                    {
                        if (dr["ProjectStep"] != DBNull.Value)
                        {
                            projectStep = Convert.ToString(dr["ProjectStep"]);
                        }

                        if (!string.IsNullOrEmpty(projectStep) && !string.IsNullOrWhiteSpace(projectStep))
                        {
                            stepExists = HierarchyBLL.StepExists(projectStep);
                        }
                        break;
                    }
                }

                return stepExists;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);

                throw new Exception("Failed to check step.");
            }
 
        }

        static Domain.ErrorHandling GetMemoNoteText(int messageId, string envName, out string formattedMemoNoteText)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            formattedMemoNoteText = string.Empty;
            try
            {
                object[] messageParams = new object[4];
                messageParams[0] = "dummyProjectVersion";
                messageParams[1] = envName;
                messageParams[2] = Domain.User;
                messageParams[3] = DateTime.Now.ToString();

                Status = ExportProjectToEnvBLL.GetMessageText(messageId, messageParams, out formattedMemoNoteText);
                if (Status.messsageId != string.Empty)
                {
                    if (Status.messsageId == "Invalid Message format")
                    {
                        String logMessage = "Message format is invalid." +
                            "\nMessage Id " + Status.messageParams[0] +
                            "\nValid format is: " + Status.messageParams[1];
                        Domain.SaveGeneralWarningLog(logMessage);
                    }
                    else if (Status.messsageId == "228")
                    {
                        String logMessage = "Message Id does not exist in PE_Messages table." +
                                                   "\nMessage Id " + Status.messageParams[0];
                        Domain.SaveGeneralWarningLog(logMessage);
                    }
                    else
                    {
                        String logMessage = "Failed to retrieve note text. Message Id is " + messageId.ToString();
                        Domain.SaveGeneralWarningLog(logMessage);
                    }
                }
                return Status;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return Status;
            }
        }

        #endregion

    }
}
