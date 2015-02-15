using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ContentManagerProvider.General;
using TraceExceptionWrapper;

namespace ContentManagerProvider
{
    internal static class SelectObjectsForUpdate
    {

        internal static void SelectParent(TreeNode treeNodes)
        {

            if (treeNodes.ParentID == 0)
                return;

            switch (treeNodes.TreeNodeType)
            {
                case TreeNodeObjectType.Folder:
                case TreeNodeObjectType.Content:
                    SelectParentFolder(treeNodes.ParentID);
                    break;

                case TreeNodeObjectType.ContentVersion:
                    SelectParentContent(treeNodes.ParentID);
                    break;
            }
        }

        internal static void Select(TreeNode treeNodes)
        {
             switch (treeNodes.TreeNodeType)
             {
                 case TreeNodeObjectType.Folder:
                     SelectFolder((Folder)treeNodes);
                     break;
                 case TreeNodeObjectType.Content:
                     SelectContent((Content)treeNodes);
                     break;
                 case TreeNodeObjectType.ContentVersion:
                     SelectContentVersion((ContentVersion)treeNodes);
                     break;
             }
        }

        internal static void SelectContentVersionLinked(ContentVersion contentVersionNode)
        {
            foreach (KeyValuePair<int, ContentVersionSubVersion> contentVersionSubVersion in contentVersionNode.ContentVersions)
                SelectContentVersion(contentVersionSubVersion.Value.ContentSubVersion);
        }

        private static void SelectParentFolder(int folderID)
         {
             DataTable dt;
             String sqlCommand;

             sqlCommand = "Select ";
             sqlCommand += "CT_LastUpdateTime as UpdateTime ";
             sqlCommand += "From ";
             sqlCommand += "ContentTree ";
             sqlCommand += "with(UPDLOCK) ";
             sqlCommand += "Where CT_ID = " + folderID;

             dt = Locator.DBprovider.ExecuteSelectCommand(sqlCommand);

             if (dt.Rows.Count == 0)
                 throw new TraceException("Parent folder deleted", true, null, Locator.ApplicationName);
         }

         private static void SelectParentContent(int contentID)
         {
             DataTable dt;
             String sqlCommand;

             sqlCommand = "Select ";
             sqlCommand += "CO_LastUpdateTime as UpdateTime, ";
             sqlCommand += "CO_LastUpdateUser as UpdateUser ";
             sqlCommand += "From ";
             sqlCommand += "Content ";
             sqlCommand += "with(UPDLOCK) ";
             sqlCommand += "Where CO_ID = " + contentID; ;

             dt = Locator.DBprovider.ExecuteSelectCommand(sqlCommand);

             if (dt.Rows.Count == 0)
                 throw new TraceException("Parent content deleted", true, null, Locator.ApplicationName);
         }

         private static void SelectFolder(Folder folder)
         {
             DataTable dt;
             String sqlCommand;

             sqlCommand = "Select ";
             sqlCommand += "CT_LastUpdateTime as UpdateTime, ";
             sqlCommand += "CT_LastUpdateUser as UpdateUser ";
             sqlCommand += "From ";
             sqlCommand += "ContentTree ";
             sqlCommand += "with(UPDLOCK) ";
             sqlCommand += "Where CT_ID = " + folder.ID;

             dt = Locator.DBprovider.ExecuteSelectCommand(sqlCommand);

             if (dt.Rows.Count == 0)
                 throw new TraceException("Folder deleted", true, new List<string>() { folder.Name }, Locator.ApplicationName);

             if (!CompareUpdateTime(((DateTime) dt.Rows[0]["UpdateTime"]), folder.LastUpdateTime))
                 throw new TraceException("Folder changed", true, new List<string>() {folder.Name, (String)dt.Rows[0]["UpdateUser"] }, Locator.ApplicationName);

             foreach (KeyValuePair<String, FolderUserGroupType> userGroupType in folder.UserGroupTypePermission)
                 SelectFolderUserGroupType(userGroupType.Value, folder);
         }

         private static void SelectFolderUserGroupType(FolderUserGroupType userGroupType, Folder folder)
        {
            DataTable dt;
            String sqlCommand;

            sqlCommand = "Select ";
            sqlCommand += "CTUGT_LastUpdateTime as UpdateTime, ";
            sqlCommand += "CTUGT_LastUpdateUser as UpdateUser ";
            sqlCommand += "From ";
            sqlCommand += "ContentTreeUserGroupType ";
            sqlCommand += "with(UPDLOCK) ";
            sqlCommand += "Where CTUGT_id_ContentTree = " + folder.ID + " and CTUGT_id_UserGroupType = '" + userGroupType.UserGroupType.ID + "'";

            dt = Locator.DBprovider.ExecuteSelectCommand(sqlCommand);

            if (dt.Rows.Count == 0 || !CompareUpdateTime(((DateTime)dt.Rows[0]["UpdateTime"]), userGroupType.LastUpdateTime))
                throw new TraceException("Folder changed", true, new List<string>() { folder.Name, (String)dt.Rows[0]["UpdateUser"] }, Locator.ApplicationName);
        }

        private static bool CompareUpdateTime(DateTime first, DateTime secont)
        {

            if (first.Year != secont.Year ||
                first.Month != secont.Month ||
                first.Day != secont.Day ||
                first.Hour != secont.Hour ||
                first.Minute != secont.Minute ||
                first.Second != secont.Second)
                return false;

            return true;
        }

