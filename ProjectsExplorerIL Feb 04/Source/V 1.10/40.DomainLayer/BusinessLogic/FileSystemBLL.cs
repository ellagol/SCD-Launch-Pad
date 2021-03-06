﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using ATSBusinessLogic.ContentMgmtBLL;
using ATSBusinessObjects.ContentMgmtModels;
using ATSDomain;


namespace ATSBusinessLogic
{
    //Ella - File System access management class

    public class FileSystemBLL
    {
        #region Error Handling

        public enum FileSystemBLLReturnCode : int
        {
            DriveNotFound, //140
            NotEnoughSpace, //121
            FileNotFound, //142
            CommonException, //105
            ExecutableNotFound, //132
            UnableToExecute, //132
            TargetDirectoryNotFound, //147
            FailedToCopyContentfiles, //145
            FailedToDeleteObsoleteFiles, //146
            exeNotSpecified, //132
            Success,
            UnauthorizedAccessException, //147
            InvalidCommandLine, //132
            
        }

        #endregion

        #region Execution

        public static int filesCompleted = 0;
        public static int filesToCopyNumber = -1;

        //Ella - check free disk space for DriveName
        public static FileSystemBLLReturnCode GetDriverFreeSpace(string driveName, out long freeDiskSpace)
        {
            FileSystemBLLReturnCode checkStatus = FileSystemBLLReturnCode.DriveNotFound;
            freeDiskSpace = -1;
            try
            {
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    if (drive.IsReady && drive.Name == driveName)
                    {
                        freeDiskSpace = drive.TotalFreeSpace;
                        checkStatus = FileSystemBLLReturnCode.Success;
                    }
                }
                if (freeDiskSpace < 0)
                {
                    checkStatus = FileSystemBLLReturnCode.DriveNotFound;
                    //Error to the log file
                    if (driveName.Equals(String.Empty))
                    {
                        Domain.SaveGeneralErrorLog("Drive is not specified in project Target Directory field");
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Failed to get available disk space value for disk ");
                        sb.Append(driveName);
                        sb.Append(". Please check project Target Directory field");
                        Domain.SaveGeneralErrorLog(Convert.ToString(sb));
                    }
                }
                return checkStatus;
            }
            catch (Exception e)
            {
                ApiBLL.TraceExceptionParameterValue.Add(e.Message);

                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return checkStatus;
            }
        }

