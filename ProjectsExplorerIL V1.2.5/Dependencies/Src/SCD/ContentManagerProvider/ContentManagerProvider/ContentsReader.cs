using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ContentManagerDefinitions;
using ContentManagerProvider.General;
using TraceExceptionWrapper;
using DatabaseProvider;

namespace ContentManagerProvider
{
    internal class ContentsReader
    {
        private Dictionary<String, ContentType> ContentTypeList { get; set; }
        private Dictionary<String, ContentStatus> ContentStatusList { get; set; }
        private Dictionary<String, UserGroupType> UserGroupTypeList { get; set; }

        internal ContentsReader()
        {
            ContentTypeList = null;
            ContentStatusList = null;
        }

        internal Dictionary<int, Content> GetContentsList(List<int> contentsID, bool fullData)
        {
            UpdateReferenceList();
            return GetContentsDictionaryByID(contentsID, null, fullData);
        }

        internal Dictionary<int, List<VersionKey>> GetContentVersionLinksWithLock()
        {
            int version;
            int versionLink;
            int versionLinkParentContent = 0;
            DataTable dt = GetVersionLinksWithLock();
            Dictionary<int, List<VersionKey>> versionLinks = new Dictionary<int, List<VersionKey>>();

            foreach (DataRow row in dt.Rows)
            {
                version = DBprovider.GetIntParam(row, "Version");
                versionLink = DBprovider.GetIntParam(row, "VersionLink");
                versionLinkParentContent = DBprovider.GetIntParam(row, "VersionLinkParentContent");

                if(versionLinks.ContainsKey(version))
                    versionLinks[version].Add(new VersionKey(){ContentID=versionLinkParentContent, VersionID = versionLink});
                else
                    versionLinks.Add(version, new List<VersionKey>() { new VersionKey() { ContentID = versionLinkParentContent, VersionID = versionLink } });
            }

            return versionLinks;
        }


        internal List<TreeNode> GetContentsTree(bool fullData, out Dictionary<int, Folder> folders, out Dictionary<int, Content> contents, out Dictionary<int, ContentVersion> versions)
        {
            UpdateReferenceList();
            
            versions = new Dictionary<int, ContentVersion>();
            List<TreeNode> foldersRoot = new List<TreeNode>();
            
            folders = GetFoldersDictionary();
            contents = GetContentsDictionaryByID(null, versions, fullData);

            AddFoldersToFoldersRootTree(folders, foldersRoot);
            AddContentsToFoldersRootTree(folders, contents);

            (new UpdatePermission(folders, contents, versions)).UpdateTreeNodeRecursive(foldersRoot);

            return foldersRoot;
        }

        private void UpdateReferenceList()
        {
            if (ContentStatusList == null)
                ContentStatusList = new ContentStatusReader().GetStatusList();

            if (ContentTypeList == null)
                ContentTypeList = new ContentTypesReader().GetTypesList();

            if (UserGroupTypeList == null)
                UserGroupTypeList = UserGroupTypesReader.GetUserGroupTypes();
        }

        private void AddFoldersToFoldersRootTree(Dictionary<int, Folder> folders, List<TreeNode> foldersRoot)
        {
            bool existFolder;
            Folder folderTemp;
            Folder currentFolder;
            
            foreach (var folder in folders)
            {
                currentFolder = folder.Value;

                if (currentFolder.ParentID == 0)
                {
                    foldersRoot.Add(folder.Value);
                }
                else 
                {
                    existFolder = folders.TryGetValue(currentFolder.ParentID, out folderTemp);

                    if (!existFolder)
                        throw new TraceException("1", false, null, Locator.ApplicationName);

                    folderTemp.Nodes.Add(currentFolder);
                }
            }
        }

        private void AddContentsToFoldersRootTree(Dictionary<int, Folder> folders, Dictionary<int, Content> contents)
        {

            bool existFolder;
            Folder folderTemp;
            Content currentContent;

            foreach (var content in contents)
            {
                currentContent = content.Value;

                existFolder = folders.TryGetValue(currentContent.ParentID, out folderTemp);

                if (!existFolder)
                    throw new TraceException("2", false, null, Locator.ApplicationName);

                folderTemp.Nodes.Add(currentContent);
            }
        }

