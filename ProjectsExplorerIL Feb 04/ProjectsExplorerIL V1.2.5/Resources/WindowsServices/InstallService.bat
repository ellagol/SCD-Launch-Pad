REM ********************************************************************************************************************
REM *** This bat file and the execution instructions are for .Net 4.0 on Windows7                                    ***
REM *** In Windows7 run as Administrator \Windows\SysWow64\Cmd.exe, change location to where this bat is, and run it ***
REM ********************************************************************************************************************

@echo on
Set SERVICE_HOME=<DirectoryWhereExecutablesReside>
Set SERVICE_NAME=Infra.DAL.SqlServerDAL
Set SERVICE_EXE=Infra.DAL.WindowsServiceHost.exe

REM the following directory is for .NET 2.0
REM Set INSTALL_UTIL_HOME=C:\Windows\Microsoft.NET\Framework\v2.0.50727

REM the following directory is for .NET 4.0
Set INSTALL_UTIL_HOME=C:\Windows\Microsoft.NET\Framework64\v4.0.30319

REM Account credentials if the service uses a user account
REM Other installation types (eg. LocalService) will not use it
Set USER_NAME=
Set PASSWORD=

CD %SERVICE_HOME%
Echo Installing Service (.Net 4.0)...
%INSTALL_UTIL_HOME%\InstallUtil.exe /LogToConsole=True /Name=%SERVICE_NAME% /Account=LocalSystem /User=%USER_NAME% /Password=%PASSWORD% %SERVICE_EXE%

Echo Done.
Pause
