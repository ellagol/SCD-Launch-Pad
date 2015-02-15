using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Infra.Domain;

namespace ATSBusinessObjects
{
    #region  Note Types Enum

    [Serializable]
    public enum NoteStatusTypes : int
    {
        A, //Active
        D, //Deactivate
    }

    [Serializable]
    public enum NoteTypes : int
    {
        [Description("Comment")]
        C,
        [Description("Work Instruction")]
        W,
        [Description("Import")]
        I,
        [Description("Export")]
        E
    }

    #endregion

    [Serializable(), Table(TableName = "Note")]
    public class NotesModel : BusinessObjectBase
    {

        #region  State Properties

        private long _Id = -1;
        [Column(ColName = "Id", DbType = DbType.Int32, IsPK = true)]
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

        private long _HierarchyId = -1;
        [Column(ColName = "HierarchyId", DbType = DbType.Int32, IsPK = true)]
        public long HierarchyId
        {
            get
            {
                return _HierarchyId;
            }
            set
            {
                _HierarchyId = value;
            }
        }

        private string _NoteType = "C";
        [Column(ColName = "NoteType", DbType = DbType.String)]
        public string NoteType
        {
            get
            {
                return _NoteType;
            }
            set
            {
                _NoteType = value;
            }
        }


        private string _NoteTitle = "";
        [Column(ColName = "NoteTitle", DbType = DbType.String)]
        public string NoteTitle
        {
            get
            {
                return _NoteTitle;
            }
            set
            {
                _NoteTitle = value;
            }
        }


        private NoteStatusTypes _NoteStatus = NoteStatusTypes.A;
        [Column(ColName = "NoteStatus", DbType = DbType.String)]
        public NoteStatusTypes NoteStatus
        {
            get
            {
                return _NoteStatus;
            }
            set
            {
                _NoteStatus = value;
            }
        }

        private string _NoteStatusDescription = "";
        [Column(ColName = "NoteStatusDescription", DbType = DbType.String)]
        public string NoteStatusDescription
        {
            get
            {
                switch (_NoteStatus.ToString())
                {
                    case "A":
                        _NoteStatusDescription = "Disable";
                        break;
                    case "D":
                        _NoteStatusDescription = "Enable";
                        break;
                    default:
                        _NoteStatusDescription = "Enable";
                        break;
                }
                return _NoteStatusDescription;
            }
            set
            {
                _NoteStatusDescription = value;
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

        private string _NoteText = "";
        [Column(ColName = "NoteText", DbType = DbType.String)]
        public string NoteText
        {
            get
            {
                return _NoteText;
            }
            set
            {
                _NoteText = value;
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

        private string _CreatedBy = "";
        [Column(ColName = "CreatedBy", DbType = DbType.String)]
        public string CreatedBy
        {
            get
            {
                return _CreatedBy;
            }
            set
            {
                _CreatedBy = value;
            }
        }

        private Boolean _SpecialInd = false;
        [Column(ColName = "SpecialInd", DbType = DbType.Binary)]
        public Boolean SpecialInd
        {
            get
            {
                return _SpecialInd;
            }
            set
            {
                _SpecialInd = value;
            }
        }

        private List<string> _noteTypes = new List<string>();
        [Column(ColName = "noteTypes", DbType = DbType.Object)]
        public List<string> noteTypes
        {
            get
            {
                return _noteTypes;
            }
            set
            {
                _noteTypes.Add(value.ToString());
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

        #endregion

        #region  Constructor

        public NotesModel()
        {
        }

        #endregion

    }


}
