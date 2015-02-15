using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ATSBusinessObjects;
using Infra.Domain;
using ResourcesProvider;

namespace ATSBusinessLogic
{
    public class CertificateBLL
    {
        #region Variables
        private DataTable ResTable;
        private Dictionary<String, Certificate> certificates;
        private HierarchyModel Hierarchy;
        public CertificateModel selectedCertificate;
        #endregion variables

        #region Full Constructor
        public CertificateBLL(HierarchyModel Hierarchy, CertificateModel selectedCertificate)
        {
            this.Hierarchy = Hierarchy;
            this.selectedCertificate = selectedCertificate;
            ResTable = new DataTable();
            certificates = new Dictionary<string, Certificate>();
            getAllCertificates();
            GetStationCertificates(out certificates);
        }
        #endregion

        #region Partial Constructor
        public CertificateBLL(HierarchyModel Hierarchy)
        {
            this.Hierarchy = (HierarchyModel)Hierarchy;
            this.selectedCertificate = null;
            ResTable = new DataTable();
            certificates = new Dictionary<string, Certificate>();
        }
        #endregion

        #region Get all Certificates

        public ObservableCollection<CertificateModel> getAllCertificates()
        {
            Dictionary<String, String> NodeCertificatesList = new Dictionary<String, String>();
            Dictionary<String, Certificate> StationCertificatesList = new Dictionary<String, Certificate>();
            ObservableCollection<CertificateModel> EqualsCertificatesList = new ObservableCollection<CertificateModel>();
            try
            {
                if (GetNodeCertificates(Hierarchy, out NodeCertificatesList) != CertificateBLLReturnResult.Success)
                {
                    throw new Exception("Failed to retrieve list of Node certificates");
                }
                if (NodeCertificatesList.Count > 0)
                {

                    CertificateBLLReturnResult status = GetStationCertificates(out StationCertificatesList);
                    if (status != CertificateBLLReturnResult.Success)
                    {
                        throw new Exception("Failed to retrieve list of station certificates");
                    }

                    if (GetCertNameByCerId(NodeCertificatesList, StationCertificatesList, out EqualsCertificatesList) != CertificateBLLReturnResult.Success)
                    {

                        String logMessage = "getAllCertificates(): Failed to retrieve list of certificates";
                        ATSDomain.Domain.SaveGeneralErrorLog(logMessage); 
                        throw new Exception("Failed to retrieve list of certificates");
                    }
                }
                return EqualsCertificatesList;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("Failed to retrieve list of certificates");
            }
        }

        private CertificateBLLReturnResult GetNodeCertificates(HierarchyModel Hierarchy, out Dictionary<String, String> cerList)
        {
            cerList = new Dictionary<String, String>();
            try
            {
                var SBstep = new StringBuilder(string.Empty);
                SBstep.Append("select * from dbo.PE_FolderCertificate where HierarchyId=" + Hierarchy.Id.ToString() + ";");
                string QrySteps = SBstep.ToString();
                // Fetch the DataTable from the database
                ResTable = ATSDomain.Domain.PersistenceLayer.FetchDataTable(SBstep.ToString(), CommandType.Text, null);
                foreach (DataRow DataRow in ResTable.Rows)
                {
                    cerList.Add(DataRow.ItemArray[2].ToString(), DataRow.ItemArray[5].ToString());
                }
                foreach (String s in Hierarchy.Certificates)
                {
                    bool has = cerList.Keys.Any(x => x == s);
                    if (!has)
                    {
                        cerList.Add(s,DateTime.Now.ToString());
                    }
                }
                if (ResTable == null)
                {
                    String logMessage = "Failed to retrieve list of node certificates, nodeId = " + Convert.ToString(Hierarchy.Id);
                    ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                    return CertificateBLLReturnResult.DBConnectionError;
                }
                else
                    return CertificateBLLReturnResult.Success;
                }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                return CertificateBLLReturnResult.DBConnectionError;
            }
        }