        private Dictionary<int, Folder> GetFoldersDictionary()
        {
            Folder folderTemp;
            DataTable foldersDataTable = GetFoldersDataTable();
            Dictionary<int, Folder> folders = new Dictionary<int, Folder>();

            foreach (DataRow row in foldersDataTable.Rows)
            {
                folderTemp = CreateFolderFromDataRow(row);
                folderTemp.UserGroupTypePermission = GetFolderUserGroupType(folderTemp.ID);
                folders.Add(folderTemp.ID,folderTemp);
            }

            return folders;
        }

        private Dictionary<String, FolderUserGroupType> GetFolderUserGroupType(int folderID)
        {
            FolderUserGroupType userGroupType;
            DataTable foldersDataTable = GetFolderUserGroupTypesDataTable(folderID);
            Dictionary<String, FolderUserGroupType> folderUserGroupTypes = new Dictionary<String, FolderUserGroupType>();

            foreach (DataRow row in foldersDataTable.Rows)
            {
                userGroupType = CreateFolderUserGroupTypesFromDataRow(row);
                folderUserGroupTypes.Add(userGroupType.UserGroupType.ID, userGroupType);
            }
            return folderUserGroupTypes;
        }

        private Dictionary<int, Content> GetContentsDictionaryByID(List<int> contentsID, Dictionary<int, ContentVersion> versions, bool fullData)
        {
            Content contentTemp;
            DataTable contentsDataTable = GetContentsDataTableByID(contentsID);
            Dictionary<int, Content> contents = new Dictionary<int, Content>();

            foreach (DataRow row in contentsDataTable.Rows)
            {
                contentTemp = CreateContentFromDataRow(row);
                UpdateContentsVersions(contentTemp, versions, fullData);
                contents.Add(contentTemp.ID, contentTemp);
            }

            if (versions != null)
            {
                foreach (KeyValuePair<int, ContentVersion> contentVersion in versions)
                    UpdateContentVersionSubVercions(contentVersion.Value, versions, contents);
            }

            return contents;
        }

        private void UpdateContentsVersions(Content content, Dictionary<int, ContentVersion> versions, bool fullData)
        {
            ContentVersion contentVersionTemp;
            DataTable contentVersionsDataTable = GetContentVersionsDataTableByContentID(content.ID);

            foreach (DataRow row in contentVersionsDataTable.Rows)
            {
                contentVersionTemp = CreateContentVersionFromDataRow(row);

                if (fullData || (contentVersionTemp.Status.ID != "Edit"))
                {
                    UpdateContentVersionFiles(contentVersionTemp);
                    content.Versions.Add(contentVersionTemp.ID, contentVersionTemp);

                    if(versions != null)
                        versions.Add(contentVersionTemp.ID, contentVersionTemp);                
                }
            }
        }

        private void UpdateContentVersionSubVercions(ContentVersion contentVersion, Dictionary<int, ContentVersion> versions, Dictionary<int, Content> contents)
        {
            int contentVersionSubVercion;
            ContentVersionSubVersion subVersion;

            DataTable contentVersionsSubVercionsDataTable = GetContentVersionSubVercionsByContentVersionID(contentVersion.ID);

            foreach (DataRow row in contentVersionsSubVercionsDataTable.Rows)
            {
                subVersion = new ContentVersionSubVersion();
                contentVersionSubVercion = DBprovider.GetIntParam(row, "VersionLink"); 
                LastUpdateUtil.UpdateObjectByDataReader(subVersion, row);
                subVersion.Order = DBprovider.GetIntParam(row, "LinkOrder");

                if(versions.ContainsKey(contentVersionSubVercion))
                {
                    subVersion.ContentSubVersion = versions[contentVersionSubVercion];
                    subVersion.Content = contents[subVersion.ContentSubVersion.ParentID];
                    contentVersion.ContentVersions.Add(contentVersionSubVercion, subVersion);
                }
            }
        }

        private void UpdateContentVersionFiles(ContentVersion contentVersion)
        {
            ContentFile contentFileTemp;
            DataTable contentFilesDataTable = GetContentVercionFilesDataTable(contentVersion.ID); 

            foreach (DataRow row in contentFilesDataTable.Rows)
            {
                contentFileTemp = CreateContentVersionFileFromDataRow(row, contentVersion);
                contentVersion.Files.Add(contentFileTemp.ID,contentFileTemp);
            }
        }   

