using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using ProfileManagerProvider;
using DatabaseProvider;
using ReferenceTableReader;

namespace ContentManagerProvider.General
{
    class Locator
    {
        public Locator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
        }

        public static Impersonation Impersonation
        {
            get
            {
                if (!SimpleIoc.Default.IsRegistered<Impersonation>())
                    SimpleIoc.Default.Register<Impersonation>(); 

                return SimpleIoc.Default.GetInstance<Impersonation>();
            }
        }
       
        public static String UserName
        {
            get
            {
                if (!SimpleIoc.Default.IsRegistered<String>("UserName"))
                    return "";

                return SimpleIoc.Default.GetInstance<String>("UserName");
            }
            set
            {
                if (SimpleIoc.Default.IsRegistered<String>("UserName"))
                    SimpleIoc.Default.Unregister<String>("UserName");

                SimpleIoc.Default.Register<String>(() => value, "UserName");
            }
        }

        public static String ComputerName
        {
            get
            {
                if (!SimpleIoc.Default.IsRegistered<String>("ComputerName"))
                    return "";

                return SimpleIoc.Default.GetInstance<String>("ComputerName");
            }
            set
            {
                if (SimpleIoc.Default.IsRegistered<String>("ComputerName"))
                    SimpleIoc.Default.Unregister<String>("ComputerName");

                SimpleIoc.Default.Register<String>(() => value, "ComputerName");
            }
        }

        public static String ApplicationName
        {
            get
            {
                if (!SimpleIoc.Default.IsRegistered<String>("ApplicationName"))
                    return "";

                return SimpleIoc.Default.GetInstance<String>("ApplicationName");
            }
            set
            {
                if (SimpleIoc.Default.IsRegistered<String>("ApplicationName"))
                    SimpleIoc.Default.Unregister<String>("ApplicationName");

                SimpleIoc.Default.Register<String>(() => value, "ApplicationName");
            }
        }

        public static DBprovider DBprovider
        {
            get
            {
                if (!SimpleIoc.Default.IsRegistered<DBprovider>("DBprovider"))
                    return null;

                return SimpleIoc.Default.GetInstance<DBprovider>("DBprovider");
            }
            set
            {
                if (SimpleIoc.Default.IsRegistered<DBprovider>("DBprovider"))
                    SimpleIoc.Default.Unregister<DBprovider>("DBprovider");

                SimpleIoc.Default.Register<DBprovider>(() => value, "DBprovider");
            }
        }

        public static ProfileProvider ProfileManagerProvider
        {
            get
            {
                if (!SimpleIoc.Default.IsRegistered<ProfileProvider>("ProfileManagerProvider"))
                    return null;

                ProfileProvider profileManagerProvider = SimpleIoc.Default.GetInstance<ProfileProvider>("ProfileManagerProvider");
                profileManagerProvider.UpdateTransaction(DBprovider.Transaction);
                return profileManagerProvider;
            }
            set
            {
                if (SimpleIoc.Default.IsRegistered<ProfileProvider>("ProfileManagerProvider"))
                    SimpleIoc.Default.Unregister<ProfileProvider>("ProfileManagerProvider");

                SimpleIoc.Default.Register<ProfileProvider>(() => value, "ProfileManagerProvider");
            }
        }

        public static Dictionary<string, string> SystemParameters
        {
            get
            {
                if (!SimpleIoc.Default.IsRegistered<Dictionary<string, string>>("SystemParameters"))
                {

                    ReferenceTable referenceTable = new ReferenceTable(DBprovider.Connection, DBprovider.Transaction, ApplicationName);
                    List<List<object>> systemParameters = referenceTable.GetReferenceTableTable("SystemParameters", null);

                    Dictionary<string, string> referenceTableParameters = new Dictionary<string, string>();

                    foreach (List<object> systemParameter in systemParameters)
                        referenceTableParameters.Add(systemParameter[0].ToString(), systemParameter[1].ToString());

                    SimpleIoc.Default.Register<Dictionary<string, string>>(() => referenceTableParameters, "SystemParameters");
                }

                return SimpleIoc.Default.GetInstance<Dictionary<string, string>>("SystemParameters");
            }
            set
            {
                if (SimpleIoc.Default.IsRegistered<Dictionary<string, string>>("SystemParameters"))
                    SimpleIoc.Default.Unregister<Dictionary<string, string>>("SystemParameters");

                SimpleIoc.Default.Register<Dictionary<string, string>>(() => value, "SystemParameters");
            }
        }
    }
}
