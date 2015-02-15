using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ReferenceTableManager
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			var application = Assembly.GetExecutingAssembly().GetName().Name;
			try
			{
				Application.Run(new ReferenceTableManager("miriamr", application, 
					"Data Source=Hermes;Initial Catalog=GenPR_Test;Persist Security Info=True;User ID=GenPR_Test_User;Password=GenPR_Test_User"));
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message + " " + e.StackTrace, application, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
