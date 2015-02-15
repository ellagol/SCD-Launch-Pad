using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Windows;
using ContentManager.ContentManagerMainWindow.ViewModel;
using ContentManager.General;
using ContentManager.Messanger.ViewModel;
using ContentManagerProvider;
using GalaSoft.MvvmLight.Messaging;

namespace ContentManager.FolderUpdater.ViewModel
{
    public class FolderUpdater
    {
        #region Add folder

        public static void AddFolderExecuteRunApiProvider(object o)
        {
            Folder folderNew = (Folder)((object[])o)[0];
            ItemNode parentFolderItem = (ItemNode)((object[])o)[1];

            try
            {
                //Add to DB
                Locator.ContentManagerApiProvider.AddTreeObject(folderNew, false, null);
            }
            catch (Exception)
            {
                ((Messenger)Messenger.Default).Send<MessageWrapperData>(new MessageWrapperData("Error add folder", true, MessageType.ApiException, MessageSeverity.Error));

                //Close UI Folder control 
                Locator.ContentManagerDataProvider.UiControlVisible = UserControlType.None;
                return;
            }

            AddFolderExecuteRunDataUpdater(folderNew, parentFolderItem);
        }

        private static void AddFolderExecuteRunDataUpdater(Folder folderNew, ItemNode parentFolderItem)
        {

            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(
                    new Action(() => AddFolderExecuteRunDataUpdater(folderNew, parentFolderItem)));
            else
            {
                //Add to dictionary
                Locator.Folders.Add(folderNew.ID, folderNew);

                //Add to UI
                if (parentFolderItem != null)
                    parentFolderItem.SubItemNode.Add(Locator.ItemTreeBuilder.BuildItemNode(folderNew, parentFolderItem));
                else
                    Locator.ContentManagerDataProvider.SubItemNode.Add(Locator.ItemTreeBuilder.BuildItemNode(folderNew,
                                                                                                             null));

                //Close UI Folder control 
                Locator.ContentManagerDataProvider.UiControlVisible = UserControlType.None;
            }
        }

        #endregion

        #region Update folder

        public static void UpdateFolderExecuteRunApiProvider(object o)
        {
            Folder folderNew = (Folder) ((object[]) o)[0];
            Folder folderOriginal = (Folder) ((object[]) o)[1];
            ItemNode folderItem = (ItemNode) ((object[]) o)[2];

            try
            {
                //Update in DB
                Locator.ContentManagerApiProvider.UpdateTreeObject(folderNew, folderOriginal, null);
            }
            catch (Exception)
            {
                ((Messenger) Messenger.Default).Send<MessageWrapperData>(new MessageWrapperData("Error update folder",
                                                                                                true,
                                                                                                MessageType.ApiException,
                                                                                                MessageSeverity.Error));

                //Close UI Folder control 
                Locator.ContentManagerDataProvider.UiControlVisible = UserControlType.None;
                return;
            }

            UpdateFolderExecuteRunDataUpdater(folderNew, folderOriginal, folderItem);
        }

        private static void UpdateFolderExecuteRunDataUpdater(Folder folderUpdated, Folder folderOriginal,
                                                              ItemNode folderItem)
        {

            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(
                    new Action(() => UpdateFolderExecuteRunDataUpdater(folderUpdated, folderOriginal, folderItem)));
            else
            {
                //Update in UI
                folderItem.Name = folderUpdated.Name;

                //Update in dictionary
                UpdateFolderParameter(folderUpdated, folderOriginal);

                //Close UI Folder control 
                Locator.ContentManagerDataProvider.UiControlVisible = UserControlType.None;
            }
        }

        private static void UpdateFolderParameter(Folder folderUpdated, Folder folderOriginal)
        {
            //Alik replace contentVersionOriginal with  contentVersionUpdated !!!!!!!!!!!!!!!!!!!!!!!! 

            folderOriginal.Name = folderUpdated.Name;
            folderOriginal.Description = folderUpdated.Description;

            folderOriginal.LastUpdateUser = folderUpdated.LastUpdateUser;
            folderOriginal.LastUpdateTime = folderUpdated.LastUpdateTime;
            folderOriginal.LastUpdateComputer = folderUpdated.LastUpdateComputer;
            folderOriginal.LastUpdateApplication = folderUpdated.LastUpdateApplication;
        }

        #endregion

        #region Delete folder

        public static void DeleteFolderExecuteRunApiProvider(object o)
        {
            Folder folderToDelete = (Folder) ((object[]) o)[0];
            ItemNode folderItem = (ItemNode) ((object[]) o)[1];

            try
            {
                //Delete from DB
                Locator.ContentManagerApiProvider.DeleteTreeObject(folderToDelete);
            }
            catch (Exception)
            {
                ((Messenger) Messenger.Default).Send<MessageWrapperData>(new MessageWrapperData("Error delete folder",
                                                                                                true,
                                                                                                MessageType.ApiException,
                                                                                                MessageSeverity.Error));

                //Close UI Folder control 
                Locator.ContentManagerDataProvider.UiControlVisible = UserControlType.None;
                return;
            }

            DeleteFolderExecuteRunDataUpdater(folderItem);
        }

        private static void DeleteFolderExecuteRunDataUpdater(ItemNode folderItem)
        {

            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(
                    new Action(() => DeleteFolderExecuteRunDataUpdater(folderItem)));
            else
            {
                //Delete from dictionary
                Locator.Folders.Remove(folderItem.ID);

                //Delete from UI
                Locator.ContentManagerDataProvider.DeleteSubItemNode(folderItem);
            }
        }

        #endregion
    }
}
