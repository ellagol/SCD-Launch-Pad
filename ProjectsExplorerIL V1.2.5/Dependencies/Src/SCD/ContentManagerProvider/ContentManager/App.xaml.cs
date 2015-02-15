using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using ADTS;
using ContentManager.General;

namespace ContentManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Locator.ArgumentException = null;
            Locator.ApplicationName = "Content manager";
            Locator.ComputerName = Environment.MachineName;

            if (e.Args.Length >= 1)
            {
                try
                {
                    var cmdLineArgs = new CommandLineArgsParser(e.Args);
                    Locator.UserName = cmdLineArgs.Single("User");
                    Locator.ConnectionString = cmdLineArgs.Single("DbConnString");
                }
                catch (ArgumentException argumentException)
                {
                    Locator.UserName = "";
                    Locator.ConnectionString = "";
                    Locator.ArgumentException = argumentException;
                }
            }
            
            base.OnStartup(e);
        }
    }
}
