using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
namespace ATSBusinessObjects
{
    [Serializable]
    public class ExecutionInfoContentModel
    {
        [XmlElement("Name")]
        public string name = string.Empty;

        [XmlElement("Description")]
        public string description = string.Empty;

        [XmlElement("ContentCategory")]
        public string category = string.Empty;

        [XmlElement("VersionName")]
        public string versionName = string.Empty;

        [XmlElement("VersionDescription")]
        public string versionDescription = string.Empty;

        [XmlElement("VersionStatus")]
        public string versionStatus = string.Empty;

        [XmlElement("ATRIndicator")]
        public bool ATRIndicator = false;

        [XmlArray("WhereUsed"), XmlArrayItem("ParentContent", typeof(ExecutionInfoWhereUsedModel))]
        public ObservableCollection<ExecutionInfoWhereUsedModel> whereUsed = new ObservableCollection<ExecutionInfoWhereUsedModel>();

    }
}// end of namespace ATSBusinessObjects


