using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;

namespace ContentManager.VersionUpdate.ViewModel
{
    public class ItemVersionLink : ObservableObject
    {

        public int ContentID { get; set; }

        private int _id;
        public int ContentVersionID
        {
            get { return _id; }
            set { Set(() => ContentVersionID, ref _id, value); }
        }
        
        private string _name;
        public string Name
        {
            get { return _name; }
            set { Set(() => Name, ref _name, value); }
        }

        private string _contentName;
        public string ContentName
        {
            get { return _contentName; }
            set { Set(() => ContentName, ref _contentName, value); }
        }


        private string _icon;
        public string Icon
        {
            get { return _icon; }
            set { Set(() => Icon, ref _icon, value); }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(() => IsSelected, ref _isSelected, value); }
        }
    }
}
