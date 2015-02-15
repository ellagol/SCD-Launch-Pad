using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using ContentManagerProvider;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Xml;

namespace ContentManagerProviderTester
{
    public partial class Form1 : Form
    {
        private string ConnectionString { get; set; }
        public Form1()
        {
            InitializeComponent();
            ConnectionString = "Data Source=HERMES;Initial Catalog=GenPR_Test;Persist Security Info=True;User ID=GenPR_Test_User;Password=GenPR_Test_User";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ContentManagerApiProvider provider = new ContentManagerApiProvider("Test", "Test", "Content manager provider", ConnectionString);
            Dictionary<String, ContentStatus> contentStatusList = provider.GetContentStatus();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SqlConnection Connection = new SqlConnection(ConnectionString);
            Connection.Open();
            SqlTransaction transaction = Connection.BeginTransaction();

            ContentManagerApiProvider provider = new ContentManagerApiProvider("Test", "Test", "Content manager provider", Connection, transaction);
            Dictionary<String, ContentStatus> contentStatusList = provider.GetContentStatus();

            transaction.Commit();
            Connection.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SqlConnection Connection = new SqlConnection(ConnectionString);
            Connection.Open();

            ContentManagerApiProvider provider = new ContentManagerApiProvider("Test", "Test", "Content manager provider", Connection, null);
            Dictionary<String, ContentStatus> contentStatusList = provider.GetContentStatus();

            Connection.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            List<int> contentsID;
            Dictionary<int, Content> contentsDictionary;

            contentsID = new List<int>();
            contentsID.Add(1);

            ContentManagerApiProvider provider = new ContentManagerApiProvider("Test", "Test", "Content manager provider", ConnectionString);
            contentsDictionary = provider.GetContents(contentsID);

        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<ContentManagerProvider.TreeNode> treeNodeList;
            ContentManagerApiProvider provider = new ContentManagerApiProvider("Test", "Test", "Content manager provider", ConnectionString);
            
            Dictionary<int, Folder> folders;
            Dictionary<int, Content> contents;
            Dictionary<int, ContentVersion> versions;
            treeNodeList = provider.GetTreeObjects(out folders, out contents, out versions);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            const string connectionString = "Data Source=HERMES;Initial Catalog=GenPR_Test;Persist Security Info=True;User ID=GenPR_Test_User;Password=GenPR_Test_User";
            DBprovider db = new DBprovider(connectionString, "");

            XmlDocument doc = new XmlDocument();
            doc.Load(@"C:\Documents and Settings\alex fruman\Desktop\CM_Errors.xml");
            XmlNodeList nodeList = doc.SelectNodes("root/Error");

            String sqlCommand;

            db.OpenConnection();
            db.ExecuteCommand("delete from ErrorDescription where ED_Application='Content manager'");
            foreach (XmlNode node in nodeList)
            {
                sqlCommand = "Insert Into  ";
                sqlCommand += "ErrorDescription (";
                sqlCommand += "ED_ID, ";
                sqlCommand += "ED_LastUpdateTime, ";
                sqlCommand += "ED_LastUpdateUser, ";
                sqlCommand += "ED_LastUpdateComputer, ";
                sqlCommand += "ED_LastUpdateApplication, ";
                sqlCommand += "ED_Application, ";
                sqlCommand += "ED_Title, ";
                sqlCommand += "ED_Description, ";
                sqlCommand += "ED_Icon, ";
                sqlCommand += "ED_Severity, ";
                sqlCommand += "ED_Type) ";
                sqlCommand += "Values (";
                sqlCommand += "'" + node.SelectSingleNode("ID").InnerText + "', ";
                sqlCommand += "'" + DBprovider.ConvertTimeToStringFormat(DateTime.Now)  + "', ";
                sqlCommand += "'Alik', ";
                sqlCommand += "'Alik', ";
                sqlCommand += "'Alik', ";
                sqlCommand += "'Content manager', ";
                sqlCommand += "'" + node.SelectSingleNode("Title").InnerText + "', ";
                sqlCommand += "'" + node.SelectSingleNode("Text").InnerText + "', ";
                sqlCommand += "'', ";
                sqlCommand += "'" + node.SelectSingleNode("Severity").InnerText + "', ";
                sqlCommand += "'" + node.SelectSingleNode("Type").InnerText + "') ";
                db.ExecuteCommand(sqlCommand);
            }
            db.CloseConnection();

        }
    }
}
