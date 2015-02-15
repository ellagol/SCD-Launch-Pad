using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Infra.Domain;



namespace ATSBusinessObjects
{
    public class ImportHierarchyModel 
    {
        private HierarchyModel _SourceFolder = new HierarchyModel();
        public HierarchyModel SourceFolder
        {
            get
            {
                return _SourceFolder;
            }
            set
            {
                _SourceFolder = value;
            }
        }

        private HierarchyModel _TargetFolder = new HierarchyModel();
        public HierarchyModel TargetFolder
        {
            get
            {
                return _TargetFolder;
            }
            set
            {
                _TargetFolder = value;
            }
        }

        private Boolean _ExistsInTargetEnv = false;
        public Boolean ExistsInTargetEnv
        {
            get
            {
                return _ExistsInTargetEnv;
            }
            set
            {
                _ExistsInTargetEnv = value;
            }
        }

        private Boolean _ChildProjectExistInTargetEnv = true;
        public Boolean ChildProjectExistInTargetEnv
        {
            get
            {
                return _ChildProjectExistInTargetEnv;
            }
            set
            {
                _ChildProjectExistInTargetEnv = value;
            }
        }

    }

} //end of root namespace