        public static FileSystemBLLReturnCode ExecuteRunningString(string User, string envName, string connString, long projectId, long prVerId, string trgPath, string commandLine)
        {

            FileSystemBLLReturnCode executeStatus = FileSystemBLLReturnCode.CommonException;

            try
            {
                String user = "\"" + User + "\"";
                String DB_CON = "\"" + connString.Replace(@"\\", @"\") + "\""; //If passed without quotation marks
                //String DB_CON = connString;
                String PROJ_VERSION_ID = Convert.ToString(prVerId);
                String PROJECT_ID = projectId.ToString();
                Process execute = new Process();
                string exeFilePath = trgPath + "\\" + commandLine;
                execute.StartInfo.FileName = exeFilePath;
                execute.StartInfo.WorkingDirectory = Path.GetDirectoryName(exeFilePath);
                execute.StartInfo.Arguments = "/User=" + user + " /Environment=" + envName + " /DbConnString=" + DB_CON + " /ProjectVersionID=" + PROJ_VERSION_ID + " /ProjectCode=" + PROJECT_ID;
                execute.Start();

                executeStatus = FileSystemBLLReturnCode.Success;
                return executeStatus;
            }
            catch (Exception ex)
            {
                ApiBLL.TraceExceptionParameterValue.Add(ex.Message);
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return executeStatus;
            }
        }

        private static FileSystemBLLReturnCode CreatefilesToCopyList(Dictionary<int, CMVersionModel> cmVersions, Dictionary<long, int> activeVersionContents, out Hashtable filesToCopyList)
        {
            filesToCopyList = new Hashtable();
            FileSystemBLLReturnCode copyStatus = FileSystemBLLReturnCode.CommonException;

            try
            {
                foreach (KeyValuePair<long, int> cm in activeVersionContents)
                {
                    foreach (CMVersionModel v in cmVersions.Values)
                    {
                        if (v.ID == cm.Key)
                        //if (v.Name.ToString().Equals(cm.version.ToString()))
                        {
                            foreach (CMContentFileModel f in v.Files.Values)
                            {
                                //filesToCopyList.Add(f, v.Path.Name + "\\" + f.FileRelativePath + "\\" + f.FileName);
                                filesToCopyList.Add(f, f.FileRelativePath + "\\" + f.FileName);
                            }
                        }
                    }
                }
                copyStatus = FileSystemBLLReturnCode.Success;
                return copyStatus;
            }
            catch (Exception ex)
            {
                ApiBLL.TraceExceptionParameterValue.Add(ex.Message);
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return copyStatus;
            }
        }

        private static FileSystemBLLReturnCode executeFileCopy(String targetPath, String source, String relativePath, ThreadStart t)
        {
            FileSystemBLLReturnCode filescopyStatus = FileSystemBLLReturnCode.CommonException;
            try
            {
                String fileName = Path.GetFileName(source);
                //string fileNmae = string.
                String fullPath = Path.GetDirectoryName(source);
                String destFile = Path.Combine(targetPath + "\\" + relativePath, fileName);
                Directory.CreateDirectory(targetPath + "\\" + relativePath);
                File.Copy(source, destFile, true);

                try
                {
                    filesCompleted++;
                    Thread thread = new Thread(t);
                    thread.Start();
                    thread.Join();
                }
                catch (Exception e) 
                {
                    String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                    Domain.SaveGeneralErrorLog(logMessage);
                }

                filescopyStatus = FileSystemBLLReturnCode.Success;
                return filescopyStatus;
            }
            catch (Exception ex)
            {
                filescopyStatus = FileSystemBLLReturnCode.FailedToCopyContentfiles;
                ApiBLL.TraceExceptionParameterValue.Add(ex.Message);
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return filescopyStatus;
            }
        }

        private static FileSystemBLLReturnCode executeFileDelete(String targetPath, List<String> filesToDelete)
        {
            FileSystemBLLReturnCode status = FileSystemBLLReturnCode.CommonException;
            try
            {
                foreach (String f in filesToDelete)
                {
                    String fullPath = Path.GetDirectoryName(f);
                    FileInfo fi = new System.IO.FileInfo(@f);
                    File.SetAttributes(f, FileAttributes.Normal);
                    fi.Delete();

                }
                status = processDirectory(targetPath);
                return status;
            }
            catch (Exception ex)
            {
                status = FileSystemBLLReturnCode.FailedToDeleteObsoleteFiles;
                ApiBLL.TraceExceptionParameterValue.Add(ex.Message);
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return status;
            }
        }

        private static FileSystemBLLReturnCode processDirectory(string startLocation)
        {
            FileSystemBLLReturnCode status = FileSystemBLLReturnCode.CommonException;
            try
            {
                foreach (var directory in Directory.GetDirectories(startLocation))
                {
                    processDirectory(directory);
                    if (Directory.GetFiles(directory).Length == 0 &&
                        Directory.GetDirectories(directory).Length == 0)
                    {
                        Directory.Delete(directory, false);
                    }
                }
                status = FileSystemBLLReturnCode.Success;
                return status;
            }
            catch (Exception e)
            {
                ApiBLL.TraceExceptionParameterValue.Add(e.Message);
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return status;
            }
        }

        public static FileSystemBLLReturnCode CopyContentVersionsFilesToLocalWithFileList(Dictionary<int, CMVersionModel> cmVersions, Dictionary<long, int> activeVersionContents, string versionTargetPath, out String faildFiles)
        {
            return CopyContentVersionsFilesToLocalWithFileList(cmVersions,activeVersionContents,versionTargetPath, out faildFiles,null);
        }

        public static FileSystemBLLReturnCode CopyContentVersionsFilesToLocalWithFileList(Dictionary<int, CMVersionModel> cmVersions, Dictionary<long, int> activeVersionContents, string versionTargetPath, out String faildFiles, ThreadStart t)
        {
            faildFiles = "";
            FileSystemBLLReturnCode copystatus = FileSystemBLLReturnCode.CommonException;
            FileSystemBLLReturnCode exeFilesCopyStatus = FileSystemBLLReturnCode.CommonException;
            FileSystemBLLReturnCode exeFilesDeleteStatus = FileSystemBLLReturnCode.CommonException;
            try
            {
                if (!Directory.Exists(versionTargetPath)) //Create Directory if not exists
                {
                    Directory.CreateDirectory(versionTargetPath);
                }

                //Create local exists file list with last modified dates
                Dictionary<String, DateTime> localFilesWithDates = new Dictionary<String, DateTime>();
                if (createLocalFileListWithDates(versionTargetPath, out localFilesWithDates) != FileSystemBLLReturnCode.Success)
                {
                    return FileSystemBLLReturnCode.CommonException;
                }

                //Create source file list with last modified dates
                Dictionary<CMContentFileModel, DateTime> SourceFilesWithDates = new Dictionary<CMContentFileModel, DateTime>();
                if (CreateSourceFilesListWithDates(cmVersions, activeVersionContents, out SourceFilesWithDates) != FileSystemBLLReturnCode.Success)
                {
                    return FileSystemBLLReturnCode.CommonException;
                }

                List<String> finalListToDelete = localFilesWithDates.Keys.ToList();
                List<CMContentFileModel> finalListToCopy = SourceFilesWithDates.Keys.ToList();


                //Create final list of files to copy and delete
                foreach (string localFile in localFilesWithDates.Keys)
                {
                    string localRelative = localFile.Substring(versionTargetPath.Length + 1);
                    foreach (CMContentFileModel sourceFile in SourceFilesWithDates.Keys)
                    {
                        string sourceRelative = Path.Combine(sourceFile.FileRelativePath, sourceFile.FileName);
                        if (localRelative.Equals(sourceRelative))
                        {
                            if (localFilesWithDates[localFile].Equals(SourceFilesWithDates[sourceFile]))
                            {
                                finalListToCopy.Remove(sourceFile);
                                finalListToDelete.Remove(localFile);
                            }
                            break;
                        }
                    }
                }

                filesToCopyNumber = finalListToCopy.Count;

                if (!versionTargetPath.Equals(null))
                {
                    exeFilesDeleteStatus = executeFileDelete(versionTargetPath, finalListToDelete);
                    foreach (CMContentFileModel s in finalListToCopy)
                    {
                        //Execute the file copy
                        exeFilesCopyStatus = executeFileCopy(versionTargetPath, s.FileFullPath.ToString(), s.FileRelativePath.ToString(),t);

                        if (exeFilesCopyStatus != FileSystemBLLReturnCode.Success)
                        {
                            faildFiles = s.FileName;
                            copystatus = exeFilesCopyStatus;
                            return copystatus;
                        }
                    }
                    if (exeFilesDeleteStatus != FileSystemBLLReturnCode.Success)
                    {
                        copystatus = exeFilesDeleteStatus;
                        return copystatus;
                    }
                    copystatus = FileSystemBLLReturnCode.Success;
                }
                else
                {
                    ApiBLL.TraceExceptionParameterValue.Add("Target path " + versionTargetPath);
                    copystatus = FileSystemBLLReturnCode.TargetDirectoryNotFound;
                }
                return copystatus;
            }
            catch (Exception e)
            {
                ApiBLL.TraceExceptionParameterValue.Add(e.Message);
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return copystatus;
            }
        }

        private static FileSystemBLLReturnCode createLocalFileListWithDates(string TargetPath, out Dictionary<String, DateTime> localFilesWithDates)
        {
            localFilesWithDates = new Dictionary<String, DateTime>();
            List<string> allLocalFiles = new List<string>();
            try
            {
                if (!TargetPath.Equals(null))
                {
                    allLocalFiles.AddRange((Directory.GetFiles(TargetPath, "*", SearchOption.AllDirectories)).ToList());
                    foreach (string s in allLocalFiles)
                    {
                        localFilesWithDates.Add(s, File.GetLastWriteTime(s));
                    }

                    return FileSystemBLLReturnCode.Success;
                }
                else
                {
                    return FileSystemBLLReturnCode.TargetDirectoryNotFound;
                }
            }
            catch (Exception e) 
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return FileSystemBLLReturnCode.TargetDirectoryNotFound;
            }
        }

