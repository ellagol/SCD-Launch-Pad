﻿using System;
using System.Collections.Generic;
using System.Data;
using ATSBusinessObjects.ContentMgmtModels;

namespace ATSBusinessLogic.ContentMgmtBLL
{
    public class CMContentTypesReaderBLL
    {
        #region Methods

        internal Dictionary<String, CMContentTypeModel> GetTypesList()
        {
            CMContentTypeModel contentTypeTemp;
            DataTable typesDataTable = CMTreeNodeBLL.GetContentTypesDataTable();
            Dictionary<String, CMContentTypeModel> types = new Dictionary<String, CMContentTypeModel>();

            foreach (DataRow row in typesDataTable.Rows)
            {
                contentTypeTemp = CreateContentTypeFromDataRow(row);
                types.Add(contentTypeTemp.ID, contentTypeTemp);
            }

            return types;
        }

        private CMContentTypeModel CreateContentTypeFromDataRow(DataRow row)
        {

            CMContentTypeModel contentTypes = new CMContentTypeModel
            {
                ID = (string)row["CTY_ID"],
                Name = (string)row["CTY_Name"]
            };
            //LastUpdateUtil.UpdateObjectByDataReader(contentTypes, row);

            return contentTypes;
        }

        #endregion
    }
}
