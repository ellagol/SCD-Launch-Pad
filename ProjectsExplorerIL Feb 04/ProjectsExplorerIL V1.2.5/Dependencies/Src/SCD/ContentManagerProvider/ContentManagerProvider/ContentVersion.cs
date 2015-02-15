using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentManagerProvider
{
    public class ContentVersion : TreeNode
    {
        public ContentVersion()
        {
            TreeNodeType = TreeNodeObjectType.ContentVersion;
        }

        public PathFS Path { get; set; }
        public String Editor { get; set; }
        public ContentStatus Status { get; set; }
        public String Description { get; set; }
        public String ECR { get; set; }
        public String DocumentID { get; set; }
        public String RunningString { get; set; }
        public Dictionary<int, ContentFile> Files { get; set; }
        public Dictionary<int, ContentVersionSubVersion> ContentVersions { get; set; }

        public override bool Contains(string text)
        {
            if (Name.Contains(text) || Description.Contains(text) || Editor.Contains(text) || ECR.Contains(text) ||
                RunningString.Contains(text) || DocumentID.Contains(text))
                return true;

            foreach (var contentFile in Files)
                if (contentFile.Value.FileName.Contains(text) || contentFile.Value.FileRelativePath.Contains(text))
                    return true;

            return false;
        } 

        public ContentVersion Clone()
        {
            ContentVersion contentVersionNew = new ContentVersion
            {
                ID = ID,
                Name = Name,
                ChildID = ChildID,
                ParentID = ParentID,
                Path = new PathFS() { Name = Path.Name, Type = Path.Type },
                Editor = Editor,
                Status = Status,
                Description = Description,
                ECR = ECR,
                DocumentID = DocumentID,
                RunningString = RunningString,

                Files = null,
                ContentVersions = null,
                Permission = null
            };

            UpdateLastUpdate(this, contentVersionNew);

            if (Files != null)
            {
                contentVersionNew.Files = new Dictionary<int, ContentFile>();

                foreach (KeyValuePair<int, ContentFile> contentFile in Files)
                {
                    ContentFile file = new ContentFile()
                    {
                        ID = contentFile.Value.ID,
                        FileName = contentFile.Value.FileName,
                        FileFullPath = contentFile.Value.FileFullPath,
                        FileRelativePath = contentFile.Value.FileRelativePath
                    };

                    UpdateLastUpdate(contentFile.Value, file);
                    contentVersionNew.Files.Add(contentFile.Key, file);
                }
            }

            if (ContentVersions != null)
            {
                contentVersionNew.ContentVersions = new Dictionary<int, ContentVersionSubVersion>();

                foreach (KeyValuePair<int, ContentVersionSubVersion> subVersion in ContentVersions)
                {

                    ContentVersionSubVersion versionSubVersion = new ContentVersionSubVersion()
                        {
                            Order = subVersion.Value.Order,
                            Content = subVersion.Value.Content,
                            ContentSubVersion = subVersion.Value.ContentSubVersion
                        };

                    UpdateLastUpdate(subVersion.Value, versionSubVersion);
                    contentVersionNew.ContentVersions.Add(subVersion.Key, versionSubVersion);
                }
            }

            if (Permission != null)
            {
                contentVersionNew.Permission = new Dictionary<string, bool>();

                foreach (KeyValuePair<String, bool> permission in Permission)
                    contentVersionNew.Permission.Add(permission.Key, permission.Value);
            }

            return contentVersionNew;            
        }
    }
}
