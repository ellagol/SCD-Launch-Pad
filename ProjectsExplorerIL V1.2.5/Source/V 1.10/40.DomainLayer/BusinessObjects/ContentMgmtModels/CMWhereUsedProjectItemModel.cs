using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATSBusinessObjects.ContentMgmtModels
{
    public class CMWhereUsedProjectItemModel
    {
        #region Data

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

        private string _code;
        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
            }
        }

        private string _step;
        public string Step
        {
            get
            {
                return _step;
            }
            set
            {
                _step = value;
            }
        }

        private string _hierarchyPath;
        public string HierarchyPath
        {
            get
            {
                return _hierarchyPath;
            }
            set
            {
                _hierarchyPath = value;
            }
        }

        private string _versionName;
        public string VersionName
        {
            get
            {
                return _versionName;
            }
            set
            {
                _versionName = value;
            }
        }

        private string _versionStatus;
        public string VersionStatus
        {
            get
            {
                return _versionStatus;
            }
            set
            {
                _versionStatus = value;
            }
        }
        #endregion
    }
}
