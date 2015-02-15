using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;

namespace ContentManager.WhereUsed.View_Model
{
    public class WhereUsedProjectItem : ObservableObject
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { Set(() => Name, ref _name, value); }
        }

        private string _code;
        public string Code
        {
            get { return _code; }
            set { Set(() => Code, ref _code, value); }
        }

        private string _step;
        public string Step
        {
            get { return _step; }
            set { Set(() => Step, ref _step, value); }
        }

        private string _hierarchyPath;
        public string HierarchyPath
        {
            get { return _hierarchyPath; }
            set { Set(() => HierarchyPath, ref _hierarchyPath, value); }
        }
    }
}
