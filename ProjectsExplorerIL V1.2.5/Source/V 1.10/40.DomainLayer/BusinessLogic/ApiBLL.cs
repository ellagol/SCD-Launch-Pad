using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ATSBusinessObjects;
using ATSBusinessObjects.ContentMgmtModels;
using ATSDomain;
using ResourcesProvider;
using TraceExceptionWrapper;
using System.Collections;
using System.Collections.ObjectModel;


namespace ATSBusinessLogic
{
    public  class ApiBLL
    {

        #region Bootstrap (ExecuteContent-2) - ella

        #region exception handler variables
        public static List<string> TraceExceptionParameterValue = new List<string>();

        static HierarchyBLL.HierarchyBLLReturnCode getProjResult = HierarchyBLL.HierarchyBLLReturnCode.Success;
        static CertificateBLL.CertificateBLLReturnResult wsCertReturnResult = CertificateBLL.CertificateBLLReturnResult.Success;
        static ContentBLL.CMApiReturnCode callCMResult = ContentBLL.CMApiReturnCode.Success;
        static CertificateBLL.CertificateBLLReturnResult projectCertResult = CertificateBLL.CertificateBLLReturnResult.Success;
        static VersionBLL.VersionBLLReturnCode getActivatedVersionResult = VersionBLL.VersionBLLReturnCode.Success;
        static VersionBLL.VersionBLLReturnCode getVersionInfoResult = VersionBLL.VersionBLLReturnCode.Success;
        static FileSystemBLL.FileSystemBLLReturnCode copyFilesStatus = FileSystemBLL.FileSystemBLLReturnCode.Success;
        static FileSystemBLL.FileSystemBLLReturnCode executeFilesStatus = FileSystemBLL.FileSystemBLLReturnCode.Success;
        static VersionBLL.VersionBLLReturnCode getOpValuestatus = VersionBLL.VersionBLLReturnCode.Success;
        #endregion

        #region Main function API

