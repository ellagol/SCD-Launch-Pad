using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using Infra.Domain;
using System.Collections.ObjectModel;

namespace ATSBusinessObjects.ContentMgmtModels
{

    #region  Tree Node Object Type Enum

    [Serializable]
    public enum TreeNodeObjectType
    {
        Root,
        Folder,
        Content,
        ContentVersion
    }

    #endregion
  
    public class CMTreeNode : BusinessObjectBase 
    {

        #region Properties

        public int ID { get; set; }
        public String Name { get; set; }
        public int ChildID { get; set; }
        public int ParentID { get; set; }
        public TreeNodeObjectType TreeNodeType { get; set; }
        public Dictionary<String, bool> Permission { get; set; }

        #endregion

        #region Exist Permission

        public bool ExistPermission(String permissionID)
        {
            if (Permission == null)
                return false;

            return Permission.ContainsKey(permissionID) && Permission[permissionID];
        }

        #endregion

        #region Methods

        public virtual bool Contains(string text)
        {
            return Name.Contains(text);
            //throw new NotImplementedException();
        }

        #endregion
    }

  
}
