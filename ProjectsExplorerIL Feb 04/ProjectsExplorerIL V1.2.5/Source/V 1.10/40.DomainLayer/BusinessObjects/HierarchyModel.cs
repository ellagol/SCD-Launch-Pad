using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Infra.Domain;



namespace ATSBusinessObjects
{

    #region  Node Types Enum

    [Serializable]
    public enum NodeTypes : int
    {
        R, //Root
        F, //Folder
        P, //Project
        C, //Content
        G, //Group
        V, //Version
        T//Template
    }
    #endregion

    #region ProjectStatus
    [Serializable]
    public enum ProjectStatusEnum : int
    {
        O, //Open
        D //Disabled
    }

    #endregion
   
    [Serializable(), Table(TableName = "Hierarchy")]
    public class HierarchyModel : BusinessObjectBase 
    {

        #region  State Properties

        private long _Id = -1;
        [Column(ColName = "HierarchyId", DbType = DbType.Int32, IsPK = true)]
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

        private long _ParentId = -1;
        [Column(ColName = "ParentId", DbType = DbType.Int32)]
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

        private int _Sequence = 10;
        [Column(ColName = "Sequence", DbType = DbType.Int16)]
        public int Sequence
        {
            get
            {
                return _Sequence;
            }
            set
            {
                _Sequence = value;
            }
        }

        private NodeTypes _NodeType = NodeTypes.F;
        [Column(ColName = "NodeType", DbType = DbType.String)]
        public NodeTypes NodeType
        {
            get
            {
                return _NodeType;
            }
            set
            {
                _NodeType = value;
            }
        }

        private string _Name = "";
        [Column(ColName = "NodeName", DbType = DbType.String)]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        private string _Description = "";
        [Column(ColName = "Description", DbType = DbType.String)]
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

        private DateTime _CreationDate;
        [Column(ColName = "CreationDate", DbType = DbType.DateTime)]
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

        private DateTime _LastUpdateTime;
        [Column(ColName = "LastUpdateTime", DbType = DbType.DateTime)]
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
        [Column(ColName = "LastUpdateUser", DbType = DbType.String)]
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
        [Column(ColName = "LastUpdateComputer", DbType = DbType.String)]
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
        [Column(ColName = "LastUpdateApplication", DbType = DbType.String)]
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

