using System;

using System.Data;
using Infra.Domain;
namespace ATSBusinessObjects
{
    [Serializable()]
    public class UserCertificateApiModel : BusinessObjectBase
    {
        #region  State Properties


        private string _UserCertificateId;
        public string UserCertificateId
        {
            get
            {
                return _UserCertificateId;
            }
            set
            {
                _UserCertificateId = value;
            }
        }
    
        private string _UserCertificateName;
        public string UserCertificateName
        {
            get
            {
                return _UserCertificateName;
            }
            set
            {
                _UserCertificateName = value;
            }
        }

        private DateTime _LastUpdateTime;
        [Column(ColName = "LastUpdateTime", DbType = DbType.DateTime)]
        public DateTime LastUpdateTime
        {
            get
            {
                return _LastUpdateTime;
            }
            set
            {
                _LastUpdateTime = value;
            }
        }

        #endregion State Properties
    }
}