        public static FileSystemBLLReturnCode CreateSourceFilesListWithDates(Dictionary<int, CMVersionModel> cmVersions, Dictionary<long, int> activeVersionContents, out Dictionary<CMContentFileModel, DateTime> SourceFilesWithDates)
        {
            SourceFilesWithDates = new Dictionary<CMContentFileModel, DateTime>();

            try
            {
                List<CMContentFileModel> filesToCopyListUniqAndSorted = new List<CMContentFileModel>();
                ContentBLL.createAllFilesSortedList(activeVersionContents, cmVersions, out filesToCopyListUniqAndSorted);

                foreach (CMContentFileModel cf in filesToCopyListUniqAndSorted)
                {
                    SourceFilesWithDates.Add(cf, File.GetLastWriteTime(cf.FileFullPath));
                }

                return FileSystemBLLReturnCode.Success;
            }
            catch (Exception ex) 
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return FileSystemBLLReturnCode.CommonException; 
            }

            
        }

        public static Domain.ErrorHandling SaveClassInstanceToXmlFile(object classObject, string parentFolderFullPath, string fileName)
        {
            Domain.ErrorHandling Status = new Domain.ErrorHandling();

            try
            {
                Status = CreateFolder(parentFolderFullPath);
                if (Status.messsageId != string.Empty)
                {
                    return Status;
                }

                Type classType = classObject.GetType();
                XmlSerializer writer = new XmlSerializer(classType);

                string fileFullPath = parentFolderFullPath + "\\" + fileName;
                StreamWriter file = new StreamWriter(fileFullPath);
                
                var classInstance = Convert.ChangeType(classObject, classObject.GetType());                               
                writer.Serialize(file, classInstance);
                
                file.Close();

                return Status;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception(logMessage);
            }
        }

        #region Content Execution
        
        public static FileSystemBLLReturnCode ExecuteRunningStringContentExecution( long prVerId,
                                                                                    string projFullPath,
                                                                                    string projCode,
                                                                                    string projVersionName,
                                                                                    string trgPath, string commandLine)
        {

            FileSystemBLLReturnCode executeStatus = FileSystemBLLReturnCode.CommonException;

            try
            {
                if (!(String.IsNullOrWhiteSpace(commandLine) || String.IsNullOrEmpty(commandLine)))
                {
                    string actualParameters = null;

                    FileSystemBLLReturnCode status = substituteValuesToCommandLineParams(/*executionParams*/ commandLine, prVerId, projCode, projFullPath, projVersionName, out  actualParameters);
                    if (status != FileSystemBLLReturnCode.Success)
                    {
                        return status;
                    }

                    ProcessStartInfo pStartInfo = new ProcessStartInfo("cmd.exe", "/c " + actualParameters);
                    Process execute = new Process();

                    pStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    pStartInfo.CreateNoWindow = true;
                    pStartInfo.WorkingDirectory = trgPath;

                    execute.StartInfo = pStartInfo;

                    execute.Start();

                    return FileSystemBLLReturnCode.Success;
                }
                else 
                {
                    return FileSystemBLLReturnCode.InvalidCommandLine;
                }
            }
            catch (Exception e)
            {
                executeStatus = FileSystemBLLReturnCode.UnableToExecute;
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return executeStatus;
            }
        }

        private static FileSystemBLLReturnCode substituteValuesToCommandLineParams(string parameters,
                                                                                    long projVerId,
                                                                                    string projCode,
                                                                                    string projName,
                                                                                    string projVersionName,
                                                                                out string actualParameters)
        {
            actualParameters = null;
            try
            {
                parameters = parameters.Replace("[USER]", "\"" + Domain.User + "\"");
                parameters = parameters.Replace("[DB_CON]", "\"" + Domain.DbConnString + "\"");
                parameters = parameters.Replace("[PROJECT_ID]", Convert.ToString(projVerId));
                parameters = parameters.Replace("[PROJECT_CODE]", projCode);
                parameters = parameters.Replace("[PROJECT_NAME]", "\"" + projName + "\"");
                parameters = parameters.Replace("[ENV]", Domain.Environment);
                parameters = parameters.Replace("[PROJECT_VERSION_NAME]", "\"" + projVersionName + "\"");

                actualParameters = parameters;

                return FileSystemBLLReturnCode.Success;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return FileSystemBLLReturnCode.InvalidCommandLine;
            }
        }

        #endregion

        #region Check Disk Space
        public static FileSystemBLLReturnCode checkDiskSpace(String TargetPath, out string drive, out int requierdSpace)
        {
            drive = Path.GetPathRoot(TargetPath);
            var SBfreeSpace = new StringBuilder(string.Empty);
            var SBreqSpace = new StringBuilder(string.Empty);

            //Get required disk space
            SBfreeSpace.Append("select value from PE_SystemParameters where variable='RequiredDiskSpace';"); //Temporary, will be loaded to domain value
            requierdSpace = Convert.ToInt32(Domain.PersistenceLayer.FetchDataValue(SBfreeSpace.ToString(), CommandType.Text, null));
            //Get actual available disk space
            long freeSpace = 0;

            //Ella - fixed bug 1755
            FileSystemBLL.FileSystemBLLReturnCode status = FileSystemBLL.GetDriverFreeSpace(drive, out freeSpace);
            if (status.Equals(FileSystemBLL.FileSystemBLLReturnCode.Success))
            {
                freeSpace = freeSpace / 1048576;

                if (freeSpace >= requierdSpace)
                {
                    return FileSystemBLLReturnCode.Success;
                }
                else
                {
                    return FileSystemBLLReturnCode.NotEnoughSpace; // 121
                }
            }
            else
            {
                return FileSystemBLLReturnCode.DriveNotFound; //140
            }


        }

        #endregion

        #endregion

        #region Export Import

        public static int exportFilesCompleted = 0;
        public static int importFilesCompleted = 0;
        public static int totalExportImportFiles = 0;
        public static int validationsCopmpleted = 0;

        #region Export project data

        #region Utilities

        public static Domain.ErrorHandling CreateFolder(string folderFullPath)
        {
            Domain.ErrorHandling Status = new Domain.ErrorHandling();
            try
            {
                Directory.CreateDirectory(folderFullPath);
                return Status;
                }
            catch (Exception ex)
                {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                    Status.messsageId = "221";
                Status.messageParams[0] = folderFullPath;
                return Status;
            }
        }

        static Domain.ErrorHandling WriteDataTableToXML(DataTable dataTable, string parentFolderFullPath, string fileName)
        {
            Domain.ErrorHandling Status = new Domain.ErrorHandling();
            try
            {
                string xmlFileName = parentFolderFullPath +"\\" + fileName;
                dataTable.WriteXml(xmlFileName, XmlWriteMode.WriteSchema);
  
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

        //public static Domain.ErrorHandling ZipAndRenameArchive(string folderFullPath)
        //{
        //    Domain.ErrorHandling Status = new Domain.ErrorHandling();
        //    try
        //    {
        //        using (ZipFile zip = new ZipFile())
        //        {
        //            zip.AlternateEncodingUsage = ZipOption.Always; // utf-8
        //            zip.ParallelDeflateThreshold = -1;
        //            zip.BufferSize = 1048576;
        //            zip.CodecBufferSize = 1048576;
        //            zip.AddDirectory(folderFullPath);

        //            zip.Save(folderFullPath + ".project");
        //        }

        //        return Status;
        //    }
        //    catch (Exception ex)
        //    {
        //        String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
        //        Domain.SaveGeneralErrorLog(logMessage);
        //        Status.messsageId = "105";
        //        return Status;
        //    }
        //}

        public static void DeleteDirectoryRecursive(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    //Delete all files from the Directory
                    foreach (string file in Directory.GetFiles(path))
                    {
                        FileAttributes attrib = File.GetAttributes(file);

                        if ((attrib & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                            File.SetAttributes(file, attrib & ~FileAttributes.ReadOnly);
                        File.Delete(file);
                    }
                    //Delete all child Directories
                    foreach (string directory in Directory.GetDirectories(path))
                    {
                        DeleteDirectoryRecursive(directory);
                    }
                    //Delete a Directory
                    Directory.Delete(path);
                }
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralWarningLog(logMessage);
                string warning = "Warning: failed to delete folder " + path;
                Domain.SaveGeneralWarningLog(warning);
            }
        }

        public static Domain.ErrorHandling CopyFolderWithFilesAndSubfolders(string sourcePath, string targetPath, ThreadStart t)
        {
            Domain.ErrorHandling Status = new Domain.ErrorHandling();
            try
            {
                foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
                }

                 //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                {
                    File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);

                    //progress bar
                    try
                    {
                        if (t != null)
                        {
                            exportFilesCompleted++;
                            Thread thread = new Thread(t);
                            thread.Start();
                            thread.Join();
                        }
                    }
                    catch (Exception e)
                    {
                        String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                        Domain.SaveGeneralErrorLog(logMessage);
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
        #endregion

        #region Export PE tables

        public static Domain.ErrorHandling ExportProjectDetailsToXml(long projectId, string fileDestinationFolder, string XmlFileName)
        {
            Domain.ErrorHandling Status = new Domain.ErrorHandling();
            try
            {
                string listOfBranchIds = string.Empty;
                HierarchyBLL.HierarchyBLLReturnCode status = HierarchyBLL.GetHierarchyBranchIdsProjectId(projectId, out listOfBranchIds);
                if (status != HierarchyBLL.HierarchyBLLReturnCode.Success)
                {
                    Status.messsageId = "220";
                    Status.messageParams[0] = "PE_Hierarchy";
                    return Status;
                }

                if (!string.IsNullOrEmpty(listOfBranchIds))
                {
                    listOfBranchIds = listOfBranchIds + "," + projectId.ToString();
                }
                else
                {
                    listOfBranchIds = projectId.ToString();
                }
                DataTable projectHierarchyDetails = HierarchyBLL.GetNodeDataTable(listOfBranchIds);
                if (projectHierarchyDetails == null || projectHierarchyDetails.Rows.Count == 0) //Failed to retrieve project properties
                {
                    Status.messsageId = "220";
                    Status.messageParams[0] = "PE_Hierarchy";
                    return Status;
                }
                //save data table to xml file
                Status = FileSystemBLL.WriteDataTableToXML(projectHierarchyDetails, fileDestinationFolder, XmlFileName);
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

        public static Domain.ErrorHandling ExportVersionContentsDetailsToXml(string versionName, long projectId, string fileDestinationFolder, string XmlFileName)
        {
            Domain.ErrorHandling Status = new Domain.ErrorHandling();
            try
            {
               //get version contents details
                DataTable activeVersionContentsDataTable = ContentBLL.GetProjectVersionContentsByNameAndProjectId(versionName, projectId);
                if (activeVersionContentsDataTable == null) //Failed to retrieve version contents properties
                {
                    Status.messsageId = "220";
                    Status.messageParams[0] = "PE_VersionContent";
                    return Status;
                }
                //save data table to xml file
                Status = FileSystemBLL.WriteDataTableToXML(activeVersionContentsDataTable, fileDestinationFolder, XmlFileName);
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

        public static Domain.ErrorHandling ExportVersionDetailsToXml(long projectId, string fileDestinationFolder, string XmlFileName)
        {
            Domain.ErrorHandling Status = new Domain.ErrorHandling();
            try
            {
                //get project's active version details
                DataTable activeVersionDataTable = VersionBLL.GetActiveVersionDataTable(projectId);
                if (activeVersionDataTable == null) //Failed to retrieve version properties
                {
                    Status.messsageId = "220";
                    Status.messageParams[0] = "PE_Version";
                    return Status;
                }
                //save data table to xml file
                Status = FileSystemBLL.WriteDataTableToXML(activeVersionDataTable, fileDestinationFolder, XmlFileName);
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

        #endregion Export PE tables

        #region Export CM tables

        public static Domain.ErrorHandling ExportContentTreeToXml(List<int> folderIds, string fileDestinationFolder, string XmlFileName)
        {
            Domain.ErrorHandling Status = new Domain.ErrorHandling();
            try
            {
                DataTable contentTreeDataTable = CMFolderBLL.GetContentTreeDataTable(folderIds);
                if (contentTreeDataTable == null) //Failed to retrieve version properties
                {
                    Status.messsageId = "220";
                    Status.messageParams[0] = "ContentTree";
                    return Status;
                }
                //save data table to xml file
                Status = FileSystemBLL.WriteDataTableToXML(contentTreeDataTable, fileDestinationFolder, XmlFileName);
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

        public static Domain.ErrorHandling ExportContentsToXml(List<int> contentIds, string fileDestinationFolder, string XmlFileName)
        {
            Domain.ErrorHandling Status = new Domain.ErrorHandling();
            try
            {
                DataTable contentsDataTable = CMContentBLL.GetContentDataTable(contentIds);
                if (contentsDataTable == null) //Failed to retrieve version properties
                {
                    Status.messsageId = "220";
                    Status.messageParams[0] = "Content";
                    return Status;
                }
                //save data table to xml file
                Status = FileSystemBLL.WriteDataTableToXML(contentsDataTable, fileDestinationFolder, XmlFileName);
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

        public static Domain.ErrorHandling ExportContentVersionsToXml(List<int> versionIds, string fileDestinationFolder, string XmlFileName)
        {
            Domain.ErrorHandling Status = new Domain.ErrorHandling();
            try
            {
                DataTable contentVersionDataTable = CMVersionBLL.GetContentVersionDataTable(versionIds);
                if (contentVersionDataTable == null) //Failed to retrieve version properties
                {
                    Status.messsageId = "220";
                    Status.messageParams[0] = "ContentVersion";
                    return Status;
                }
                //save data table to xml file
                Status = FileSystemBLL.WriteDataTableToXML(contentVersionDataTable, fileDestinationFolder, XmlFileName);
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

        public static Domain.ErrorHandling ExportContentTreeUserGroupTypesToXml(List<int> folderIds, string fileDestinationFolder, string XmlFileName)
        {
            Domain.ErrorHandling Status = new Domain.ErrorHandling();
            try
            {
                DataTable ContentTreeUserGroupTypeDataTable = CMFolderBLL.GetContentTreeUserGroupTypeDataTable(folderIds);
                if (ContentTreeUserGroupTypeDataTable == null) //Failed to retrieve version properties
                {
                    Status.messsageId = "220";
                    Status.messageParams[0] = "ContentTreeUserGroupType";
                    return Status;
                }
                //save data table to xml file
                Status = FileSystemBLL.WriteDataTableToXML(ContentTreeUserGroupTypeDataTable, fileDestinationFolder, XmlFileName);
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

        public static Domain.ErrorHandling ExportContentVersionFilesToXml(List<int> versionIds, string fileDestinationFolder, string XmlFileName)
        {
            Domain.ErrorHandling Status = new Domain.ErrorHandling();
            try
            {
                DataTable versionFilesDataTable = CMVersionBLL.GetContentVersionFileDataTable(versionIds);
                if (versionFilesDataTable == null) //Failed to retrieve version properties
                {
                    Status.messsageId = "220";
                    Status.messageParams[0] = "ContentVersionFile";
                    return Status;
                }
                //save data table to xml file
                Status = FileSystemBLL.WriteDataTableToXML(versionFilesDataTable, fileDestinationFolder, XmlFileName);
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

        public static Domain.ErrorHandling ExportContentVersionVersionLinksToXml(List<int> versionIds, string fileDestinationFolder, string XmlFileName)
        {
            Domain.ErrorHandling Status = new Domain.ErrorHandling();
            try
            {
                DataTable allLinkedVersions = CMVersionBLL.GetContentVersionVersionLinksDataTable(versionIds);
                if (allLinkedVersions == null) //Failed to retrieve
                {
                    Status.messsageId = "220";
                    Status.messageParams[0] = "ContentVersionVersionLink";
                    return Status;
                }
                //save data table to xml file
                Status = FileSystemBLL.WriteDataTableToXML(allLinkedVersions, fileDestinationFolder, XmlFileName);
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

        #endregion Export CM Tables

        #region Export System Parameters tables

        public static Domain.ErrorHandling ExportCMSystemParametersToXml(string fileDestinationFolder, string XmlFileName)
        {
            Domain.ErrorHandling Status = new Domain.ErrorHandling();
            try
            {
                DataTable SystemParameters = Domain.CMSystemParameters();
                if (SystemParameters == null || SystemParameters.Rows.Count == 0) //Failed to retrieve 
                {
                    Status.messsageId = "220";
                    Status.messageParams[0] = "SystemParameters";
                    return Status;
                }
                //save data table to xml file
                Status = FileSystemBLL.WriteDataTableToXML(SystemParameters, fileDestinationFolder, XmlFileName);
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

        public static Domain.ErrorHandling ExportPESystemParametersToXml(string fileDestinationFolder, string XmlFileName)
        {
            Domain.ErrorHandling Status = new Domain.ErrorHandling();
            try
            {
                //get version contents details
                DataTable PESystemParametersDataTable = Domain.PESystemParameters();
                if (PESystemParametersDataTable == null) //Failed to retrieve 
                {
                    Status.messsageId = "220";
                    Status.messageParams[0] = "PE_SystemParameters";
                    return Status;
                }
                //save data table to xml file
                Status = FileSystemBLL.WriteDataTableToXML(PESystemParametersDataTable, fileDestinationFolder, XmlFileName);
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

        #endregion Export System Parameters tables

        #endregion Export project data

        #region Import Project data

        public static DataTable ImportDataFromXml(string importParentFolderFullPath, string subFolderName, string dataXmlFileName)
        {
            try
            {
                string fileFullPath = importParentFolderFullPath + "\\" +
                                                    subFolderName + "\\" +
                                                    dataXmlFileName;
                DataTable dt = ReadDataTableFromXML(fileFullPath);
                return dt;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return null;
            }
        }

        #region Utilities
        //public static Domain.ErrorHandling UnzipArchive(string archiveFileFullPath, string extractFolderPath)
        //{
        //    Domain.ErrorHandling Status = new Domain.ErrorHandling();
        //    try
        //    {
        //        using (ZipFile zip = ZipFile.Read(archiveFileFullPath))
        //        {
        //            zip.AlternateEncodingUsage = ZipOption.Always;  // utf-8
        //            zip.ExtractAll(extractFolderPath, ExtractExistingFileAction.OverwriteSilently);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
        //        Domain.SaveGeneralErrorLog(logMessage);
        //        Status.messsageId = "226";
        //        Status.messageParams[0] = archiveFileFullPath;
        //    }
        //    return Status;
        //}

        public static Domain.ErrorHandling ValidateInputFolder(string archiveFileFullPath)
        {
            Domain.ErrorHandling Status = new Domain.ErrorHandling();
            try
            {
                if (!Directory.Exists(archiveFileFullPath + "\\" + ImportProjectBLL.archiveCMFolderName + "\\" + ImportProjectBLL.archiveCMFilesFolderName) ||
                    !Directory.Exists(archiveFileFullPath + "\\" + ImportProjectBLL.archiveCMFolderName) ||
                    !Directory.Exists(archiveFileFullPath + "\\" + ImportProjectBLL.archiveCMFolderName + "\\" + ImportProjectBLL.archiveIconsFolderName) ||
                    !Directory.Exists(archiveFileFullPath + "\\" + ImportProjectBLL.archiveSPFolderName) ||
                    !Directory.Exists(archiveFileFullPath + "\\" + ImportProjectBLL.archivePEFolderName))
                {
                    Status.messsageId = "226";
                    Status.messageParams[0] = archiveFileFullPath;
                    return Status;
                }

                return Status;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Status.messsageId = "226";
                Status.messageParams[0] = archiveFileFullPath;
                return Status;
            }
        }

        static DataTable ReadDataTableFromXML(string xmlFileName)
        {
            DataTable dt = new DataTable();
            try
            {
               dt.ReadXml(xmlFileName);

                return dt;
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                logMessage = "Failed to read data from xml file." + "\n" + logMessage;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("Failed to read data from xml file.");
            }
        }

        #endregion Utilities

        #endregion Import Project Data

        #endregion Export Import

    }
}
