using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;

namespace ContentManager.ContentUpdate.ViewModel
{
    public class ObservableContentType : ObservableObject
    {
        public String ID { get; set; }

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
