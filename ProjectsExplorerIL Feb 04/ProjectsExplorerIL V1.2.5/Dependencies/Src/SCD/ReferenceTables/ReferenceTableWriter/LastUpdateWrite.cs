using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LastUpdateUtilities;

namespace ReferenceTableWriter
{
	class LastUpdateWrite
	{
		public static LastUpdate Details = new LastUpdate();
		public static string DllName = Assembly.GetExecutingAssembly().GetName().Name;
	}
}
