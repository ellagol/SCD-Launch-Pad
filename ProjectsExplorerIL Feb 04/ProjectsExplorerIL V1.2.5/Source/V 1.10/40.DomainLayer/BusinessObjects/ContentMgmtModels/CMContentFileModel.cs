using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Infra.Domain;

namespace ATSBusinessObjects.ContentMgmtModels
{
    public class CMContentFileModel : CMLastUpdateModel
    {
        #region Properties

        public int ID { get; set; }
        public String FileName { get; set; }
        public String FileFullPath { get; set; }
        public String FileRelativePath { get; set; }
        public String FileSize { get; set; }
        public DateTime FileLastWriteTime { get; set; }

        #endregion

        #region Methods

        public void UpdateFileFullPath(ATSBusinessObjects.ContentMgmtModels.CMVersionModel.PathFS path)
        {
            switch (path.Type)
            {
                case ATSBusinessObjects.ContentMgmtModels.CMVersionModel.PathType.Relative:
                    //FileFullPath = getRootPath() + "\\" + path.Name + "\\";

                    if (!String.IsNullOrEmpty(FileRelativePath))
                        FileFullPath += FileRelativePath + "\\";

                    FileFullPath += FileName;

                    break;
                case ATSBusinessObjects.ContentMgmtModels.CMVersionModel.PathType.Full:
                    FileFullPath = path.Name;

                    if (!String.IsNullOrEmpty(FileRelativePath))
                        FileFullPath += FileRelativePath + "\\";

                    FileFullPath += FileName;
                    break;
            }
        }

        public void GetDestinationPath(string destinationPath, out string destinationFullPath, out string destinationDirectoryPath)
        {
            destinationDirectoryPath = FileRelativePath == String.Empty ? destinationPath : destinationPath + "\\" + FileRelativePath;
            destinationFullPath = destinationDirectoryPath + "\\" + FileName;
        }

        #endregion
    }
}
