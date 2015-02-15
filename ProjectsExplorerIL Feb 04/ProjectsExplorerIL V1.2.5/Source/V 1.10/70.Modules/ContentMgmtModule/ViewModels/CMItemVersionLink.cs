using System;
using System.Windows.Media;

namespace ContentMgmtModule
{
    public class CMItemVersionLink
    {
        #region Data

        private int _ContentID;
        public int ContentID
        {
            get
            {
                return _ContentID;
            }
            set
            {
                _ContentID = value;
            }
        }

        private int _ContentVersionID;
        public int ContentVersionID
        {
            get
            {
                return _ContentVersionID;
            }
            set
            {
                _ContentVersionID = value;
            }
        }

        private string _Name;
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

        private string _Status;
        public string Status
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

        private string _ContentName;
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

        private ImageSource _Icon;
        public ImageSource Icon
        {
            get
            {
                return _Icon;
            }
            set
            {
                _Icon = value;
            }
        }

        private bool _IsSelected;
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                _IsSelected = value;
            }
        }

        private DateTime _LastUpdateTime;
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

        #endregion
    }
}