        #region Get data row from DB

        public static DataTable GetVersionLinksWithLock()
        {
            String sqlCommand;


            sqlCommand = "SELECT ";
            sqlCommand += "ContentVersionVersionLink.CVVL_id_ContentVersion_Parent AS Version, ";
            sqlCommand += "ContentVersionVersionLink.CVVL_id_ContentVersion_Link AS VersionLink, ";
            sqlCommand += "ContentVersion.CV_id_Content AS VersionLinkParentContent ";
            sqlCommand += "FROM ";
            sqlCommand += "ContentVersionVersionLink INNER JOIN ContentVersion WITH (UPDLOCK) ";
            sqlCommand += "ON ContentVersionVersionLink.CVVL_id_ContentVersion_Link = ContentVersion.CV_ID";

            return Locator.DBprovider.ExecuteSelectCommand(sqlCommand);
        }

        public static DataTable GetContentVersionSubVercionsByContentVersionID(int contentVersionID)
        {
            String sqlCommand;

            sqlCommand = "Select ";
            sqlCommand += "CVVL_id_ContentVersion_Link as VersionLink, ";
            sqlCommand += "CVVL_ChildNumber as LinkOrder, ";
            sqlCommand += "CVVL_LastUpdateUser as UpdateUser, ";
            sqlCommand += "CVVL_LastUpdateComputer as UpdateComputer, ";
            sqlCommand += "CVVL_LastUpdateApplication as UpdateApplication, ";
            sqlCommand += "CVVL_LastUpdateTime as UpdateTime ";
            sqlCommand += "From ";
            sqlCommand += "ContentVersionVersionLink ";
            sqlCommand += "Where ";
            sqlCommand += "CVVL_id_ContentVersion_Parent = " + contentVersionID;
            sqlCommand += " Order by LinkOrder";

            return Locator.DBprovider.ExecuteSelectCommand(sqlCommand);
        }

        private DataTable GetContentVercionFilesDataTable(int contentVersionID)
        {
            String sqlCommand;

            sqlCommand = "Select ";
            sqlCommand += "CVF_ID as ID, ";
            sqlCommand += "CVF_LastUpdateUser as UpdateUser, ";
            sqlCommand += "CVF_LastUpdateComputer as UpdateComputer, ";
            sqlCommand += "CVF_LastUpdateApplication as UpdateApplication, ";
            sqlCommand += "CVF_LastUpdateTime as UpdateTime, ";
            sqlCommand += "CVF_Name as FileName, ";
            sqlCommand += "CVF_Path as FilePath ";
            sqlCommand += "From ";
            sqlCommand += "ContentVersionFile ";
            sqlCommand += "Where ";
            sqlCommand += "CVF_id_ContentVersion = " + contentVersionID;

            return Locator.DBprovider.ExecuteSelectCommand(sqlCommand);
        }

        private DataTable GetContentVersionsDataTableByContentID(int contentID)
        {
            String sqlCommand;

            sqlCommand = "Select ";
            sqlCommand += "CV_ID as ID, ";
            sqlCommand += "CV_LastUpdateUser as UpdateUser, ";
            sqlCommand += "CV_LastUpdateComputer as UpdateComputer, ";
            sqlCommand += "CV_LastUpdateApplication as UpdateApplication, ";
            sqlCommand += "CV_LastUpdateTime as UpdateTime, ";
            sqlCommand += "CV_Name as Name, ";
            sqlCommand += "CV_id_Content as Content, ";
            sqlCommand += "CV_ECR as ECR, ";
            sqlCommand += "CV_DocumentID as DocumentID, ";
            sqlCommand += "CV_id_ContentVersionStatus as ContentStatus, ";
            sqlCommand += "CV_Description as Description, ";
            sqlCommand += "CV_Path as Path, ";
            sqlCommand += "CV_ChildNumber as ChildNumber, ";
            sqlCommand += "CV_CommandLine as RunningString, ";
            sqlCommand += "CV_LockWithDescription as Editor, ";
            sqlCommand += "CV_id_PathType as PathType ";
            sqlCommand += "From ";
            sqlCommand += "ContentVersion ";
            sqlCommand += "Where ";
            sqlCommand += "CV_id_Content = " + contentID + " ";
            sqlCommand += "ORDER BY ChildNumber ";

            return Locator.DBprovider.ExecuteSelectCommand(sqlCommand);
        }
        
