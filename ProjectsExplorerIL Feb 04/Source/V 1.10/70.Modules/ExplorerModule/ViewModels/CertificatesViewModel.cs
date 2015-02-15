using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using ATSBusinessLogic;
using ATSDomain;
using Infra.MVVM;
using ResourcesProvider;


namespace ExplorerModule
{

    public class CertificatesViewModel : ViewModelBase
    {
        private Guid WorkspaceId { get; set; }
        protected MessengerService MessageMediator = new MessengerService();

        public CertificatesViewModel(Guid workspaceId)
        {
            WorkspaceId = workspaceId;
            //Message Box Service
            MessageMediator = GetService<MessengerService>();
        }

        public IEnumerable<CertificateModel> Certificates
        {
            get
            {
                Boolean bol = true;
                Dictionary<String, Certificate> certificates = new Dictionary<string, Certificate>();

                try
                {
                    //API Needed connection string
                    SqlConnection connectionString = new SqlConnection(Domain.DbConnString);
                    //Resource Provider Object
                    ResourcesProviderApi resourceProvider = new ResourcesProviderApi(connectionString);
                    connectionString.Open(); //Create connection to the API
                    if (connectionString.State.Equals(ConnectionState.Open))
                    {
                        //Get certificates from API
                        certificates = resourceProvider.getAllCertificates();
                        connectionString.Close(); //Close connection with API
                    }
                    else
                    {
                        bol = false;
                        Object[] ArgsList = new Object[] {0};
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(136, ArgsList);            

                    }
                }
                catch (Exception e)
                {
                    bol = false;
                    Collection<string> StatusBarParameters = new Collection<string>();
                    //var SB = new StringBuilder(string.Empty);
                    //SB.Append("SELECT Description FROM PE_Messages where id=105;");
                    //string QrySteps = SB.ToString();
                    //StatusBarParameters.Add((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null)).ToString());
                    //StatusBarParameters.Add("White"); //Foreground
                    //StatusBarParameters.Add("Red"); //Background
                    //MessageMediator.NotifyColleagues("StatusBarParameters", StatusBarParameters); //Send message to the MainViewModel

                    String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                    Domain.SaveGeneralErrorLog(logMessage);
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(137, ArgsList);    
                }
                if(bol)
                    foreach (var c in certificates) //Add all certificates to the hierarchy model
                    {
                        var info = new Certificate(c.Value.Name.ToString(), c.Value.Description.ToString());

                        yield return new CertificateModel(info, c.Key.ToString(), DateTime.Now.ToString());
                    }
            }
        }
    }






} //end of root namespace    