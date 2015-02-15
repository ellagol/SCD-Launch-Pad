using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATSBusinessObjects.ContentMgmtModels
{
    public class CMDoubleCopyFileModel
    {
        #region Properties

        public string SourceFileFullPath { get; set; }
        public string LocalCopyFullPath { get; set; }
        public string LocalCopyDirectoryPath { get; set; }
        public string DestinationFileFullPath { get; set; }
        public string DestinationDirectoryPath { get; set; }

        #endregion
    }
}
