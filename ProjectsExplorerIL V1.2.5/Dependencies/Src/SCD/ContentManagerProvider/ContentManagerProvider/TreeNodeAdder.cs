using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using ContentManagerProvider.General;
using TraceExceptionWrapper;
using DatabaseProvider;

namespace ContentManagerProvider
{
    class TreeNodeAdder
    {
        internal void Add(TreeNode treeNode, bool updateNodeName, ICopyFilesProgress progressBar)
         {
             try
             {
                 Locator.DBprovider.OpenConnection();
                 Locator.DBprovider.BeginTransaction();

                 SelectObjectsForUpdate.SelectParent(treeNode);

                 switch (treeNode.TreeNodeType)
                 {
                     case TreeNodeObjectType.Folder:
                         AddFolder((Folder)treeNode, updateNodeName);
                         break;
                     case TreeNodeObjectType.Content:
                         Locator.Impersonation.Logon();
                         AddContent((Content)treeNode, updateNodeName);
                         Locator.Impersonation.Dispose();
                         break;
                     case TreeNodeObjectType.ContentVersion:
                         (new ContentVersionLinkConfirmer()).ConfirmerContentVersion((ContentVersion)treeNode);
                         Locator.Impersonation.Logon();
                         AddContentVersion((ContentVersion)treeNode, updateNodeName, progressBar);
                         Locator.Impersonation.Dispose();
                         break;
                 }

                 Locator.DBprovider.CommitTransaction();
                 Locator.DBprovider.CloseConnection();
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

        private void AddFolder(Folder folder, bool updateNodeName)
        {
            if (!updateNodeName)
                CheckFolderName(folder);
            else
                GenerateFolderName(folder);

            AddFolderData(folder);
            AddFolderUserGroupTypesData(folder);
        }

        private void CheckFolderName(Folder folder)
        {

            String sqlCommand;
            sqlCommand = "Select Count(CT_ID) From ContentTree where CT_Name = '" + DBprovider.UpdateStringForSqlFormat(folder.Name) + "' and CT_ParentID = " + folder.ParentID;

            if (Locator.DBprovider.ExecuteScalarCommand(sqlCommand) > 0)
                throw new TraceException("Adding existing folder", false, new List<string>() { folder.Name }, Locator.ApplicationName);
        }

        private void GenerateFolderName(Folder folder)
        {
            int index = 0;
            String sqlCommand;

            sqlCommand = "Select Count(CT_ID) From ContentTree where CT_Name = '" + DBprovider.UpdateStringForSqlFormat(folder.Name) + "' and CT_ParentID = " + folder.ParentID;

            if (Locator.DBprovider.ExecuteScalarCommand(sqlCommand) == 0)
                return;

            do
            {
                index++;
                sqlCommand = "Select Count(CT_ID) From ContentTree where CT_Name = '" + DBprovider.UpdateStringForSqlFormat(folder.Name) + index + "' and CT_ParentID = " + folder.ParentID;

            } while (Locator.DBprovider.ExecuteScalarCommand(sqlCommand) > 0);

            folder.Name = folder.Name + index;
        }

        private void AddFolderData(Folder folder)
         {
             String sqlCommand;
             LastUpdateUtil.UpdateObjectLastUpdate(folder);

             sqlCommand = "Insert Into  ";
             sqlCommand += "ContentTree (";
             sqlCommand += "CT_Name, ";
             sqlCommand += "CT_LastUpdateUser, ";
             sqlCommand += "CT_LastUpdateComputer, ";
             sqlCommand += "CT_LastUpdateApplication, ";
             sqlCommand += "CT_LastUpdateTime, ";
             sqlCommand += "CT_ParentID, ";
             sqlCommand += "CT_ChildNumber, ";
             sqlCommand += "CT_Description) ";
             sqlCommand += "OUTPUT INSERTED.CT_ID ";
             sqlCommand += "Values (";
             sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(folder.Name) + "', ";
             sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(folder.LastUpdateUser) + "', ";
             sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(folder.LastUpdateComputer) + "', ";
             sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(folder.LastUpdateApplication) + "', ";
             sqlCommand += "'" + DBprovider.ConvertTimeToStringFormat(folder.LastUpdateTime) + "', ";
             sqlCommand += folder.ParentID + ", ";
             sqlCommand += folder.ChildID + ", ";
             sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(folder.Description) + "')";

             folder.ID = Locator.DBprovider.ExecuteScalarCommand(sqlCommand);
         }

        private void AddFolderUserGroupTypesData(Folder folder)
         {
            String sqlCommand;

            foreach (KeyValuePair<string, FolderUserGroupType> folderUserGroupType in folder.UserGroupTypePermission)
            {
                LastUpdateUtil.UpdateObjectLastUpdate(folderUserGroupType.Value);

                sqlCommand = "Insert Into  ";
                sqlCommand += "ContentTreeUserGroupType (";
                sqlCommand += "CTUGT_id_ContentTree, ";
                sqlCommand += "CTUGT_id_UserGroupType, ";
                sqlCommand += "CTUGT_LastUpdateUser, ";
                sqlCommand += "CTUGT_LastUpdateComputer, ";
                sqlCommand += "CTUGT_LastUpdateApplication, ";
                sqlCommand += "CTUGT_LastUpdateTime) ";
                sqlCommand += "Values (";
                sqlCommand += folder.ID + ", ";
                sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(folderUserGroupType.Value.UserGroupType.ID) + "', ";
                sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(folder.LastUpdateUser) + "', ";
                sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(folder.LastUpdateComputer) + "', ";
                sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(folder.LastUpdateApplication) + "', ";
                sqlCommand += "'" + DBprovider.ConvertTimeToStringFormat(folder.LastUpdateTime) + "') ";

                Locator.DBprovider.ExecuteCommand(sqlCommand);
            }
         }

         private void AddContent(Content content, bool updateNodeName)
         {
             if (!updateNodeName)
                CheckContentName(content);
             else
                GenerateContentName(content);

             AddContentIconOnFs(content);
             AddContentData(content);
         }

         private void AddContentData(Content content)
         {
             String sqlCommand;
             LastUpdateUtil.UpdateObjectLastUpdate(content);

             sqlCommand = "Insert Into  ";
             sqlCommand += "Content (";
             sqlCommand += "CO_Name, ";
             sqlCommand += "CO_LastUpdateUser, ";
             sqlCommand += "CO_LastUpdateComputer, ";
             sqlCommand += "CO_LastUpdateApplication, ";
             sqlCommand += "CO_LastUpdateTime, ";
             sqlCommand += "CO_id_ContentTree, ";
             sqlCommand += "CO_ChildNumber, ";
             sqlCommand += "CO_Icon, ";
             sqlCommand += "CO_id_ContentType, ";
             sqlCommand += "CO_CertificateFree, ";
             sqlCommand += "CO_Description) ";
             sqlCommand += "OUTPUT INSERTED.CO_ID ";
             sqlCommand += "Values (";
             sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(content.Name) + "', ";
             sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(content.LastUpdateUser) + "', ";
             sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(content.LastUpdateComputer) + "', ";
             sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(content.LastUpdateApplication) + "', ";
             sqlCommand += "'" + DBprovider.ConvertTimeToStringFormat(content.LastUpdateTime) + "', ";
             sqlCommand += content.ParentID + ", ";
             sqlCommand += content.ChildID + ", ";
             sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(content.IconFileFullPath) + "', ";
             sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(content.ContentType.ID) + "', ";

             if(content.CertificateFree)
                 sqlCommand += "'YES', ";
             else
                 sqlCommand += "'NOT', ";

             sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(content.Description) + "')";

             content.ID = Locator.DBprovider.ExecuteScalarCommand(sqlCommand);
         }

         private void CheckContentName(Content content)
         {
             String sqlCommand;
             sqlCommand = "Select Count(CO_ID) From Content where CO_Name = '" + DBprovider.UpdateStringForSqlFormat(content.Name) + "'";

             if (Locator.DBprovider.ExecuteScalarCommand(sqlCommand) > 0)
                 throw new TraceException("Adding existing content", false, new List<string>() { content.Name }, Locator.ApplicationName);
         }

         private void GenerateContentName(Content content)
         {
             int index = 0;
             String sqlCommand;

             do
             {
                 index++;
                 sqlCommand = "Select Count(CO_ID) From Content where CO_Name = '" + DBprovider.UpdateStringForSqlFormat(content.Name) + index + "'";

             } while (Locator.DBprovider.ExecuteScalarCommand(sqlCommand) > 0);

             content.Name = content.Name + index;
         }

         private void AddContentIconOnFs(Content content)
         {
             if (content.IconFileFullPath == "")
                 return;

             String folderName = FileSystemUpdater.CreateFolder(content);
             content.IconFileFullPath = FileSystemUpdater.AddFile(folderName, content.IconFileFullPath);
         }

         private void AddContentVersion(ContentVersion contentVersion, bool updateNodeName, ICopyFilesProgress progressBar)
         {
             if (!updateNodeName)
                 CheckContentVersionName(contentVersion);
             else
                 GenerateContentVersionName(contentVersion);

             AddContentVersionFilesOnFs(contentVersion, progressBar);
             AddContentVersionData(contentVersion);
             AddContentVersionFiles(contentVersion);
             AddContentVersionVersionLink(contentVersion);
         }

        private void CheckContentVersionName(ContentVersion contentVersion)
        {
            String sqlCommand;
            sqlCommand = "Select Count(CV_ID) From ContentVersion where CV_Name = '" + DBprovider.UpdateStringForSqlFormat(contentVersion.Name) + "' and CV_id_Content = " + contentVersion.ParentID;

            if (Locator.DBprovider.ExecuteScalarCommand(sqlCommand) > 0)
                throw new TraceException("Adding existing version", false, new List<string>() { contentVersion.Name }, Locator.ApplicationName);
        }

        private void GenerateContentVersionName(ContentVersion contentVersion)
        {
            int index = 0;
            String sqlCommand;
            sqlCommand = "Select Count(CV_ID) From ContentVersion where CV_Name = '" + DBprovider.UpdateStringForSqlFormat(contentVersion.Name) + "' and CV_id_Content = " + contentVersion.ParentID;

            if (Locator.DBprovider.ExecuteScalarCommand(sqlCommand) == 0)
                return;

            do
            {
                index++;
                sqlCommand = "Select Count(CV_ID) From ContentVersion where CV_Name = '" + DBprovider.UpdateStringForSqlFormat(contentVersion.Name) + index + "' and CV_id_Content = " + contentVersion.ParentID;

            } while (Locator.DBprovider.ExecuteScalarCommand(sqlCommand) > 0);

            contentVersion.Name = contentVersion.Name + index;
        }

        private void AddContentVersionFilesOnFs(ContentVersion contentVersion, ICopyFilesProgress progressBar)
        {
            int i = 0;
            string destinationFullPath;
            string destinationDirectoryPath;

            String relativePath;
            String folderName = FileSystemUpdater.CreateFolder(contentVersion, out relativePath);
            contentVersion.Path = new PathFS() { Type = PathType.Relative, Name = relativePath };

            if (progressBar != null)
            {
                progressBar.Init(contentVersion.Files.Count);
                progressBar.Show();
            }

            foreach (KeyValuePair<int, ContentFile> file in contentVersion.Files)
            {
                i++;
                file.Value.GetDestinationPath(folderName, out destinationFullPath, out destinationDirectoryPath);

                if (progressBar != null)
                {
                    progressBar.IncreaseProgress(file.Value.FileFullPath, destinationFullPath, i);

                    if (progressBar.Canceled)
                        throw new TraceException("Aborted by user", false, null, Locator.ApplicationName);
                }

                FileSystemUpdater.AddFile(file.Value.FileFullPath, destinationFullPath, destinationDirectoryPath);
                file.Value.UpdateFileFullPath(contentVersion.Path);
            }

            if (progressBar != null)
                progressBar.Close();
        }

        private void AddContentVersionData(ContentVersion contentVersion)
         {
             String sqlCommand;
             LastUpdateUtil.UpdateObjectLastUpdate(contentVersion);
            
             sqlCommand = "Insert Into ";
             sqlCommand += "ContentVersion (";
             sqlCommand += "CV_Name, ";
             sqlCommand += "CV_LastUpdateUser, ";
             sqlCommand += "CV_LastUpdateComputer, ";
             sqlCommand += "CV_LastUpdateApplication, ";
             sqlCommand += "CV_LastUpdateTime, ";
             sqlCommand += "CV_id_ContentVersionStatus, ";
             sqlCommand += "CV_Description, ";
             sqlCommand += "CV_ECR, ";
             sqlCommand += "CV_DocumentID, ";
             sqlCommand += "CV_id_Content, ";
             sqlCommand += "CV_Path, ";
             sqlCommand += "CV_ChildNumber, ";
             sqlCommand += "CV_id_PathType, ";
             sqlCommand += "CV_LockWithDescription, ";
             sqlCommand += "CV_CommandLine) ";
             sqlCommand += "OUTPUT INSERTED.CV_ID ";
             sqlCommand += "Values (";
             sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(contentVersion.Name) + "', ";
             sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(contentVersion.LastUpdateUser) + "', ";
             sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(contentVersion.LastUpdateComputer) + "', ";
             sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(contentVersion.LastUpdateApplication) + "', ";
             sqlCommand += "'" + DBprovider.ConvertTimeToStringFormat(contentVersion.LastUpdateTime) + "', ";
             sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(contentVersion.Status.ID) + "', ";
             sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(contentVersion.Description) + "', ";
             sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(contentVersion.ECR) + "', ";
             sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(contentVersion.DocumentID) + "', ";
             sqlCommand += contentVersion.ParentID + ", ";
             sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(contentVersion.Path.Name) + "', ";
             sqlCommand += contentVersion.ChildID + ", ";

            switch (contentVersion.Path.Type)
            {
                case PathType.Full:
                    sqlCommand += "'Full', ";
                    break;

                case PathType.Relative:
                    sqlCommand += "'Rel', ";
                    break;
            }

             sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(contentVersion.Editor) + "', ";
             sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(contentVersion.RunningString) + "')";

             contentVersion.ID = Locator.DBprovider.ExecuteScalarCommand(sqlCommand);
         }

        private void AddContentVersionVersionLink(ContentVersion contentVersion)
        {
            foreach (KeyValuePair<int, ContentVersionSubVersion> versionLink in contentVersion.ContentVersions)
                AddContentVersionLink(contentVersion, versionLink.Value);
        }

        public static void AddContentVersionLink(ContentVersion contentVersion, ContentVersionSubVersion versionLink)
        {
            String sqlCommand;
            LastUpdateUtil.UpdateObjectLastUpdate(versionLink);

            sqlCommand = "Insert Into  ";
            sqlCommand += "ContentVersionVersionLink (";
            sqlCommand += "CVVL_LastUpdateUser, ";
            sqlCommand += "CVVL_LastUpdateComputer, ";
            sqlCommand += "CVVL_LastUpdateApplication, ";
            sqlCommand += "CVVL_LastUpdateTime, ";
            sqlCommand += "CVVL_ChildNumber, ";
            sqlCommand += "CVVL_id_ContentVersion_Parent, ";
            sqlCommand += "CVVL_id_ContentVersion_Link) ";
            sqlCommand += "Values (";
            sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(versionLink.LastUpdateUser) + "', ";
            sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(versionLink.LastUpdateComputer) + "', ";
            sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(versionLink.LastUpdateApplication) + "', ";
            sqlCommand += "'" + DBprovider.ConvertTimeToStringFormat(versionLink.LastUpdateTime) + "', ";
            sqlCommand += versionLink.Order + ", ";
            sqlCommand += contentVersion.ID + ", ";
            sqlCommand += versionLink.ContentSubVersion.ID + ")";

            Locator.DBprovider.ExecuteCommand(sqlCommand);
        }    

        private void AddContentVersionFiles(ContentVersion contentVersion)
        {
            Dictionary<int, ContentFile> filesWithUpdatedIndex = new Dictionary<int, ContentFile>();

            foreach (KeyValuePair<int, ContentFile> file in contentVersion.Files)
            {
                AddContentVersionFile(contentVersion, file.Value);
                filesWithUpdatedIndex.Add(file.Value.ID, file.Value);
            }

            contentVersion.Files = filesWithUpdatedIndex;
        }

        public static void AddContentVersionFile(ContentVersion contentVersion, ContentFile file)
        {
            String sqlCommand;
            LastUpdateUtil.UpdateObjectLastUpdate(file);

            sqlCommand = "Insert Into  ";
            sqlCommand += "ContentVersionFile (";
            sqlCommand += "CVF_LastUpdateUser, ";
            sqlCommand += "CVF_LastUpdateComputer, ";
            sqlCommand += "CVF_LastUpdateApplication, ";
            sqlCommand += "CVF_LastUpdateTime, ";
            sqlCommand += "CVF_Name, ";
            sqlCommand += "CVF_Path, ";
            sqlCommand += "CVF_id_ContentVersion) ";
            sqlCommand += "OUTPUT INSERTED.CVF_ID ";
            sqlCommand += "Values (";
            sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(file.LastUpdateUser) + "', ";
            sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(file.LastUpdateComputer) + "', ";
            sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(file.LastUpdateApplication) + "', ";
            sqlCommand += "'" + DBprovider.ConvertTimeToStringFormat(file.LastUpdateTime) + "', ";
            sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(file.FileName) + "', ";
            sqlCommand += "'" + DBprovider.UpdateStringForSqlFormat(file.FileRelativePath) + "', ";
            sqlCommand += contentVersion.ID + ")";

            file.ID = Locator.DBprovider.ExecuteScalarCommand(sqlCommand);
        }        
    }
}
