using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ContentManagerProvider.General;
using ProjectExplorerTester;
using DatabaseProvider;

namespace ContentManagerProvider
{
    public class UpdatePermission
    {

        private String UserGroupType { get; set; }
        private Dictionary<int, Folder> Folders { get; set; }
        private Dictionary<int, Content> Contents { get; set; }
        private Dictionary<int, ContentVersion> Versions { get; set; }
        private  ProjectExplorerApi ProjectExplorer { get; set; }

        private bool ApplicationPermissionUpdateUserGroup { get; set; }
        private bool ApplicationPermissionUpdateCheckouted { get; set; }
        private bool ApplicationPermissionChangeRetirement { get; set; }
        private bool ApplicationPermissionUpdateAllUserGroups { get; set; }

        public UpdatePermission(Dictionary<int, Folder> folders, Dictionary<int, Content> contents, Dictionary<int, ContentVersion> versions)
        {
            Folders = folders;
            Contents = contents;
            Versions = versions;
            UserGroupType = UserGroupTypesReader.GetUserGroupType();
            List<String> applicationPermission = Locator.ProfileManagerProvider.GetApplicationPermission(Locator.UserName);
            ProjectExplorer = new ProjectExplorerApi(Locator.UserName, Locator.ComputerName, Locator.ApplicationName, Locator.DBprovider.Connection, Locator.DBprovider.Transaction);

            ApplicationPermissionUpdateCheckouted = applicationPermission.Contains("UpdateEditor");
            ApplicationPermissionUpdateUserGroup = applicationPermission.Contains("Update user group");
            ApplicationPermissionChangeRetirement = applicationPermission.Contains("ChangeRetirement");
            ApplicationPermissionUpdateAllUserGroups = applicationPermission.Contains("Update all user groups");
        }

        public void UpdateTreeNodeRecursive(List<TreeNode> treeNodesList)
        {
            foreach (TreeNode treeNode in treeNodesList)
                UpdateTreeNodeRecursive(treeNode);
        }

        public void UpdateTreeNodeRecursive(TreeNode treeNode)
        {
            UpdateTreeNode(treeNode);

            switch (treeNode.TreeNodeType)
            {
                case TreeNodeObjectType.Folder:
                    foreach (TreeNode node in ((Folder) treeNode).Nodes)
                        UpdateTreeNodeRecursive(node);
                    break;

                case TreeNodeObjectType.Content:
                    foreach (KeyValuePair<int, ContentVersion> node in ((Content) treeNode).Versions)
                        UpdateTreeNodeRecursive(node.Value);
                    break;
            }
        }

        public void UpdateTreeNode(TreeNode treeNode)
        {
            UpdateTreeNodePermission(treeNode);
        }

        private void UpdateTreeNodePermission(TreeNode node)
        {
            if (!ApplicationPermissionUpdateAllUserGroups)
            {
                Folder parentFolder = TreeNodeParentFolder(node);
                Dictionary<String, FolderUserGroupType> userGroupTypePermission = GetUserGroupTypePermission(parentFolder);

                if (!userGroupTypePermission.ContainsKey(UserGroupType))
                {
                    node.Permission = new Dictionary<String, bool>();
                    return;
                }
            }

            switch (node.TreeNodeType)
            {
                case TreeNodeObjectType.Folder:
                    GetContentTreesPermission(node);
                    break;
                case TreeNodeObjectType.Content:
                    GetContentPermission(node);
                    break;
                case TreeNodeObjectType.ContentVersion:
                    GetContentVersionPermission((ContentVersion)node);
                    break;
            }
        }

        private Dictionary<String, FolderUserGroupType> GetUserGroupTypePermission(Folder folder)
        {
            while (folder != null)
            {
                if (folder.UserGroupTypePermission.Count > 0)
                    return folder.UserGroupTypePermission;

                folder = folder.ParentID == 0 ? null : Folders[folder.ParentID];
            }

            return new Dictionary<String, FolderUserGroupType>();
        }

        private Folder TreeNodeParentFolder(TreeNode node)
        {

            switch (node.TreeNodeType)
            {
                case TreeNodeObjectType.Folder:
                    return (Folder)node;

                case TreeNodeObjectType.Content:
                    return node.ParentID == 0 ? null : Folders[node.ParentID];

                case TreeNodeObjectType.ContentVersion:
                    int folderID = Contents[node.ParentID].ParentID;
                    return folderID == 0 ? null : Folders[folderID];
            }

            return null;
        }

        private void GetContentPermission(TreeNode node)
        {
            Dictionary<String, bool> objectPermission = new Dictionary<String, bool> {{"Add", true}, {"Update", true}};

            if (((Content) node).Versions.Count == 0)
                objectPermission.Add("Delete", true);

            node.Permission = objectPermission;
        }

        private void GetContentTreesPermission(TreeNode node)
        {
            Dictionary<String, bool> objectPermission = new Dictionary<String, bool> {{"Add", true}, {"Update", true}};

            if (ApplicationPermissionUpdateUserGroup)
                objectPermission.Add("UpdateUserGroup", true);

            if (((Folder)node).Nodes.Count == 0)
                    objectPermission.Add("Delete", true);

            node.Permission = objectPermission;
        }

        private void GetContentVersionPermission(ContentVersion node)
        {
            Dictionary<String, bool> objectPermission = new Dictionary<String, bool>();

            if(node.Path.Type == PathType.Full)
            {
                node.Permission = objectPermission;
                return;
            }

            if (!String.IsNullOrEmpty(node.Editor) && node.LastUpdateUser != Locator.UserName)
            {
                if (ApplicationPermissionUpdateCheckouted)
                {
                    objectPermission.Add("Update", true);
                    objectPermission.Add("UpdateEditor", true);
                }

                node.Permission = objectPermission;
                return;                    
            }

            if (ProjectExplorer.GetProjectsByContentVersionID(node.ID).Count == 0 && !ContentVersionLinked(node))
                objectPermission.Add("Delete", true); 

            if (node.Status.ID != "Ret")
            {
                objectPermission.Add("Update", true);
                objectPermission.Add("UpdateProperty", true);

                objectPermission.Add("UpdateStatus", true);

                if (node.Status.ID == "Edit")
                {
                    objectPermission.Add("UpdateEditor", true);
                    objectPermission.Add("UpdateData", true);
                }
            }
            else
            {
                if (ApplicationPermissionChangeRetirement)
                {
                    objectPermission.Add("Update", true);
                    objectPermission.Add("UpdateStatus", true);
                }
            }

            node.Permission = objectPermission;
        }

        private bool ContentVersionLinked(ContentVersion node)
        {
            foreach (KeyValuePair<int, ContentVersion> contentVersion in Versions)
            {
                foreach (KeyValuePair<int, ContentVersionSubVersion> contentVersionSubVersion in contentVersion.Value.ContentVersions)
                {
                    if (contentVersionSubVersion.Value.ContentSubVersion.ID == node.ID)
                        return true;
                }
            }

            return false;
        }
    }
}
