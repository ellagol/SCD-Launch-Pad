@Echo Off
ATS.EXE /User=SysAdmin /Environment=ProductionRoni /DbConnString="Data Source=(local)\SQLExpress;Initial Catalog=GenPR_TestRoni;Integrated Security=true; Connection Timeout=30"
EXIT