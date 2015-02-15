using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ContentManager.ContentManagerMainWindow.ViewModel;
using ContentManager.General;
using ContentManagerProvider;
using GalaSoft.MvvmLight;

namespace ContentManager.VersionUpdate.ViewModel
{
    public class ItemNodeVersion : ObservableObject, IDataErrorInfo
    {

        public bool AllPropertiesValid { set; get; }
        private readonly Dictionary<string, bool> _validProperties;
        public ObservableCollection<ItemFileNode> SubItemNode { set; get; }
        public ObservableCollection<ItemVersionLink> SubItemVersionLinkNode { set; get; }

        public void SubItemVersionLinkNodeAdd(ItemVersionLink sourceItemVersionLinkNode)
        {
            SubItemVersionLinkNode.Add(sourceItemVersionLinkNode);
            RaisePropertyChanged("Status");
        }

        public void SubItemVersionLinkNodeRemove(ItemVersionLink sourceItemVersionLinkNode)
        {
            SubItemVersionLinkNode.Remove(sourceItemVersionLinkNode);
            RaisePropertyChanged("Status");
        }

        public ItemNodeVersion()
        {
            SubItemNode = new ObservableCollection<ItemFileNode>();
            SubItemVersionLinkNode = new ObservableCollection<ItemVersionLink>();
            _validProperties = new Dictionary<string, bool> { { "Name", false }, { "Status", false }, { "Editor", false }, { "Description", false }, { "RunningString", false }, { "ECR", false }, { "DocumentID", false } };
        }

        public void InitParameters(int contentVersionID)
        {
            if (contentVersionID == 0)
                InitAddParameters();
            else
                InitUpdateParameters(contentVersionID);
        }

        private void InitAddParameters()
        {
            ECR = String.Empty;
            DocumentID = String.Empty;
            Path = String.Empty;
            Name = String.Empty;
            Editor = String.Empty;
            EditorName = String.Empty;
            IsVisibleEditorName = false;
            Description = String.Empty;
            RunningString = String.Empty;

            Status = Locator.VersionDataProvider.ContentStatusList[0];

            if (Locator.VersionDataProvider.ContentStatusList.Count > 1) // Bug of WPF
            {
                Status = Locator.VersionDataProvider.ContentStatusList[1];
                Status = Locator.VersionDataProvider.ContentStatusList[0];
            }

            SubItemNode.Clear();
            SubItemVersionLinkNode.Clear();
        }

        private void InitUpdateParameters(int contentVersionID)
        {

            ContentVersion version = Locator.ContentVersions[contentVersionID];

            ECR = version.ECR;
            DocumentID = version.DocumentID;
            Name = version.Name;
            Editor = version.Editor;
            EditorName = Editor == String.Empty ? String.Empty : version.LastUpdateUser;
            IsVisibleEditorName = EditorName != String.Empty; 
            Description = version.Description;
            RunningString = version.RunningString;

            if (version.Path.Type == PathType.Full)
                Path = version.Path.Name;
            else
                Path = Locator.SystemParameters["RootPathFS"] + "\\" + version.Path.Name;

            SubItemNode.Clear();
            UpdateFile(version.Files);
            InitUpdateParameterSubVersions(version.ContentVersions);
            Status = GetObservableContentStatusByID(version.Status.ID);
        }

        private void InitUpdateParameterSubVersions(Dictionary<int, ContentVersionSubVersion> subVersions)
        {
            SubItemVersionLinkNode.Clear();

            foreach (KeyValuePair<int, ContentVersionSubVersion> subVersion in subVersions)
            {
                    ItemVersionLink versionLink = new ItemVersionLink
                    {
                        ContentVersionID = subVersion.Value.ContentSubVersion.ID,
                        ContentID = subVersion.Value.Content.ID,
                        Icon = ItemTreeBuilder.GetContentVersionIcon(subVersion.Value.ContentSubVersion),
                        ContentName = subVersion.Value.Content.Name,
                        Name = subVersion.Value.ContentSubVersion.Name
                    };
                SubItemVersionLinkNode.Add(versionLink);
            }
        }

        private ObservableContentStatus GetObservableContentStatusByID(String contentID)
        {
            foreach (ObservableContentStatus observableContentStatuse in Locator.VersionDataProvider.ContentStatusList)
            {
                if (observableContentStatuse.ID == contentID)
                    return observableContentStatuse;
            }

            return Locator.VersionDataProvider.ContentStatusList[0];
        }

        #region Init files

        private void UpdateFile(Dictionary<int, ContentFile> files)
        {
            ItemFileNode fileNode;
            foreach (KeyValuePair<int, ContentFile> file in files)
            {
                fileNode = CreateItemFileNode(file.Value);
                AddFileItemToFolder(file.Value.FileRelativePath, fileNode);
            }
        }

        private void AddFileItemToFolder(string filePath, ItemFileNode fileNode)
        {
            bool existSubFolder;
            filePath = filePath.Trim();

            ItemFileNode parentNode = null;
            ObservableCollection<ItemFileNode> parentSubItemNodes = SubItemNode;

            if (filePath == String.Empty)
            {
                fileNode.Parent = null;
                SubItemNode.Add(fileNode);
                return;
            }

            string[] folders = filePath.Split('\\');

            foreach (string folderName in folders)
            {
                existSubFolder = false;
                foreach (ItemFileNode itemFolder in parentSubItemNodes)
                {
                    if (itemFolder.Name == folderName && itemFolder.Type == ItemFileNodeType.Folder)
                    {
                        parentNode = itemFolder;
                        parentSubItemNodes = itemFolder.SubItemNode;
                        existSubFolder = true;
                    }
                }

                if (!existSubFolder)
                {
                    ItemFileNode newSubFolder = CreateItemFolderNode(folderName, parentNode);
                    parentNode = newSubFolder;
                    parentSubItemNodes.Add(newSubFolder);
                    parentSubItemNodes = newSubFolder.SubItemNode;
                }
            }
            fileNode.Parent = parentNode;
            parentSubItemNodes.Add(fileNode);
        }

