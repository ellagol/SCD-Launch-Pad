using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ContentManager.General;
using ContentManagerProvider;
using GalaSoft.MvvmLight;

namespace ContentManager.FolderUpdate.ViewModel
{
    public class ItemNodeFolder : ObservableObject, IDataErrorInfo
    {

        public bool AllPropertiesValid { set; get; }
        private readonly Dictionary<string, bool> _validProperties;

        public ItemNodeFolder()
        {
            UpdateUserGroupTypeTypes();
            _validProperties = new Dictionary<string, bool> {{"Name", false}, {"Description", false}};
        }

        public void InitItemNodeFolder()
        {
            Name = String.Empty;
            Description = String.Empty;

            foreach (UserGroupTypeObservable userGroupTypeObservable in UserGroupTypeList)
                userGroupTypeObservable.Checked = false;
        }

        public void InitItemNodeFolder(Folder folder)
        {
            Name = folder.Name;
            Description = folder.Description;

            foreach (UserGroupTypeObservable userGroupTypeObservable in UserGroupTypeList)
                userGroupTypeObservable.Checked = folder.UserGroupTypePermission.ContainsKey(userGroupTypeObservable.UserGroupType.ID);
        }

        private void UpdateUserGroupTypeTypes()
        {
            if (UserGroupTypeList != null || Locator.UserGroupTypes == null)
                return;

            UserGroupTypeObservable ooUserGroupType;
            UserGroupTypeList = new ObservableCollection<UserGroupTypeObservable>();

            foreach (var userGroupType in Locator.UserGroupTypes)
            {
                ooUserGroupType = new UserGroupTypeObservable
                {
                    UserGroupType = userGroupType.Value,
                    Checked = false
                };
                UserGroupTypeList.Add(ooUserGroupType);
            }
        }

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

        private ObservableCollection<UserGroupTypeObservable> _cserGroupTypeList;
        public ObservableCollection<UserGroupTypeObservable> UserGroupTypeList
        {
            get { return _cserGroupTypeList; }
            set { Set(() => UserGroupTypeList, ref _cserGroupTypeList, value); }
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
                            validationResult = "Folder Name needs to be entered.";
                        }
                        else
                        {
                            if(Name.Length > 50)
                                validationResult = "Max length of folder name is 50 characters.";
                            else
                                validationResult = String.Empty;
                        }
                        break;
                    case "Description":

                        if (Description.Length > 1000)
                            validationResult = "Max length of folder description is 1000 characters.";
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
