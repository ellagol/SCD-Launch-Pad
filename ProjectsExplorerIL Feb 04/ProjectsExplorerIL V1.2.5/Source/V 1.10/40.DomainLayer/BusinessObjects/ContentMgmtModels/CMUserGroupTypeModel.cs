using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATSBusinessObjects.ContentMgmtModels
{
    public class CMUserGroupTypeModel : CMLastUpdateModel
    {
        #region Properties

        public String ID { get; set; }
        public String Name { get; set; }
        public bool Checked { get; set; }

        #endregion
    }
}