        private ItemFileNode CreateItemFolderNode(string name, ItemFileNode parent)
        {
            ItemFileNode fileNode = new ItemFileNode
                {
                    ID = 0,
                    Name = name,
                    Parent = parent,
                    Type = ItemFileNodeType.Folder,
                    Status = ItemFileStatus.Exist,
                    SubItemNode = new ObservableCollection<ItemFileNode>()
                };

            return fileNode;
        }

        private ItemFileNode CreateItemFileNode(ContentFile file)
        {
            ItemFileNode fileNode = new ItemFileNode
                {
                    ID = file.ID,
                    Name = file.FileName,
                    Path = file.FileRelativePath,
                    Type = ItemFileNodeType.File,
                    Status = ItemFileStatus.Exist
                };

            return fileNode;
        }

        #endregion
        
        #region Observable objects

        private string _name = "Name";

        public string Name
        {
            get { return _name; }
            set { Set(() => Name, ref _name, value); }
        }

        private string _description = "Description";
        public string Description
        {
            get { return _description; }
            set { Set(() => Description, ref _description, value); }
        }

        private string _editor = "Editor";
        public string Editor
        {
            get { return _editor; }
            set { Set(() => Editor, ref _editor, value); }
        }

        private string _editorName;
        public string EditorName
        {
            get { return _editorName; }
            set { Set(() => EditorName, ref _editorName, value); }
        }

        private bool _isVisibleEditorName;
        public bool IsVisibleEditorName
        {
            get { return _isVisibleEditorName; }
            set { Set(() => IsVisibleEditorName, ref _isVisibleEditorName, value); }
        }

        private string _ecr = "ECR";
        public string ECR
        {
            get { return _ecr; }
            set { Set(() => ECR, ref _ecr, value); }
        }

        private string _documentID = "DocumentID";
        public string DocumentID
        {
            get { return _documentID; }
            set { Set(() => DocumentID, ref _documentID, value); }
        }

        private string _path;
        public string Path
        {
            get { return _path; }
            set { Set(() => Path, ref _path, value); }
        }

        private string _runningString = "RunningString";
        public string RunningString
        {
            get { return _runningString; }
            set { Set(() => RunningString, ref _runningString, value); }
        }

        private ObservableContentStatus _status = null;
        public ObservableContentStatus Status
        {
            get { return _status; }
            set { Set(() => Status, ref _status, value); }
        }

        #endregion

        #region IDataErrorInfo Members

        public string Error
        {
            get { throw new System.NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get
            {
                string validationResult = null;
                switch (columnName)
                {
                    case "Name":
                        if (String.IsNullOrEmpty(Name))
                        {
                            validationResult = "Version Name needs to be entered.";
                        }
                        else
                        {
                            if (Name.Length > 50)
                                validationResult = "Max length of version name is 50 characters.";
                            else
                                validationResult = String.Empty;
                        }
                        break;

                    case "Status":
                        validationResult = StatusValidation();
                        break;

                    case "Description":
                        if (Description.Length > 1000)
                            validationResult = "Max length of version description is 1000 characters.";
                        else
                            validationResult = String.Empty;
                        break;

                    case "ECR":
                        if (ECR.Length > 50)
                            validationResult = "Max length of ECR is 50 characters.";
                        else
                            validationResult = String.Empty;
                        break;

                    case "DocumentID":
                        if (DocumentID.Length > 50)
                            validationResult = "Max length of DocumentID is 50 characters.";
                        else
                            validationResult = String.Empty;
                        break;

                    case "RunningString":
                        if (RunningString.Length > 100)
                            validationResult = "Max length of running string is 100 characters.";
                        else
                            validationResult = String.Empty;
                        break;

                    case "Editor":
                        if (Editor.Length > 100)
                            validationResult = "Max length of editor is 100 characters.";
                        else
                            validationResult = String.Empty;
                        break;
                    default:
                        throw new ApplicationException("Unknown Property being validated on Product.");
                }

                _validProperties[columnName] = String.IsNullOrEmpty(validationResult) ? true : false;
                ValidateProperties();
                return validationResult;
            }
        }

        private string StatusValidation()
        {

            if(Status == null || Status.ID == "Sel") 
                return "Version status needs to be selected.";

            if (Status.ID != "Edit")
            {
                foreach (ItemVersionLink itemVersionLink in SubItemVersionLinkNode)
                {
                    ContentVersion contentVersion = Locator.ContentVersions[itemVersionLink.ContentVersionID];

                    if (contentVersion.Status.ID == "Edit")
                        return "Must be 'Editing' when at least one linked content is 'Editing': Content " + contentVersion.Name;

                }
            }

            return String.Empty;
        }



        private void ValidateProperties()
        {
            foreach (bool isValid in _validProperties.Values)
            {
                if (!isValid)
                {
                    AllPropertiesValid = false;
                    return;
                }
            }
            AllPropertiesValid = true;
        }

        #endregion

    }
}
