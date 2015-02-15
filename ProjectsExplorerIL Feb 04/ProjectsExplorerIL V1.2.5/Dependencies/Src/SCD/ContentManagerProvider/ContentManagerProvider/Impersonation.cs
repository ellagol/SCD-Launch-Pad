using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Principal;
using ContentManagerProvider.General;

namespace ContentManagerProvider
{
    internal class Impersonation
    {
        #region LogonUser 32

        public const int Logon32LogonInteractive = 2;
        public const int Logon32LogonProviderDefault = 0;

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUserName, String lpszDomain, String lpszPassword,
                                            int dwLogonType, int dwLogonPassword, ref IntPtr phTokeb);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handel);

        private IntPtr _userHandle = new IntPtr(0);
        private WindowsImpersonationContext _windowsImpersonationUser = null;

        #endregion

        private String ImpersonationUser { get; set; }
        private String ImpersonationDomain { get; set; }
        private String ImpersonationPassword { get; set; }

        public Impersonation()
        {
            ImpersonationUser = Locator.SystemParameters["UserNameFS"];
            ImpersonationDomain = Locator.SystemParameters["DomainFS"];
            ImpersonationPassword = Locator.SystemParameters["PasswordFS"];
        }

        public bool Logon()
        {

            bool res = LogonUser(ImpersonationUser, ImpersonationDomain, ImpersonationPassword, Logon32LogonInteractive, Logon32LogonProviderDefault,
                                 ref _userHandle);

            if (!res)
                return false;

            WindowsIdentity newID = new WindowsIdentity(_userHandle);
            _windowsImpersonationUser = newID.Impersonate();

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
    }
}