        private DataTable GetContentsDataTableByID(List<int> contentsID)
        {
            String sqlCommand;
            String whereInList = "";

            if (contentsID == null)
                whereInList = "";
            else
            {
                for (int i = 0; i < contentsID.Count; i++)
                {
                    whereInList += contentsID[i].ToString();

                    if (i + 1 != contentsID.Count)
                        whereInList += ",";
                }

                whereInList = "Where CO_ID IN (" + whereInList + ") ";
            }

            sqlCommand = "Select ";
            sqlCommand += "CO_ID as ID, ";
            sqlCommand += "CO_LastUpdateUser as UpdateUser, ";
            sqlCommand += "CO_LastUpdateComputer as UpdateComputer, ";
            sqlCommand += "CO_LastUpdateApplication as UpdateApplication, ";
            sqlCommand += "CO_LastUpdateTime as UpdateTime, ";

            sqlCommand += "CO_Name as Name, ";
            sqlCommand += "CO_id_ContentTree as ContentTree, ";
            sqlCommand += "CO_id_ContentType as ContentType, ";
            sqlCommand += "CO_CertificateFree as CertificateFree, ";
            sqlCommand += "CO_ChildNumber as ChildNumber, ";
            sqlCommand += "CO_Icon as Icon, ";
            sqlCommand += "CO_Description as Description ";
            sqlCommand += "From ";
            sqlCommand += "Content ";
            sqlCommand += whereInList;

            return Locator.DBprovider.ExecuteSelectCommand(sqlCommand);
        }

        private DataTable GetFoldersDataTable()
        {
            String sqlCommand;

            sqlCommand = "Select ";
            sqlCommand += "CT_ID as ID, ";
            sqlCommand += "CT_Name as Name, ";
            sqlCommand += "CT_LastUpdateUser as UpdateUser, ";
            sqlCommand += "CT_LastUpdateComputer as UpdateComputer, ";
            sqlCommand += "CT_LastUpdateApplication as UpdateApplication, ";
            sqlCommand += "CT_LastUpdateTime as UpdateTime, ";
            sqlCommand += "CT_ParentID as Parent, ";
            sqlCommand += "CT_ChildNumber as ChildNumber, ";
            sqlCommand += "CT_Description as Description ";
            sqlCommand += "From ";
            sqlCommand += "ContentTree ";
            sqlCommand += "Where ";
            sqlCommand += "(CT_ID <> 0) ";
            sqlCommand += "ORDER BY CT_ChildNumber";

            return Locator.DBprovider.ExecuteSelectCommand(sqlCommand);
        }

        private DataTable GetFolderUserGroupTypesDataTable(int folderID)
        {
            String sqlCommand;

            sqlCommand = "Select ";
            sqlCommand += "CTUGT_id_UserGroupType as UserGroupType, ";
            sqlCommand += "CTUGT_LastUpdateUser as UpdateUser, ";
            sqlCommand += "CTUGT_LastUpdateComputer as UpdateComputer, ";
            sqlCommand += "CTUGT_LastUpdateApplication as UpdateApplication, ";
            sqlCommand += "CTUGT_LastUpdateTime as UpdateTime ";
            sqlCommand += "From ";
            sqlCommand += "ContentTreeUserGroupType ";
            sqlCommand += "Where ";
            sqlCommand += "CTUGT_id_ContentTree = " + folderID;

            return Locator.DBprovider.ExecuteSelectCommand(sqlCommand);
        }

        #endregion

        #region Create object from DataRow

        private static Folder CreateFolderFromDataRow(DataRow row)
        {
            Folder folder = new Folder
                {
                    ID = DBprovider.GetIntParam(row, "ID"),
                    Name = DBprovider.GetStringParam(row, "Name"),
                    ParentID = DBprovider.GetIntParam(row, "Parent"),
                    ChildID = DBprovider.GetIntParam(row, "ChildNumber"),
                    Description = DBprovider.GetStringParam(row, "Description"),
                    Nodes = new List<TreeNode>(),
                    UserGroupTypePermission = new Dictionary<string, FolderUserGroupType>()
                };

            LastUpdateUtil.UpdateObjectByDataReader(folder, row);
            return folder;
        }

