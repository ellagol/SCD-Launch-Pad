using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;

namespace ContentManager.WhereUsed.View_Model
{
    public class WhereUsedContentLinkItem : ObservableObject
    {
        private string _contentName;
        public string ContentName
        {
            get { return _contentName; }
            set { Set(() => ContentName, ref _contentName, value); }
        }

        private string _versionName;
        public string VersionName
        {
            get { return _versionName; }
            set { Set(() => VersionName, ref _versionName, value); }
        }
    }
}