        public static void BootstrapContentExecution(string projectFullPath, string projectVersionName, string contentName, string connString, string usrName, string environmentName)
        {
            try
            {    
                long projectId = (long)HierarchyBLL.HierarchyBLLReturnCode.NoProjectId;

                Domain.DomainInitForAPI(connString);

                //CM and RM API connection settings
                SqlConnection connectionString = new SqlConnection(connString);
                ResourcesProviderApi rmAPI = new ResourcesProviderApi(connectionString);

                //Setting parameters values for exception handler
                TraceExceptionParameterValue.Add("Project Full Path: " + projectFullPath);
                TraceExceptionParameterValue.Add("Project Version Name: " + projectVersionName);
                TraceExceptionParameterValue.Add("Content Name: " + contentName);
                TraceExceptionParameterValue.Add("Connection String: " + connString);
                TraceExceptionParameterValue.Add("User Name: " + usrName);
                TraceExceptionParameterValue.Add("Environment Name: " + environmentName);
                TraceExceptionParameterValue.Add("WorkStation: " + System.Environment.MachineName);

                try
                {
                    //Retrive project Id by project full path 
                    getProjResult = HierarchyBLL.GetNodeIdByFullPath(projectFullPath, out projectId);
                    TraceExceptionParameterValue.Add("Project Id: " + projectId);

                    if (getProjResult != HierarchyBLL.HierarchyBLLReturnCode.Success) //No Project Id to work with
                    {
                        ExceptionHandler();
                    }

                    //Got ProjectId --> Continue
                    Dictionary<int, CMFolderModel> cmFolders = new Dictionary<int, CMFolderModel>();
                    Dictionary<int, CMContentModel> cmContents = new Dictionary<int, CMContentModel>();
                    Dictionary<int, CMVersionModel> cmVersions = new Dictionary<int, CMVersionModel>();

                    //Get Contents tree from CM
                    connectionString.Open();
                    callCMResult = ContentBLL.GetContentsTree(out cmFolders, out cmContents, out cmVersions);
                    connectionString.Close();

                    if (callCMResult != ContentBLL.CMApiReturnCode.Success) //Failed to get Contents tree
                    {
                        ExceptionHandler();
                    }

                    //Got Contents tree --> Continue                
                    long conVersionToActivate = -1;
                    Dictionary<long, int> activeVersionContents = new Dictionary<long, int>();
                    string versionTargetPath = string.Empty;
                    long prVersionId = -1;

                    //Retrieve project version info
                    getActivatedVersionResult = VersionBLL.GetActivatedCntVersionIdByCntNameAndPrVersionName(contentName, projectVersionName, projectId, cmContents, cmVersions, out conVersionToActivate);
                    getVersionInfoResult = VersionBLL.GetVersionInfoByProjctIdAndVersionName(projectId, projectVersionName, out activeVersionContents, out versionTargetPath, out prVersionId);

                    ////Get version target directory drive name
                    //string drive = Path.GetPathRoot(versionTargetPath);

                    //TraceExceptionParameterValue.Add("Drive: " + drive);
                    TraceExceptionParameterValue.Add("Activated Content Version Id: " + conVersionToActivate);
                    TraceExceptionParameterValue.Add("Project Version Id: " + prVersionId);
                    TraceExceptionParameterValue.Add("Project Version Target Path: " + versionTargetPath);

                    if (getActivatedVersionResult != VersionBLL.VersionBLLReturnCode.Success || getVersionInfoResult != VersionBLL.VersionBLLReturnCode.Success)
                    {
                        ExceptionHandler(); //Failed to retrive version info by input parameters
                    }

                    //Get version target directory drive name
                    if (String.IsNullOrEmpty(versionTargetPath) || String.IsNullOrWhiteSpace(versionTargetPath))
                    {
                        copyFilesStatus = FileSystemBLL.FileSystemBLLReturnCode.DriveNotFound;
                        ExceptionHandler();
                    }

                    string drive = Path.GetPathRoot(versionTargetPath);
                    TraceExceptionParameterValue.Add("Drive: " + drive);

                    //Check that executable file is specified for activated Content Version
                    string commandLine = cmVersions[Convert.ToInt32(conVersionToActivate)].RunningString;
                    if (commandLine.Equals(string.Empty))
                    {
                        executeFilesStatus = FileSystemBLL.FileSystemBLLReturnCode.exeNotSpecified; //No executable file for activated Content Version
                        ExceptionHandler();
                    }

                    //Check whether certificate is required for activated content:
                    int versionParentId = cmVersions[Convert.ToInt32(conVersionToActivate)].ParentID;
                    bool ContentCertFreeFlag = cmContents[versionParentId].CertificateFree;

                    if (!ContentCertFreeFlag)
                    {
                        // Verify whether project requires certificate for execution
                        List<string> projectCertList = new List<string>();
                        projectCertResult = CertificateBLL.GetProjectCertificatesByProjectId(projectId, out projectCertList);
                        if (projectCertResult != CertificateBLL.CertificateBLLReturnResult.Success)
                        {
                            ExceptionHandler(); //Failed to retrieve project certificates
                        }

                        //Succeeded to access Project certificate info --> Continue
                        if (projectCertList.Count > 0)
                        {
                            Dictionary<String, Certificate> stationCertificates = new Dictionary<String, Certificate>();

                            //Retrieve Machine certificates list to compare to project certificates list
                            connectionString.Open();
                            wsCertReturnResult = CertificateBLL.GetCertificatesByWorkstationId(rmAPI, out stationCertificates);
                            connectionString.Close();

                            if (wsCertReturnResult != CertificateBLL.CertificateBLLReturnResult.Success)
                            {
                                ExceptionHandler(); //Failed to retrieve list of workstation certificates
                            }

                            //Compare lists of Project and Machine certificates
                            List<String> missingCerts = new List<string>();
                            bool IsCertified = IsWorkstationCertified(projectCertList, stationCertificates, out missingCerts);

                            if (!IsCertified)
                            {
                                TraceExceptionParameterValue.Add("Missing certificates: ");
                                TraceExceptionParameterValue.AddRange(missingCerts);
                                throw new TraceException("Current Workstation is not certified to execute this content.", TraceExceptionParameterValue, "PE");
                            }
                        }
                    }

                    //check disk space, copy content files and execute
                    long freeDiskSpace = -1;
                    copyFilesStatus = FileSystemBLL.GetDriverFreeSpace(drive, out freeDiskSpace);
                    TraceExceptionParameterValue.Add("Available Disk Space, MB: " + freeDiskSpace);

                    if (copyFilesStatus != FileSystemBLL.FileSystemBLLReturnCode.Success)
                    {
                        ExceptionHandler(); //Failed to retrieve free disk space information for Drive specified in Project Version target directory
                    }

                    int reqDiskSpace = 0;
                    getOpValuestatus = VersionBLL.GetRequiredDiskspace(out reqDiskSpace);
                    TraceExceptionParameterValue.Add("Required Disk Space, MB: " + reqDiskSpace);

                    if (getOpValuestatus != VersionBLL.VersionBLLReturnCode.Success)
                    {
                        ExceptionHandler(); //Failed to retrieve required disk space value from operational table
                    }

                    freeDiskSpace = (freeDiskSpace / 1048576);

                    if (reqDiskSpace > freeDiskSpace) //Not enough disk space on Target Directory drive
                    {
                        copyFilesStatus = FileSystemBLL.FileSystemBLLReturnCode.NotEnoughSpace;
                        ExceptionHandler();
                    }

                    //All validations succeeded --> Continue to Files Copy and Content execution
                    string faildFiles = "";
                    copyFilesStatus = FileSystemBLL.CopyContentVersionsFilesToLocalWithFileList(cmVersions, activeVersionContents, versionTargetPath, out faildFiles);

                    //Failed to copy contents files to local Machine
                    if (copyFilesStatus != FileSystemBLL.FileSystemBLLReturnCode.Success)
                    {
                        TraceExceptionParameterValue.Add("Failed to copy file: " + faildFiles);
                        ExceptionHandler();
                    }

                    executeFilesStatus = FileSystemBLL.ExecuteRunningString(usrName, environmentName, connString, projectId, prVersionId, versionTargetPath, commandLine);

                    //Failed to execute Content executable file
                    if (executeFilesStatus != FileSystemBLL.FileSystemBLLReturnCode.Success)
                    {
                        ExceptionHandler();
                    }
                }
                catch (TraceException trex)
                {
                    throw trex;
                    //throw new TraceException(trex.ApplicationErrorID, TraceExceptionParameterValue, "PE");
                }
            }
            catch (TraceException ex)
            {
                ex.AddTrace(new StackFrame(1, true));
                throw ex;
                //throw new Exception(ex.Message); //Common exception
            }
        }
        #endregion

