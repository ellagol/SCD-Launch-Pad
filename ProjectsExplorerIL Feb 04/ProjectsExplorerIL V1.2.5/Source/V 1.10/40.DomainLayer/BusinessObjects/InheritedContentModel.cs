using System;
using System.IO;
using Infra.Domain;
namespace ATSBusinessObjects
{
    public class InheritedContentModel
    {
        public int contentVersionId { get; set; }
        public int cvPriority { get; set; }
        public int parentFolderId { get; set; }
        public int parentFolderLevel { get; set; }
        public int templateProjectId { get; set; }
        public string stepDescription { get; set; }
    }
}// end of namespace ATSBusinessObjects


