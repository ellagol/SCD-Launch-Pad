using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Diagnostics;
using ContentManagerProvider.General;
using TraceExceptionWrapper;
using DatabaseProvider;

namespace ContentManagerProvider
{
    class TreeNodeUpdater
    {
        internal void Update(TreeNode treeNodeUpdated, TreeNode treeNodeOriginal, ICopyFilesProgress progressBar)
         {
             try
             {
                 Locator.DBprovider.OpenConnection();
                 Locator.DBprovider.BeginTransaction();

                 SelectObjectsForUpdate.Select(treeNodeOriginal);
                 SelectObjectsForUpdate.SelectParent(treeNodeUpdated);

                 switch (treeNodeUpdated.TreeNodeType)
                 {
                     case TreeNodeObjectType.Folder:
                         UpdateFolder((Folder)treeNodeUpdated, (Folder)treeNodeOriginal);
                         break;
                     case TreeNodeObjectType.Content:
                         Locator.Impersonation.Logon();
                         UpdateContent((Content)treeNodeUpdated, (Content)treeNodeOriginal);
                         Locator.Impersonation.Dispose();
                         break;
                     case TreeNodeObjectType.ContentVersion:
                         (new ContentVersionLinkConfirmer()).ConfirmerContentVersion((ContentVersion)treeNodeUpdated);
                         SelectObjectsForUpdate.SelectContentVersionLinked((ContentVersion)treeNodeUpdated);
                         Locator.Impersonation.Logon();
                         UpdateContentVersion((ContentVersion)treeNodeUpdated, (ContentVersion)treeNodeOriginal, progressBar);
                         Locator.Impersonation.Dispose();
                         break;
                 }

                 Locator.DBprovider.CommitTransaction();
                 Locator.DBprovider.CloseConnection();
                 PostUpdate(treeNodeUpdated, treeNodeOriginal);
             }
             catch (TraceException te)
             {
                 Locator.DBprovider.RollbackTransaction();
                 Locator.DBprovider.CloseConnection();
                 Locator.Impersonation.Dispose();
                 te.AddTrace(new StackFrame(1, true));
                 throw te;
             }
             catch (Exception e)
             {
                 Locator.DBprovider.RollbackTransaction();
                 Locator.DBprovider.CloseConnection();
                 Locator.Impersonation.Dispose();
                 TraceException te = new TraceException(new StackFrame(1, true), e, Locator.ApplicationName);
                 throw te;
             }
         }

         internal void PostUpdate(TreeNode treeNodeUpdated, TreeNode treeNodeOriginal)
         {
             try
             {
                 switch (treeNodeUpdated.TreeNodeType)
                 {
                     case TreeNodeObjectType.Content:
                         PostUpdateContent((Content)treeNodeUpdated, (Content)treeNodeOriginal);
                         break;
                     case TreeNodeObjectType.ContentVersion:
                         PostUpdateContentVersion((ContentVersion)treeNodeUpdated, (ContentVersion)treeNodeOriginal);
                         break;
                 }
             }
             catch{}
         }

         private void UpdateFolder(Folder folderUpdated, Folder folderOriginal)
         {
             CheckFolderName(folderUpdated, folderOriginal);
             UpdateFolderData(folderUpdated);
             UpdateFolderUserGroupTypes(folderUpdated, folderOriginal);
         }

         private void CheckFolderName(Folder folderUpdated, Folder folderOriginal)
         {
             String sqlCommand;
             sqlCommand = "Select Count(CT_ID) From ContentTree where CT_Name = '" + DBprovider.UpdateStringForSqlFormat(folderUpdated.Name) + "' and CT_ID <> " +
                          folderOriginal.ID + " and CT_ParentID = " + folderUpdated.ParentID;

             if (Locator.DBprovider.ExecuteScalarCommand(sqlCommand) > 0)
                 throw new TraceException("Update folder name to existing", false, new List<string>() { folderOriginal.Name }, Locator.ApplicationName);
         }

