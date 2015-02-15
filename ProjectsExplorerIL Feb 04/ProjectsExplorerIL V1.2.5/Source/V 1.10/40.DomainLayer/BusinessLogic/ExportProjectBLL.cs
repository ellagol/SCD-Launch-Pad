using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using ATSBusinessLogic.ContentMgmtBLL;
using ATSBusinessObjects;
using ATSBusinessObjects.ContentMgmtModels;
using ATSDomain;
using System.Threading;

namespace ATSBusinessLogic
{
    public class ExportProjectBLL
    {

        #region Data

        //Folders names
        public static string archivePEFolderName = "PE";
        public static string archiveCMFolderName = "CM";
        public static string archiveCMFilesFolderName = "Files";
        public static string archiveSPFolderName = "SystemParameters";
        public static string archiveIconsFolderName = "Icons";

        //Project tables
        public static string projectDetailsXmlFileName = "PE_Hierarchy.xml";
        public static string versionDetailsXmlFileName = "PE_Version.xml";
        public static string versionContentsXmlFileName = "PE_VersionContent.xml";

        //CM tables
        public static string cmContentTreeXmlFileName = "ContentTree.xml";
        public static string cmContentDetailsXmlFileName = "Content.xml";
        public static string cmContentVersionXmlFileName = "ContentVersion.xml";
        public static string cmContentTreeUserGroupTypeXmlFileName = "ContentTreeUserGroupType.xml";
        public static string cmContentVersionFileXmlFileName = "ContentVersionFile.xml";
        public static string cmContentVersionVersionLinkXmlFileName = "ContentVersionVersionLink.xml";

        //System Parameters tables
        public static string CMSystemParametersXmlFileName = "SystemParameters.xml";
        public static string PESystemParametersXmlFileName = "PE_SystemParameters.xml";

        //CM Tree
        static Dictionary<int, CMFolderModel> outFolders;
        static Dictionary<int, CMContentModel> outContents;
        static Dictionary<int, CMVersionModel> outVersions;

        #endregion

        #region Main

        public static ATSDomain.Domain.ErrorHandling ExportProject(HierarchyModel projectModel, string fsParentFolderFullPath, 
                                                                string packageName)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            string archiveFullPath = GetArchiveFullPath(fsParentFolderFullPath, packageName);
            try
            {
                //extract project tables and files.                
                Status = ExtractProjectData(projectModel, fsParentFolderFullPath, packageName, archiveFullPath, null);
                if (Status.messsageId != string.Empty) 
                {
                    FileSystemBLL.DeleteDirectoryRecursive(archiveFullPath); //Clean if failed
                    return Status;
                }

                return Status;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Status.messsageId = "105";

                //Housekeeping - delete archive folder if any
                FileSystemBLL.DeleteDirectoryRecursive(archiveFullPath);
                return Status;
            }
        }

        public static ATSDomain.Domain.ErrorHandling ExtractProjectData(HierarchyModel projectModel, string fsParentFolderFullPath,
                                                                 string packageName, string archiveFullPath, ThreadStart t)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                //create common folder for archive files                
                Status = FileSystemBLL.CreateFolder(archiveFullPath);
                if (Status.messsageId != string.Empty) //Failed to create archive folder
                {
                    return Status;
                }

                //Get project's active version contents
                ContentBLL contentBLL = new ContentBLL(projectModel.Id);
                ObservableCollection<ContentModel> activeVersionCMs = contentBLL.getActiveContents(projectModel.ActiveVersion);
                FileSystemBLL.totalExportImportFiles = 3 * ContentBLL.CountProjectCMFiles(activeVersionCMs);

                ExportProjectToEnvBLL.progressBarTitle = "Extracting project data from source environment ...";
                if (t != null)
                {
                    FileSystemBLL.exportFilesCompleted = 0;
                    Thread thread = new Thread(t);
                    thread.Start();
                    thread.Join();
                }

