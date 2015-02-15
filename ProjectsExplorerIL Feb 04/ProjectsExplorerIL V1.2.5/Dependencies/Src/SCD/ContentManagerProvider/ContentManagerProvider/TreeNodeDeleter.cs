using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using ContentManagerProvider.General;
using TraceExceptionWrapper;

namespace ContentManagerProvider
{
    class TreeNodeDeleter
    {
         internal void Delete(TreeNode treeNode)
         {
             try
             {
                 Locator.DBprovider.OpenConnection();
                 Locator.DBprovider.BeginTransaction();

                 SelectObjectsForUpdate.Select(treeNode);

                 switch (treeNode.TreeNodeType)
                 {
                     case TreeNodeObjectType.Folder:
                         DeleteFolder((Folder)treeNode);
                         break;
                     case TreeNodeObjectType.Content:
                         Locator.Impersonation.Logon();
                         DeleteContent((Content)treeNode);
                         Locator.Impersonation.Dispose();
                         break;
                     case TreeNodeObjectType.ContentVersion:
                         Locator.Impersonation.Logon();
                         DeleteContentVersion((ContentVersion)treeNode);
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

         internal void DeleteFolder(Folder folder)
         {
             if(GetFolderChildCount(folder) > 0)
                 throw new TraceException("Folder delete with subitems", true, null, Locator.ApplicationName);

             DeleteFolderUserGroupType(folder);
             DeleteFolderData(folder);
         }

         internal int GetFolderChildCount(Folder folder)
         {
             int subFolders;
             int subContents;
             String sqlCommand;

             sqlCommand = "Select Count(CT_ID) from ";
             sqlCommand += "ContentTree ";
             sqlCommand += "Where ";
             sqlCommand += "CT_ParentID = " + folder.ID;
             subFolders = Locator.DBprovider.ExecuteScalarCommand(sqlCommand);

             sqlCommand = "Select Count(CO_ID) from ";
             sqlCommand += "Content ";
             sqlCommand += "Where ";
             sqlCommand += "CO_id_ContentTree = " + folder.ID;
             subContents = Locator.DBprovider.ExecuteScalarCommand(sqlCommand);

             return subFolders + subContents;
         }         

         internal void DeleteFolderData(Folder folder)
         {
             String sqlCommand;

             sqlCommand = "delete from ";
             sqlCommand += "ContentTree ";
             sqlCommand += "Where ";
             sqlCommand += "CT_ID = " + folder.ID;

             Locator.DBprovider.ExecuteCommand(sqlCommand);
         }

        private void DeleteFolderUserGroupType(Folder folder)
        {
            String sqlCommand;

            sqlCommand = "delete from ";
            sqlCommand += "ContentTreeUserGroupType ";
            sqlCommand += "Where ";
            sqlCommand += "CTUGT_id_ContentTree = " + folder.ID;

            Locator.DBprovider.ExecuteCommand(sqlCommand);
        }
        
         internal void DeleteContentVersion(ContentVersion contentVersion)
         {

             CheckVersionLinkedVersion(contentVersion);
             DeleteContentVersionSubVersions(contentVersion);
             DeleteContentVersionFilesDb(contentVersion);
             DeleteContentVersionData(contentVersion);
             DeleteContentVersionFilesFs(contentVersion);
         }

         internal void CheckVersionLinkedVersion(ContentVersion contentVersion)
         {
             DataTable dt;
             String sqlCommand;

             sqlCommand = "SELECT [Content].CO_Name AS Content, ContentVersion.CV_Name AS ContentVersion ";
             sqlCommand += "FROM ContentVersionVersionLink INNER JOIN ";
             sqlCommand += "ContentVersion ON ContentVersionVersionLink.CVVL_id_ContentVersion_Parent = ContentVersion.CV_ID INNER JOIN ";     
             sqlCommand += "[Content] ON ContentVersion.CV_id_Content = [Content].CO_ID ";
             sqlCommand += "WHERE (ContentVersionVersionLink.CVVL_id_ContentVersion_Link = " + contentVersion.ID + ") ";
             dt = Locator.DBprovider.ExecuteSelectCommand(sqlCommand);

             if (dt.Rows.Count > 0)
                 throw new TraceException("Version linked deleted", true, new List<string>() { (String)dt.Rows[0]["Content"], (String)dt.Rows[0]["ContentVersion"] }, Locator.ApplicationName);
         } 

         private void DeleteContentVersionSubVersions(ContentVersion contentVersion)
        {
            String sqlCommand;

            sqlCommand = "delete from ";
            sqlCommand += "ContentVersionVersionLink ";
            sqlCommand += "Where ";
            sqlCommand += "CVVL_id_ContentVersion_Parent = " + contentVersion.ID;

            Locator.DBprovider.ExecuteCommand(sqlCommand);
        }

        private void DeleteContentVersionFilesDb(ContentVersion contentVersion)
        {
            String sqlCommand;

            sqlCommand = "delete from ";
            sqlCommand += "ContentVersionFile ";
            sqlCommand += "Where ";
            sqlCommand += "CVF_id_ContentVersion = " + contentVersion.ID;

            Locator.DBprovider.ExecuteCommand(sqlCommand);
        }

        private void DeleteContentVersionData(ContentVersion contentVersion)
        {
            String sqlCommand;

            sqlCommand = "delete from ";
            sqlCommand += "ContentVersion ";
            sqlCommand += "Where ";
            sqlCommand += "CV_ID = " + contentVersion.ID;

            Locator.DBprovider.ExecuteCommand(sqlCommand);
        }

        internal void DeleteContentVersionFilesFs(ContentVersion contentVersion)
         {
             try
             {
                 switch (contentVersion.Path.Type)
                 {
                     case PathType.Relative:
                         FileSystemUpdater.DeleteFolder(Locator.SystemParameters["RootPathFS"] + "\\" + contentVersion.Path.Name);
                         break;
                     case PathType.Full:
                         FileSystemUpdater.DeleteFolder(contentVersion.Path.Name);
                         break;
                 }
             }
             catch { }
         }

         internal void DeleteContent(Content content)
         {
             if (GetContentChildCount(content) > 0)
                 throw new TraceException("Content delete with subitems", true, null, Locator.ApplicationName);

             DeleteContentData(content);
             DeleteContentFiles(content);
         }

         internal int GetContentChildCount(Content content)
         {
             String sqlCommand;

             sqlCommand = "Select Count(CV_ID) from ";
             sqlCommand += "ContentVersion ";
             sqlCommand += "Where ";
             sqlCommand += "CV_id_Content = " + content.ID;
             return Locator.DBprovider.ExecuteScalarCommand(sqlCommand);
         }         

         internal void DeleteContentFiles(Content content)
         {
             try
             {

                 string folderFullPath = Path.GetDirectoryName(content.IconFileFullPath);

                 FileSystemUpdater.DeleteFile(content.IconFileFullPath);
                 FileSystemUpdater.DeleteFolder(folderFullPath);
             }
             catch {}
         }

         internal void DeleteContentData(Content content)
         {
             String sqlCommand;

             sqlCommand = "delete from ";
             sqlCommand += "Content ";
             sqlCommand += "Where ";
             sqlCommand += "CO_ID = " + content.ID;

             Locator.DBprovider.ExecuteCommand(sqlCommand);
         }
    }
}
