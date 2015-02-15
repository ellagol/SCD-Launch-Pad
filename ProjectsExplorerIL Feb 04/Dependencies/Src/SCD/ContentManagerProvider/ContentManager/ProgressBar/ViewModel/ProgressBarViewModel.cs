using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Windows;
using ContentManager.ContentManagerMainWindow.ViewModel;
using ContentManager.General;
using ContentManager.ProgressBar.View;
using ContentManagerProvider;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ContentManager.ProgressBar.ViewModel
{
    public class ProgressBarViewModel : ViewModelBase, ICopyFilesProgress
    {
        public bool Canceled { get; private set; }
        public RelayCommand Cancel { get; set; }

        public void IncreaseProgress(string copySource, string copyDestination, int progress)
        {
            Progress = progress;
            ProgressMessage = Progress + "/" + ProgressMax;

            ProgressMessageSource = copySource;
            ProgressMessageDestination = copyDestination;
        }

        public ProgressBarViewModel()
        {
            Canceled = false;
            Cancel = new RelayCommand(CancelRelayCommandFun);
        }

        private void CancelRelayCommandFun()
        {
            Canceled = true;
            Close();
        }

        private string _progressMessageSource = "From file";
        public string ProgressMessageSource
        {
            get { return _progressMessageSource; }
            set { Set(() => ProgressMessageSource, ref _progressMessageSource, value); }
        }

        private string _progressMessageDestination = "To file";
        public string ProgressMessageDestination
        {
            get { return _progressMessageDestination; }
            set { Set(() => ProgressMessageDestination, ref _progressMessageDestination, value); }
        }

        private string _progressMessage = "1/100";
        public string ProgressMessage
        {
            get { return _progressMessage; }
            set { Set(() => ProgressMessage, ref _progressMessage, value); }
        }

        private int _progress = 0;
        public int Progress
        {
            get { return _progress; }
            set
            { Set(() => Progress, ref _progress, value); }
        }

        private int _progressMax = 100;
        public int ProgressMax
        {
            get { return _progressMax; }
            set { Set(() => ProgressMax, ref _progressMax, value); }
        }

        public void Init(int progressMax)
        {
            Progress = 0;
            Canceled = false;
            ProgressMax = progressMax;
        }

        public void Show()
        {
            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(new Action(Show));
            else
                Locator.ContentManagerDataProvider.UserControlVisible = UserControlTypeVisible.ProgressBar;
        }

        public void Close()
        {
            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(new Action(Close));
            else
                Locator.ContentManagerDataProvider.UserControlVisible = UserControlTypeVisible.Na;
        }
    }
}
