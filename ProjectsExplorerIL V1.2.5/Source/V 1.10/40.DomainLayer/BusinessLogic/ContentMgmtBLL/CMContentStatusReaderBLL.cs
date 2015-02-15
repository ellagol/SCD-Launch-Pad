using System;
using System.Collections.Generic;
using System.Data;
using ATSBusinessObjects.ContentMgmtModels;

namespace ATSBusinessLogic.ContentMgmtBLL
{
    class CMContentStatusReaderBLL
    {
        #region Methods

        internal Dictionary<String, CMContentStatusModel> GetStatusList()
        {
            CMContentStatusModel contentStatusTemp;
            DataTable typesDataTable = CMTreeNodeBLL.GetContentVersionStatusDataTable();
            Dictionary<String, CMContentStatusModel> status = new Dictionary<String, CMContentStatusModel>();

            foreach (DataRow row in typesDataTable.Rows)
            {
                contentStatusTemp = CreateContentTypeFromDataRow(row);
                status.Add(contentStatusTemp.ID, contentStatusTemp);
            }

            return status;
        }

        private CMContentStatusModel CreateContentTypeFromDataRow(DataRow row)
        {

            CMContentStatusModel contentStatus = new CMContentStatusModel
            {
                ID = (string)row["CVS_ID"],
                Name = (string)row["CVS_Name"],
                Icon = (string)row["CVS_Icon"]
                //LastUpdateUtil.UpdateObjectByDataReader(contentStatus, row);
            };
            return contentStatus;

        }
        #endregion
    }
}
