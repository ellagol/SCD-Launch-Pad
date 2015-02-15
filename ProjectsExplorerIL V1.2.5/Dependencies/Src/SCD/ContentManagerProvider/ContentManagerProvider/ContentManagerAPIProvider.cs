using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.SqlClient;
using ContentManagerDefinitions;
using ContentManagerProvider.General;
using ProfileManagerProvider;
using TraceExceptionWrapper;
using DatabaseProvider;

namespace ContentManagerProvider
{
    public class ContentManagerApiProvider
    {
        #region Constructors

        public ContentManagerApiProvider(String user, String computer, String application, SqlConnection connection,
                                         SqlTransaction transaction)
        {
            Locator.UserName = user;
            Locator.ComputerName = computer;
            Locator.ApplicationName = application;
            Locator.DBprovider = new DBprovider(connection, transaction, Locator.ApplicationName);
            Locator.ProfileManagerProvider = new ProfileProvider(Locator.DBprovider.Connection, null, Locator.ApplicationName);
        }

        public ContentManagerApiProvider(String user, String computer, String application, String connectionString)
        {
            Locator.UserName = user;
            Locator.ComputerName = computer;
            Locator.ApplicationName = application;
            Locator.DBprovider = new DBprovider(connectionString, Locator.ApplicationName);
            Locator.ProfileManagerProvider = new ProfileProvider(Locator.DBprovider.Connection, null, Locator.ApplicationName);
        }

        #endregion

        #region Manage data

        public void AddTreeObject(TreeNode treeNode, bool updateNodeName, ICopyFilesProgress progressBar)
        {
            (new TreeNodeAdder()).Add(treeNode, updateNodeName, progressBar);
        }

        public void UpdateTreeObject(TreeNode treeNodeUpdated, TreeNode treeNodeOriginal, ICopyFilesProgress progressBar)
        {
            (new TreeNodeUpdater()).Update(treeNodeUpdated, treeNodeOriginal, progressBar);
        }

        public void DeleteTreeObject(TreeNode treeNode)
        {
            (new TreeNodeDeleter()).Delete(treeNode);
        }

        #endregion

        #region Get data

        public void UpdatePermission(List<TreeNode> treeNodes, Dictionary<int, Folder> folders, Dictionary<int, Content> contents, Dictionary<int, ContentVersion> versions)
        {
            try
            {
                Locator.DBprovider.OpenConnection();
                (new UpdatePermission(folders, contents, versions)).UpdateTreeNodeRecursive(treeNodes);
                Locator.DBprovider.CloseConnection();
            }
            catch (TraceException te)
            {
                Locator.DBprovider.CloseConnection();
                te.AddTrace(new StackFrame(1, true));
                throw te;
            }
            catch (Exception e)
            {
                Locator.DBprovider.CloseConnection();
                TraceException te = new TraceException(new StackFrame(1, true), e, Locator.ApplicationName);
                throw te;
            }
        }

        public Dictionary<String, bool> GetApplicationPermission(List<String> permissions)
        {
            try
            {
                Locator.DBprovider.OpenConnection();
                List<String> cmPermissions = Locator.ProfileManagerProvider.GetApplicationPermission(Locator.UserName);
                Locator.DBprovider.CloseConnection();

                Dictionary<String, bool> dictionaryPermissions = new Dictionary<string, bool>();
                foreach (String permission in permissions)
                    dictionaryPermissions.Add(permission, cmPermissions.Contains(permission));

                return dictionaryPermissions;

            }
            catch (TraceException te)
            {
                Locator.DBprovider.CloseConnection();
                te.AddTrace(new StackFrame(1, true));
                throw te;
            }
            catch (Exception e)
            {
                Locator.DBprovider.CloseConnection();
                TraceException te = new TraceException(new StackFrame(1, true), e, Locator.ApplicationName);
                throw te;
            }
        }

        public Dictionary<string, string> GetSystemParameters()
        {
            try
            {
                Dictionary<string, string> systemParameters;
                Locator.DBprovider.OpenConnection();
                systemParameters = Locator.SystemParameters;
                Locator.DBprovider.CloseConnection();
                return systemParameters;
            }
            catch (TraceException te)
            {
                Locator.DBprovider.CloseConnection();
                te.AddTrace(new StackFrame(1, true));
                throw te;
            }
            catch (Exception e)
            {
                Locator.DBprovider.CloseConnection();
                TraceException te = new TraceException(new StackFrame(1, true), e, Locator.ApplicationName);
                throw te;
            }
        }

