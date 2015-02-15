using System;
using Infra.Domain;


namespace ATSBusinessObjects.ContentMgmtModels
{
    [Serializable(), Table(TableName = "Content")]
    public class ImportCMContentModel
    {

        private CMContentModel _SourceContent = new CMContentModel();
        public CMContentModel SourceContent
        {
            get
            {
                return _SourceContent;
            }
            set
            {
                _SourceContent = value;
            }
        }

        private CMContentModel _TargetContent = new CMContentModel();
        public CMContentModel TargetContent
        {
            get
            {
                return _TargetContent;
            }
            set
            {
                _TargetContent = value;
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
    }
}