        private static bool IsWorkstationCertified(List<String> certificates, Dictionary<String, Certificate> certificateStation, out List<String> missingCertificates)
        {
            Boolean isCertifiedFlag = true;
            missingCertificates = new List<string>(); //List of missing certificates

            foreach (String c in certificates)
                if (!certificateStation.Keys.Contains(c))
                {
                    isCertifiedFlag = false;
                    missingCertificates.Add(c); //Adding missing certificates to the list
                }
            return isCertifiedFlag;
        }

        private static void ExceptionHandler()
        {
            if (getProjResult != HierarchyBLL.HierarchyBLLReturnCode.Success)
            {
                if (getProjResult == HierarchyBLL.HierarchyBLLReturnCode.CommonException)
                {
                    throw new TraceException("Failed to retrieve ProjectId by full path", TraceExceptionParameterValue, "PE");
                }
                else
                {
                    switch (getProjResult)
                    {
                        case HierarchyBLL.HierarchyBLLReturnCode.NoProjectId:
                            string errorId = "Project with given path does not exist";                            
                            throw new TraceException(errorId, TraceExceptionParameterValue, "PE");

                        case HierarchyBLL.HierarchyBLLReturnCode.DBException:
                            throw new TraceException("DB Error occured when retrieving project Id by path", TraceExceptionParameterValue, "PE");
                        default:
                            throw new TraceException("Project with given path does not exist", TraceExceptionParameterValue, "PE");
                    }
                }
            }
            if (getActivatedVersionResult != VersionBLL.VersionBLLReturnCode.Success)
            {
                switch (getActivatedVersionResult)
                {
                    case VersionBLL.VersionBLLReturnCode.VersionToActivateNotFound:
                        throw new TraceException("Content version is not associated to project", TraceExceptionParameterValue, "PE");

                    case VersionBLL.VersionBLLReturnCode.DBException:
                        throw new TraceException("Database connection error occured when trying to retrieve Project versions.", TraceExceptionParameterValue, "PE");

                    case VersionBLL.VersionBLLReturnCode.MoreThanOneVerForCntName:
                        throw new TraceException("Found more than one Content Version belonging to the Content", TraceExceptionParameterValue, "PE");

                    default:
                        throw new TraceException("Failed to retrieve activated Content Version id", TraceExceptionParameterValue, "PE");
                }
            }
            if (callCMResult != ContentBLL.CMApiReturnCode.Success)
            {
                switch (callCMResult)
                {
                    case ContentBLL.CMApiReturnCode.ConnectionError:
                        throw new TraceException("Failed to connect to CM Database", TraceExceptionParameterValue, "PE");

                    case ContentBLL.CMApiReturnCode.EmptyCMTree:
                        throw new TraceException("CM API returned empty tree", TraceExceptionParameterValue, "PE");

                    case ContentBLL.CMApiReturnCode.GetTreeObjectsException:
                        throw new TraceException("CM API returned exception", TraceExceptionParameterValue, "PE");

                    default:
                        throw new TraceException("Failed to retrieve Contents tree (GetTreeObjects)", TraceExceptionParameterValue, "PE");
                }
            }
            if (projectCertResult != CertificateBLL.CertificateBLLReturnResult.Success)
            {
                switch (projectCertResult)
                {
                    case CertificateBLL.CertificateBLLReturnResult.DBConnectionError:
                        throw new TraceException("Database connection error occured when trying to retrieve project certificates", TraceExceptionParameterValue, "PE");

                    case CertificateBLL.CertificateBLLReturnResult.RMCommonException:
                        throw new TraceException("Failed to retrieve project certificates", TraceExceptionParameterValue, "PE");

                    default:
                        throw new TraceException("Failed to retrieve project certificates", TraceExceptionParameterValue, "PE");
                }
            }
            if (wsCertReturnResult != CertificateBLL.CertificateBLLReturnResult.Success)
            {
                switch (wsCertReturnResult)
                {
                    case CertificateBLL.CertificateBLLReturnResult.RMException:
                        throw new TraceException("Error occured when trying to retrieve Work Station certificates", TraceExceptionParameterValue, "PE");

                    case CertificateBLL.CertificateBLLReturnResult.RMCommonException:
                        throw new TraceException("Error occured when trying to retrieve Work Station certificates", TraceExceptionParameterValue, "PE");

                    default:
                        throw new TraceException("Error occured when trying to retrieve Work Station certificates", TraceExceptionParameterValue, "PE");
                }
            }
            if (copyFilesStatus != FileSystemBLL.FileSystemBLLReturnCode.Success)
                switch (copyFilesStatus)
                {
                    case FileSystemBLL.FileSystemBLLReturnCode.DriveNotFound:
                        throw new TraceException("Drive specified in Project Version Target Path not found or unable to get Drive info.", TraceExceptionParameterValue, "PE");

                    case FileSystemBLL.FileSystemBLLReturnCode.FileNotFound:
                        throw new TraceException("One of the Content versions files is not found under source directory.", TraceExceptionParameterValue, "PE");

                    case FileSystemBLL.FileSystemBLLReturnCode.NotEnoughSpace:
                        throw new TraceException("There is not enough disk space on target drive", TraceExceptionParameterValue, "PE");

                    case FileSystemBLL.FileSystemBLLReturnCode.CommonException:
                        throw new TraceException("Failed to copy content version files from source directory to target.", TraceExceptionParameterValue, "PE");

                    case FileSystemBLL.FileSystemBLLReturnCode.TargetDirectoryNotFound:
                        throw new TraceException("Target directory is not specified correctly.", TraceExceptionParameterValue, "PE");

                    default:
                        throw new TraceException("Failed to copy content version files from source directory to target.", TraceExceptionParameterValue, "PE");
                }
            if (executeFilesStatus != FileSystemBLL.FileSystemBLLReturnCode.Success)
            {
                switch (executeFilesStatus)
                {
                    case FileSystemBLL.FileSystemBLLReturnCode.ExecutableNotFound:
                        throw new TraceException("Executable file is not found under specified directory", TraceExceptionParameterValue, "PE");

                    case FileSystemBLL.FileSystemBLLReturnCode.UnableToExecute:
                        throw new TraceException("Unable to execute specified file. Check file type and permissions", TraceExceptionParameterValue, "PE");

                    case FileSystemBLL.FileSystemBLLReturnCode.CommonException:
                        throw new TraceException("Failed to execute file specified in the command line", TraceExceptionParameterValue, "PE");

                    case FileSystemBLL.FileSystemBLLReturnCode.exeNotSpecified:
                        throw new TraceException("Command line is empty.", TraceExceptionParameterValue, "PE");

                    default:
                        throw new TraceException("Failed to execute file specified in the command line.", TraceExceptionParameterValue, "PE");

                }
            }
            if (getOpValuestatus != VersionBLL.VersionBLLReturnCode.Success)
            {
                switch (getOpValuestatus)
                {
                    case VersionBLL.VersionBLLReturnCode.DBException:
                        throw new TraceException("Unable to retrieve required disk space value from operational table.", TraceExceptionParameterValue, "PE");

                    case VersionBLL.VersionBLLReturnCode.CommonException:
                        throw new TraceException("Unable to retrieve required disk space value from operational table.", TraceExceptionParameterValue, "PE");

                    default:
                        throw new TraceException("Unable to retrieve required disk space value from operational table.", TraceExceptionParameterValue, "PE");
                }
            }
        }