        public Dictionary<String, ContentStatus> GetContentStatus()
        {
            try
            {
                Dictionary<String, ContentStatus> contentStatus;
                ContentStatusReader typesReader = new ContentStatusReader();
                Locator.DBprovider.OpenConnection();
                contentStatus = typesReader.GetStatusList();
                Locator.DBprovider.CloseConnection();
                return contentStatus;
            }
            catch (TraceException te)
            {
                Locator.DBprovider.CloseConnection();
                te.AddTrace(new StackFrame(1, true));
                throw te;
            }
            catch (Exception e)
            {
                Locator.DBprovider.CloseConnection();
                TraceException te = new TraceException(new StackFrame(1, true), e, Locator.ApplicationName);
                throw te;
            }
        }

        public Dictionary<String, UserGroupType> GetUserGroupTypes()
        {
            try
            {
                Dictionary<String, UserGroupType> userGroupTypes;
                Locator.DBprovider.OpenConnection();
                userGroupTypes = UserGroupTypesReader.GetUserGroupTypes();

                Locator.DBprovider.CloseConnection();
                return userGroupTypes;
            }
            catch (TraceException te)
            {
                Locator.DBprovider.CloseConnection();
                te.AddTrace(new StackFrame(1, true));
                throw te;
            }
            catch (Exception e)
            {
                Locator.DBprovider.CloseConnection();
                TraceException te = new TraceException(new StackFrame(1, true), e, Locator.ApplicationName);
                throw te;
            }
        }

        public Dictionary<String, ContentType> GetContentType()
        {
            try
            {
                Dictionary<String, ContentType> contentType;
                ContentTypesReader typesReader = new ContentTypesReader();
                Locator.DBprovider.OpenConnection();
                contentType = typesReader.GetTypesList();
                Locator.DBprovider.CloseConnection();
                return contentType;
            }
            catch (TraceException te)
            {
                Locator.DBprovider.CloseConnection();
                te.AddTrace(new StackFrame(1, true));
                throw te;
            }
            catch (Exception e)
            {
                Locator.DBprovider.CloseConnection();
                TraceException te = new TraceException(new StackFrame(1, true), e, Locator.ApplicationName);
                throw te;
            }
        }

        public Dictionary<int, Content> GetContents(List<int> contentsID)
        {
            try
            {
                Dictionary<int, Content> contents;
                ContentsReader contentsReader = new ContentsReader();
                Locator.DBprovider.OpenConnection();
                contents = contentsReader.GetContentsList(contentsID, false);
                Locator.DBprovider.CloseConnection();
                return contents;
            }
            catch (TraceException te)
            {
                Locator.DBprovider.CloseConnection();
                te.AddTrace(new StackFrame(1, true));
                throw te;
            }
            catch (Exception e)
            {
                Locator.DBprovider.CloseConnection();
                TraceException te = new TraceException(new StackFrame(1, true), e, Locator.ApplicationName);
                throw te;
            }
        }

        public List<TreeNode> GetTreeObjects(out Dictionary<int, Folder> folders, out Dictionary<int, Content> contents, out Dictionary<int, ContentVersion> versions)
        {
            return GetTreeObjectsData(false, out folders, out contents, out versions);
        }

        public List<TreeNode> GetTreeObjectsCm(out Dictionary<int, Folder> folders, out Dictionary<int, Content> contents, out Dictionary<int, ContentVersion> versions)
        {
            return GetTreeObjectsData(true, out folders, out contents, out versions);
        }

        private List<TreeNode> GetTreeObjectsData(bool fullData, out Dictionary<int, Folder> folders, out Dictionary<int, Content> contents, out Dictionary<int, ContentVersion> versions)
        {
            try
            {
                List<TreeNode> tree;
                ContentsReader contentsReader = new ContentsReader();
                Locator.DBprovider.OpenConnection();
                tree = contentsReader.GetContentsTree(fullData, out folders, out contents, out versions);
                Locator.DBprovider.CloseConnection();
                return tree;
            }
            catch (TraceException te)
            {
                Locator.DBprovider.CloseConnection();
                te.AddTrace(new StackFrame(1, true));
                throw te;
            }
            catch (Exception e)
            {
                Locator.DBprovider.CloseConnection();
                TraceException te = new TraceException(new StackFrame(1, true), e, Locator.ApplicationName);
                throw te;
            }
        }

        public Dictionary<int, List<VersionKey>> GetContentVersionLinksWithLock()
        {
            try
            {
                Dictionary<int, List<VersionKey>> versionLinks;
                ContentsReader contentsReader = new ContentsReader();
                Locator.DBprovider.OpenConnection();
                versionLinks = contentsReader.GetContentVersionLinksWithLock();
                Locator.DBprovider.CloseConnection();
                return versionLinks;
            }
            catch (TraceException te)
            {
                Locator.DBprovider.CloseConnection();
                te.AddTrace(new StackFrame(1, true));
                throw te;
            }
            catch (Exception e)
            {
                Locator.DBprovider.CloseConnection();
                TraceException te = new TraceException(new StackFrame(1, true), e, Locator.ApplicationName);
                throw te;
            }
        }

        #endregion
    }
}
