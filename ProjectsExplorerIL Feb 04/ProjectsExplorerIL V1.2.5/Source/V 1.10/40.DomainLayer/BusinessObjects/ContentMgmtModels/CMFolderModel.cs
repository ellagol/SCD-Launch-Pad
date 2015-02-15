using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using System.Data;
using System.Reflection;
using Infra.Domain;
using System.Collections.ObjectModel;

namespace ATSBusinessObjects.ContentMgmtModels
{
    [Serializable(), Table(TableName = "ContentTree")]
    public class CMFolderModel : CMTreeNode
    {

        #region  State Properties

        private long _Id = -1;
        [Column(ColName = "CT_ID", DbType = DbType.Int32, IsPK = true)]
        public long Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }

        private long _ChildNumber = -1;
        [Column(ColName = "CT_ChildNumber", DbType = DbType.Int32)]
        public long ChildNumber
        {
            get
            {
                return _ChildNumber;
            }
            set
            {
                _ChildNumber = value;
            }
        }

        private long _ParentId = -1;
        [Column(ColName = "CT_ParentID", DbType = DbType.Int32)]
        public long ParentId
        {
            get
            {
                return _ParentId;
            }
            set
            {
                _ParentId = value;
            }
        }

        private string _FolderName = "";
        [Column(ColName = "CT_Name", DbType = DbType.String)]
        public string FolderName
        {
            get
            {
                return _FolderName;
            }
            set
            {
                _FolderName = value;
            }
        }

        private string _Description = "";
        [Column(ColName = "CT_Description", DbType = DbType.String)]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }

        private string _DefaultVNPrefix = "";
        [Column(ColName = "CT_DefaultVNPrefix", DbType = DbType.String)]
        public string DefaultVNPrefix
        {
            get
            {
                return _DefaultVNPrefix;
            }
            set
            {
                _DefaultVNPrefix = value;
            }
        }

        private string _DefaultVNStartValue = "";
        [Column(ColName = "CT_DefaultVNStartValue", DbType = DbType.String)]
        public string DefaultVNStartValue
        {
            get
            {
                return _DefaultVNStartValue;
            }
            set
            {
                _DefaultVNStartValue = value;
            }
        }

        private string _DefaultVNIncrement = "";
        [Column(ColName = "CT_DefaultVNIncrement", DbType = DbType.String)]
        public string DefaultVNIncrement
        {
            get
            {
                return _DefaultVNIncrement;
            }
            set
            {
                _DefaultVNIncrement = value;
            }
        }

        private DateTime _LastUpdateTime;
        [Column(ColName = "CT_LastUpdateTime", DbType = DbType.DateTime)]
        public DateTime LastUpdateTime
        {
            get
            {
                return _LastUpdateTime;
            }
            set
            {
                _LastUpdateTime = value;
            }
        }

        private string _LastUpdateUser = "";
        [Column(ColName = "CT_LastUpdateUser", DbType = DbType.String)]
        public string LastUpdateUser
        {
            get
            {
                return _LastUpdateUser;
            }
            set
            {
                _LastUpdateUser = value;
            }
        }

        private string _LastUpdateComputer = "";
        [Column(ColName = "CT_LastUpdateComputer", DbType = DbType.String)]
        public string LastUpdateComputer
        {
            get
            {
                return _LastUpdateComputer;
            }
            set
            {
                _LastUpdateComputer = value;
            }
        }

        private string _LastUpdateApplication = "";
        [Column(ColName = "CT_LastUpdateApplication", DbType = DbType.String)]
        public string LastUpdateApplication
        {
            get
            {
                return _LastUpdateApplication;
            }
            set
            {
                _LastUpdateApplication = value;
            }
        }

        private DateTime _CreationDate;
        [Column(ColName = "CT_CreationDate", DbType = DbType.DateTime)]
        public DateTime CreationDate
        {
            get
            {
                return _CreationDate;
            }
            set
            {
                _CreationDate = value;
            }
        }

        private Boolean _UpdateMode = false;
        public Boolean UpdateMode
        {
            get
            {
                return _UpdateMode;
            }
            set
            {
                _UpdateMode = value;
            }
        }

        private List<CMTreeNode> _Nodes = new List<CMTreeNode>();
        public List<CMTreeNode> Nodes 
        {
            get
            {
                return _Nodes;
            }
            set
            {
                _Nodes = value;
            }
        }

        private Dictionary<String, CMFolderUserGroupTypeModel> _UserGroupTypePermission = new Dictionary<string, CMFolderUserGroupTypeModel>();
        public Dictionary<String, CMFolderUserGroupTypeModel> UserGroupTypePermission
        {
            get
            {
                return _UserGroupTypePermission;
            }
            set
            {
                _UserGroupTypePermission = value;
            }
        }

        #endregion

        #region  Constructor

        public CMFolderModel()
        {
            TreeNodeType = TreeNodeObjectType.Folder;
        }

        #endregion

        #region Methods

        public override bool Contains(string text)
        {
            return Name.Contains(text) || Description.Contains(text);
        }

        #endregion
    }
    
}
