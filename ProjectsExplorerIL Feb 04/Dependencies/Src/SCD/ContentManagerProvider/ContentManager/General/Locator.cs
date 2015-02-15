using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContentManager.ContentManagerMainWindow.ViewModel;
using ContentManager.ContentUpdate;
using ContentManager.ContentUpdate.ViewModel;
using ContentManager.FolderUpdate.ViewModel;
using ContentManager.FolderUpdate;
using ContentManager.Messanger.ViewModel;
using ContentManager.ProgressBar.ViewModel;
using ContentManager.Search.ViewModel;
using ContentManager.VersionUpdate.ViewModel;
using ContentManager.WhereUsed.View_Model;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using ContentManagerProvider;

namespace ContentManager.General
{
    public class Locator
    {
        public Locator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<ItemTreeBuilder>();
            SimpleIoc.Default.Register<SearchDataProvider>();
            SimpleIoc.Default.Register<VersionDataProvider>();
            SimpleIoc.Default.Register<FoldersDataProvider>();
            SimpleIoc.Default.Register<ContentsDataProvider>();
            SimpleIoc.Default.Register<MessangerDataProvider>();
            SimpleIoc.Default.Register<ContentManagerDataProvider>();
            SimpleIoc.Default.Register<ProgressBarViewModel>();
            SimpleIoc.Default.Register<WhereUsedViewModel>();
        }

        public static ItemTreeBuilder ItemTreeBuilder
        {
            get { return SimpleIoc.Default.GetInstance<ItemTreeBuilder>(); }
        }

        public static ProgressBarViewModel ProgressBarDataProvider
        {
            get { return SimpleIoc.Default.GetInstance<ProgressBarViewModel>(); }
        }

        public static WhereUsedViewModel WhereUsedViewModel
        {
            get { return SimpleIoc.Default.GetInstance<WhereUsedViewModel>(); }
        }

        public static ContentManagerDataProvider ContentManagerDataProvider
        {
            get { return SimpleIoc.Default.GetInstance<ContentManagerDataProvider>(); }
        }

        public static VersionDataProvider VersionDataProvider
        {
            get { return SimpleIoc.Default.GetInstance<VersionDataProvider>(); }
        }

        public static ContentManagerApiProvider ContentManagerApiProvider
        {
            get
            {
                if(!SimpleIoc.Default.IsRegistered<ContentManagerApiProvider>())
                    SimpleIoc.Default.Register<ContentManagerApiProvider>(() => new ContentManagerApiProvider(Locator.UserName, Locator.ComputerName, Locator.ApplicationName, Locator.ConnectionString));

                return SimpleIoc.Default.GetInstance<ContentManagerApiProvider>();
            }
        }

        public static FoldersDataProvider FoldersDataProvider
        {
            get { return SimpleIoc.Default.GetInstance<FoldersDataProvider>(); }
        }

        public static ContentsDataProvider ContentsDataProvider
        {
            get { return SimpleIoc.Default.GetInstance<ContentsDataProvider>(); }
        }

        public static MessangerDataProvider MessangerDataProvider
        {
            get { return SimpleIoc.Default.GetInstance<MessangerDataProvider>(); }
        }

        public static SearchDataProvider SearchDataProvider
        {
            get { return SimpleIoc.Default.GetInstance<SearchDataProvider>(); }
        }

