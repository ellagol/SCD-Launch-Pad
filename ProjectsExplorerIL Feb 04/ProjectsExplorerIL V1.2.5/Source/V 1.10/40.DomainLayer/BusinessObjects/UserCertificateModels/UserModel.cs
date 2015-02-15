using System.Linq;
using System.Xml.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using System.Data;
using System.Text;
using System.Reflection;
using Infra.Domain;
using System.Collections.ObjectModel;



namespace ATSBusinessObjects
{

  
    
    [Serializable(), Table(TableName = "User")]
    public class UserModel : BusinessObjectBase 
    {

        #region  State Properties

        private long _UserId = -1;
        [Column(ColName = "UserId", DbType = DbType.Int32, IsPK = true)]
        public long UserId
        {
            get
            {
                return _UserId;
            }
            set
            {
                _UserId = value;
            }
        }

        private string _UserName = "";
        [Column(ColName = "UserName", DbType = DbType.String)]
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
            }
        }



        private ObservableCollection<UserCertificatePartialModel> _UserCertificates = new ObservableCollection<UserCertificatePartialModel>();
        public ObservableCollection<UserCertificatePartialModel> UserCertificates
       {
           get
           {
               return _UserCertificates;
           }
           set
           {
               _UserCertificates = value;
           }
       }

        #endregion



    }//end of UserModel

    [Serializable(), Table(TableName = "User")]
    public class UserPartialModel : BusinessObjectBase
    {

        #region  State Properties

        private string _UserName = "";
        [Column(ColName = "UserName", DbType = DbType.String)]
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
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



        
        #endregion



    }//end of UserPartialModel

} //end of root namespace