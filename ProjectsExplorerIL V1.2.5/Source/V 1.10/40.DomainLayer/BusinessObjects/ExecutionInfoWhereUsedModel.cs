using System;
using System.Xml.Serialization;

namespace ATSBusinessObjects
{
    [Serializable]
    public class ExecutionInfoWhereUsedModel
    {
        #region Data

        [XmlElement("ContentName")]
        public string contentName = string.Empty;

        [XmlElement("VersionName")]
        public string versionName = string.Empty;
 
        #endregion
    }
}
