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

namespace ATSBusinessObjects.ContentMgmtModels
{
    [Serializable(), Table(TableName = "Content")]
    public class CMVersionModel : CMTreeNode
    {

        #region  Path Type Enum

        public enum PathType
        {
            Relative,
            Full
        }

         #endregion

        #region  State Properties

        private long _Id = -1;
        [Column(ColName = "CV_ID", DbType = DbType.Int32, IsPK = true)]
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
        [Column(ColName = "CV_ChildNumber", DbType = DbType.Int32)]
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

        private string _VersionName = "";
        [Column(ColName = "CV_Name", DbType = DbType.String)]
        public string VersionName
        {
            get
            {
                return _VersionName;
            }
            set
            {
                _VersionName = value;
            }
        }

        private string _Description = "";
        [Column(ColName = "CV_Description", DbType = DbType.String)]
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

        private string _id_ContentVersionStatus = "Edit";
        [Column(ColName = "CV_id_ContentVersionStatus", DbType = DbType.String)]
        public string id_ContentVersionStatus
        {
            get
            {
                return _id_ContentVersionStatus;
            }
            set
            {
                _id_ContentVersionStatus = value;
            }
        }

        private string _ECR = "";
        [Column(ColName = "CV_ECR", DbType = DbType.String)]
        public string ECR
        {
            get
            {
                return _ECR;
            }
            set
            {
                _ECR = value;
            }
        }

        private string _DocumentID = "";
        [Column(ColName = "CV_DocumentID", DbType = DbType.String)]
        public string DocumentID
        {
            get
            {
                return _DocumentID;
            }
            set
            {
                _DocumentID = value;
            }
        }

        private string _PDMDocumentVersion = "";
        [Column(ColName = "CV_PDMDocumentVersion", DbType = DbType.String)]
        public string PDMDocumentVersion
        {
            get
            {
                return _PDMDocumentVersion;
            }
            set
            {
                _PDMDocumentVersion = value;
            }
        }

        private string _ConfigurationManagementLink = "";
        [Column(ColName = "CV_ConfigurationManagementLink", DbType = DbType.String)]
        public string ConfigurationManagementLink
        {
            get
            {
                return _ConfigurationManagementLink;
            }
            set
            {
                _ConfigurationManagementLink = value;
            }
        }

        private long _id_Content = -1;
        [Column(ColName = "CV_id_Content", DbType = DbType.Int32, IsPK = true)]
        public long id_Content
        {
            get
            {
                return _id_Content;
            }
            set
            {
                _id_Content = value;
            }
        }

        private string _id_PathType = "Rel";
        [Column(ColName = "CV_id_PathType", DbType = DbType.String)]
        public string id_PathType
        {
            get
            {
                return _id_PathType;
            }
            set
            {
                _id_PathType = value;
            }
        }

        private string _CommandLine = "";
        [Column(ColName = "CV_CommandLine", DbType = DbType.String)]
        public string CommandLine
        {
            get
            {
                return _CommandLine;
            }
            set
            {
                _CommandLine = value;
            }
        }

        private string _LockWithDescription = "";
        [Column(ColName = "CV_LockWithDescription", DbType = DbType.String)]
        public string LockWithDescription
        {
            get
            {
                return _LockWithDescription;
            }
            set
            {
                _LockWithDescription = value;
            }
        }

        private DateTime _LastUpdateTime;
        [Column(ColName = "CV_LastUpdateTime", DbType = DbType.DateTime)]
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
        [Column(ColName = "CV_LastUpdateUser", DbType = DbType.String)]
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
        [Column(ColName = "CV_LastUpdateComputer", DbType = DbType.String)]
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
        [Column(ColName = "CV_LastUpdateApplication", DbType = DbType.String)]
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
        [Column(ColName = "CV_CreationDate", DbType = DbType.DateTime)]
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

        private String _RunningString = "";
        public String RunningString
        {
            get
            {
                return _RunningString;
            }
            set
            {
                _RunningString = value;
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

        private PathFS _Path;
        public PathFS Path
        {
            get
            {
                return _Path;
            }
            set
            {
                _Path = value;
            }
        }

        private ContentStatus _Status;
        public ContentStatus Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;
            }
        }

        private Dictionary<int, CMContentVersionSubVersionModel> _ContentVersions = new Dictionary<int, CMContentVersionSubVersionModel>();
        public Dictionary<int, CMContentVersionSubVersionModel> ContentVersions
        {
            get
            {
                return _ContentVersions;
            }
            set
            {
                _ContentVersions = value;
            }
        }


        private Dictionary<int, CMContentVersionSubVersionModel> _BeforeUpdateLinkedVersions = new Dictionary<int, CMContentVersionSubVersionModel>();
        public Dictionary<int, CMContentVersionSubVersionModel> BeforeUpdateLinkedVersions
        {
            get
            {
                return _BeforeUpdateLinkedVersions;
            }
            set
            {
                _BeforeUpdateLinkedVersions = value;
            }
        }

        private Dictionary<int, CMContentFileModel> _Files = new Dictionary<int, CMContentFileModel>();
        public Dictionary<int, CMContentFileModel> Files
        {
            get
            {
                return _Files;
            }
            set
            {
                _Files = value;
            }
        }

        private Dictionary<int, CMContentFileModel> _BeforeUpdateFiles = new Dictionary<int, CMContentFileModel>();
        public Dictionary<int, CMContentFileModel> BeforeUpdateFiles
        {
            get
            {
                return _BeforeUpdateFiles;
            }
            set
            {
                _BeforeUpdateFiles = value;
            }
        }

        #endregion

        #region  Constructor

        public CMVersionModel()
        {
            TreeNodeType = TreeNodeObjectType.ContentVersion;

            this.Status = new ContentStatus();
            this.Status.ID = "";
            this.Status.Name = "";
            this.Status.Icon = "";

            this.Path = new PathFS();
            this.Path.Type = PathType.Relative;
            this.Path.Name = "";  
        }

        #endregion

        #region Path FD Class
        
        public class PathFS
        {
            public String Name { get; set; }
            public PathType Type { get; set; }
        }

        #endregion

        #region Content Status Class

        public class ContentStatus 
        {
            public String ID { get; set; }
            public String Name { get; set; }
            public String Icon { get; set; }
        }

        #region Version Key Class

        #endregion

        public class VersionKey
        {
            public int ContentID { get; set; }
            public int VersionID { get; set; }
        }

         #endregion

        #region Content File Class

        public class ContentFile 
        {
            public int ID { get; set; }
            public String FileName { get; set; }
            public String FileFullPath { get; set; }
            public String FileRelativePath { get; set; }
        }

        #endregion

        #region Contains

        public override bool Contains(string text)
        {
            if (Name.Contains(text) || Description.Contains(text) || LockWithDescription.Contains(text) || ECR.Contains(text) ||
                RunningString.Contains(text) || DocumentID.Contains(text))
                return true;

            foreach (var contentFile in Files)
                if (contentFile.Value.FileName.Contains(text) || contentFile.Value.FileRelativePath.Contains(text))
                    return true;

            return false;
        }

        #endregion

    }
}