         private void UpdateFolderUserGroupTypes(Folder folderUpdated, Folder folderOriginal)
         {
             foreach (KeyValuePair<string, FolderUserGroupType> folderUserGroupType in folderUpdated.UserGroupTypePermission)
             {
                 if (!folderOriginal.UserGroupTypePermission.ContainsKey(folderUserGroupType.Value.UserGroupType.ID))
                     UpdateFolderUserGroupTypesAdd(folderUpdated.ID, folderUserGroupType.Value);
             }

             foreach (KeyValuePair<string, FolderUserGroupType> folderUserGroupType in folderOriginal.UserGroupTypePermission)
             {
                 if (!folderUpdated.UserGroupTypePermission.ContainsKey(folderUserGroupType.Value.UserGroupType.ID))
                     UpdateFolderUserGroupTypesDelete(folderUpdated.ID, folderUserGroupType.Value);
             }
         }

         private void UpdateFolderUserGroupTypesAdd(int folderID, FolderUserGroupType folderUserGroupType)
         {
            String sqlCommand;

            LastUpdateUtil.UpdateObjectLastUpdate(folderUserGroupType);

            sqlCommand = "Insert Into  ";
            sqlCommand += "ContentTreeUserGroupType (";
            sqlCommand += "CTUGT_id_ContentTree, ";
            sqlCommand += "CTUGT_id_UserGroupType, ";
            sqlCommand += "CTUGT_LastUpdateUser, ";
            sqlCommand += "CTUGT_LastUpdateComputer, ";
            sqlCommand += "CTUGT_LastUpdateApplication, ";
            sqlCommand += "CTUGT_LastUpdateTime) ";
            sqlCommand += "Values (";
            sqlCommand += folderID + ", ";
            sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(folderUserGroupType.UserGroupType.ID) + "', ";
            sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(folderUserGroupType.LastUpdateUser) + "', ";
            sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(folderUserGroupType.LastUpdateComputer) + "', ";
            sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(folderUserGroupType.LastUpdateApplication) + "', ";
            sqlCommand += "'" + DBprovider.ConvertTimeToStringFormat(folderUserGroupType.LastUpdateTime) + "') ";

            Locator.DBprovider.ExecuteCommand(sqlCommand);
         }

         private void UpdateFolderUserGroupTypesDelete(int folderID, FolderUserGroupType folderUserGroupType)
         {
             String sqlCommand;

             sqlCommand = "Delete from ";
             sqlCommand += "ContentTreeUserGroupType ";
             sqlCommand += "where CTUGT_id_ContentTree = " + folderID;
             sqlCommand += " and CTUGT_id_UserGroupType = '" + DBprovider.UpdateStringForSqlFormat(folderUserGroupType.UserGroupType.ID) + "'";

             Locator.DBprovider.ExecuteCommand(sqlCommand);
         }

        private void UpdateFolderData(Folder folderUpdated)
         {
             String sqlCommand;
             LastUpdateUtil.UpdateObjectLastUpdate(folderUpdated);

             sqlCommand = "Update ";
             sqlCommand += "ContentTree ";
             sqlCommand += "Set ";
             sqlCommand += "CT_Name = '" + DBprovider.UpdateStringForSqlFormat(folderUpdated.Name) + "', ";
             sqlCommand += "CT_LastUpdateUser = '" + DBprovider.UpdateStringForSqlFormat(folderUpdated.LastUpdateUser) + "', ";
             sqlCommand += "CT_LastUpdateComputer = '" + DBprovider.UpdateStringForSqlFormat(folderUpdated.LastUpdateComputer) + "', ";
             sqlCommand += "CT_LastUpdateApplication = '" + DBprovider.UpdateStringForSqlFormat(folderUpdated.LastUpdateApplication) + "', ";
             sqlCommand += "CT_LastUpdateTime = '" + DBprovider.ConvertTimeToStringFormat(folderUpdated.LastUpdateTime) + "', ";
             sqlCommand += "CT_ParentID = '" + folderUpdated.ParentID + "', ";
             sqlCommand += "CT_ChildNumber = '" + folderUpdated.ChildID + "', ";
             sqlCommand += "CT_Description = '" + DBprovider.UpdateStringForSqlFormat(folderUpdated.Description) + "' ";
             sqlCommand += "Where ";
             sqlCommand += "CT_ID = " + folderUpdated.ID;

             Locator.DBprovider.ExecuteCommand(sqlCommand);
         }