        private string _Code = "";
        [Column(ColName = "ProjectCode", DbType = DbType.String)]
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                _Code = value;


            }
        }

        private string _SelectedStep = " ";
        [Column(ColName = "ProjectStep", DbType = DbType.String)]
        public String SelectedStep
        {
            get
            {
                return _SelectedStep;
            }
            set
            {
                _SelectedStep = value;

            }
        }

        private Boolean _Synchronization = false;
        [Column(ColName = "SCDUSASyncInd", DbType = DbType.Boolean)]
        public Boolean Synchronization
        {
            get
            {
                return _Synchronization;
            }
            set
            {
                _Synchronization = value;

            }
        }


        private string _GroupDescription = "";
        [Column(ColName = "Description", DbType = DbType.String)]
        public String GroupDescription
        {
            get
            {
                return _GroupDescription;
            }
            set
            {
                _GroupDescription = value;

            }
        }



        private string _GroupName = "";
        [Column(ColName = "Name", DbType = DbType.String)]
        public String GroupName
        {
            get
            {
                return _GroupName;
            }
            set
            {
                _GroupName = value;

            }
        }

        private string _ProjectStatus = "";
        [Column(ColName = "ProjectStatus", DbType = DbType.String)]
        public String ProjectStatus
        {
            get
            {
                return _ProjectStatus;
            }
            set
            {
                _ProjectStatus = value;

            }
        }


        private ProjectStatusEnum _status = ProjectStatusEnum.O;
        public ProjectStatusEnum Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;

            }
        }

        private string _ActiveVersion = "";
        [Column(ColName = "VersionName", DbType = DbType.String)]
        public String ActiveVersion
        {
            get
            {
                return _ActiveVersion;
            }
            set
            {
                _ActiveVersion = value;

            }
        }


        private string _TreeHeader = "";
        public string TreeHeader
        {
            get
            {
                return _TreeHeader;
            }
            set
            {
                _TreeHeader = value;

            }
        }


        private VersionModel _VM = new VersionModel();
        public VersionModel VM
        {
            get
            {
                return _VM;
            }
            set
            {
                _VM = value;
            }
        }

        private  List<String> _Certificates = new List<String>();
        public List<String> Certificates
        {
            get
            {
                return _Certificates;
            }
            set
            {
                _Certificates = value;
            }
        }
    private  ObservableCollection<UserCertificateApiModel> _UserCertificates = new ObservableCollection<UserCertificateApiModel>();
        public ObservableCollection<UserCertificateApiModel> UserCertificates
        {
            get
            {
                return _UserCertificates;
            }
            set
            {
                _UserCertificates = value;
            }
        } 


       private ObservableCollection<VersionModel> _GetAllVersions ;
       public ObservableCollection<VersionModel> GetAllVersions
       {
           get
           {
               return _GetAllVersions;
           }
           set
           {
               _GetAllVersions = value;
           }
       }

   

       private  Boolean _IsCloned = false;
       public Boolean IsCloned
       {
           get
           {
               return _IsCloned;
           }
           set
           {
               _IsCloned = value;
           }
       }


       private Boolean _IsClonedRelated = false;
       public Boolean IsClonedRelated
       {
           get
           {
               return _IsClonedRelated;
           }
           set
           {
               _IsClonedRelated = value;
           }
       }

       private Boolean _IsClonedRelatedUpdate = false;
       public Boolean IsClonedRelatedUpdate
       {
           get
           {
               return _IsClonedRelatedUpdate;
           }
           set
           {
               _IsClonedRelatedUpdate = value;
           }
       }


       private Boolean _IsClonedRelatedSplit = false;
       public Boolean IsClonedRelatedSplit
       {
           get
           {
               return _IsClonedRelatedSplit;
           }
           set
           {
               _IsClonedRelatedSplit = value;
           }
       }
       private long _GroupId= -1;
       [Column(ColName = "GroupId", DbType = DbType.Int32)]
       public long GroupId
       {
           get
           {
               return _GroupId;
           }
           set
           {
               _GroupId = value;
           }
       }

       private DateTime _GroupLastUpdateTime;
       [Column(ColName = "LastUpdateTime", DbType = DbType.DateTime)]
       public DateTime GroupLastUpdateTime
       {
           get
           {
               return _GroupLastUpdateTime;
           }
           set
           {
               _GroupLastUpdateTime = value;
           }
       }

  

        #endregion

        #region  Constructor

        public HierarchyModel()
        {
            // _ActiveVersionModel = new VersionModel();
        }

        #endregion

        #region Bulk Update Properties

        private Boolean _IsBulkUpdatedChecked = true;
        public Boolean IsBulkUpdatedChecked
        {
            get
            {
                return _IsBulkUpdatedChecked;
            }
            set
            {
                _IsBulkUpdatedChecked = value;
            }
        }

        private Boolean _IsBulkUpdatedEnabled = true;
        public Boolean IsBulkUpdatedEnabled
        {
            get
            {
                return _IsBulkUpdatedEnabled;
            }
            set
            {
                _IsBulkUpdatedEnabled = value;
            }
        }

        private Boolean _IsToolTipEnabled = false;
        public Boolean IsToolTipEnabled
        {
            get
            {
                return _IsToolTipEnabled;
            }
            set
            {
                _IsToolTipEnabled = value;
            }
        }
        #endregion Bulk Update Properties

        private Boolean _IRISTechInd = false;
        public Boolean IRISTechInd
        {
            get
            {
                return _IRISTechInd;
            }
            set
            {
                _IRISTechInd = value;
            }
        }

        

    }

} //end of root namespace