                //If no contents are associated to active project version - proceed with PE data only. Else - start with CM data export
                if (activeVersionCMs != null && activeVersionCMs.Count != 0)
                {
                    //continue to CM
                    Status = ExportCMData(projectModel, activeVersionCMs, archiveFullPath, t);
                    if (Status.messsageId != string.Empty) //Failed to export CM data
                    {
                        return Status;
                    }
                }

                //Proceed with PE
                Status = ExportPEData(projectModel, archiveFullPath);
                if (Status.messsageId != string.Empty) //Failed to export PE data
                {
                    return Status;
                }

                Status = ExportSystemParametersData(archiveFullPath);
                if (Status.messsageId != string.Empty) //Failed to export PE data
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

        public static ATSDomain.Domain.ErrorHandling ExportProjectFromSourceEnv(HierarchyModel projectModel, string fsParentFolderFullPath, string packageName,
                                                                        ThreadStart t)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            string archiveFullPath = GetArchiveFullPath(fsParentFolderFullPath, packageName);
            try
            {
                //extract project tables and files.   
                Status = ExtractProjectData(projectModel, fsParentFolderFullPath, packageName, archiveFullPath, t);
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

        #endregion

        #region Export PE Objects

        static Domain.ErrorHandling ExportPEData(HierarchyModel Project, string archiveCommonFolderFullPath)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                //create folder for PE files
                string archivePEFullPath = GetPEFolderFullPath(archiveCommonFolderFullPath);
                Status = FileSystemBLL.CreateFolder(archivePEFullPath);
                if (Status.messsageId != string.Empty) //Failed to create PE folder under archive folder
                {
                    return Status;
                }

                Status = ExportProjectTablesToXmlFiles(Project.ActiveVersion, Project.Id, archivePEFullPath);
                if (Status.messsageId != string.Empty) //Failed to export tables to xml
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

        static Domain.ErrorHandling ExportProjectTablesToXmlFiles(string versionName, long projectId, string fileDestinationFolder)
        {
            Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                //Export project details from PE_Hierarchy to XML
                Status = FileSystemBLL.ExportProjectDetailsToXml(projectId, fileDestinationFolder, projectDetailsXmlFileName);
                if (Status.messsageId != string.Empty) //Failed to export data to xml
                {
                    return Status;
                }
                Status = FileSystemBLL.ExportVersionDetailsToXml(projectId, fileDestinationFolder, versionDetailsXmlFileName);
                if (Status.messsageId != string.Empty) //Failed to export data to xml
                {
                    return Status;
                }
                Status = FileSystemBLL.ExportVersionContentsDetailsToXml(versionName, projectId, fileDestinationFolder, versionContentsXmlFileName);
                if (Status.messsageId != string.Empty) //Failed to export data to xml
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

        #endregion Export PE Objects

        #region Export CM Objects

        static ATSDomain.Domain.ErrorHandling ExportCMData(HierarchyModel Project, ObservableCollection<ContentModel> projectsActiveVersions, 
                                                           string archiveCommonFolderFullPath,
                                                           ThreadStart t)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                //create folder for CM tables xml files
                string archiveCMFullPath = GetCMFolderFullPath(archiveCommonFolderFullPath);
                Status = FileSystemBLL.CreateFolder(archiveCMFullPath);
                if (Status.messsageId != string.Empty) //Failed to create CM folder under archive folder
                {
                    return Status;
                }

                Status = ExportCMTablesToXmlFiles(projectsActiveVersions, archiveCMFullPath);
                if (Status.messsageId != string.Empty) //Failed to export CM tables
                {
                    return Status;
                }

                Status = CopyContentsFilesToArchive(projectsActiveVersions, archiveCommonFolderFullPath, t);
                if (Status.messsageId != string.Empty) //Failed to export CM tables
                {
                    return Status;
                }

                Status = CopyContentsIconsToArchive(projectsActiveVersions, archiveCommonFolderFullPath, t);
                if (Status.messsageId != string.Empty) //Failed to export CM tables
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

        static Domain.ErrorHandling ExportCMTablesToXmlFiles(ObservableCollection<ContentModel> projectsActiveVersions, string fileDestinationFolder)
        {
            Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            List<int> versionIds = new List<int>();
            List<int> contentIds = new List<int>();
            List<int> folderIds = new List<int>();

            try
            {
                //Export contents details from CM tables to XML
                Status = GetListOfAllProjectVersionsWithLinked(projectsActiveVersions, out versionIds);
                if (Status.messsageId != string.Empty) //Failed to export data to xml
                {
                    return Status;
                }

                //Validate there are no versions in status Edit
                foreach (int vId in versionIds)
                {
                    if (ContentBLL.versions.ContainsKey(vId))
                    {
                        if (ContentBLL.versions[vId].Status.ID == "Edit")
                        {
                            Status.messsageId = "233";
                            int parentContentId = ContentBLL.versions[vId].ParentID;
                            Status.messageParams[0] = ContentBLL.contents[parentContentId].Name;
                            Status.messageParams[1] = ContentBLL.versions[vId].Name;
                            return Status;
                        }
                    }
                    else //refresh tree
                    {
                        ContentBLL.allContents = ContentBLL.LoadContentTreeToMemory(out ContentBLL.folders, 
                                                                                    out ContentBLL.contents, 
                                                                                    out ContentBLL.versions);
                        if (ContentBLL.versions[vId].Status.ID == "Edit")
                        {
                            Status.messsageId = "233";
                            int parentContentId = ContentBLL.versions[vId].ParentID;
                            Status.messageParams[0] = ContentBLL.contents[parentContentId].Name;
                            Status.messageParams[1] = ContentBLL.versions[vId].Name;
                            return Status;
                        }
                    }
                }

                Status = FileSystemBLL.ExportContentVersionVersionLinksToXml(versionIds, fileDestinationFolder, cmContentVersionVersionLinkXmlFileName);
                if (Status.messsageId != string.Empty) //Failed to export data to xml
                {
                    return Status;
                }

                Status = FileSystemBLL.ExportContentVersionFilesToXml(versionIds, fileDestinationFolder, cmContentVersionFileXmlFileName);
                if (Status.messsageId != string.Empty) //Failed to export data to xml
                {
                    return Status;
                }

                Status = FileSystemBLL.ExportContentVersionsToXml(versionIds, fileDestinationFolder, cmContentVersionXmlFileName);
                if (Status.messsageId != string.Empty) //Failed to export data to xml
                {
                    return Status;
                }

                Status = GetListOfAllVersionsParentContents(versionIds, out contentIds);
                if (Status.messsageId != string.Empty) //Failed to export data to xml
                {
                    return Status;
                }
                Status = FileSystemBLL.ExportContentsToXml(contentIds, fileDestinationFolder, cmContentDetailsXmlFileName);
                if (Status.messsageId != string.Empty) //Failed to export data to xml
                {
                    return Status;
                }

                Status = GetListOfAllContentsParentFolders(contentIds, out folderIds);
                if (Status.messsageId != string.Empty) //Failed to export data to xml
                {
                    return Status;
                }
                Status = FileSystemBLL.ExportContentTreeUserGroupTypesToXml(folderIds, fileDestinationFolder, cmContentTreeUserGroupTypeXmlFileName);
                if (Status.messsageId != string.Empty) //Failed to export data to xml
                {
                    return Status;
                }

                Status = FileSystemBLL.ExportContentTreeToXml(folderIds, fileDestinationFolder, cmContentTreeXmlFileName);
                if (Status.messsageId != string.Empty) //Failed to export data to xml
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

        static Domain.ErrorHandling CopyContentsFilesToArchive(ObservableCollection<ContentModel> projectContentVersions, string archiveFolder, ThreadStart t)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();

            List<int> listOfAllRelevantCMVersionsIds = new List<int>();
            try
            {
                Status = GetListOfAllProjectVersionsWithLinked(projectContentVersions, out listOfAllRelevantCMVersionsIds);
                if (Status.messsageId != String.Empty || listOfAllRelevantCMVersionsIds != null && listOfAllRelevantCMVersionsIds.Count>0
                            && projectContentVersions != null && projectContentVersions.Count > 0)
                {
                    ContentExecutionBLL.ErrorHandling status = ContentExecutionBLL.GetCMSubTree(projectContentVersions,
                                                    out outFolders,  
                                                    out outContents, 
                                                    out outVersions);
                    if (status.errorId != string.Empty || 
                        outContents == null || outContents.Count<1 ||
                        outVersions == null || outVersions.Count < 1) //Failed to get CM tree
                    {
                       Status.messsageId = status.errorId;
                       return Status;
                    }

                    //create folder for CM versions files
                    string archiveCMFilesFullPath = GetFilesFolderFullPath(archiveFolder);
                    Status = FileSystemBLL.CreateFolder(archiveCMFilesFullPath);
                    if (Status.messsageId != string.Empty) //Failed to create folder
                    {
                        return Status;
                    }

                    //Copy version files to archive folder
                    foreach (int vId in listOfAllRelevantCMVersionsIds)
                    {
                        Status = CopyVersionFilesToArchive(vId, archiveFolder, t);
                        if (Status.messsageId != string.Empty) //Failed to export data to xml
                        {
                            Status.messsageId = "231";
                            Status.messageParams[0] = outContents[outVersions[vId].ParentID].Name;
                            Status.messageParams[1] = outVersions[vId].Name;
                            return Status;
                        }
                    }
                }
                else
                {
                    String logMessage = "Empty versions list.";
                    Domain.SaveGeneralErrorLog(logMessage);
                    Status.messsageId = "105";
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

        static Domain.ErrorHandling CopyContentsIconsToArchive(ObservableCollection<ContentModel> projectContentVersions, string fileDestinationFolder,
                                                                                ThreadStart t)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();

            List<int> listOfAllRelevantCMVersionsIds = new List<int>();
            List<int> listOfContents = new List<int>();
            try
            {
                Status = GetListOfAllProjectVersionsWithLinked(projectContentVersions, out listOfAllRelevantCMVersionsIds);
                if (Status.messsageId != String.Empty || listOfAllRelevantCMVersionsIds != null && listOfAllRelevantCMVersionsIds.Count > 0
                            && projectContentVersions != null && projectContentVersions.Count > 0)
                {
                    Status = GetListOfAllVersionsParentContents(listOfAllRelevantCMVersionsIds, out listOfContents);
                    if (Status.messsageId != String.Empty) //Failed
                    {
                        return Status;
                    }


                    //create folder for CM Icons files
                    string archiveCMIconsFullPath = GetIconsFolderPath(fileDestinationFolder);
                    Status = FileSystemBLL.CreateFolder(archiveCMIconsFullPath);
                    if (Status.messsageId != string.Empty) //Failed to create folder
                    {
                        return Status;
                    }

                    //Copy icons to archive folder
                    foreach (int cId in listOfContents)
                    {
                        Status = CopyContentIconToArchive(cId, fileDestinationFolder, t);
                        if (Status.messsageId != string.Empty) //Failed to export data to xml
                        {
                            return Status;
                        }
                    }
                }
                else
                {
                    String logMessage = "Empty versions list.";
                    Domain.SaveGeneralErrorLog(logMessage);
                    Status.messsageId = "105";
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

        static string CreateVersionFilesFolder(string path, int versionId)
        {
            Domain.ErrorHandling Status = new Domain.ErrorHandling();
            try
            {
                //string versionFilesFolderName = VersionFilesFolderName(versionId);

                string versionFilesFolderName = VersionFilesFolderName(versionId);
                string versionFilesFolderFullPath = GetVersionFilesFolderFullPath(path, versionId);
                Status = FileSystemBLL.CreateFolder(versionFilesFolderFullPath);
                if (Status.messsageId != string.Empty) //Failed to create folder
                {
                    return string.Empty;
                }

                return versionFilesFolderFullPath;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                    return string.Empty;
                }
        }

        static string CreateIconFolder(string path, int contentId)
        {
            Domain.ErrorHandling Status = new Domain.ErrorHandling();
            try
            {
                string iconFileFolderName = IconFileFolderName(contentId);

                if (string.IsNullOrEmpty(iconFileFolderName) || string.IsNullOrWhiteSpace(iconFileFolderName))
                {
                    return string.Empty;
                }

                string iconFileFolderFullPath = GetContentIconFolderFullPath(path, contentId);

                Status = FileSystemBLL.CreateFolder(iconFileFolderFullPath);
                if (Status.messsageId != string.Empty) //Failed to create folder
                {
                    return string.Empty;
                }

                return iconFileFolderFullPath;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return string.Empty;
            }
        }

        static ATSDomain.Domain.ErrorHandling CopyVersionFilesToArchive(int versionId, string path, ThreadStart t)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                string targetPath = CreateVersionFilesFolder(path, versionId);
                string sourcePath = VersionFilesSourcePath(versionId);

                if (!string.IsNullOrEmpty(targetPath) && !string.IsNullOrEmpty(sourcePath))
                {
                    Status = FileSystemBLL.CopyFolderWithFilesAndSubfolders(sourcePath, targetPath, t);
                }
                else
                {
                    Status.messsageId = "105";
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

        static ATSDomain.Domain.ErrorHandling CopyContentIconToArchive(int contentId, string path, ThreadStart t)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                string targetPath = CreateIconFolder(path, contentId);
                string sourcePath = IconSourcePath(contentId);

                if (!string.IsNullOrEmpty(targetPath) && !string.IsNullOrEmpty(sourcePath))
                {
                    Status = FileSystemBLL.CopyFolderWithFilesAndSubfolders(sourcePath, targetPath, t);
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


        #region CM Export Helpers

        static ATSDomain.Domain.ErrorHandling GetListOfAllProjectVersionsWithLinked(ObservableCollection<ContentModel> projectContents, out List<int> listOfCMVersionsIds)
        {
            listOfCMVersionsIds = new List<int>();

            Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            List<int> vIds = new List<int>();
            List<int> linkedVersionIds = new List<int>();

            try
            {
                if (projectContents != null && projectContents.Count > 0)
                {
                    foreach (ContentModel v in projectContents)
                    {
                        vIds.Add(v.id);
                    }

                    listOfCMVersionsIds = ContentBLL.GetVersionAllLinkedSubVersions(vIds);
                    listOfCMVersionsIds.AddRange(vIds);
                    return Status;
                }
                Status.messsageId = "105";

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

        static ATSDomain.Domain.ErrorHandling GetListOfAllVersionsParentContents(List<int> listOfCMVersionsIds, out List<int> listOfCMContentIds)
        {
            listOfCMContentIds = new List<int>();

            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            List<int> vIds = new List<int>();
            List<int> linkedVersionIds = new List<int>();

            try
            {
                if (listOfCMVersionsIds != null && listOfCMVersionsIds.Count > 0)
                {
                    listOfCMContentIds = ContentBLL.GetAllContentIds(listOfCMVersionsIds);
                    return Status;
                }
                Status.messsageId = "105";

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

        static ATSDomain.Domain.ErrorHandling GetListOfAllContentsParentFolders(List<int> listOfCMContentIds, out List<int> listOfCMFolderIds)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
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
                        else
                        {
                            Status.messsageId = "105";
                            break;
                        }
                    }
                }
                else
                {
                    Status.messsageId = "105";
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

        public static string VersionFilesFolderName(int versionId)
        {
            try
            {
                if (versionId > 0)
                {
                    //string versionName = outVersions[versionId].Name;
                    //string contentName = outContents[outVersions[versionId].ParentID].Name;
                    //string folderName = contentName + " " + versionName;
                    string folderName = versionId.ToString();
                return folderName;
            }
                else
                {
                return null;
            }
        }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return null;
            }
        }

        public static string IconFileFolderName(int contentId)
        {
            try
            {
                if (contentId > 0)
                {
                    string folderName = contentId.ToString();
                    return folderName;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return null;
            }
        }

        public static string GetCMFolderFullPath(string archiveParentFolderFullPath)
        {
            string archiveCMFullPath = archiveParentFolderFullPath + "\\" + archiveCMFolderName;
            return archiveCMFullPath;
        }

        public static string GetIconsFolderPath(string archiveFolderFullPath)
        {
            string cmPath = GetCMFolderFullPath(archiveFolderFullPath);
            string archiveCMIconsFullPath = cmPath + "\\" + archiveIconsFolderName;
            return archiveCMIconsFullPath;
        }
        
        public static string GetPEFolderFullPath(string archiveParentFolderFullPath)
        {
            string fullPath = archiveParentFolderFullPath + "\\" + archivePEFolderName;
            return fullPath;
        }

        public static string GetSPFolderFullPath(string archiveParentFolderFullPath)
        {
            string fullPath = archiveParentFolderFullPath + "\\" + archiveSPFolderName;
            return fullPath;
        }

        public static string GetVersionFilesFolderFullPath(string archiveFullPath, int versionId)
        {
            string versionFilesFolderName = VersionFilesFolderName(versionId);
            string filesFolderFullPath = GetFilesFolderFullPath(archiveFullPath);
            string fullPath = filesFolderFullPath + "\\" + versionFilesFolderName;
            return fullPath;
        }

        public static string GetContentIconFolderFullPath(string archiveFullPath, int contentId)
        {
            string iconFolderName = IconFileFolderName(contentId);
            string iconFolderFullPath = GetIconsFolderPath(archiveFullPath);
            string fullPath = iconFolderFullPath + "\\" + iconFolderName;
            return fullPath;
        }

        public static string GetFilesFolderFullPath(string archiveFolderFullPath)
        {
            string fullPath = GetCMFolderFullPath(archiveFolderFullPath) + "\\" + archiveCMFilesFolderName;
            return fullPath;
        }

        public static string GetArchiveFullPath(string parentFolder, string packageName)
        {
            string fullPath = parentFolder + "\\" + packageName;
            return fullPath;
        }

        static string VersionFilesSourcePath(int versionId)
        {
            try
            {
                string rootPathFS = CMTreeNodeBLL.getRootPath();
                string sourcePath = rootPathFS + "\\" + outVersions[versionId].Path.Name;

                return sourcePath;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return string.Empty;
            }
        }

        static string IconSourcePath(int contentId)
        {
            try
            {
                if (!string.IsNullOrEmpty(outContents[contentId].IconPath) && !string.IsNullOrWhiteSpace(outContents[contentId].IconPath))
                {
                    FileInfo iconFullPath = new FileInfo(outContents[contentId].IconPath);

                    string sourcePath = iconFullPath.DirectoryName;

                    return sourcePath;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return string.Empty;
            }
        }

        #endregion Helpers

        #endregion Export CM Objects

        #region Export System Parameters

        static Domain.ErrorHandling ExportSystemParametersData(string archiveCommonFolderFullPath)
        {
            ATSDomain.Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                //create folder for System Parameters file files
                string archiveSPFullPath = GetSPFolderFullPath(archiveCommonFolderFullPath);
                Status = FileSystemBLL.CreateFolder(archiveSPFullPath);
                if (Status.messsageId != string.Empty) //Failed to create folder under archive folder
                {
                    return Status;
                }

                Status = ExportSystemParametersTablesToXmlFiles(archiveSPFullPath);
                if (Status.messsageId != string.Empty) //Failed to export tables to xml
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

        static Domain.ErrorHandling ExportSystemParametersTablesToXmlFiles(string fileDestinationFolder)
        {
            Domain.ErrorHandling Status = new ATSDomain.Domain.ErrorHandling();
            try
            {
                //Export project details from PE_Hierarchy to XML
                Status = FileSystemBLL.ExportCMSystemParametersToXml(fileDestinationFolder, CMSystemParametersXmlFileName);
                if (Status.messsageId != string.Empty) //Failed to export data to xml
                {
                    return Status;
                }
                Status = FileSystemBLL.ExportPESystemParametersToXml(fileDestinationFolder, PESystemParametersXmlFileName);
                if (Status.messsageId != string.Empty) //Failed to export data to xml
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

        #endregion Export PE Objects

    }
}