         private void UpdateContent(Content contentUpdated, Content contentOriginal)
         {
             CheckContentName(contentUpdated, contentOriginal);
             UpdateContentIconOnFs(contentUpdated, contentOriginal);
             UpdateContentData(contentUpdated, contentOriginal);
         }

         private void PostUpdateContent(Content contentUpdated, Content contentOriginal)
         {
             if (contentUpdated.IconFileFullPath == contentOriginal.IconFileFullPath)
                 return;

             String folderName = FileSystemUpdater.GetFileFolder(contentOriginal.IconFileFullPath);
             FileSystemUpdater.DeleteFolder(folderName);
         }

        private void CheckContentName(Content contentUpdated, Content contentOriginal)
        {
            String sqlCommand;
            sqlCommand = "Select Count(CO_ID) From Content where CO_Name = '" + DBprovider.UpdateStringForSqlFormat(contentUpdated.Name) + "' and CO_ID <> " +
                         contentOriginal.ID;

            if (Locator.DBprovider.ExecuteScalarCommand(sqlCommand) > 0)
                throw new TraceException("Update content name to existing", false, new List<string>() { contentOriginal.Name }, Locator.ApplicationName);
        }

        private void UpdateContentIconOnFs(Content contentUpdated, Content contentOriginal)
        {
            if (contentUpdated.IconFileFullPath =="" ||
                contentUpdated.IconFileFullPath == contentOriginal.IconFileFullPath)
                return;

            String folderName = FileSystemUpdater.CreateFolder(contentUpdated);
            contentUpdated.IconFileFullPath = FileSystemUpdater.AddFile(folderName, contentUpdated.IconFileFullPath);
        }

        private void UpdateContentData(Content contentUpdated, Content contentOriginal)
        {
             String sqlCommand;
             LastUpdateUtil.UpdateObjectLastUpdate(contentUpdated);

             sqlCommand = "Update ";
             sqlCommand += "Content ";
             sqlCommand += "Set ";
             sqlCommand += "CO_LastUpdateUser = '" + DBprovider.UpdateStringForSqlFormat(contentUpdated.LastUpdateUser) + "', ";
             sqlCommand += "CO_LastUpdateComputer = '" + DBprovider.UpdateStringForSqlFormat(contentUpdated.LastUpdateComputer) + "', ";
             sqlCommand += "CO_LastUpdateApplication = '" + DBprovider.UpdateStringForSqlFormat(contentUpdated.LastUpdateApplication) + "', ";
             sqlCommand += "CO_LastUpdateTime = '" + DBprovider.ConvertTimeToStringFormat(contentUpdated.LastUpdateTime) + "', ";
             sqlCommand += "CO_Description = '" + DBprovider.UpdateStringForSqlFormat(contentUpdated.Description) + "', ";
             sqlCommand += "CO_Name = '" + DBprovider.UpdateStringForSqlFormat(contentUpdated.Name) + "', ";
             sqlCommand += "CO_ChildNumber = '" + contentUpdated.ChildID + "', "; 
             sqlCommand += "CO_Icon = '" + DBprovider.UpdateStringForSqlFormat(contentUpdated.IconFileFullPath) + "', ";
             sqlCommand += "CO_id_ContentType = '" + DBprovider.UpdateStringForSqlFormat(contentUpdated.ContentType.ID) + "', ";

            if(contentUpdated.CertificateFree)
                sqlCommand += "CO_CertificateFree = 'YES', ";
            else
                sqlCommand += "CO_CertificateFree = 'NOT', ";

            sqlCommand += "CO_id_ContentTree = '" + contentUpdated.ParentID + "' "; 
             sqlCommand += "Where ";
             sqlCommand += "CO_ID = " + contentOriginal.ID;

             Locator.DBprovider.ExecuteCommand(sqlCommand);
        }

