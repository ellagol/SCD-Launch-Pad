using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentManagerProvider
{
    public interface ICopyFilesProgress
    {
        bool Canceled { get; }
        int ProgressMax { get; set; }

        void Show();
        void Close();
        void Init(int progressMax);
        void IncreaseProgress(string copySource, string copyDestination, int progress);
    }
}