        public static ArgumentException ArgumentException
        {
            get
            {
                if (!SimpleIoc.Default.IsRegistered<ArgumentException>("ArgumentException"))
                    return null;

                return SimpleIoc.Default.GetInstance<ArgumentException>("ArgumentException");
            }
            set
            {
                if (SimpleIoc.Default.IsRegistered<ArgumentException>("ArgumentException"))
                    SimpleIoc.Default.Unregister<ArgumentException>("ArgumentException");

                SimpleIoc.Default.Register<ArgumentException>(() => value, "ArgumentException");
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

        public static String ConnectionString
        {
            get
            {
                if (!SimpleIoc.Default.IsRegistered<String>("ConnectionString"))
                    return "";

                return SimpleIoc.Default.GetInstance<String>("ConnectionString");
            }
            set 
            {
                if (SimpleIoc.Default.IsRegistered<String>("ConnectionString"))
                    SimpleIoc.Default.Unregister<String>("ConnectionString");

                SimpleIoc.Default.Register<String>(() => value, "ConnectionString");
            }
        }

        public static Dictionary<int, Folder> Folders
        {
            get
            {
                if (!SimpleIoc.Default.IsRegistered <Dictionary<int, Folder>>("Folders"))
                    return null;

                return SimpleIoc.Default.GetInstance <Dictionary<int, Folder>>("Folders");
            }
            set 
            {
                if (SimpleIoc.Default.IsRegistered<Dictionary<int, Folder>>("Folders"))
                    SimpleIoc.Default.Unregister<Dictionary<int, Folder>>("Folders");

                SimpleIoc.Default.Register<Dictionary<int, Folder>>(() => value, "Folders");
            }
        }

        public static Dictionary<int, Content> Contents
        {
            get
            {
                if (!SimpleIoc.Default.IsRegistered<Dictionary<int, Content>>("Contents"))
                    return null;

                return SimpleIoc.Default.GetInstance<Dictionary<int, Content>>("Contents");
            }
            set 
            {
                if (SimpleIoc.Default.IsRegistered<Dictionary<int, Content>>("Contents"))
                    SimpleIoc.Default.Unregister<Dictionary<int, Content>>("Contents");

                SimpleIoc.Default.Register<Dictionary<int, Content>>(() => value, "Contents");
            }
        }

        public static Dictionary<int, ContentVersion> ContentVersions
        {
            get
            {
                if (!SimpleIoc.Default.IsRegistered<Dictionary<int, ContentVersion>>("ContentVersions"))
                    return null;

                return SimpleIoc.Default.GetInstance<Dictionary<int, ContentVersion>>("ContentVersions");
            }
            set 
            {
                if (SimpleIoc.Default.IsRegistered<Dictionary<int, ContentVersion>>("ContentVersions"))
                    SimpleIoc.Default.Unregister<Dictionary<int, ContentVersion>>("ContentVersions");

                SimpleIoc.Default.Register<Dictionary<int, ContentVersion>>(() => value, "ContentVersions");
            }
        }

        public static Dictionary<String, ContentStatus> ContentStatus
        {
            get
            {
                if (!SimpleIoc.Default.IsRegistered<Dictionary<String, ContentStatus>>("ContentStatus"))
                    return null;

                return SimpleIoc.Default.GetInstance<Dictionary<String, ContentStatus>>("ContentStatus");
            }
            set 
            {
                if (SimpleIoc.Default.IsRegistered<Dictionary<String, ContentStatus>>("ContentStatus"))
                    SimpleIoc.Default.Unregister<Dictionary<String, ContentStatus>>("ContentStatus");

                SimpleIoc.Default.Register<Dictionary<String, ContentStatus>>(() => value, "ContentStatus");
            }
        }

        public static Dictionary<String, UserGroupType> UserGroupTypes
        {
            get
            {
                if (!SimpleIoc.Default.IsRegistered<Dictionary<String, UserGroupType>>("UserGroupTypes"))
                    return null;

                return SimpleIoc.Default.GetInstance<Dictionary<String, UserGroupType>>("UserGroupTypes");
            }
            set 
            {
                if (SimpleIoc.Default.IsRegistered<Dictionary<String, UserGroupType>>("UserGroupTypes"))
                    SimpleIoc.Default.Unregister<Dictionary<String, UserGroupType>>("UserGroupTypes");

                SimpleIoc.Default.Register<Dictionary<String, UserGroupType>>(() => value, "UserGroupTypes");
            }
        }

        public static Dictionary<String, ContentType> ContentType
        {
            get
            {
                if (!SimpleIoc.Default.IsRegistered<Dictionary<String, ContentType>>("ContentType"))
                    return null;

                return SimpleIoc.Default.GetInstance<Dictionary<String, ContentType>>("ContentType");
            }
            set 
            {
                if (SimpleIoc.Default.IsRegistered<Dictionary<String, ContentType>>("ContentType"))
                    SimpleIoc.Default.Unregister<Dictionary<String, ContentType>>("ContentType");

                SimpleIoc.Default.Register<Dictionary<String, ContentType>>(() => value, "ContentType");
            }
        }

        public static Dictionary<string, string> SystemParameters
        {
            get
            {
                if (!SimpleIoc.Default.IsRegistered<Dictionary<string, string>>("SystemParameters"))
                    return null;

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