         #endregion

        #region GetProjectsByContentVersionID

        //public static Dictionary<int, HierarchyModel> GetProjectsByContentVersionID(int contentVersionID, string ConnStr)
            public static Dictionary<int, HierarchyModel> GetProjectsByContentVersionID(int contentVersionID)
        {
            Dictionary<int, HierarchyModel> ContentHirearchy = new Dictionary<int, HierarchyModel>();
            try
            {
                //if (ConnStr != null && ConnStr != string.Empty)
                //{
                //    // Initialize the Domain
                //    //Domain.DomainInitForAPI(ConnStr);
                //}
                StringBuilder SB = new StringBuilder();
                SB.Append("SELECT DISTINCT (CASE H1.NodeType WHEN 'P' THEN H1.Id WHEN 'T' THEN H1.Id WHEN 'G' THEN H2.Id END) AS Id ");
                    SB.Append(" FROM PE_Hierarchy AS H1 INNER JOIN PE_Version AS V ON H1.Id = V.HierarchyId ");
                SB.Append(" LEFT OUTER JOIN PE_Hierarchy AS H2 ON H1.Id = H2.GroupId WHERE EXISTS ");
                SB.Append (" (SELECT 1 FROM PE_VersionContent AS vc WHERE (VersionId = V.VersionId) AND (ContentVersionId = '" + contentVersionID +"'))");

                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);
                if (ResTable != null)
                {
                    foreach (DataRow DataRow in ResTable.Rows)
                    {
                        long HierarchyID = Convert.ToInt32(DataRow["Id"]);
                        HierarchyModel HM = HierarchyBLL.GetHierarchyRow(HierarchyID);
                        HM.VM = VersionBLL.GetVersionRow(HierarchyID);
                        HM.GetAllVersions = VersionBLL.GetVersion(HierarchyID);
                        //HM.TreeHeader = VersionBLL.getParentName(HierarchyID.ToString());
                        //Performance
                        string fullPath = string.Empty;
                        HierarchyBLL.HierarchyBLLReturnCode status = HierarchyBLL.GetProjectFullPathByProjectId(HierarchyID, out fullPath);
                        HM.TreeHeader = fullPath;
                        ContentHirearchy.Add(Convert.ToInt32(HierarchyID), HM);
                    }
                }

                return ContentHirearchy;
            }
            catch (Exception e)
            {

                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                return ContentHirearchy;
            }
        }
        #endregion GetProjectsByContentVersionID

