using System;
using System.Collections.ObjectModel;

namespace ATSBusinessObjects.ContentMgmtModels
{
    #region  File Status and Types Enum

    public enum ItemFileStatus
    {
        New,
        Exist,
        Updated,
        Selected
    }

    public enum ItemFileNodeType
    {
        File,
        Folder
    }

    #endregion

    public class CMItemFileNode
    {

        #region data

        private const string DefaultFolderImage = "pack://application:,,,/ContentMgmtModule;component/Resources/Icons/32x32/{0}.png";
        private const string DefaultFileImage = "pack://application:,,,/ContentMgmtModule;component/Resources/Icons/32x32/{0}.png";

        public CMItemFileNode Parent { set; get; }
        public ObservableCollection<CMItemFileNode> SubItemNode { set; get; }

        #endregion

        #region Constructor

        public CMItemFileNode()
        {
            SubItemNode = new ObservableCollection<CMItemFileNode>();
        }

        #endregion

        #region Properties

        private int _ID;
        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }

        private ItemFileStatus _status;
        public ItemFileStatus Status
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

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        private string _path;
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
            }
        }

        private string _ExecutePath;
        public string ExecutePath
        {
            get
            {
                return _ExecutePath;
            }
            set
            {
                _ExecutePath = value;
            }
        }

        private string _FileSize;
        public string FileSize
        {
            get
            {
                long numCommasFormat = Convert.ToInt64(_FileSize);
                return string.Format("{0:#,###0}", numCommasFormat);
            }
            set
            {
                _FileSize = value;
            }
        }

        private string _ModifiedOn;
        public string ModifiedOn
        {
            get
            {
                return _ModifiedOn;
            }
            set
            {
                _ModifiedOn = value;
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
            }
        }

        private ItemFileNodeType _type;
        public ItemFileNodeType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        #endregion

    }
}
