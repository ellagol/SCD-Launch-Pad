using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentManagerProvider
{
    public enum TreeNodeObjectType
    {
        Folder,
        Content,
        ContentVersion
    } 

    public class TreeNode : LastUpdate
    {
        public int ID { get; set; }
        public String Name { get; set; }
        public int ChildID { get; set; }
        public int ParentID { get; set; }
        public TreeNodeObjectType TreeNodeType { get; set; }
        public Dictionary<String, bool> Permission { get; set; }

        public bool ExistPermission(String permissionID)
        {
            if (Permission == null)
                return false;

            return Permission.ContainsKey(permissionID) && Permission[permissionID];
        }

        public virtual bool Contains(string text)
        {
            throw new NotImplementedException();
        }
    }
}
