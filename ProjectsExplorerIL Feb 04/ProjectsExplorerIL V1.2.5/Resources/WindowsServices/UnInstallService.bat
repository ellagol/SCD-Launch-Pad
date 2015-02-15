REM ********************************************************************************************************************
REM *** This bat file and the execution instructions are for .Net 4.0 on Windows7                                    ***
REM *** In Windows7 run as Administrator \Windows\SysWow64\Cmd.exe, change location to where this bat is, and run it ***
REM ********************************************************************************************************************

@echo on
Set SERVICE_HOME=<DirectoryWhereExecutablesReside>
Set SERVICE_NAME=Infra.DAL.SqlServerDAL
Set SERVICE_EXE=Infra.DAL.WindowsServiceHost.exe

REM the following directory is for .NET 2.0 and above
REM Set INSTALL_UTIL_HOME=C:\Windows\Microsoft.NET\Framework\v2.0.50727

REM the following directory is for .NET 4.0
Set INSTALL_UTIL_HOME=C:\Windows\Microsoft.NET\Framework64\v4.0.30319

CD %SERVICE_HOME%
Echo Uninstalling Service (.Net 4.0)...
%INSTALL_UTIL_HOME%\InstallUtil.exe /LogToConsole=True /U /Name=%SERVICE_NAME% %SERVICE_EXE%

Echo Done.
Pause
