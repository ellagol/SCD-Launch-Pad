using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ContentManagerProvider.General;

namespace ContentManagerProvider
{
    public class LastUpdateUtil
    {
        public static void UpdateObjectByDataReader(LastUpdate objectLastUpdate, DataRow row)
        {
            UpdateObjectByDataReader(objectLastUpdate, row, "UpdateTime", "UpdateUser", "UpdateComputer", "UpdateApplication");
        }

        public static void UpdateObjectByDataReader(LastUpdate objectLastUpdate, DataRow row, string columnNameUpdateTime, string columnNameUpdateUser, string columnNameUpdateComputer, string columnNameUpdateApplication)
        {
            objectLastUpdate.LastUpdateTime = (DateTime)row[columnNameUpdateTime];
            objectLastUpdate.LastUpdateUser = (String)row[columnNameUpdateUser];
            objectLastUpdate.LastUpdateComputer = (String)row[columnNameUpdateComputer];
            objectLastUpdate.LastUpdateApplication = (String)row[columnNameUpdateApplication];
        }

        public static void UpdateObjectLastUpdate(LastUpdate objectLastUpdate)
        {
            objectLastUpdate.LastUpdateUser = Locator.UserName;
            objectLastUpdate.LastUpdateComputer = Locator.ComputerName;
            objectLastUpdate.LastUpdateApplication = Locator.ApplicationName;
            objectLastUpdate.LastUpdateTime = DateTime.Now;
        }
    }
}
