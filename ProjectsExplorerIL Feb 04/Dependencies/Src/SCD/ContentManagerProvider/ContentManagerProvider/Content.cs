using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentManagerProvider
{
    public class Content : TreeNode
    {
        public Content()
        {
            TreeNodeType = TreeNodeObjectType.Content;
        }

        public bool CertificateFree { get; set; }
        public ContentType ContentType { get; set; }
        public String Description { get; set; }
        public String IconFileFullPath { get; set; }
        public Dictionary<int, ContentVersion> Versions { get; set; }

        public override bool Contains(string text)
        {
            return Name.Contains(text) || Description.Contains(text) || IconFileFullPath.Contains(text);
        }

        public Content Clone()
        {
            Content contentNew = new Content
            {
                Name = Name,
                Description = Description,
                ID = ID,
                ChildID = ChildID,
                ParentID = ParentID,
                CertificateFree = CertificateFree,
                ContentType = ContentType,
                IconFileFullPath = IconFileFullPath,
                Versions = new Dictionary<int, ContentVersion>()
            };

            UpdateLastUpdate(this, contentNew);
            return contentNew;
        }
    }
}
