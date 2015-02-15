using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATSBusinessObjects.ContentMgmtModels
{
    public class CMContentVersionSubVersionModel : CMLastUpdateModel
    {
        #region Properties

        public int Order;
        public CMContentModel Content;
        public CMVersionModel ContentSubVersion;

        #endregion
    }
}
