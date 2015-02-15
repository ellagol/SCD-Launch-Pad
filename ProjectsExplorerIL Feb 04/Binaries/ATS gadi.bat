@Echo Off
ATS.EXE /User="gadi kabakov" /Environment=Production /DbConnString="Data Source=(local)\SQLExpress;Initial Catalog=GenPR_Test;Integrated Security=true; Connection Timeout=30"
EXIT