        private static CertificateBLLReturnResult GetStationCertificates(out Dictionary<String, Certificate> certificates)
        {
            certificates = new Dictionary<String, Certificate>();
            try
            {
                    //API Needed connection string
                    SqlConnection connectionString = new SqlConnection(ATSDomain.Domain.DbConnString);
                    //Resource Provider Object
                    ResourcesProviderApi resourceProvider = new ResourcesProviderApi(connectionString);
                
                    connectionString.Open(); //Create connection to the API
                    //Get certificates from API
                    certificates = resourceProvider.getAllCertificates();
                    connectionString.Close(); //Close connection with API
                    if (certificates.Count == 0)
                    {
                        return CertificateBLLReturnResult.AllCertListEmpty;
                    }
                    else
                    {
                        return CertificateBLLReturnResult.Success;
                    }
                }
                catch (Exception e) 
                {
                    String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                    ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                    return CertificateBLLReturnResult.RMException; 
                }
            }

            private CertificateBLLReturnResult GetCertNameByCerId(Dictionary<String, String> NodeCertificatesList, Dictionary<String, Certificate> StationCertificatesList, out ObservableCollection<CertificateModel> EqualsCertificatesList)
            {
                EqualsCertificatesList = new ObservableCollection<CertificateModel>();

                try
                {
                    foreach (String s in NodeCertificatesList.Keys)
                    {
                        foreach (var c in StationCertificatesList) //Add all certificates to the hierarchy model
                        {
                            if (s.Equals(c.Key.ToString()))
                            {
                                var info = new Certificate(c.Value.Name.ToString(), c.Value.Description.ToString());
                                CertificateModel CM = new CertificateModel(info, c.Key.ToString(), NodeCertificatesList[s]);
                                CM.IsNew = false;
                                EqualsCertificatesList.Add(CM);
                            }
                        }
                    }

                if (EqualsCertificatesList.Count == 0)
                {
                    return CertificateBLLReturnResult.AllCertListEmpty;
                }
                else
                {
                    return CertificateBLLReturnResult.Success;
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                return CertificateBLLReturnResult.DBConnectionError;
            }
        }

        #endregion

        #region Delete Certificate
        public String ExecuteDeleteCertificateCommand()
        {
            try
            {
                foreach (var c in certificates) //Get selected certificate key
                {
                    if (selectedCertificate.CerName.Equals(c.Value.Name.ToString()))
                    {
                        var SB = new StringBuilder(string.Empty);
                        SB.Append("select rowid from PE_FolderCertificate where CertificateId = '" + selectedCertificate.key + "' and HierarchyId = '" + Hierarchy.Id + "'");
                        var exists = (ATSDomain.Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null));
                        SB.Append("delete from dbo.PE_FolderCertificate where (HierarchyId=" + Hierarchy.Id.ToString() + " and CertificateId='" + c.Key.ToString() + "');");
                        long result = Convert.ToInt64(ATSDomain.Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null));

                        if (result>-1 || exists.Equals(null))
                            return "0";
                        else
                            return "104";

                    }
                }
                return "104";

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                return "104";
            }

        }
        #endregion

        #region CertificateBLL enumeration
        public enum CertificateBLLReturnResult : int
        {
            Success, //137
            DBConnectionError, //137
            RMCommonException, //137
            RMException, //137
            AllCertListEmpty //137
        }
        #endregion

        #region Retrive Project certificates by Project Id - for Bootstrap API

        public static CertificateBLLReturnResult GetProjectCertificatesByProjectId(long projectId, out List<String> projectCertificates)
        {
            CertificateBLLReturnResult returnResult = CertificateBLLReturnResult.RMCommonException;
            projectCertificates = new List<string>();

            try
            {
                string selectCertQry = "Select CertificateId from dbo.PE_FolderCertificate where ExpirationDate is null and HierarchyId=" + projectId + ";";
                // Fetch the DataTable from the database
                DataTable listOfProjectCertificates = ATSDomain.Domain.PersistenceLayer.FetchDataTable(selectCertQry.ToString(), CommandType.Text, null);

                if (listOfProjectCertificates != null)
                {
                    if (listOfProjectCertificates.Rows.Count > 0)
                    {
                        foreach (DataRow DataRow in listOfProjectCertificates.Rows)
                        {
                            projectCertificates.Add(DataRow.ItemArray[0].ToString());
                        }
                    }
                    returnResult = CertificateBLLReturnResult.Success;
                }
                else
                {
                    returnResult = CertificateBLLReturnResult.DBConnectionError;
                }
                return returnResult;
            }
            catch (Exception e)
            {
                ApiBLL.TraceExceptionParameterValue.Add(e.Message);
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                return CertificateBLLReturnResult.RMCommonException;
            }
        }

        #endregion

        #region Get Certificates by Workstation Id - API

        public static CertificateBLLReturnResult GetCertificatesByWorkstationId(ResourcesProviderApi resourceProvider, out Dictionary<String, Certificate> stationCertificates)
        {

            CertificateBLLReturnResult rmApiReturnResult = CertificateBLLReturnResult.RMCommonException;
            stationCertificates = new Dictionary<String, Certificate>();

            try
            {

                stationCertificates = resourceProvider.getCertificatesByStationId(System.Environment.MachineName);
                rmApiReturnResult = CertificateBLLReturnResult.Success;
                return rmApiReturnResult;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("StationID") && e.Message.Contains("does not exist"))
                {
                    rmApiReturnResult = CertificateBLLReturnResult.Success; //if Workstation is not found in RM DB - valid scenario
                }
                else
                {
                    ApiBLL.TraceExceptionParameterValue.Add(e.Message);
                    String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                    ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                }
                return rmApiReturnResult; //returns Exception enum value to indicate RM exception
            }

        }

        #endregion

        #region Compare certificates

        public static List<String> IsWorkstationCertified(List<String> certificates, Dictionary<String, Certificate> certificateStation)
        {
            List<String> missingCertificates = new List<String>(); //List of missing certificates
            List<String> missingCertificatesDesc = new List<String>(); //List of missing certificate descriptions
            Dictionary<String, Certificate> allCertificates;
            try
            {
                if (certificates.Count > 0 && certificateStation.Count > 0)
                {
                    foreach (String c in certificates)
                    {
                        if (!certificateStation.Keys.Contains(c))
                        {
                            missingCertificates.Add(c); //Adding missing certificates to the list
                        }
                    }
                }
                else if (certificates.Count > 0 && certificateStation.Count == 0)
                {
                    missingCertificates.AddRange(certificates);
                }

                if (missingCertificates.Count > 0)
                {
                    CertificateBLLReturnResult success = GetStationCertificates(out allCertificates);
                    if (success != CertificateBLLReturnResult.Success)
                    {
                        throw new Exception("Failed to retrieve list of station certificates");
                    }
                    foreach (string c in missingCertificates)
                    {
                        missingCertificatesDesc.Add(allCertificates[c].Name);
                    }
                }
                return missingCertificatesDesc;
            }
            catch (Exception e)
            {                
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception();
            }
        }

        #endregion

        #region LastUpdateCertificate

        //public static string CheckLastUpdateCertificateDelete(IEnumerable<CertificateModel> Certificates, long HierarchyID, List<CertificateModel> DeleteCertificate)
        //{
        //    try
        //    {
        //        System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
        //        QryStr.Append("SELECT HierarchyId, CertificateId, LastUpdateTime FROM PE_FolderCertificate WHERE HierarchyId = " + HierarchyID + " ");

        //        DataTable ResTable = ATSDomain.Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
        //        Dictionary<string, CertificateModel> CertificateDicatonary = new Dictionary<string, CertificateModel>();
        //        foreach (var i in Certificates)
        //        {
        //            if (i.IsNew == false)
        //            {
        //                CertificateDicatonary.Add(i.key, i);
        //            }
        //        }
        //        foreach (var i in DeleteCertificate)
        //        {
        //            CertificateDicatonary.Add(i.key, i);
        //        }
        //        if (ResTable != null)
        //        {
        //            if (ResTable.Rows.Count == CertificateDicatonary.Count)
        //            {
        //                foreach (DataRow DataRow in ResTable.Rows)
        //                {

        //                    if (!(CertificateDicatonary.ContainsKey((string)DataRow["CertificateId"])))
        //                        return "104";
        //                }
        //            }
        //            else
        //                return "104";

        //        }

        //        else
        //            return "105";
        //        return string.Empty;
        //    }
        //    catch (Exception e)
        //    {
        //        String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
        //        ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
        //        return "105";
        //    }

        //}

        //public static string CheckLastUpdateCertificateAdd(IEnumerable<CertificateModel> Certificates, long HierarchyID, List<string> AddCertificate)
        //{
        //    try
        //    {

        //        System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
        //        QryStr.Append("SELECT HierarchyId, CertificateId, LastUpdateTime FROM PE_FolderCertificate WHERE HierarchyId = " + HierarchyID + " ");

        //        // Fetch the DataTable from the database
        //        DataTable ResTable = ATSDomain.Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
        //        Dictionary<string, CertificateModel> CertificateDictionary = new Dictionary<string, CertificateModel>();
        //        Dictionary<string, string> AddCertificateDictionary = new Dictionary<string, string>();
        //        foreach (var i in Certificates)
        //        {
        //            if (i.IsNew == false)
        //            {
        //                CertificateDictionary.Add(i.key, i);
        //            }
        //        }
        //        foreach (var j in AddCertificate)
        //        {
        //            AddCertificateDictionary.Add(j, j);
        //        }
        //        if (ResTable != null)
        //        {
        //            if (ResTable.Rows.Count == CertificateDictionary.Count)
        //            {
        //                foreach (DataRow DataRow in ResTable.Rows)
        //                {
        //                    //Not exists on DB
        //                    if (!(CertificateDictionary.ContainsKey((string)DataRow["CertificateId"])))
        //                        return "104";
        //                    //Exists in Certificates to add and in the DB.
        //                    if (AddCertificateDictionary.ContainsKey((string)DataRow["CertificateId"]))
        //                        return "104";

        //                }
        //            }
        //            else
        //                return "104";

        //        }

        //        else
        //            return "105";
        //        return string.Empty;
        //    }
        //    catch (Exception e)
        //    {
        //        String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
        //        ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
        //        return "105";
        //    }
        //}

        public static string CheckLastUpdateCertificate(Dictionary<string, string> InitialCertificates, long HierarchyID)
        {
            try
            {

                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("SELECT HierarchyId, CertificateId, LastUpdateTime FROM PE_FolderCertificate WHERE HierarchyId = " + HierarchyID + " ");

                // Fetch the DataTable from the database
                DataTable ResTable = ATSDomain.Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);

                if (ResTable != null && ResTable.Rows.Count>0)
                {
                    if (ResTable.Rows.Count == InitialCertificates.Count)
                    {
                        foreach (DataRow DataRow in ResTable.Rows)
                        {
                            //Saved in Hierarchy model certificates exist in db and have not changed
                            if (InitialCertificates.ContainsKey((string)DataRow["CertificateId"]))
                            {
                                var diff = (Convert.ToDateTime(DataRow["LastUpdateTime"]) - Convert.ToDateTime(InitialCertificates[(string)DataRow["CertificateId"]])).TotalSeconds;
                                if (diff <= 1)
                return string.Empty;
            }
                            else
                                return "104";
                        }
                    }
                    else
                        return "104";
                }
                else if (ResTable.Rows.Count == 0)
                    return string.Empty;
                else
                {
                    String logMessage = "Failed to get list of initial Certificates from DB.";
                    ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                return "105";
                }
                return string.Empty;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                return "105";
            }
        }

        #endregion LastUpdateCertificate

        public static CertificateBLLReturnResult GetNodeCertificatesDB(HierarchyModel Hierarchy, out Dictionary<String, String> cerList)
        {
            cerList = new Dictionary<String, String>();
            try
            {
                var SBstep = new StringBuilder(string.Empty);
                SBstep.Append("select * from dbo.PE_FolderCertificate where HierarchyId=" + Hierarchy.Id.ToString() + ";");
                string QrySteps = SBstep.ToString();
                // Fetch the DataTable from the database
                DataTable ResTable = ATSDomain.Domain.PersistenceLayer.FetchDataTable(SBstep.ToString(), CommandType.Text, null);
                foreach (DataRow DataRow in ResTable.Rows)
                {
                    cerList.Add(DataRow.ItemArray[2].ToString(), DataRow.ItemArray[5].ToString());
                }
                if (ResTable == null)
                {
                    String logMessage = "Failed to retrieve list of node certificates, nodeId = " + Convert.ToString(Hierarchy.Id);
                    ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                    return CertificateBLLReturnResult.DBConnectionError;
                }
                else
                    return CertificateBLLReturnResult.Success;
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                return CertificateBLLReturnResult.DBConnectionError;
            }
        }

        #region StationCertificateStation

        public static CertificateBLLReturnResult getStationCertificateStation(IEnumerable<CertificateModel> Certificates, out List<string> FinalList)
        {
           FinalList = new List<string>();
            try
            {

                List<string> Tempcertlist = new List<string>();
                List<string> ListToRemove = new List<string>();
                int i = 0;

                ResourcesProviderApi rp = new ResourcesProviderApi(ATSDomain.Domain.DbConnString);
                foreach (CertificateModel cm in Certificates)
                {
                    Tempcertlist = rp.GetListOfCertifiedStationsByCertId(cm.key);
                    //For the first time we initialize the two lists with the same values. 
                    if (i == 0)
                    {
                        FinalList = Tempcertlist;
                    }
                    //next itra 
                    else
                    {
                        if (FinalList.Count > 0)
                        {
                            foreach (string s in FinalList)
                            {
                                if (!Tempcertlist.Contains(s))
                                {
                                    ListToRemove.Add(s);
                                    
                                }
                            }
                            foreach (string s in ListToRemove)
                            {
                                FinalList.Remove(s);
                            }
                            ListToRemove.Clear();
                        }
                        else
                            return CertificateBLLReturnResult.Success;
                    }
                    i++;
                }//end of for each

                return CertificateBLLReturnResult.Success;
                
            }
            catch (Exception)
            {
                return CertificateBLLReturnResult.RMException;
            }
        }

       
        #endregion StationCertificateStation
    }

    #region CertificateClient - Inner Class
    public class CertificateModel : BusinessObjectBase
    {
        public CertificateModel(Certificate info, String key, String lastUpdateTime)
        {
            Info = info;
            this.key = key;
            this.lastUpdateTime = lastUpdateTime;
        }
        public CertificateModel()
        {
        }

        //public string CerName { get { return Info.Name.ToString(); } }
        public string Description { get { return Info.Description.ToString(); } }

        public string key { get; set; }

        public Certificate Info
        {
            get;
            private set;
        }

        public string _CerName;
        public string CerName
        {
            get { return Info.Name.ToString(); }
            set
            {
                _CerName = value;

            }

        }

        public string lastUpdateTime { get; set; }

        private Boolean _IsNewCert = false;
        public Boolean IsNewCert
        {
            get { return _IsNewCert; }
            set
            {
                _IsNewCert = value;

            }

        }


    }
    #endregion
}
