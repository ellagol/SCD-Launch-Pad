using System;
using GalaSoft.MvvmLight;

namespace ContentManager.VersionUpdate.ViewModel
{
    public class ObservableContentStatus : ObservableObject
    {
        public String ID { get; set; }
        public String Icon { get; set; }

        private String _name;
        public String Name
        {
            get
            {
                return _name;
            }
            set
            {
                Set(() => Name, ref _name, value);
            }
        }
    }
}