        private void UpdateContentVersion(ContentVersion contentVersionUpdated, ContentVersion contentVersionOriginal, ICopyFilesProgress progressBar)
        {
            bool updateFs;

            CheckContentVersionName(contentVersionUpdated, contentVersionOriginal);
            UpdateContentVersionFilesOnFs(contentVersionUpdated, contentVersionOriginal, out updateFs, progressBar);
            UpdateContentVersionData(contentVersionUpdated);

            if (updateFs)
                UpdateContentVersionFiles(contentVersionUpdated);

            UpdateContentVersionVersionLinks(contentVersionUpdated, contentVersionOriginal);
        }

        private void PostUpdateContentVersion(ContentVersion contentVersionUpdated, ContentVersion contentVersionOriginal)
        {
            if (contentVersionUpdated.Path.Name == contentVersionOriginal.Path.Name &&
                    contentVersionUpdated.Path.Type == contentVersionOriginal.Path.Type)
                return;

            if (contentVersionOriginal.Path.Type == PathType.Relative)
                FileSystemUpdater.DeleteFolder(Locator.SystemParameters["RootPathFS"] + "\\" + contentVersionOriginal.Path.Name);
        }

        private void CheckContentVersionName(ContentVersion contentVersionUpdated, ContentVersion contentVersionOriginal)
        {
            String sqlCommand;
            sqlCommand = "Select Count(CV_ID) From ContentVersion where CV_Name = '" + DBprovider.UpdateStringForSqlFormat(contentVersionUpdated.Name) +
                         "' and CV_id_Content = " + contentVersionUpdated.ParentID + " and CV_ID <> " +
                         contentVersionUpdated.ID;

            if (Locator.DBprovider.ExecuteScalarCommand(sqlCommand) > 0)
                throw new TraceException("Update version name to existing", false, new List<string>() { contentVersionOriginal.Name }, Locator.ApplicationName);
        }

        private void UpdateContentVersionFilesOnFs(ContentVersion contentVersion, ContentVersion contentVersionOriginal, out bool updateFs, ICopyFilesProgress progressBar)
        {

            updateFs = false;
            ContentFile contentFileOriginal;

            if (contentVersion.Files.Count != contentVersionOriginal.Files.Count)
                updateFs = true;
            else
            {
                foreach (KeyValuePair<int, ContentFile> file in contentVersion.Files)
                {
                    if (contentVersionOriginal.Files.ContainsKey(file.Value.ID))
                    {
                        contentFileOriginal = contentVersionOriginal.Files[file.Value.ID];
                        if (file.Value.FileName != contentFileOriginal.FileName ||
                            file.Value.FileRelativePath != contentFileOriginal.FileRelativePath)
                        {
                            updateFs = true;
                            break;
                        }
                    }
                    else
                    {
                        updateFs = true;
                        break;                        
                    }
                }
            }
        
            if (!updateFs)
            {
                contentVersion.Path = new PathFS() { Type = contentVersionOriginal.Path.Type, Name = contentVersionOriginal.Path.Name };
                return;
            }

            int i = 0;
            string folderName;
            string destinationFullPath;
            string destinationDirectoryPath;

            String relativePath;
            contentVersion.Path.Type = PathType.Relative;
            contentVersion.Path.Name = FileSystemUpdater.CreateFolder(contentVersion, out relativePath);
            contentVersion.Path = new PathFS() { Type = PathType.Relative, Name = relativePath };

            if (progressBar != null)
            {
                progressBar.Init(contentVersion.Files.Count);
                progressBar.Show();
            }

            foreach (KeyValuePair<int, ContentFile> file in contentVersion.Files)
            {
                i++;
                folderName = Locator.SystemParameters["RootPathFS"] + "\\" + contentVersion.Path.Name;
                file.Value.GetDestinationPath(folderName, out destinationFullPath, out destinationDirectoryPath);

                if (progressBar != null)
                {
                    progressBar.IncreaseProgress(file.Value.FileFullPath, destinationFullPath, i);

                    if(progressBar.Canceled)
                        throw new TraceException("Aborted by user", false, null, Locator.ApplicationName);
                }

                FileSystemUpdater.AddFile(file.Value.FileFullPath, destinationFullPath, destinationDirectoryPath);
                file.Value.UpdateFileFullPath(contentVersion.Path);
            }

            if (progressBar != null)
                progressBar.Close();
        }

