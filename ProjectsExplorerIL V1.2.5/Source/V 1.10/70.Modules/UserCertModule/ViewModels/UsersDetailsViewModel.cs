using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Windows;
using ATSBusinessLogic;
using ATSBusinessObjects;
using ATSDomain;
using Infra.DragDrop;
using Infra.MVVM;

namespace UserCertModule
{
    public class UsersDetailsViewModel : ViewModelBase, IDropTarget
    
    {
        #region  Data

        
        private StringCollection Permissions; //Collection of permissions this user is allowed to perform

        protected MessengerService MessageMediator = new MessengerService();
        private IMessageBoxService MsgBoxService = null;

        private UserModel User;
        private ObservableCollection<UserCertificatePartialModel> DeletedUserCertificates = new ObservableCollection<UserCertificatePartialModel>();

        private Guid WorkspaceId;

        #endregion

        #region  Ctor
        public UsersDetailsViewModel(UserModel UM, Guid WorkspaceID)
        {
            //Initialize this VM
            MsgBoxService = GetService<IMessageBoxService>();
            //Messenger Service (to exchange messages between VMs)
            MessageMediator = GetService<MessengerService>();
            //Register as Subscriber to messages from other VM;
            this.WorkspaceId = WorkspaceID;
            User = UM;
            MessageMediator.Register(this.WorkspaceId + "OnApplySaveUserCommand", new Action<UserModel>(OnApplySaveUserCommand)); //Register to recieve a message from usersDetailsViewModel
        
        }
         
        #endregion

        #region Presentation Data

        private ObservableCollection<UserCertificatePartialModel> _UserCertificates = new ObservableCollection<UserCertificatePartialModel>();
        public ObservableCollection<UserCertificatePartialModel> UserCertificates
        {
            get
            {
                if (User != null)
                {
                    if (User.UserCertificates != null)
                        return User.UserCertificates;
                    else
                        return null;
                }
                else
                    return null;
            }
            set
            {
                if (User != null)
                {
                    if (User.UserCertificates != null)
                    {
                        _UserCertificates = User.UserCertificates;
                        RaisePropertyChanged("UserCertificates");

                    }
                }
            }

        }


        private UserCertificatePartialModel _SelectedCertificate;
        public UserCertificatePartialModel SelectedCertificate
        {
            get
            {
                return _SelectedCertificate;
            }
            set
            {
                // Because we added MouseUp handling event, the code must move; only identifying the value of the selected ListView item should remain here
                _SelectedCertificate = value;
             
            }
        }


        #endregion 

        #region  IDropTarget Members & Other Drop Activities

        public void DragOver(Infra.DragDrop.IDropInfo DropInfo)
        {
            try
            {
                
                string SourceItemType = DropInfo.Data.GetType().ToString();
                if (SourceItemType.Contains("UserCertificateModel"))
                {
                    UserCertificateModel SourceItem = DropInfo.Data as UserCertificateModel;
                    if (SourceItem != null)
                    {
                        DropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                        DropInfo.Effects = DragDropEffects.Move;
                    }
                }
                
              
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                Object[] ArgsList = new Object[] { 0 };
                UsersCertificatesViewModel.ShowErrorAndInfoMessage(105, ArgsList);
            }
        }

        public void Drop(Infra.DragDrop.IDropInfo DropInfo)
        {
            try
            {
                //Identify dropped Entity Type
                string SourceItemType = DropInfo.Data.GetType().ToString();
                string DropCollectionType = DropInfo.TargetCollection.GetType().ToString();
                
                //If dropping Certificate, verify we drop on the right container and add to certificates list
                if (SourceItemType.Contains("UserCertificateModel") && DropCollectionType.Contains("UserCertificatePartialModel"))
                {

                    UserCertificateModel SourceItem = DropInfo.Data as UserCertificateModel;
                    //Check that the certificate status is active.
                    if (SourceItem.Status == UserCertificateStatusEnum.A)
                    {
                        UserCertificatePartialModel CPM = new UserCertificatePartialModel();
                        CPM.Id = SourceItem.Id;
                        CPM.LastUpdateCertTime = SourceItem.LastUpdateTime ;
                        CPM.CertificateName = SourceItem.Name;
                        bool flgUser = false;
                        foreach(var i in User.UserCertificates)
                        {
                            //Already exist.
                            if (i.Id == CPM.Id)
                            {
                                flgUser = true;
                            }

                        }
                        if (flgUser == false)//Drop successfull.
                        {
                            User.IsDirty = true;
                            MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyUserReceived", User);
                            User.UserCertificates.Add(CPM);
                            RaisePropertyChanged("UserCertificates");
                            UserPartialModel UPM = new UserPartialModel();
                            UPM.UserName = User.UserName;
                            
                            SourceItem.Users.Add(UPM);

                            MessageMediator.NotifyColleagues(this.WorkspaceId + "OnUpdateUserReceived", User);
                        }
                    }
                }
             
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", ex); // TODO: Log error
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                Object[] ArgsList = new Object[] { 0 };
                UsersCertificatesViewModel.ShowErrorAndInfoMessage(105, ArgsList);
            }
        }

