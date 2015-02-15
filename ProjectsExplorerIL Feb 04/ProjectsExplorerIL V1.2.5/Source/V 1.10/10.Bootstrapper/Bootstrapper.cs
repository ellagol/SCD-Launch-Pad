using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ATSUI;
using ATSDomain;

namespace ATS
{

    /// <summary>
    /// Contains the <c>Main()</c> method that starts the application
    /// </summary>
    public static class Bootstrapper
    {

        /// <summary>
        /// Starts the application
        /// </summary>
        /// <param name="args">The collection of program arguments.</param>
        [STAThread]
        public static int Main(string[] args)
        {
            // Setup the main thread's name so we can easily identify it
            System.Threading.Thread.CurrentThread.Name = "Main application thread";

            #region Analyze the arguments passed via the command line (if any)

            string UserName = string.Empty;
            string Environment = string.Empty;
            string DbConnString = string.Empty;

            if (args.Length >= 1)
            {
                var CmdLineArgs = new CommandLineArgsParser(args);
                UserName = CmdLineArgs["User"][0];
                Environment = CmdLineArgs.Single("Environment"); // Same as previous statemnt; just showing different syntax
                DbConnString = CmdLineArgs.Single("DbConnString"); // Same as previous statemnt; just showing different syntax
            }

            #endregion

            #region Display Splash screen

            bool ShowSplash = false; //Show Splash screen?
            // Show Splash Screen
            if (ShowSplash)
            {
                Splasher.Splash = new SplashWindowView();
                Splasher.ShowSplash();

                //Contact server for latest version
                MessageListener.Instance.ReceiveMessage("Contacting Server...");
                for (long I = 1; I <= 300; I++)
                {
                    Thread.Sleep(1); //Do the work; Refresh the UI to show activities, if need be
                }

                //Check for latest version; update it
                MessageListener.Instance.ReceiveMessage("Checking for latest software updates...");
                for (long I = 1; I <= 300; I++)
                {
                    Thread.Sleep(1); //Do the work; Refresh the UI to show activities, if need be
                }

                //Initialize Application
                MessageListener.Instance.ReceiveMessage("Initializing Application...");
                for (long I = 1; I <= 300; I++)
                {
                    Thread.Sleep(1); //Do the work; Refresh the UI to show activities, if need be
                }
            }

            #endregion

#if !DEBUG
			// Don't catch exceptions when debugging - we want to have Visual Studio catch them where and when
			// they are thrown
			try {
#endif

            //	Initializes the Domain
            Domain.User = UserName;
            Domain.Environment = Environment;
            Domain.DbConnString = DbConnString;
            Domain.DomainInit();

            //	Initializes an instance of your System.Windows.Application derived class
            var application = new ATSUI.App();

            //	Instantiates the main window class
            var mainWindow = new ATSUI.MainWindowView();

            #region	Hide the splash screen

            if (ShowSplash)
            {
                Splasher.CloseSplash();
            }

            #endregion

            //	Calls Application.Run( mainWindow );
            application.Run(mainWindow);

            //	Perform application cleanup
            System.Diagnostics.Debug.WriteLine("The program exited normally");
            return 1;

#if !DEBUG
			}
			catch( Exception e ) {
				System.Diagnostics.Debug.WriteLine("There was a program exception: " + e);
				return -1;
			}//catch
#endif

        } // Main

    } // Class

} // Namespace
