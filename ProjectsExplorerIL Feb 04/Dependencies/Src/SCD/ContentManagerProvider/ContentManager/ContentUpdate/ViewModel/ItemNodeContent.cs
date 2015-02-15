using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;
using ContentManager.General;
using ContentManager.VersionUpdate.ViewModel;
using GalaSoft.MvvmLight;

namespace ContentManager.ContentUpdate.ViewModel
{
    public class ItemNodeContent : ObservableObject, IDataErrorInfo
    {
        public bool AllPropertiesValid { set; get; }
        private readonly Dictionary<string, bool> _validProperties;

        public ItemNodeContent()
        {
            _validProperties = new Dictionary<string, bool> { { "Name", false }, { "Description", false }, { "Icon", false }, { "Type", false } };
            Type = new ObservableContentType() { ID = "Type", Name = "Type" };
        }

        private String _name = "Name";
        public String Name
        {
            get { return _name; }
            set
            {
                Set(() => Name, ref _name, value);
            }
        }

        private String _description = "Description";
        public String Description
        {
            get { return _description; }
            set
            {
                Set(() => Description, ref _description, value);
            }
        }

        private bool _certificateFree;
        public bool CertificateFree
        {
            get { return _certificateFree; }
            set
            {
                Set(() => CertificateFree, ref _certificateFree, value);
            }
        }

        private String _icon;
        public String Icon
        {
            get { return _icon; }
            set
            {
                Set(() => Icon, ref _icon, value);
            }
        }

        private ObservableContentType _type;
        public ObservableContentType Type
        {
            get { return _type; }
            set { Set(() => Type, ref _type, value); }
        }

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
                            validationResult = "Content Name needs to be entered.";
                        }
                        else
                        {
                            if (Name.Length > 200)
                                validationResult = "Max length of content name is 200 characters.";
                            else
                                validationResult = String.Empty;
                        }

                        break;
                    case "Icon":

                        if (String.IsNullOrEmpty(Icon))
                        {
                            validationResult = String.Empty;
                            break;
                        }


                        if (!File.Exists(Icon))
                        {
                            validationResult = "Icon not exist";
                            break;
                        }

                        if (!IsValidImage(Icon))
                        {
                            validationResult = "Incorrect icon file format";
                            break;
                        }

                        validationResult = String.Empty;
                        break;

                    case "Description":

                        if (Description.Length > 1000)
                            validationResult = "Max length of content description is 1000 characters.";
                        else
                            validationResult = String.Empty;

                        break;
                    case "Type":
                        validationResult = (Type == null || Locator.ContentsDataProvider.ContentTypeList == null || Type == Locator.ContentsDataProvider.ContentTypeList[0]) ? "Content type needs to be selected." : String.Empty;
                        break;
                    default:
                        throw new ApplicationException("Unknown Property being validated on Product.");
                }

                _validProperties[columnName] = String.IsNullOrEmpty(validationResult) ? true : false;
                ValidateProperties();
                return validationResult;
            }
        }

        private bool IsValidImage(String image)
        {
            try
            {
                BitmapImage bi = new BitmapImage(new Uri(image));
                return true;
            }
            catch (Exception)
            {
                return false;
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
