using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Principal;
using TraceExceptionWrapper;

namespace ATSBusinessLogic.ContentMgmtBLL
{
    public class CMImpersonationBLL
    {
        #region LogonUser 32

        enum LogonType
        {
            Interactive = 2,
            Network = 3,
            Batch = 4,
            Service = 5,
            Unlock = 7,
            NetworkClearText = 8,
            NewCredentials = 9
        }

        enum LogonProvider
        {
            Default = 0,
            WinNT35 = 1,
            WinNT40 = 2,
            WinNT50 = 3
        }

        public const int Logon32LogonInteractive = 2;
        public const int Logon32LogonProviderDefault = 0;

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUserName, String lpszDomain, String lpszPassword,
                                            int dwLogonType, int dwLogonPassword, ref IntPtr phTokeb);

        //public static extern bool LogonUser(String lpszUserName, String lpszDomain, String lpszPassword,
        //                            int dwLogonType, int dwLogonPassword, out SafeTokenHandle phTokeb);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handel);

        private IntPtr _userHandle = new IntPtr(0);
        private WindowsImpersonationContext _windowsImpersonationUser = null;

        #endregion

        #region Data

        private String ImpersonationUser { get; set; }
        private String ImpersonationDomain { get; set; }
        private String ImpersonationPassword { get; set; }

        #endregion

        #region Constructor

        public CMImpersonationBLL()
        {
            Dictionary<string, string> SystemParameters = new Dictionary<string, string>();
            SystemParameters = CMTreeNodeBLL.getSystemParameters();

            ImpersonationUser = SystemParameters["UserNameFS"];
            ImpersonationDomain = SystemParameters["DomainFS"];
            ImpersonationPassword = SystemParameters["PasswordFS"];
        }

        #endregion

        #region Methods

        public bool Logon()
        {
            try
            {

                bool returnValue = LogonUser(ImpersonationUser, ImpersonationDomain, ImpersonationPassword,
                                                (int)LogonType.NewCredentials,
                                                (int)LogonProvider.WinNT50,
                                                ref _userHandle);

                if (!returnValue)
                    throw new TraceException("Impersonation fail", false, new List<string>() { "LogonUser function return false result" }, "Content Manager");

                _windowsImpersonationUser = WindowsIdentity.Impersonate(_userHandle);
            }
            catch (Exception e)
            {
                throw new TraceException("Impersonation fail", false, new List<string>() { e.Message }, "Content Manager");
            }


            return true;
        }

        public void Dispose()
        {
            if (_windowsImpersonationUser != null)
            {
                _windowsImpersonationUser.Undo();
                _windowsImpersonationUser = null;
                CloseHandle(_userHandle);
            }
        }

        #endregion
    }
}
