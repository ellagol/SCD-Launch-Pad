﻿using System;
using System.Collections.Generic;
using System.Data;
using ATSBusinessObjects.ContentMgmtModels;

namespace ATSBusinessLogic.ContentMgmtBLL
{
    class CMUserGroupTypesReaderBLL
    {
        #region Methods

        static public Dictionary<String, CMUserGroupTypeModel> GetUserGroupTypes()
        {
            CMUserGroupTypeModel userGroupType;
            DataTable typesDataTable = CMTreeNodeBLL.GetUserGroupTypesDataTable();
            Dictionary<String, CMUserGroupTypeModel> userGroupTypes = new Dictionary<String, CMUserGroupTypeModel>();

            foreach (DataRow row in typesDataTable.Rows)
            {
                userGroupType = CreateUserGroupTypeFromDataRow(row);
                userGroupTypes.Add(userGroupType.ID, userGroupType);
            }

            return userGroupTypes;
        }

        static private CMUserGroupTypeModel CreateUserGroupTypeFromDataRow(DataRow row)
        {
            CMUserGroupTypeModel userGroupType = new CMUserGroupTypeModel
            {
                ID = (string)row["GT_ID"],
                Name = (string)row["GT_Name"]
            };

            return userGroupType;
        }

        #endregion
    }
}
