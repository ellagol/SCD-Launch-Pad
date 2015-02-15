using System.ComponentModel.DataAnnotations;
using Infra.MVVM;
using System;
using System.Collections.Generic;
using ATSBusinessObjects;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading;
using ATSBusinessLogic;

namespace ExplorerModule
{
    public class ProgressBarViewModel : ViewModelBase
    {
        #region  Data

        private MessengerService MessageMediator = null;

        private Guid WorkspaceId;
        #endregion

        #region  Properties

        private String p_ProgressText = string.Empty;
        public String ProgressText
        {
            get { return p_ProgressText; }

            set
            {
                RaisePropertyChanged("ProgressText");
                p_ProgressText = value;
                RaisePropertyChanged("ProgressText");
            }
        }

        private String p_TitleText = string.Empty;
        public String TitleText
        {
            get { return p_TitleText; }

            set
            {
                RaisePropertyChanged("TitleText");
                p_TitleText = value;
                RaisePropertyChanged("TitleText");
            }
        }

        //The maximum progress value.
        private int p_ProgressMax = 10;
        public int ProgressMax
        {
            get { return p_ProgressMax; }

            set
            {
                RaisePropertyChanged("ProgressMax");
                p_ProgressMax = value;
                RaisePropertyChanged("ProgressMax");
            }
        }

        private float p_Progress = 0;
        public float Progress
        {
            get { return p_Progress; }

            set
            {
                RaisePropertyChanged("Progress");
                p_Progress = value;
                RaisePropertyChanged("Progress");
            }
        }
        #endregion

        #region  Constructor

        public ProgressBarViewModel(Guid WSId)
        {
            //Messenger Service (to exchange messages between VMs)
            MessageMediator = GetService<MessengerService>();
            this.WorkspaceId = WSId;

            MessageMediator.Register(this.WorkspaceId + "OnExportImportReceived", new Action<object[]>(OnExportImportReceived)); 
        }

        #endregion

        #region  CloseOverlay Command

        //private RelayCommand _CloseProgressBarDialogCommand;
        //public RelayCommand CloseProgressBarDialogCommand
        //{
        //    get
        //    {
        //        if (_CloseProgressBarDialogCommand == null)
        //        {
        //            _CloseProgressBarDialogCommand = new RelayCommand(ExecuteCloseProgressBarDialogCommand, CanExecuteCloseProgressBarDialogCommand);
        //        }
        //        return _CloseProgressBarDialogCommand;
        //    }
        //}

        //private bool CanExecuteCloseProgressBarDialogCommand()
        //{
        //    return true;
        //}

        private void ExecuteCloseProgressBarDialogCommand()
        {
            MessageMediator.NotifyColleagues(WorkspaceId + "CloseProgressBar"); //Will return to tree view
        }

        #endregion

        #region Progress bar methods

        public void IncrementProgressCounter()
        {
            if (FileSystemBLL.totalExportImportFiles != 0)
            {
                float totalCompleted = (float)FileSystemBLL.exportFilesCompleted 
                                            + (float)FileSystemBLL.importFilesCompleted
                                            + (float)FileSystemBLL.validationsCopmpleted;
                float upPrecent = (totalCompleted / ((float)FileSystemBLL.totalExportImportFiles)) * 10;
                IncrementProgressCounter(upPrecent);
            }
            else
            {
                IncrementProgressCounter(10);
            }
        }
        private float totalProgress = 0f;
        public void IncrementProgressCounter(float incrementClicks)
        {
            incrementClicks = (int)(incrementClicks * 10);

            totalProgress = incrementClicks;
            if (totalProgress > 99)
            {
                totalProgress = 100;
                Progress = p_ProgressMax;
                ProgressText = "100 %";
            }
            else if (totalProgress < 0)
            {
                totalProgress = 0;

                ProgressText = 0 + "%";
            }
            else
            {
                // Increment counter
                this.Progress = incrementClicks / 10;
                ProgressText = totalProgress + "%";
            }

            // Update progress message
            float progress = (p_Progress);
            float progressMax = (p_ProgressMax);
            float f = (progress / progressMax);
            float percentComplete = Single.IsNaN(f) ? 0 : (f);
            TitleText = ExportProjectToEnvBLL.progressBarTitle;
        }

        #endregion

        #region MessageMediator

        private void OnExportImportReceived(object[] parameteres)
        {
            ThreadStart threadStart = new ThreadStart(IncrementProgressCounter);
            Thread t = new Thread(threadStart);
            t.Start();
        }

        #endregion

    }
}
