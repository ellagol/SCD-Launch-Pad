using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATSBusinessObjects.ContentMgmtModels
{
    public class CMWhereUsedContentLinkItemModel
    {
        #region Data

        private string _contentName;
        public string ContentName
        {
            get
            {
                return _contentName;
            }
            set
            {
                _contentName = value;
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

        #endregion
    }
}
