using System;
using Infra.Domain;

namespace ATSBusinessObjects.ContentMgmtModels
{
    [Serializable(), Table(TableName = "ContentTree")]
    public class ImportCMFolderModel : CMTreeNode
    {

        private CMFolderModel _SourceFolder = new CMFolderModel();
        public CMFolderModel SourceFolder
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

        private CMFolderModel _TargetFolder = new CMFolderModel();
        public CMFolderModel TargetFolder
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

        private Boolean _ChildContentsExistInTargetEnv = true;
        public Boolean ChildContentsExistInTargetEnv
        {
            get
            {
                return _ChildContentsExistInTargetEnv;
            }
            set
            {
                _ChildContentsExistInTargetEnv = value;
            }
        }
    }
    
}
