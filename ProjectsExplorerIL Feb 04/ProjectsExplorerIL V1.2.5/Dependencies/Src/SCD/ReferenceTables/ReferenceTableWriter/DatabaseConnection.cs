using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseProvider;

namespace ReferenceTableWriter
{
	public class DatabaseConnection
	{
		public static DBprovider DBaseRTable = null;
		public static string ConnectionString { get; set; }
	}

}
