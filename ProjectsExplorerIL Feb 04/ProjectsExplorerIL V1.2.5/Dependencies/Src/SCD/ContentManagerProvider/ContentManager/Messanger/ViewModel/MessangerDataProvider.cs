using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using ContentManager.ContentManagerMainWindow.ViewModel;
using ContentManager.General;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using TraceExceptionWrapper;

namespace ContentManager.Messanger.ViewModel
{
    public class MessangerDataProvider : ViewModelBase
    {
        public MessangerDataProvider()
        {
            MessageText = "Error text";
            MessageTitle = "Title";

            ActionCancel = new RelayCommand(ActionCancelFn);
            ActionExecute = new RelayCommand(ActionExecuteFn);

            Messenger.Default.Register<MessageWrapperData>(this, OnMessage);

            if (!Locator.ContentManagerDataProvider.IsLoaded)
            {
                TraceException traceException;
                if (Locator.ArgumentException != null)
                {
                    traceException = new TraceException("Argument exception", false, null, Locator.ApplicationName);
                }
                else
                {
                    if (Locator.UserName == "" || Locator.ConnectionString == "")
                        traceException = Locator.UserName == "" ? new TraceException("Application not get parameters", false, new List<string>() { "UserName" }, Locator.ApplicationName) : new TraceException("Application not get parameters", false, new List<string>() { "ConnectionString" }, Locator.ApplicationName);
                    else
                        traceException = Locator.ContentManagerDataProvider.LoadedTraceException;                    
                }


                ((Messenger)Messenger.Default).Send<MessageWrapperData>(new MessageWrapperData(traceException));
            }
        }
        
        private MessageWrapperData MessageWrapperessage { get; set; }

        private String _messageTitle;
        public String MessageTitle
        {
            get { return _messageTitle; }
            set { Set(() => MessageTitle, ref _messageTitle, value); }
        }

        private String _messageIcon;
        public String MessageIcon
        {
            get { return _messageIcon; }
            set { Set(() => MessageIcon, ref _messageIcon, value); }
        }

        private String _messageText;
        public String MessageText
        {
            get { return _messageText; }
            set { Set(() => MessageText, ref _messageText, value); }
        }

        private String _messageBgColor;
        public String MessageBgColor
        {
            get { return _messageBgColor; }
            set { Set(() => MessageBgColor, ref _messageBgColor, value); }
        }

        private bool _isAcknowledge;
        public bool IsAcknowledge
        {
            get { return _isAcknowledge; }
            set { Set(() => IsAcknowledge, ref _isAcknowledge, value); }
        }

        public RelayCommand ActionCancel { get; set; }
        public RelayCommand ActionExecute { get; set; }

        private void ActionCancelFn()
        {
            if (!Locator.ContentManagerDataProvider.IsLoaded)
            {
                if (Application.Current.Windows.Count > 0 && Application.Current.Windows[0] != null)
                    Application.Current.Windows[0].Close();

                return;
            }

            Locator.ContentManagerDataProvider.UserControlVisible = UserControlTypeVisible.Na;
            ReloadData();
        }

        private void ActionExecuteFn()
        {
            Locator.ContentManagerDataProvider.UserControlVisible = UserControlTypeVisible.Na;

            MessageWrapperessage.ActiveAction();
            ReloadData();
        }

        private void ReloadData()
        {

            if (MessageWrapperessage != null && MessageWrapperessage.NeedReloadData)
            {
                ObservableCollection<ItemNode> list = Locator.ItemTreeBuilder.BuildItemTree(Locator.ContentManagerDataProvider.ApplicationWritePermission);

                while (Locator.ContentManagerDataProvider.SubItemNode.Count > 0)
                    Locator.ContentManagerDataProvider.SubItemNode.RemoveAt(0);

                foreach (ItemNode itemNode in list)
                    Locator.ContentManagerDataProvider.SubItemNode.Add(itemNode);
            }
        }

        private void OnMessage(MessageWrapperData messageWrapperessage)
        {
            MessageIcon = messageWrapperessage.Image;
            MessageText = messageWrapperessage.MessageString;
            MessageTitle = messageWrapperessage.MessageTitle;
            MessageBgColor = messageWrapperessage.BgColor;
            MessageWrapperessage = messageWrapperessage;
            IsAcknowledge = messageWrapperessage.Type == MessageType.Acknowledge;

           if (!IsInDesignMode)
                Locator.ContentManagerDataProvider.UserControlVisible = UserControlTypeVisible.Message;
        }
    }
}
