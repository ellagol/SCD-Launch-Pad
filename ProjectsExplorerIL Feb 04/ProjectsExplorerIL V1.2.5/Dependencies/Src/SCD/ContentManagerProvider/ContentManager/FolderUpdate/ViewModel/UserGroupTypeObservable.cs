
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContentManagerProvider;
using GalaSoft.MvvmLight;


namespace ContentManager.FolderUpdate.ViewModel
{
    public class UserGroupTypeObservable : ObservableObject
    {
        public UserGroupType UserGroupType { get; set; }

        private bool _checked;
        public bool Checked
        {
            get { return _checked; }
            set { Set(() => Checked, ref _checked, value); }
        }
    }
}