        private FolderUserGroupType CreateFolderUserGroupTypesFromDataRow(DataRow row)
        {
            FolderUserGroupType folderUserGroupType = new FolderUserGroupType
                {
                    UserGroupType = UserGroupTypeList[DBprovider.GetStringParam(row, "UserGroupType")]
                };

            LastUpdateUtil.UpdateObjectByDataReader(folderUserGroupType, row);
            return folderUserGroupType;
        }

        private Content CreateContentFromDataRow(DataRow row)
        {
            bool existContentType;
            ContentType contentType;

            Content content = new Content
                {
                    ID = DBprovider.GetIntParam(row, "ID"),
                    Name = DBprovider.GetStringParam(row, "Name"),
                    ParentID = DBprovider.GetIntParam(row, "ContentTree"),
                    Description = DBprovider.GetStringParam(row, "Description"),
                    IconFileFullPath = DBprovider.GetStringParam(row, "Icon"),
                    CertificateFree = DBprovider.GetStringParam(row, "CertificateFree") == "YES",
                    Versions = new Dictionary<int, ContentVersion>()
                };

            existContentType = ContentTypeList.TryGetValue(DBprovider.GetStringParam(row, "ContentType"), out contentType);
            if (!existContentType)
                throw new TraceException("3", false, null, Locator.ApplicationName);

            content.ContentType = contentType;

            LastUpdateUtil.UpdateObjectByDataReader(content, row);

            return content;
        }

        private ContentVersion CreateContentVersionFromDataRow(DataRow row)
        {
            bool existContentStatus;
            ContentStatus contentStatus;

            ContentVersion contentVersion = new ContentVersion
                {
                    ID = DBprovider.GetIntParam(row, "ID"),
                    Name = DBprovider.GetStringParam(row, "Name"),
                    ECR = DBprovider.GetStringParam(row, "ECR"),
                    DocumentID = DBprovider.GetStringParam(row, "DocumentID"),
                    Description = DBprovider.GetStringParam(row, "Description"),
                    ChildID = DBprovider.GetIntParam(row, "ChildNumber"),
                    RunningString = DBprovider.GetStringParam(row, "RunningString"),
                    Editor = DBprovider.GetStringParam(row, "Editor"),
                    Files = new Dictionary<int, ContentFile>(),
                    ContentVersions = new Dictionary<int, ContentVersionSubVersion>(),
                    ParentID = DBprovider.GetIntParam(row, "Content"),
                    Path = new PathFS
                        {
                            Name = DBprovider.GetStringParam(row, "Path"),
                            Type = DBprovider.GetStringParam(row, "PathType") == "Full" ? PathType.Full : PathType.Relative
                        }
                };

            existContentStatus = ContentStatusList.TryGetValue(DBprovider.GetStringParam(row, "ContentStatus"), out contentStatus);
            if (!existContentStatus)
                throw new TraceException("3", false, null, Locator.ApplicationName);

            contentVersion.Status = contentStatus;
            LastUpdateUtil.UpdateObjectByDataReader(contentVersion, row);

            return contentVersion;
        }

        private ContentFile CreateContentVersionFileFromDataRow(DataRow row, ContentVersion contentVersion)
        {
            ContentFile contentFile = new ContentFile
                {
                    ID = DBprovider.GetIntParam(row, "ID"),
                    FileName = DBprovider.GetStringParam(row, "FileName"),
                    FileRelativePath = DBprovider.GetStringParam(row, "FilePath")
                };


            if (contentVersion.Path.Type == PathType.Full)
                contentFile.FileFullPath = "";
            else
                contentFile.FileFullPath = Locator.SystemParameters["RootPathFS"] + "\\";

            contentFile.FileFullPath += contentVersion.Path.Name + "\\";

            if (contentFile.FileRelativePath != "")
                contentFile.FileFullPath += contentFile.FileRelativePath + "\\";

            contentFile.FileFullPath += contentFile.FileName;

            LastUpdateUtil.UpdateObjectByDataReader(contentFile, row);
            return contentFile;
        }

        #endregion
    }
}
