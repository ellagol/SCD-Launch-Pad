using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ContentManager.ContentManagerMainWindow.ViewModel;
using ContentManager.General;
using ContentManagerProvider;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ProjectExplorerTester;

namespace ContentManager.WhereUsed.View_Model
{
    public class WhereUsedViewModel : ViewModelBase
    {
        public RelayCommand Close { get; set; }
        public WhereUsedViewModel()
        {
            Close = new RelayCommand(CloseRelayCommandFun);
            ContentProjects = new ObservableCollection<WhereUsedProjectItem>();
            ContentLinks = new ObservableCollection<WhereUsedContentLinkItem>();

        }

        public void UpdateContentVersion(int contentVersionID)
        {
            ProjectExplorerApi projectExplorer = new ProjectExplorerApi(Locator.UserName, Locator.ComputerName, Locator.ApplicationName, Locator.ConnectionString);
            Dictionary<int, Project> contentVersionProjects = projectExplorer.GetProjectsByContentVersionID(contentVersionID);

            ContentProjects = new ObservableCollection<WhereUsedProjectItem>();

            foreach (KeyValuePair<int, Project> versionProject in contentVersionProjects)
            {
                ContentProjects.Add(new WhereUsedProjectItem
                    {
                        Name = versionProject.Value.Name,
                        Step = versionProject.Value.Step,
                        Code = versionProject.Value.Code,
                        HierarchyPath = versionProject.Value.HierarchyPath
                    });
            }

            ContentLinks = new ObservableCollection<WhereUsedContentLinkItem>();

            foreach (KeyValuePair<int, ContentVersion> contentVersion in Locator.ContentVersions)
            {
                foreach (KeyValuePair<int, ContentVersionSubVersion> contentVersionSubVersion in contentVersion.Value.ContentVersions)
                {
                    if (contentVersionSubVersion.Value.ContentSubVersion.ID == contentVersionID)
                    {
                        ContentLinks.Add(new WhereUsedContentLinkItem
                        {
                            ContentName = Locator.Contents[contentVersion.Value.ParentID].Name,
                            VersionName = contentVersion.Value.Name
                        });
                    }
                }
            }
        }

        private ObservableCollection<WhereUsedProjectItem> _contentProjects;
        public ObservableCollection<WhereUsedProjectItem> ContentProjects
        {
            get { return _contentProjects; }
            set { Set(() => ContentProjects, ref _contentProjects, value); }
        }

        private ObservableCollection<WhereUsedContentLinkItem> _contentLinks;
        public ObservableCollection<WhereUsedContentLinkItem> ContentLinks
        {
            get { return _contentLinks; }
            set { Set(() => ContentLinks, ref _contentLinks, value); }
        }

        private void CloseRelayCommandFun()
        {
            Locator.ContentManagerDataProvider.UserControlVisible = UserControlTypeVisible.Na;
        }
    }
}
