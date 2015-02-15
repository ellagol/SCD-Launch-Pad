using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ContentManagerProvider.General;
using DatabaseProvider;

namespace ContentManagerProvider
{
    class ContentStatusReader
    {
         internal Dictionary<String, ContentStatus> GetStatusList()
         {
             ContentStatus contentStatusTemp;
             DataTable typesDataTable = GetTypesDataTable();
             Dictionary<String, ContentStatus> status = new Dictionary<String, ContentStatus>();

             foreach (DataRow row in typesDataTable.Rows)
             {
                 contentStatusTemp = CreateContentTypeFromDataRow(row);
                 status.Add(contentStatusTemp.ID, contentStatusTemp);
             }

             return status;
         }

         private ContentStatus CreateContentTypeFromDataRow(DataRow row)
         {

             ContentStatus contentStatus = new ContentStatus
                 {
                     ID = DBprovider.GetStringParam(row, "ID"),
                     Name = DBprovider.GetStringParam(row, "Name"),
                     Icon = DBprovider.GetStringParam(row, "Icon")
                 };
             LastUpdateUtil.UpdateObjectByDataReader(contentStatus, row);

             return contentStatus;
         } 

         private DataTable GetTypesDataTable()
         {
             string sqlCommand = "Select ";
             sqlCommand += "CVS_ID as ID, ";
             sqlCommand += "CVS_LastUpdateUser as UpdateUser, ";
             sqlCommand += "CVS_LastUpdateComputer as UpdateComputer, ";
             sqlCommand += "CVS_LastUpdateApplication as UpdateApplication, ";
             sqlCommand += "CVS_LastUpdateTime as UpdateTime, ";
             sqlCommand += "CVS_Name as Name, ";
             sqlCommand += "CVS_Icon as Icon ";
             sqlCommand += "From ";
             sqlCommand += "ContentVersionStatus";

             return Locator.DBprovider.ExecuteSelectCommand(sqlCommand);
         }
    }
}
