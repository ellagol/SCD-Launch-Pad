using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATSBusinessObjects.ContentMgmtModels
{
    public interface CMICopyFilesProgressModel
    {
        #region Properties

        bool Canceled { get; }
        int ProgressMax { get; set; }

        #endregion

        #region Methods

        void Show();
        void Close();
        void Init(int progressMax);
        void IncreaseProgress(string copySource, string copyDestination, int progress);

        #endregion
    }
}
