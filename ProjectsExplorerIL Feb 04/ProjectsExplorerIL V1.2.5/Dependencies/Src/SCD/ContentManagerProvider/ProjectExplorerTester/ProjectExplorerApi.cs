using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ContentManagerDefinitions;
using DatabaseProvider;

namespace ProjectExplorerTester
{
    public class ProjectExplorerApi
    {
        private string UserName { get; set; }
        private string ComputerName { get; set; }
        private string ApplicationName { get; set; }
        private DBprovider DBprovider { get; set; }

        public ProjectExplorerApi(String user, String computer, String application, SqlConnection connection, SqlTransaction transaction)
        {
            UserName = user;
            ComputerName = computer;
            ApplicationName = application;
            DBprovider = new DBprovider(connection, transaction, ApplicationName);
        }

        public ProjectExplorerApi(String user, String computer, String application, string connectionString)
        {
            UserName = user;
            ComputerName = computer;
            ApplicationName = application;
            DBprovider = new DBprovider(connectionString, ApplicationName);
        }

        public Dictionary<int, Project> GetProjectsByContentVersionID(int contentVersionID)
        {
            Dictionary<int, Project> projects = new Dictionary<int, Project>();

            //DBprovider.OpenConnection();
            //DBprovider.ExecuteSelectCommand("SELECT * FROM PathType");
            //DBprovider.CloseConnection();

            //if (contentVersionID != 116)
            //{
            //    projects.Add(1, new Project { Name = "Project 1", Code = "AA", Step = "Step2", Version = "1", HierarchyPath = "1/2" });
            //    projects.Add(2, new Project { Name = "Project 2", Code = "A1", Step = "Step1a", Version = "1", HierarchyPath = "1/2" });
            //    projects.Add(3, new Project { Name = "Project 3", Code = "A2", Step = "Step2", Version = "1", HierarchyPath = "1/2" });
            //    projects.Add(4, new Project { Name = "Project 4", Code = "AB", Step = "Step1a", Version = "1", HierarchyPath = "1/2" });
            //}
            return projects;
        }

        public void Bootstrap(String projectName, String projectVersion, String contentName)
        {
            //   projectName = Full path of project: Tools\Projects\PE. Tools - Root folder, Projects folder, PE - project
            //   projectVersion = Name of version for run
            //   contentName = Content name for run
        }
    }
}