        private void UpdateContentVersionData(ContentVersion contentVersion)
        {
            String sqlCommand;
            LastUpdateUtil.UpdateObjectLastUpdate(contentVersion);

            sqlCommand = "Update ";
            sqlCommand += "ContentVersion ";
            sqlCommand += "Set ";
            sqlCommand += "CV_LastUpdateUser = '" + DBprovider.UpdateStringForSqlFormat(contentVersion.LastUpdateUser) + "', ";
            sqlCommand += "CV_LastUpdateComputer = '" + DBprovider.UpdateStringForSqlFormat(contentVersion.LastUpdateComputer) + "', ";
            sqlCommand += "CV_LastUpdateApplication = '" + DBprovider.UpdateStringForSqlFormat(contentVersion.LastUpdateApplication) + "', ";
            sqlCommand += "CV_LastUpdateTime = '" + DBprovider.ConvertTimeToStringFormat(contentVersion.LastUpdateTime) + "', ";
            sqlCommand += "CV_Description = '" + DBprovider.UpdateStringForSqlFormat(contentVersion.Description) + "', ";
            sqlCommand += "CV_Name = '" + DBprovider.UpdateStringForSqlFormat(contentVersion.Name) + "', ";
            sqlCommand += "CV_id_ContentVersionStatus = '" + DBprovider.UpdateStringForSqlFormat(contentVersion.Status.ID) + "', ";
            sqlCommand += "CV_ECR = '" + DBprovider.UpdateStringForSqlFormat(contentVersion.ECR) + "', ";
            sqlCommand += "CV_DocumentID = '" + DBprovider.UpdateStringForSqlFormat(contentVersion.DocumentID) + "', ";
            sqlCommand += "CV_id_Content = '" + contentVersion.ParentID + "', "; //Ailk !!!!!!!!!!!!
            sqlCommand += "CV_Path = '" + DBprovider.UpdateStringForSqlFormat(contentVersion.Path.Name) + "', ";
            sqlCommand += "CV_ChildNumber = '" + contentVersion.ChildID + "', ";//Ailk !!!!!!!!!!!!

            switch (contentVersion.Path.Type)
            {
                case PathType.Full:
                    sqlCommand += "CV_id_PathType = 'Full', ";
                    break;
                case PathType.Relative:
                    sqlCommand += "CV_id_PathType = 'Rel', ";
                    break;
            }

            sqlCommand += "CV_CommandLine = '" + DBprovider.UpdateStringForSqlFormat(contentVersion.RunningString) + "', ";
            sqlCommand += "CV_LockWithDescription = '" + DBprovider.UpdateStringForSqlFormat(contentVersion.Editor) + "' ";
            sqlCommand += "Where ";
            sqlCommand += "CV_ID = " + contentVersion.ID;

            Locator.DBprovider.ExecuteCommand(sqlCommand);
        }

        private void UpdateContentVersionVersionLinks(ContentVersion contentVersion, ContentVersion contentVersionOriginal)
        {
            int versionLink;
            String strVersionLinksToDelete = String.Empty;
            List<int> versionLinksToLeave = new List<int>();

            DataTable contentVersionsSubVercionsDataTable = ContentsReader.GetContentVersionSubVercionsByContentVersionID(contentVersion.ID);

            foreach (DataRow row in contentVersionsSubVercionsDataTable.Rows)
            {
                versionLink = DBprovider.GetIntParam(row, "VersionLink");

                if (!contentVersion.ContentVersions.ContainsKey(versionLink) ||
                    contentVersion.ContentVersions[versionLink].Order != DBprovider.GetIntParam(row, "LinkOrder"))
                {
                    if (strVersionLinksToDelete == String.Empty)
                        strVersionLinksToDelete = versionLink.ToString();
                    else
                        strVersionLinksToDelete += "," + versionLink.ToString();
                }
                else
                {
                    contentVersion.ContentVersions[versionLink] = contentVersionOriginal.ContentVersions[versionLink];
                    versionLinksToLeave.Add(versionLink);
                }
            }

            UpdateContentVersionVersionLinksDelete(contentVersion, strVersionLinksToDelete);

            foreach (KeyValuePair<int, ContentVersionSubVersion> contentVersionLink in contentVersion.ContentVersions)
            {
                if(!versionLinksToLeave.Contains(contentVersionLink.Key))
                    TreeNodeAdder.AddContentVersionLink(contentVersion, contentVersionLink.Value);
            }
        }
        
