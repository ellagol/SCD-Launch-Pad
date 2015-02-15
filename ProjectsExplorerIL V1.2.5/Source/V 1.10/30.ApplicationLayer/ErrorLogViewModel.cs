using System;
using System.Collections.ObjectModel;
using System.IO; //To support ImageSource property that holds the Thumb of the Workspace
using Infra.MVVM;
//To support ImageSource property that holds the Thumb of the Workspace

namespace ATSVM
{
    public class ErrorLogViewModel : WorkspaceViewModelBase
    {

        #region  Fields

        private IMessageBoxService MsgBoxService = null;
        private MessengerService MessageMediator = null;

        #endregion //Fields

        #region  Constructor

        public ErrorLogViewModel()
            : base("ErrorLogView", "")
        {
            //Initialize this VM
            DisplayName = "DAL Log";
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
            //Messenger Service (to exchange messages between VMs)
            MessageMediator = GetService<MessengerService>();
            //
            LogEntries = new ObservableCollection<LogEntry>();
            ReadLogFile();
        }

        #endregion //Constructor

        #region  Properties

        public ObservableCollection<LogEntry> LogEntries { get; set; }

        public String logfile = string.Empty;

        public string strLogEntries = string.Empty;

        #endregion //Properties

        #region  Private Helpers

        private void ReadLogFile()
        {
            string dPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\DataAccessServiceLog";
            string fName = dPath + "\\Errorlog.txt";
            if (!Directory.Exists(dPath))
                return;
            if (!File.Exists(fName))
                return;

            //using (StreamReader sr = File.OpenText(fName))
            //{
            //    String input;
            //    while ((input = sr.ReadLine()) != null)
            //    {
            //        LogEntry le = new LogEntry()
            //        {
            //            Message = input,
            //        };
            //        LogEntries.Add(le);
            //    }
            //}

            using (StreamReader sr = File.OpenText(fName))
            {
                String input;
                LogEntry le = new LogEntry();
                string eMessage = string.Empty;
                while ((input = sr.ReadLine()) != null)
                {
                    if (input.Contains("-.-.-.-.-.-.-.-.-.-.-.-"))
                    {
                        le.Message = eMessage + "\n" + le.Message;
                        eMessage = input;                         
                    }
                    else
                    {
                        eMessage = eMessage + "\n" + input;
                    }                
                }
                le.Message = eMessage + "\n" + le.Message;
                LogEntries.Add(le);
            }
        }


        #endregion //Private Helpers

        #region  Commands

        #endregion //Commands

        #region  Presentation Properties

        private string _DisplayName = string.Empty;
        public override string DisplayName
        {
            get
            {
                return _DisplayName;
            }
            set
            {
                _DisplayName = value;
            }
        }

        #endregion // Presentation Properties

    }

    #region LogEntry Class

    public class LogEntry
    {
        public string Message { get; set; }
    }

    #endregion

}
