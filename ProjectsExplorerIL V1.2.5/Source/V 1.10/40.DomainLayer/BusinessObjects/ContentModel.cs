using System;
using System.IO;
using Infra.Domain;
namespace ATSBusinessObjects
{

    [Serializable(), Table(TableName = "PE_Version")]
    public class ContentModel : BusinessObjectBase 
    {
        public string name { get; set; }
        public string version { get; set; }
        public int id { get; set; }
        private int _seq = 1;
        public int seq
        {
            get
            {
                return _seq;
            }
            set
            {
                _seq = value;
                //NotifyPropertyChanged("seq");
               
            }
        }

        //public event PropertyChangedEventHandler PropertyChanged;

        //protected void NotifyPropertyChanged(String propertyName)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
        public String IconFileFullPath { get; set; }
        public String lastUpdateTime { get; set; }
        public string status { get; set; }
        public string contentCategory { get; set; }


        public ContentModel(string name, string version, int id, String lastUpdateTime, String IconFileFullPath, string contentCategory)
        {
            this.name = name;
            this.version = version;
            this.id = id;
            this.lastUpdateTime = lastUpdateTime;
            if (File.Exists(IconFileFullPath))
            {
                this.IconFileFullPath = IconFileFullPath;
            }
            else
            {
                this.IconFileFullPath = "pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/Content.png";
            }
            this.contentCategory = contentCategory;
        }

        public ContentModel(string contentName, string versionName, int contentVersionId, int sequenceNumber, String lastUpdateTime, String IconFileFullPath)
        {
            this.name = contentName;
            this.version = versionName;
            this.id = contentVersionId;
            this.seq = sequenceNumber;
            this.lastUpdateTime = lastUpdateTime;
            if (File.Exists(IconFileFullPath))
            {
                this.IconFileFullPath = IconFileFullPath;
            }
            else
            {
                this.IconFileFullPath = "pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/Content.png";
            }
        }


        public override bool Equals(Object obj)
        {
            var ContentClient = obj as ContentModel;
            if (ContentClient == null)
                return false;
            return this.version == ContentClient.version && this.id == ContentClient.id;
        }


    }
}// end of namespace ATSBusinessObjects