        private void UpdateContentVersionVersionLinksDelete(ContentVersion contentVersion, String strVersionLinksToDelete)
        {
            String sqlCommand;

            if (strVersionLinksToDelete == String.Empty)
                return;

            sqlCommand = "Delete from ";
            sqlCommand += "ContentVersionVersionLink ";
            sqlCommand += "where CVVL_id_ContentVersion_Parent = " + contentVersion.ID;
            sqlCommand += " and CVVL_id_ContentVersion_Link IN (" + strVersionLinksToDelete + ")";

            Locator.DBprovider.ExecuteCommand(sqlCommand); 
        }
        
        private void UpdateContentVersionFiles(ContentVersion contentVersion)
        {
            String filesNotToDelete = String.Empty;
            Dictionary<int, ContentFile> contentFileUpdated = new Dictionary<int, ContentFile>();

            foreach (KeyValuePair<int, ContentFile> contentFile in contentVersion.Files)
            {
                if (contentFile.Value.ID != 0)
                    UpdateContentVersionFileUpdate(contentFile.Value);
                else
                    TreeNodeAdder.AddContentVersionFile(contentVersion, contentFile.Value);

                    filesNotToDelete = filesNotToDelete == String.Empty
                                            ? contentFile.Value.ID.ToString()
                                            : filesNotToDelete + "," + contentFile.Value.ID;

                    contentFileUpdated.Add(contentFile.Value.ID, contentFile.Value);
            }

            contentVersion.Files = contentFileUpdated;
            UpdateContentVersionFilesDelete(contentVersion, filesNotToDelete);
        }

        private void UpdateContentVersionFileUpdate(ContentFile file)
        {
            String sqlCommand;
            LastUpdateUtil.UpdateObjectLastUpdate(file);

            sqlCommand = "Update ";
            sqlCommand += "ContentVersionFile ";
            sqlCommand += "Set ";
            sqlCommand += "CVF_LastUpdateUser = '" + DBprovider.UpdateStringForSqlFormat(file.LastUpdateUser) + "', ";
            sqlCommand += "CVF_LastUpdateComputer = '" + DBprovider.UpdateStringForSqlFormat(file.LastUpdateComputer) + "', ";
            sqlCommand += "CVF_LastUpdateApplication = '" + DBprovider.UpdateStringForSqlFormat(file.LastUpdateApplication) + "', ";
            sqlCommand += "CVF_LastUpdateTime = '" + DBprovider.ConvertTimeToStringFormat(file.LastUpdateTime) + "', ";
            sqlCommand += "CVF_Name = '" + DBprovider.UpdateStringForSqlFormat(file.FileName) + "', ";
            sqlCommand += "CVF_Path = '" + DBprovider.UpdateStringForSqlFormat(file.FileRelativePath) + "' ";
            sqlCommand += "Where ";
            sqlCommand += "CVF_ID = " + file.ID;

            Locator.DBprovider.ExecuteCommand(sqlCommand);
        }

        private void UpdateContentVersionFilesDelete(ContentVersion contentVersion, String filesNotToDelete)
        {
            String sqlCommand;

            sqlCommand = "Delete from ";
            sqlCommand += "ContentVersionFile ";
            sqlCommand += "where CVF_id_ContentVersion = " + contentVersion.ID;

            if(filesNotToDelete != String.Empty)
                sqlCommand += " and CVF_ID NOT IN (" + filesNotToDelete + ")"; 

            Locator.DBprovider.ExecuteCommand(sqlCommand);
        }
    }
}
