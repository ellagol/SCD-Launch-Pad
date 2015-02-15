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


    #region UserCertificateStatus
    [Serializable]
    public enum UserCertificateStatusEnum : int
    {
        A, //Active
        D, //Draft
        R //Retired
    }

    #endregion


    
    [Serializable(), Table(TableName = "UserCertificates")]
    public class UserCertificateModel : BusinessObjectBase 
    {

        #region  State Properties

        private string _Id = "";
        [Column(ColName = "Id", DbType = DbType.Int32, IsPK = true)]
        public string Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }



        private string _Name = "";
        [Column(ColName = "Name", DbType = DbType.String)]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        private string _Description = "";
        [Column(ColName = "Description", DbType = DbType.String)]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }

        private DateTime _CreationDate;
        [Column(ColName = "CreationDate", DbType = DbType.DateTime)]
        public DateTime CreationDate
        {
            get
            {
                return _CreationDate;
            }
            set
            {
                _CreationDate = value;
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

        private string _LastUpdateUser = "";
        [Column(ColName = "LastUpdateUser", DbType = DbType.String)]
        public string LastUpdateUser
        {
            get
            {
                return _LastUpdateUser;
            }
            set
            {
                _LastUpdateUser = value;
            }
        }

        private string _LastUpdateComputer = "";
        [Column(ColName = "LastUpdateComputer", DbType = DbType.String)]
        public string LastUpdateComputer
        {
            get
            {
                return _LastUpdateComputer;
            }
            set
            {
                _LastUpdateComputer = value;
            }
        }

        private string _LastUpdateApplication = "";
        [Column(ColName = "LastUpdateApplication", DbType = DbType.String)]
        public string LastUpdateApplication
        {
            get
            {
                return _LastUpdateApplication;
            }
            set
            {
                _LastUpdateApplication = value;
            }
        }

        private UserCertificateStatusEnum _Status = UserCertificateStatusEnum.D;
        public UserCertificateStatusEnum Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;

            }
        }

        private ObservableCollection<UserPartialModel> _Users = new ObservableCollection<UserPartialModel>();
        public ObservableCollection<UserPartialModel> Users
       {
           get
           {
               return _Users;
           }
           set
           {
               _Users = value;
           }
       }

   
        #endregion


    }// end of UserCertificateModel


    #region public inner class UserCertificatePartialModel
    [Serializable(), Table(TableName = "UserCertificates")]
    public class UserCertificatePartialModel : BusinessObjectBase
    {

        #region  State Properties

      private string _Id = "";
        [Column(ColName = "Id", DbType = DbType.String , IsPK = true)]
        public string Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }

        private string _CertificateName = "";
        [Column(ColName = "Name", DbType = DbType.String, IsPK = true)]
        public string CertificateName
        {
            get
            {
                return _CertificateName;
            }
            set
            {
                _CertificateName = value;
            }
        }

        private DateTime _LastUpdateCertTime;
        [Column(ColName = "LastUpdateTime", DbType = DbType.DateTime)]
        public DateTime LastUpdateCertTime
        {
            get
            {
                return _LastUpdateCertTime;
            }
            set
            {
                _LastUpdateCertTime = value;
            }
        }




        private DateTime _LastUpdateAssignedTime;
        [Column(ColName = "LastUpdateTime", DbType = DbType.DateTime)]
        public DateTime LastUpdateAssignedTime
        {
            get
            {
                return _LastUpdateAssignedTime;
            }
            set
            {
                _LastUpdateAssignedTime = value;
            }
        }


    
        #endregion

    } // end of UserCertificatePartialModel

    #endregion public inner class UserCertificatePartialModel

} //end of root namespace