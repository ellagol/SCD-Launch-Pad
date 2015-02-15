﻿using System;
using System.IO;
using Infra.Domain;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
namespace ATSBusinessObjects
{
    [Serializable]
    [XmlRoot("ProjectVersionExecutionInfo")]
    public class ExecutionInfoModel
    {
        [XmlElement("ProjectName")]
        public string projectName = string.Empty;

        [XmlElement("ProjectCode")]
        public string projectCode = string.Empty;

        [XmlElement("ProjectStep")]
        public string projectStep = string.Empty;

        [XmlElement("VersionId")]
        public int versionId = 0;

        [XmlElement("UserName")]
        public string user = string.Empty;

        [XmlElement("Environment")]
        public string environment = string.Empty;

        [XmlElement("Station")]
        public string station = string.Empty;

        [XmlElement("TimeStamp")]
        public string timestamp = string.Empty;

        [XmlArray("Contents"), XmlArrayItem("Content", typeof(ExecutionInfoContentModel))]
        public ObservableCollection<ExecutionInfoContentModel> contents = new ObservableCollection<ExecutionInfoContentModel>();
    }
}// end of namespace ATSBusinessObjects

