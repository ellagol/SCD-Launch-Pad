using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContentManagerProvider.General;

namespace ContentManagerProvider
{
    public class ContentFile : LastUpdate
    {
        public int ID { get; set; }
        public String FileName { get; set; }
        public String FileFullPath { get; set; }
        public String FileRelativePath { get; set; }

        public void UpdateFileFullPath(PathFS path)
        {
            switch (path.Type)
            {
                case PathType.Relative:
                    FileFullPath = Locator.SystemParameters["RootPathFS"] + "\\" + path.Name + "\\";

                    if(!String.IsNullOrEmpty(FileRelativePath))
                        FileFullPath += FileRelativePath + "\\";

                    FileFullPath += FileName;

                    break;
                case PathType.Full:
                    FileFullPath = path.Name;

                    if(!String.IsNullOrEmpty(FileRelativePath))
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
    }
}
