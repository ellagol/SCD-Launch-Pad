@Echo Off
ATS.EXE /User=SysAdmin /Environment=Production /DbConnString="Data Source=(local)\SQLExpress;Initial Catalog=FilesMigration;Integrated Security=true; Connection Timeout=30"
EXIT