        #region Get Project By ProjectCode - Eli to Test API

        public static string WhoModifiedProjectCode(string ProjectCode, string ConnectionString)
        {
            // Initialize the Domain
            Domain.DomainInitForAPI(ConnectionString);
            // Build The Query String
            string QryStr = ("SELECT TOP 1 * FROM PE_Hierarchy WHERE ProjectCode = '" + ProjectCode + "'");
            // Fetch the DataTable from the database
            try
            {
                DataTable DT = Domain.PersistenceLayer.FetchDataTable(QryStr, CommandType.Text, null);
                if (DT != null)
                {
                    if (DT.Rows.Count == 0)
                    {
                        return "Error: Project Code not found";
                    }
                    else
                    {
                        DataRow DataRow = DT.Rows[0];
                        string User = (string)DataRow["LastUpdateUser"];
                        return User;
                    }
                }
                else
                {
                    return "Error: Failed to access DB";
                }
            }
            catch (Exception ex)
            {
                return ("Error: Failed to execute command. Inner Error: " + Environment.NewLine + "Message: " + ex.Message + Environment.NewLine + "Source: " + ex.Source + Environment.NewLine + "Stack Trace: " + Environment.NewLine + ex.StackTrace);
            }
        }

        #endregion

        #region Extractor API

        public static Dictionary<string, string> GetProjectActiveVersionFilesPaths(int projectId, string connectionString)
        {
            Dictionary<string, string> activeVersionFilesPaths = new Dictionary<string, string>();
            try
            {
                if (projectId < 0)
                {
                    throw new Exception("Invalid Project Id.");
                }

                if (string.IsNullOrEmpty(connectionString) || string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new Exception("Invalid Connection String.");
                }

                Domain.DomainInitForAPI(connectionString);

                Domain.ErrorHandling Status = GetActiveVersionFilesPaths(projectId, out activeVersionFilesPaths);
                if (Status.messsageId != string.Empty)
                {
                    string innerExceptionMessage = Status.messsageId;
                    foreach (var prm in Status.messageParams)
                    {
                        if (prm != null)
                        {
                            innerExceptionMessage = innerExceptionMessage + "\n" + Convert.ToString(prm);
                        }
                    }
                    Exception innerException = new Exception(innerExceptionMessage);
                    throw new Exception("Internal error occurred.", innerException);
                }

                return activeVersionFilesPaths;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message + "\n" + ex.StackTrace;
                throw new Exception(errorMessage, ex.InnerException);
            }
        }

