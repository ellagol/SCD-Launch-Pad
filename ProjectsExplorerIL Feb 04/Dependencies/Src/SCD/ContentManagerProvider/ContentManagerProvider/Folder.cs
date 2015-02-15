using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentManagerProvider
{
    public class Folder : TreeNode
    {
        public Folder()
        {
            TreeNodeType = TreeNodeObjectType.Folder;
        }

        public override bool Contains(string text)
        {
            return Name.Contains(text) || Description.Contains(text);
        }

        public String Description { get; set; }
        public List<TreeNode> Nodes { get; set; }
        public Dictionary<String, FolderUserGroupType> UserGroupTypePermission { get; set; }

        public Folder Clone()
        {
            Folder folderNew = new Folder
            {
                Name = Name,
                Description = Description,
                ID = ID,
                ChildID = ChildID,
                ParentID = ParentID,
                Nodes = new List<TreeNode>(),
                UserGroupTypePermission = new Dictionary<String, FolderUserGroupType>()
            };

            foreach (KeyValuePair<string, FolderUserGroupType> userGroupType in UserGroupTypePermission)
            {
                FolderUserGroupType userGroupTypeClone = new FolderUserGroupType { UserGroupType = userGroupType.Value.UserGroupType };
                UpdateLastUpdate(userGroupType.Value, userGroupTypeClone);
                folderNew.UserGroupTypePermission.Add(userGroupType.Key, userGroupTypeClone);
            }
            UpdateLastUpdate(this, folderNew);
            return folderNew;
        }
    }
}
