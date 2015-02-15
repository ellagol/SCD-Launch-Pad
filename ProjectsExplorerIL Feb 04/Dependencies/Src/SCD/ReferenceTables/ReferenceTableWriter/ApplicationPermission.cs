using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using DatabaseProvider;

namespace ReferenceTableWriter
{
	public static class ApplicationPermission
	{
		private static bool _writePermission = false;

		public static void SetPermission(string user, string application, string connection)
		{
			var db = new DBprovider(connection, application);
			_writePermission = true;
		}

	
		public static bool GetWritePermission()
		{
			return _writePermission;
		}
	}
}
 