        static Domain.ErrorHandling GetActiveVersionFilesPaths(int projectId, out Dictionary<string, string> activeVersionFilesPaths)
        {
            activeVersionFilesPaths = new Dictionary<string, string>();

            Domain.ErrorHandling Status = new Domain.ErrorHandling();
            ContentExecutionBLL.ErrorHandling result = new ContentExecutionBLL.ErrorHandling();

            Dictionary<int, CMFolderModel> folders = new Dictionary<int, CMFolderModel>();
            Dictionary<int, CMContentModel> contents = new Dictionary<int, CMContentModel>();
            Dictionary<int, CMVersionModel> versions = new Dictionary<int, CMVersionModel>();

            try
            {
                ObservableCollection<ContentModel> activeContents = new ObservableCollection<ContentModel>();

                Status = HierarchyBLL.GetProjectActiveContents(projectId, out activeContents);
                if (Status.messsageId != string.Empty)
                {
                    Status.messsageId = "Failed to retrieve project's contents.";
                    Status.messageParams[0] = "Project Id: " + projectId.ToString();
                    return Status;
                }

                if (activeContents == null || activeContents.Count == 0)
                {
                    return Status;
                }

                result = ContentExecutionBLL.GetCMSubTree(activeContents, out folders, out contents, out versions);
                if (result.errorId != string.Empty)
                {
                    Status.messsageId = "Failed to retrieve Contents sub tree.";
                    Status.messageParams = result.errorParams;
                    return Status;
                }

                Dictionary<long, int> allProjectCVSorted = new Dictionary<long, int>();
                result = ContentExecutionBLL.GetAllProjectContentsListSorted(versions, contents, activeContents,
                                                                             out allProjectCVSorted);
                if (result.errorId != string.Empty)
                {
                    if (result.errorId != "158")
                    {
                        Status.messsageId = "Failed to get all project's content versions.";
                        Status.messageParams = result.errorParams;
                    }
                    else
                    {
                        Status.messsageId = "There is a conflict in linked versions.";
                        Status.messageParams[0] = "Content: " + result.errorParams[0];
                        Status.messageParams[1] = "Content version: " + result.errorParams[1];
                    }
                    return Status;
                }

                List<CMContentFileModel> distinctFilesModelList = new List<CMContentFileModel>();
                ContentBLL.CMApiReturnCode rc = ContentBLL.createAllFilesSortedList(allProjectCVSorted, versions, out distinctFilesModelList);

                if (distinctFilesModelList != null)
                {
                    foreach (CMContentFileModel cf in distinctFilesModelList)
                    {
                        activeVersionFilesPaths[cf.FileFullPath] = cf.FileRelativePath;
                    }
                }

                return Status;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message + "\n" + ex.StackTrace;
                Status.messsageId = "Internal error occurred.";
                Status.messageParams[0] = errorMessage;
                return Status;
            }

        }
        #endregion

    }
}
