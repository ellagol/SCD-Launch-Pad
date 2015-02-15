@Echo Off
ATS.EXE /User=SysAdmin /Environment=Production /DbConnString="Data Source=(local)\SQLExpress;Initial Catalog=GenPR_Test;Integrated Security=true; Connection Timeout=30"
EXIT