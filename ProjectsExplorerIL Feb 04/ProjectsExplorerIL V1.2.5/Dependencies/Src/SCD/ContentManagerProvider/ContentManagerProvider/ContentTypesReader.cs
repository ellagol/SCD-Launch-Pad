using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ContentManagerProvider.General;
using DatabaseProvider;

namespace ContentManagerProvider
{
    class ContentTypesReader
    {
        internal Dictionary<String, ContentType> GetTypesList()
        {
            ContentType contentTypeTemp;
            DataTable typesDataTable = GetTypesDataTable();
            Dictionary<String, ContentType> types = new Dictionary<String, ContentType>();

            foreach (DataRow row in typesDataTable.Rows)
            {
                contentTypeTemp = CreateContentTypeFromDataRow(row);
                types.Add(contentTypeTemp.ID, contentTypeTemp);
            }

            return types;
        }

        private ContentType CreateContentTypeFromDataRow(DataRow row)
        {

            ContentType contentTypes = new ContentType
            {
                ID = DBprovider.GetStringParam(row, "ID"),
                Name = DBprovider.GetStringParam(row, "Name")
            };
            LastUpdateUtil.UpdateObjectByDataReader(contentTypes, row);

            return contentTypes;
        }

        private DataTable GetTypesDataTable()
        {
            string sqlCommand = "Select ";
            sqlCommand += "CTY_ID as ID, ";
            sqlCommand += "CTY_LastUpdateUser as UpdateUser, ";
            sqlCommand += "CTY_LastUpdateComputer as UpdateComputer, ";
            sqlCommand += "CTY_LastUpdateApplication as UpdateApplication, ";
            sqlCommand += "CTY_LastUpdateTime as UpdateTime, ";
            sqlCommand += "CTY_Name as Name ";
            sqlCommand += "From ";
            sqlCommand += "ContentType";

            return Locator.DBprovider.ExecuteSelectCommand(sqlCommand);
        }
    }
}
