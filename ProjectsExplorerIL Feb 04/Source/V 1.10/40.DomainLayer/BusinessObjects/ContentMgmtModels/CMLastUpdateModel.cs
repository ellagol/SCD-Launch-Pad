using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATSBusinessObjects.ContentMgmtModels
{
    public class CMLastUpdateModel
    {
        #region Properties

        public DateTime LastUpdateTime { get; set; }
        public String LastUpdateUser { get; set; }
        public String LastUpdateComputer { get; set; }
        public String LastUpdateApplication { get; set; }

        #endregion

        #region Methods

        public static void UpdateLastUpdate(CMLastUpdateModel source, CMLastUpdateModel destination)
        {
            destination.LastUpdateUser = source.LastUpdateUser;
            destination.LastUpdateTime = source.LastUpdateTime;
            destination.LastUpdateComputer = source.LastUpdateComputer;
            destination.LastUpdateApplication = source.LastUpdateApplication;
        }

        #endregion
    }
}
