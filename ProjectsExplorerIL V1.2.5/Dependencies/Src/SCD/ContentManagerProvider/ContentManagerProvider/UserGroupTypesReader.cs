using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ContentManagerProvider.General;
using DatabaseProvider;

namespace ContentManagerProvider
{
    class UserGroupTypesReader
    {
        static public Dictionary<String, UserGroupType> GetUserGroupTypes()
        {
            UserGroupType userGroupType;
            DataTable typesDataTable = GetUserGroupTypesDataTable();
            Dictionary<String, UserGroupType> userGroupTypes = new Dictionary<String, UserGroupType>();

            foreach (DataRow row in typesDataTable.Rows)
            {
                userGroupType = CreateUserGroupTypeFromDataRow(row);
                userGroupTypes.Add(userGroupType.ID, userGroupType);
            }

            return userGroupTypes;
        }

        static public String GetUserGroupType()
        {
            string sqlCommand = "SELECT UGT_id_GroupTypes as ID ";
            sqlCommand += "FROM UserGroupType ";
            sqlCommand += "WHERE (UGT_id_UserName = '" + Locator.UserName + "')";

            DataTable dataTable = Locator.DBprovider.ExecuteSelectCommand(sqlCommand);
            return dataTable.Rows.Count == 0 ? "" : DBprovider.GetStringParam(dataTable.Rows[0], "ID");
        }

        static private DataTable GetUserGroupTypesDataTable()
        {
            string sqlCommand = "Select ";
            sqlCommand += "GT_ID as ID, ";
            sqlCommand += "GT_LastUpdateUser as UpdateUser, ";
            sqlCommand += "GT_LastUpdateComputer as UpdateComputer, ";
            sqlCommand += "GT_LastUpdateApplication as UpdateApplication, ";
            sqlCommand += "GT_LastUpdateTime as UpdateTime, ";
            sqlCommand += "GT_Name as Name ";
            sqlCommand += "From ";
            sqlCommand += "GroupTypes"; 

            return Locator.DBprovider.ExecuteSelectCommand(sqlCommand);
        }

        static private UserGroupType CreateUserGroupTypeFromDataRow(DataRow row)
        {
            UserGroupType userGroupType = new UserGroupType
                {
                    ID = DBprovider.GetStringParam(row, "ID"),
                    Name = DBprovider.GetStringParam(row, "Name")
                };

            return userGroupType;
        }
    }
}
