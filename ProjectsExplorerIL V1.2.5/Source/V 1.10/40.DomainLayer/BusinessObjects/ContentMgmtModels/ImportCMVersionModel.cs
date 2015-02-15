using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Infra.Domain;

namespace ATSBusinessObjects.ContentMgmtModels
{
    [Serializable(), Table(TableName = "ContentVersion")]
    public class ImportCMVersionModel
    {
        private CMVersionModel _SourceVersion = new CMVersionModel();
        public CMVersionModel SourceVersion
        {
            get
            {
                return _SourceVersion;
            }
            set
            {
                _SourceVersion = value;
            }
        }

        private CMVersionModel _TargetVersion = new CMVersionModel();
        public CMVersionModel TargetVersion
        {
            get
            {
                return _TargetVersion;
            }
            set
            {
                _TargetVersion = value;
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
