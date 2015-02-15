using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ContentManager.ContentManagerMainWindow.ViewModel;
using ContentManager.General;
using ContentManagerProvider;
using GalaSoft.MvvmLight.Messaging;
using ReferenceTableReader;
using TraceExceptionWrapper;

namespace ContentManager.Messanger.ViewModel
{

    public enum MessageSender
    {
        MessageSenderItemNode,
        MessageSenderException
    }

    public enum MessageType
    {
        NotImplemented,
        ApiException,
        Acknowledge
    }

    public enum MessageSeverity
    {
        Error,
        Warning,
        Information
    }

    public class MessageWrapperData
    {
        private MessageSender MessageSender { get; set; }
        private List<object> ActiveActionParameters { get; set; }

        public MessageType Type { private set; get; }
        private MessageSeverity Severity { set; get; }
        public bool NeedReloadData { private set; get; }

        public MessageWrapperData(TraceException te)
        {
            MessageWrapperApplicationErrorData(te.ApplicationErrorID, te.Parameters, te.ReloadData);
        }

        public MessageWrapperData(String applicationErrorID, List<String> parameters, bool reloadData, MessageSender sender, List<object> activeActionParameters)
        {
            MessageSender = sender;
            ActiveActionParameters = activeActionParameters;
            MessageWrapperApplicationErrorData(applicationErrorID, parameters, reloadData);
        }

        public MessageWrapperData(String text, bool reloadData, MessageType type, MessageSeverity severity)
        {
            MessageWrapperInit(text, reloadData, type, severity, "Title", "");
        }

        private void MessageWrapperApplicationErrorData(String applicationErrorID, List<String> parameters, bool reloadData)
        {
            String messageText = "";
            String messageTitle = "";
            String messageIcon = "";
            MessageType messageType = MessageType.NotImplemented;
            MessageSeverity messageSeverity = MessageSeverity.Error;

            if (Locator.ArgumentException == null && (applicationErrorID != "Open DB connection" && applicationErrorID != "Application not get parameters"))
            { 
                ReferenceTable referenceTable = new ReferenceTable(Locator.ConnectionString, Locator.ApplicationName);
                List<object> errorDescription = referenceTable.GetReferenceTableLine("ErrorDescription", new List<string> { Locator.ApplicationName, applicationErrorID });

                if (errorDescription.Count == 0)
                {
                    messageTitle = "Error message not found";
                    messageText = "Error message not found: " + applicationErrorID;
                    messageIcon = "";
                    messageSeverity = MessageSeverity.Error;
                    messageType = MessageType.NotImplemented;
                }
                else
                {
                    messageTitle = errorDescription[0].ToString();
                    messageText = UpdateMessageStringParameters(errorDescription[1].ToString(), parameters);
                    messageIcon = errorDescription[2].ToString();
                    messageSeverity = GetMessageSeverityFromString(errorDescription[3].ToString());
                    messageType = GetMessageTypeFromString(errorDescription[4].ToString());
                }            
            }

            if (Locator.ArgumentException != null)
            {
                messageTitle = "Parameter Error";
                messageText = "Parameter Error. " + Locator.ArgumentException.Message;
                messageIcon = "";
                messageSeverity = MessageSeverity.Error;
                messageType = MessageType.ApiException;
            }
            else
            {
                if (applicationErrorID == "Open DB connection")
                {
                    messageTitle = "Database Connection Error";
                    messageText = "Open DB connection failed. ";

                    if (parameters.Count > 0)
                        messageText += parameters[0];

                    messageIcon = "";
                    messageSeverity = MessageSeverity.Error;
                    messageType = MessageType.ApiException;
                }

                if(applicationErrorID == "Application not get parameters") 
                {
                    messageTitle = "Parameter Error";

                    if (parameters.Count > 0)
                        messageText = "Parameter " + parameters[0] + " was not found in parameters to application.";
                    else
                        messageText = "Absent parameters to application.";

                    messageIcon = "";
                    messageSeverity = MessageSeverity.Error;
                    messageType = MessageType.ApiException;                   
                }                
            }

            MessageWrapperInit(messageText, reloadData, messageType, messageSeverity, messageTitle, messageIcon);
        }

        private string UpdateMessageStringParameters(String message, List<String> parameteres)
        {
            if (parameteres != null)
            {
                for (int i = 0; i < parameteres.Count; i++)
                    message = message.Replace("{" + i + "}", parameteres[i]);
            }

            return message;
        }

        private MessageType GetMessageTypeFromString(String type)
        {
            switch (type)
            {
                case "Acknowledge":
                    return MessageType.Acknowledge;

                case "ApiException":
                    return MessageType.ApiException;

                default:
                    return MessageType.NotImplemented;
            }
        }

        private MessageSeverity GetMessageSeverityFromString(String severity)
        {
            switch (severity)
            {
                case "Warning":
                    return MessageSeverity.Warning;

                case "Information":
                    return MessageSeverity.Information;

                default:
                    return MessageSeverity.Error;
            }
        }

        public void MessageWrapperInit(String text, bool reloadData, MessageType type, MessageSeverity severity, String title, String image)
        {
            Type = type;
            Severity = severity;
            NeedReloadData = reloadData;

            Image = image;
            MessageTitle = title;
            MessageString = text;

            switch (Type)
            {
                case MessageType.NotImplemented:
                    BgColor = "Gray";
                    break;
                case MessageType.ApiException:
                    BgColor = "Red";
                    break;
                case MessageType.Acknowledge:
                    BgColor = "Green";
                    break;
                default:
                    BgColor = "Red";
                    break;
            }
       }

        #region Active action

        public void ActiveAction()
        {
            if (Type != MessageType.Acknowledge)
                return;

            switch (MessageSender)
            {
                case MessageSender.MessageSenderItemNode:
                    ActiveActionSenderItemNode();
                    break;
            }
        }

        public void ActiveActionSenderItemNode()
        {
            if (ActiveActionParameters == null)
                return;

            ItemNodeActionType itemNodeActionType = (ItemNodeActionType)ActiveActionParameters[0];

            switch (itemNodeActionType)
            {
                case ItemNodeActionType.Copy:
                    ItemNodeCopyMove.Copy((ItemNode)ActiveActionParameters[1], (ItemNode)ActiveActionParameters[2]);
                    break;

                case ItemNodeActionType.Move:
                    ItemNodeCopyMove.Move((ItemNode)ActiveActionParameters[1], (ItemNode)ActiveActionParameters[2]);
                    break;

                case ItemNodeActionType.Delete:

                    ItemNode itemToDelete = (ItemNode)ActiveActionParameters[1];

                    switch (itemToDelete.Type)
                    {
                        case TreeNodeObjectType.Folder:
                            Locator.ContentManagerDataProvider.ActiveUserControl(UserControlType.Folder, ItenNodeAction.Delete, itemToDelete, true);
                            break;
                        case TreeNodeObjectType.Content:
                            Locator.ContentManagerDataProvider.ActiveUserControl(UserControlType.Content, ItenNodeAction.Delete, itemToDelete, true);
                            break;
                        case TreeNodeObjectType.ContentVersion:
                            Locator.ContentManagerDataProvider.ActiveUserControl(UserControlType.Version, ItenNodeAction.Delete, itemToDelete, true);
                            break;
                    }
                    break;
            }
        }

        #endregion

        #region Message parameters

        public String Image { get; private set; }
        public String BgColor { get; private set; }
        public String MessageTitle { get; private set; }
        public String MessageString { get; private set; }

        #endregion
    }
}