        private static void SelectContent(Content content)
         {
             DataTable dt;
             String sqlCommand;

             sqlCommand = "Select ";
             sqlCommand += "CO_LastUpdateTime as UpdateTime, ";
             sqlCommand += "CO_LastUpdateUser as UpdateUser ";
             sqlCommand += "From ";
             sqlCommand += "Content ";
             sqlCommand += "with(UPDLOCK) ";
             sqlCommand += "Where CO_ID = " + content.ID; ;

             dt = Locator.DBprovider.ExecuteSelectCommand(sqlCommand);

             if (dt.Rows.Count == 0)
                 throw new TraceException("Content deleted", true, new List<string>() { content.Name }, Locator.ApplicationName);

            if (!CompareUpdateTime(((DateTime) dt.Rows[0]["UpdateTime"]), content.LastUpdateTime))
                throw new TraceException("Content changed", true, new List<string>() { content.Name, (String)dt.Rows[0]["UpdateUser"] }, Locator.ApplicationName);
         }
     
         private static void SelectContentVersion(ContentVersion contentVersion)
         {
             DataTable dt;
             String updateUser;
             String sqlCommand;

             sqlCommand = "Select ";
             sqlCommand += "CV_LastUpdateTime as UpdateTime, ";
             sqlCommand += "CV_LastUpdateUser as UpdateUser, ";
             sqlCommand +=
                 "(SELECT COUNT(CVVL_id_ContentVersion_Parent) AS Expr1 FROM ContentVersionVersionLink WHERE (CVVL_id_ContentVersion_Parent = " +
                 contentVersion.ID + ")) AS VersionLinksAmount, ";

             sqlCommand += "(SELECT COUNT(CVF_ID) AS Expr1 FROM ContentVersionFile WHERE (CVF_id_ContentVersion = " +
                           contentVersion.ID + ")) AS FilesAmount ";

             sqlCommand += "From ";
             sqlCommand += "ContentVersion ";
             sqlCommand += "with(UPDLOCK) ";
             sqlCommand += "Where CV_ID = " + contentVersion.ID;

             dt = Locator.DBprovider.ExecuteSelectCommand(sqlCommand);

             if (dt.Rows.Count == 0)
                 throw new TraceException("Version deleted", true, new List<string>() { contentVersion.Name }, Locator.ApplicationName);

             updateUser = (String) dt.Rows[0]["UpdateUser"];

             if (!CompareUpdateTime(((DateTime)dt.Rows[0]["UpdateTime"]), contentVersion.LastUpdateTime))
                 throw new TraceException("Version changed", true, new List<string>() { contentVersion.Name, updateUser }, Locator.ApplicationName);

             if (((int)dt.Rows[0]["FilesAmount"]) != contentVersion.Files.Count)
                 throw new TraceException("Version changed", true, new List<string>() { contentVersion.Name, updateUser }, Locator.ApplicationName);

             if (((int)dt.Rows[0]["VersionLinksAmount"]) != contentVersion.ContentVersions.Count)
                 throw new TraceException("Version changed", true, new List<string>() { contentVersion.Name, updateUser }, Locator.ApplicationName);

             foreach (KeyValuePair<int, ContentFile> file in contentVersion.Files)
                 SelectContentVersionFilse(contentVersion, file.Value, updateUser);

             foreach (KeyValuePair<int, ContentVersionSubVersion> subVersion in contentVersion.ContentVersions)
                 SelectContentVersionVersionLink(contentVersion, subVersion.Value, updateUser);
         }

         private static void SelectContentVersionVersionLink(ContentVersion contentVersion, ContentVersionSubVersion versionLink, String updateUser)
         {
             DataTable dt;
             String sqlCommand;

             sqlCommand = "Select ";
             sqlCommand += "CVVL_LastUpdateTime as UpdateTime, ";
             sqlCommand += "CVVL_LastUpdateUser as UpdateUser ";
             sqlCommand += "From ";
             sqlCommand += "ContentVersionVersionLink ";
             sqlCommand += "with(UPDLOCK) ";
             sqlCommand += "Where CVVL_id_ContentVersion_Parent = " + contentVersion.ID + " and CVVL_id_ContentVersion_Link = " + versionLink.ContentSubVersion.ID;

             dt = Locator.DBprovider.ExecuteSelectCommand(sqlCommand);
             
             if (dt.Rows.Count == 0)
                 throw new TraceException("Version changed", true, new List<string>() { contentVersion.Name, updateUser }, Locator.ApplicationName);

             if (!CompareUpdateTime(((DateTime)dt.Rows[0]["UpdateTime"]), versionLink.LastUpdateTime))
                 throw new TraceException("Version changed", true, new List<string>() { contentVersion.Name, (String)dt.Rows[0]["UpdateUser"] }, Locator.ApplicationName);
         }

         private static void SelectContentVersionFilse(ContentVersion contentVersion, ContentFile contentVersionFile, String updateUser)
        {
            DataTable dt;
            String sqlCommand;

            sqlCommand = "Select ";
            sqlCommand += "CVF_LastUpdateTime as UpdateTime, ";
            sqlCommand += "CVF_LastUpdateUser as UpdateUser ";
            sqlCommand += "From ";
            sqlCommand += "ContentVersionFile ";
            sqlCommand += "with(UPDLOCK) ";
            sqlCommand += "Where CVF_ID = " + contentVersionFile.ID;

            dt = Locator.DBprovider.ExecuteSelectCommand(sqlCommand);

            if (dt.Rows.Count == 0)
                throw new TraceException("Version changed", true, new List<string>() { contentVersion.Name, updateUser }, Locator.ApplicationName);

            if (!CompareUpdateTime(((DateTime)dt.Rows[0]["UpdateTime"]), contentVersionFile.LastUpdateTime))
                throw new TraceException("Version changed", true, new List<string>() { contentVersion.Name, (String)dt.Rows[0]["UpdateUser"] }, Locator.ApplicationName);
        }
    }
}
