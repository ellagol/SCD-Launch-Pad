using System;
using System.Collections.ObjectModel;
using ATSBusinessLogic;
using ATSBusinessObjects;
using ATSDomain;
using Infra.MVVM;


namespace ExplorerModule
{

    public class UserCertificatesViewModel : ViewModelBase
    {
        private Guid WorkspaceId { get; set; }
        protected MessengerService MessageMediator = new MessengerService();

        public UserCertificatesViewModel(Guid workspaceId)
        {
            WorkspaceId = workspaceId;
            //Message Box Service
            MessageMediator = GetService<MessengerService>();
            PopulateUsersCertificate();
            RaisePropertyChanged("UsersCertificate");

        }

        private ObservableCollection<UserCertificateApiModel> _UsersCertificate;
        public ObservableCollection<UserCertificateApiModel> UsersCertificate
        {
            get
            {
                return _UsersCertificate;
            }
            set
            {
                _UsersCertificate = value;
                RaisePropertyChanged("UsersCertificate");

            }
        }

        #region PopulateUsersCertificate
        private void PopulateUsersCertificate()
        {
            try
            {
                string result = string.Empty;
                result = UserCertificateBLL.GetAllUserCertificate(out _UsersCertificate);
                if (!(string.IsNullOrEmpty(result)))
                {
                    //-----TODO - ADD ERROR CATCH -----
                }
                if(result.Length==0)
                {
                    result = "None of workstations is associated ...";
                }
                RaisePropertyChanged("UsersCertificate");

            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);

                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
            }

        }
        #endregion PopulateUsersCertificate

    }






} //end of root namespace    