        #endregion

        #region  Save Command

        private RelayCommand _SaveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null)
                {
                    _SaveCommand = new RelayCommand(ExecuteSaveCommand, CanExecuteSaveCommand);
                }
                return _SaveCommand;
            }
        }

        private bool CanExecuteSaveCommand()
        {
            if (Domain.IsPermitted("203", "UCM"))
            {
                if (User.IsDirty == true)
                {
                    return true;
                }

                else
                    return false;
            }
            else
                return false;
        }



        private void ExecuteSaveCommand()
        {
            try
            {
                Domain.PersistenceLayer.BeginTransWithIsolation(IsolationLevel.Serializable);
                if (this.DeletedUserCertificates.Count > 0)
                {
                    foreach (var i in DeletedUserCertificates)
                    {
                      string deleteCertificate = CertificateUserBLL.deleteAssignedUserCertificates(i.Id, User.UserName);
                      if (!String.IsNullOrEmpty(deleteCertificate))
                      {
                          Domain.PersistenceLayer.AbortTrans();
                          Object[] ArgsList = new Object[] { 0 };
                          UsersCertificatesViewModel.ShowErrorAndInfoMessage(105, ArgsList);
                          return;

                      }
                    }
                    DeletedUserCertificates.Clear();
                }
                string result =  UserBLL.PersistUser(ref User);
                UserCertModule.UsersCertificatesViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(result), new Object[] { 0 });
                if (result != "203")
                {
                    Domain.PersistenceLayer.AbortTrans();
                }
                else
                {
                    Domain.PersistenceLayer.CommitTrans();
                }
                MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyUserReceived", null);
                
            }
            catch (Exception e)
            {
                Domain.PersistenceLayer.AbortTrans();
                if (e.Message == "DB Error")
                {
                    Object[] ArgsList = new Object[] { 0 };
                    UsersCertificatesViewModel.ShowErrorAndInfoMessage(141, ArgsList);
                }
                else
                {
                    Object[] ArgsList = new Object[] { 0 };
                    UsersCertificatesViewModel.ShowErrorAndInfoMessage(105, ArgsList);
                }
            }
        }

        #endregion

        #region  Remove Certificate From  User Command

        private RelayCommand _RemoveCertificateCommand;
        public RelayCommand RemoveCertificateCommand
        {
            get
            {
                if (_RemoveCertificateCommand == null)
                {
                    _RemoveCertificateCommand = new RelayCommand(ExecuteRemoveCertificateCommand, CanExecuteRemoveCertificateCommand);
                }
                return _RemoveCertificateCommand;
            }
        }

        private bool CanExecuteRemoveCertificateCommand()
        {
            return Domain.IsPermitted("203", "UCM");
        }


        private void ExecuteRemoveCertificateCommand()
        {
            try
            {
                _SelectedCertificate.IsDirty = true;
                MessageMediator.NotifyColleagues(this.WorkspaceId + "OnRemoveCertificateReceived", User);
                DeletedUserCertificates.Add(_SelectedCertificate);
                User.UserCertificates.Remove(_SelectedCertificate);
                RaisePropertyChanged("UserCertificates");
                User.IsDirty = true;
                MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyUserReceived", User);
                //MessageMediator.NotifyColleagues(this.WorkspaceId + "OnRemoveUserReceived", UserCertificate);

            }
            catch (Exception e)
            {
                Object[] ArgsList = new Object[] { 0 };
                UsersCertificatesViewModel.ShowErrorAndInfoMessage(105, ArgsList);
            }
        }

        #endregion

        #region  Apply Save Command
        private void OnApplySaveUserCommand(UserModel UM)
        {
            //TODO : Add is permitted.
            if (Domain.IsPermitted("203", "UCM"))
            {
                ExecuteSaveCommand();

            }
         
        }
        #endregion  Apply Save Command

    }// end of UsersDetailsViewModel
}
