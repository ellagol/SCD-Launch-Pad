using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ContentManagerDefinitions;
using ContentManagerProvider.General;
using ProjectExplorerTester;
using TraceExceptionWrapper;
using DatabaseProvider;

namespace ContentManagerProvider
{
    public class ContentVersionLinkConfirmer
    {
        private Dictionary<int, List<int>> VersionLinksToConfirm { get; set; }
        private Dictionary<int, List<VersionKey>> ContentVersionLinks { get; set; }

        public void ConfirmerContentVersion(ContentVersion version)
        {
            if (version.ContentVersions.Count == 0)
                return;

            ContentVersionLinks = (new ContentsReader()).GetContentVersionLinksWithLock(); // Link to content

            if (version.ID != 0)
                ContentVersionLinks.Remove(version.ID);

            List<VersionKey> newVersionLinks = new List<VersionKey>();

            foreach (KeyValuePair<int, ContentVersionSubVersion> versionLink in version.ContentVersions)
            {
                if (newVersionLinks.Any(versionKey => versionKey.ContentID == versionLink.Value.ContentSubVersion.ParentID))
                    throw new Exception();

                newVersionLinks.Add(new VersionKey() { ContentID = versionLink.Value.ContentSubVersion.ParentID, VersionID = versionLink.Value.ContentSubVersion.ID });
            }

            ContentVersionLinks.Add(version.ID, newVersionLinks);
            ConfirmerContentVersionsList(version);
        }

        private void ConfirmerContentVersionsList(ContentVersion updatedVersion)
        {
            VersionLinksToConfirm = new Dictionary<int, List<int>>();
            AddVersionToConfirm(updatedVersion.ID);

            foreach (KeyValuePair<int, List<int>> versionToConfirm in VersionLinksToConfirm)
                ConfirmVersionLinks(versionToConfirm.Key, ContentVersionLinks[versionToConfirm.Key]);

        }

        private void ConfirmVersionLinks(int versionToConfirm, List<VersionKey> versionsToAdd)
        {
            foreach (VersionKey versionKey in versionsToAdd)
            {
                if(VersionLinksToConfirm[versionToConfirm].Contains(versionKey.ContentID))
                    throw new TraceException("Vresion Link loop", true, new List<string>() { GetVersionName(versionToConfirm), GetContentName(versionKey.ContentID) }, Locator.ApplicationName);

                VersionLinksToConfirm[versionToConfirm].Add(versionKey.ContentID);

                if(ContentVersionLinks.ContainsKey(versionKey.VersionID))
                    ConfirmVersionLinks(versionToConfirm, ContentVersionLinks[versionKey.VersionID]);
            }
        }

        private void AddVersionToConfirm(int newVersionID)
        {
            if (!VersionLinksToConfirm.ContainsKey(newVersionID))
                VersionLinksToConfirm.Add(newVersionID, new List<int>());

            foreach (KeyValuePair<int, List<VersionKey>> versionUseBy in ContentVersionLinks)
            {
                foreach (VersionKey versionKey in versionUseBy.Value)
                {
                    if(versionKey.VersionID == newVersionID)
                        AddVersionToConfirm(versionUseBy.Key);
                }
            }
        }

        private String GetVersionName(int versionID)
        {
            DataTable dt;
            String sqlCommand;

            sqlCommand = "Select CV_Name as Name ";
            sqlCommand += "From ContentVersion ";
            sqlCommand += "Where CV_ID = " + versionID;

            dt = Locator.DBprovider.ExecuteSelectCommand(sqlCommand);

            return dt.Rows.Count < 1 ? "" : DBprovider.GetStringParam(dt.Rows[0], "Name");
        }

        private String GetContentName(int contentID)
        {

            DataTable dt;
            String sqlCommand;

            sqlCommand = "Select CO_Name as Name ";
            sqlCommand += "From Content ";
            sqlCommand += "Where CO_ID = " + contentID;

            dt = Locator.DBprovider.ExecuteSelectCommand(sqlCommand);

            return dt.Rows.Count < 1 ? "" : DBprovider.GetStringParam(dt.Rows[0], "Name");
        }

    }
}
