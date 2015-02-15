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
using System.ComponentModel;
using System.IO;


namespace ATSBusinessObjects.ContentMgmtModels
{
    [Serializable(), Table(TableName = "Content")]
    public class CMContentModel : CMTreeNode
    {

        #region  State Properties

        private long _Id = -1;
        [Column(ColName = "CO_ID", DbType = DbType.Int32, IsPK = true)]
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
        [Column(ColName = "CO_ChildNumber", DbType = DbType.Int32)]
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

        private long _Id_ContentTree = -1;
        [Column(ColName = "CO_id_ContentTree", DbType = DbType.Int32)]
        public long Id_ContentTree
        {
            get
            {
                return _Id_ContentTree;
            }
            set
            {
                _Id_ContentTree = value;
            }
        }

        private string _Id_ContentType = "AN";
        [Column(ColName = "CO_id_ContentType", DbType = DbType.String)]
        public string Id_ContentType
        {
            get
            {
                return _Id_ContentType;
            }
            set
            {
                _Id_ContentType = value;
            }
        }

        private CMContentTypeModel _ContentType;
        public CMContentTypeModel ContentType
        {
            get
            {
                return _ContentType;
            }
            set
            {
                _ContentType = value;
            }
        }

        private Boolean _CertificateFree;
        [Column(ColName = "CO_ExtATRInd", DbType = DbType.String)]
        public Boolean CertificateFree
        {
            get
            {
                return _CertificateFree;
            }
            set
            {
                _CertificateFree = value;
            }
        }

        private Boolean _ATRInd;
        [Column(ColName = "CO_ExtATRInd", DbType = DbType.Byte)]
        public Boolean ATRInd
        {
            get
            {
                return _ATRInd;
            }
            set
            {
                _ATRInd = value;
            }
        }

        private string _ContentName = "";
        [Column(ColName = "CO_Name", DbType = DbType.String)]
        public string ContentName
        {
            get
            {
                return _ContentName;
            }
            set
            {
                _ContentName = value;
            }
        }

        private string _Description = "";
        [Column(ColName = "CO_Description", DbType = DbType.String)]
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

        private string _IconPath = "";
        [Column(ColName = "CO_Icon", DbType = DbType.String)]
        public string IconPath
        {
            get
            {
                return _IconPath;
            }
            set
            {
                _IconPath = value;
            }
        }

        private DateTime _LastUpdateTime;
        [Column(ColName = "CO_LastUpdateTime", DbType = DbType.DateTime)]
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
        [Column(ColName = "CO_LastUpdateUser", DbType = DbType.String)]
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
        [Column(ColName = "CO_LastUpdateComputer", DbType = DbType.String)]
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
        [Column(ColName = "CO_LastUpdateApplication", DbType = DbType.String)]
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
        [Column(ColName = "CO_CreationDate", DbType = DbType.DateTime)]
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

        private Dictionary<int, CMVersionModel> _Versions = new Dictionary<int, CMVersionModel>();
        public Dictionary<int, CMVersionModel> Versions
        {
            get
            {
                return _Versions;
            }
            set
            {
                _Versions = value;
            }
        }

        #endregion

        #region  Constructor

        public CMContentModel()
        {
            TreeNodeType = TreeNodeObjectType.Content;
        }

        #endregion

        #region Methods

        public override bool Contains(string text)
        {
            return Name.Contains(text) || Description.Contains(text) || IconPath.Contains(text);
        }

        #endregion

    }
}
