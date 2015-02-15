using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Text;
using ATSBusinessObjects;
using ATSDomain;
using Infra.DAL;

namespace ATSBusinessLogic
{
    public class NoteBLL
    {
        #region Retrieve Note from database and return as ObservableCollection

        public static ObservableCollection<NotesModel> GetNotes(long HierarchyId)
        {
            try
            {
                // Initialize work fields
                ObservableCollection<NotesModel> Notes = new ObservableCollection<NotesModel>();
                NotesModel EmptyNode = Domain.GetBusinessObject<NotesModel>();

                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                string status = NoteStatusTypes.D.ToString();
                //Verify that user is authorized to view Disable Notes
                if (Domain.IsPermitted("104") || Domain.IsPermitted("999"))
                {
                    QryStr.Append("SELECT n.* FROM PE_Note n WHERE HierarchyId = " + HierarchyId + " ");
                }
                else
                {
                    QryStr.Append("SELECT n.* FROM PE_Note n WHERE HierarchyId = " + HierarchyId + " AND n.NoteStatus = '" + NoteStatusTypes.A + "' ");
                }
                QryStr.Append("ORDER BY CreationDate desc");

                // Fetch the DataTable from the database
                DataTable ResTableNote = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate the collection
                if (ResTableNote != null)
                {
                    foreach (DataRow DataRow in ResTableNote.Rows)
                    {
                        NotesModel Node = (NotesModel)Domain.DeepCopy(EmptyNode); // DeepCopy an empty instance to save on the Reflection work

                        Node.Id = (int)DataRow["Id"];
                        Node.IsNew = false;
                        Node.IsDirty = false;
                        Node.HierarchyId = (int)DataRow["HierarchyId"];
                        Node.NoteType = (string)DataRow["NoteType"];

                        switch (((string)DataRow["NoteStatus"]).Trim())
                        {
                            case "A":
                                Node.NoteStatus = NoteStatusTypes.A;
                                break;
                            case "D":
                                Node.NoteStatus = NoteStatusTypes.D;
                                break;
                            default:
                                Node.NoteStatus = NoteStatusTypes.A;
                                break;
                        }

                        Node.NoteTitle = (string)DataRow["NoteTitle"];

                        foreach (NoteTypes NT in Enum.GetValues(typeof(NoteTypes)))
                        {
                            FieldInfo fieldInfo = NT.GetType().GetField(NT.ToString());
                            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                            if (fieldInfo.Name == Node.NoteType.ToString().Trim())
                            {
                                Node.Description = attributes[0].Description;
                            }
                        }

                        Node.NoteText = (string)DataRow["NoteText"];
                        Node.CreationDate = (DateTime)DataRow["CreationDate"];
                        Node.CreatedBy = (string)DataRow["CreatedByUser"];
                        Node.LastUpdateTime = (DateTime)DataRow["LastUpdateTime"];
                        Node.LastUpdateUser = (string)DataRow["LastUpdateUser"];
                        Node.LastUpdateComputer = (string)DataRow["LastUpdateComputer"];
                        Node.LastUpdateApplication = (string)DataRow["LastUpdateapplication"];

                        if (DataRow["SpecialInd"] == DBNull.Value)
                        {
                            Node.SpecialInd = false;
                        }
                        else
                        {
                            Node.SpecialInd = (Boolean)DataRow["SpecialInd"];
                        }

                        Notes.Add(Node);
                    }
                    return Notes;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return null;
            }
        }

        #region select notes by search pattern

        public static List<string> ListOfHierarchyIdWithNotesSearchPattern = new List<string>();

        public static void GetHierarchyIds(string searchPattern)
        {
            try
            {
                ListOfHierarchyIdWithNotesSearchPattern.Clear();
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                QryStr.Append("SELECT distinct HierarchyId FROM PE_Note ");
                QryStr.Append("WHERE NoteText like '%" + searchPattern + "%' ");
                QryStr.Append("OR NoteTitle like '%" + searchPattern + "%' ");
                QryStr.Append("ORDER BY HierarchyId");

                // Fetch the DataTable from the database
                DataTable ResTableNote = Domain.PersistenceLayer.FetchDataTable(QryStr.ToString(), CommandType.Text, null);
                // Populate list
                if (ResTableNote != null)
                {
                    foreach (DataRow DataRow in ResTableNote.Rows)
                    {
                        ListOfHierarchyIdWithNotesSearchPattern.Add(Convert.ToString(DataRow.ItemArray[0]));                       
                    }
                }               
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
            }
        }
        #endregion

        #endregion

        #region Add Note

        public static long AddNewNote(ref NotesModel n)
        {
            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                List<ParamStruct> CommandParams;
                Boolean DatabaseSupportsBatchQueries = Domain.PersistenceLayer.GetSupportsBatchQueries();

                // Prior to updating, check if object has changed since it was loaded and alert the user if it has
                SB.Clear();
                SB.Append("SELECT LastUpdateTime FROM PE_Note WHERE Id = ");
                SB.Append(n.Id);
                object LastDateObj = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null);
                if (LastDateObj != null)
                {
                    string LastDate = Convert.ToDateTime(LastDateObj).ToString();
                    string ObjDate = n.LastUpdateTime.ToString();
                    if (LastDate != ObjDate) //If TimeStamp on the file is AFTER what we had when row was loaded into memory
                    {
                        return 104;
                    }
                }

                UpdateControlFields(ref n);
                SB.Clear();
                // Set Creation DateTime
                n.CreationDate = DateTime.Now;
                // n.NoteStatus = "N";
                // Build the Query
                SB.Append("INSERT INTO PE_Note (HierarchyId, NoteTitle, NoteType, NoteStatus, NoteText, SpecialInd, CreationDate, CreatedByUser, LastUpdateTime, LastUpdateUser, LastUpdateComputer, LastUpdateApplication) ");
                SB.Append("VALUES (@HierarchyId, @NoteTitle, @NoteType, @NoteStatus, @NoteText, @SpecialInd, @CreationDate, @CreatedByUser, @LastUpdateTime, @LastUpdateUser, @LastUpdateComputer, @LastUpdateApplication) ");
                if (DatabaseSupportsBatchQueries)
                {
                    SB.Append("; Select Scope_Identity()"); // To retrieve the Id of the inserted row
                }

                // Set the parameters
                CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "Id", DataType = DbType.Int32, Value = n.Id },
                new ParamStruct { ParamName = "HierarchyId", DataType = DbType.Int32, Value = n.HierarchyId },
                new ParamStruct { ParamName = "NoteTitle", DataType = DbType.String, Value = n.NoteTitle },
                new ParamStruct { ParamName = "NoteType", DataType = DbType.String, Value = n.NoteType },
                new ParamStruct { ParamName = "NoteStatus", DataType = DbType.String, Value = n.NoteStatus},
                new ParamStruct { ParamName = "NoteText", DataType = DbType.String, Value = n.NoteText },
                new ParamStruct { ParamName = "SpecialInd", DataType = DbType.Boolean, Value = n.SpecialInd },
                new ParamStruct { ParamName = "CreatedByUser", DataType = DbType.String, Value = n.LastUpdateUser},
                new ParamStruct { ParamName = "CreationDate", DataType = DbType.DateTime, Value = n.CreationDate },     
                new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = n.LastUpdateTime },
                new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = n.LastUpdateUser },
                new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = n.LastUpdateComputer },
                new ParamStruct { ParamName = "LastUpdateApplication", DataType = DbType.String, Value = n.LastUpdateApplication } 
                };

                //Execute the query
                object RV = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());

                if (RV != null)
                {
                    n.Id = Convert.ToInt64(RV);
                    n.IsNew = false;
                    n.IsLoading = false;
                    n.IsDirty = false;
                    return n.Id;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        #endregion

        #region Enable Disable Note

        public static long EnableDisableNote(ref NotesModel n)
        {
            try
            {
                // Work Fields
                var SB = new StringBuilder(string.Empty);
                List<ParamStruct> CommandParams;

                // Prior to updating, check if object has changed since it was loaded and alert the user if it has
                SB.Clear();
                SB.Append("SELECT LastUpdateTime FROM PE_Note WHERE Id = ");
                SB.Append(n.Id);
                object LastDateObj = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null);
                if (LastDateObj != null)
                {
                    string LastDate = Convert.ToDateTime(LastDateObj).ToString();
                    string ObjDate = n.LastUpdateTime.ToString();
                    if (LastDate != ObjDate) //If TimeStamp on the file is AFTER what we had when row was loaded into memory
                    {
                        return 104;
                    }
                }

                switch (n.NoteStatus.ToString()) //switch status
                {
                    case "A":
                        n.NoteStatus = NoteStatusTypes.D;
                        break;
                    case "D":
                        n.NoteStatus = NoteStatusTypes.A;
                        break;
                }

                // Build the Query   
                SB.Clear();
                SB.Append("UPDATE PE_Note SET NoteStatus=@NoteStatus, LastUpdateTime=@LastUpdateTime, LastUpdateUser=@LastUpdateUser, LastUpdateComputer=@LastUpdateComputer, LastUpdateApplication=@LastUpdateApplication ");
                SB.Append("WHERE Id=@Id");

                UpdateControlFields(ref n);  // Update Control fields

                // Set the parameters
                CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "Id", DataType = DbType.Int32, Value = n.Id },
                new ParamStruct { ParamName = "NoteStatus", DataType = DbType.String, Value = n.NoteStatus},
                new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = n.LastUpdateTime },
                new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = n.LastUpdateUser },
                new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = n.LastUpdateComputer },
                new ParamStruct { ParamName = "LastUpdateApplication", DataType = DbType.String, Value = n.LastUpdateApplication }
                };

                object RV = Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());

                if (RV != null)
                {
                    return Convert.ToInt64(RV);
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        #endregion

        #region Save Note Changes

        public static long SaveNoteChanges(ref NotesModel n)
        {
            try
            {
                // Work fields
                var SB = new StringBuilder(string.Empty);
                List<ParamStruct> CommandParams;

                // Prior to updating, check if object has changed since it was loaded and alert the user if it has
                SB.Clear();
                SB.Append("SELECT LastUpdateTime FROM PE_Note WHERE Id = ");
                SB.Append(n.Id);
                object LastDateObj = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null);
                if (LastDateObj != null)
                {
                    string LastDate = Convert.ToDateTime(LastDateObj).ToString();
                    string ObjDate = n.LastUpdateTime.ToString();
                    if (LastDate != ObjDate) //If TimeStamp on the file is AFTER what we had when row was loaded into memory
                    {
                        return 104;
                    }
                }

                UpdateControlFields(ref n);  // Update Control fields

                // Build the Query
                SB.Clear();
                SB.Append("UPDATE PE_Note SET NoteTitle=@NoteTitle, NoteType=@NoteType, CreationDate=@CreationDate, CreatedByUser=@CreatedByUser, NoteText=@NoteText, SpecialInd=@SpecialInd, LastUpdateTime=@LastUpdateTime, LastUpdateUser=@LastUpdateUser, LastUpdateComputer=@LastUpdateComputer, LastUpdateApplication=@LastUpdateApplication ");
                SB.Append("WHERE Id=@Id");

                // Set the parameters
                CommandParams = new List<ParamStruct>()
                {
                new ParamStruct { ParamName = "Id", DataType = DbType.Int32, Value = n.Id },
                new ParamStruct { ParamName = "NoteTitle", DataType = DbType.String, Value = n.NoteTitle },
                new ParamStruct { ParamName = "NoteType", DataType = DbType.String, Value = n.NoteType },                   
                new ParamStruct { ParamName = "CreationDate", DataType = DbType.DateTime, Value = n.CreationDate },
                new ParamStruct { ParamName = "CreatedByUser", DataType = DbType.String, Value = n.CreatedBy},
                new ParamStruct { ParamName = "NoteText", DataType = DbType.String, Value = n.NoteText },
                new ParamStruct { ParamName = "SpecialInd", DataType = DbType.Boolean, Value = n.SpecialInd },
                new ParamStruct { ParamName = "LastUpdateTime", DataType = DbType.DateTime, Value = n.LastUpdateTime },
                new ParamStruct { ParamName = "LastUpdateUser", DataType = DbType.String, Value = n.LastUpdateUser },
                new ParamStruct { ParamName = "LastUpdateComputer", DataType = DbType.String, Value = n.LastUpdateComputer },
                new ParamStruct { ParamName = "LastUpdateApplication", DataType = DbType.String, Value = n.LastUpdateApplication }
                };
                //Execute the query
                object RV = Domain.PersistenceLayer.ExecuteDbCommand(SB.ToString(), System.Data.CommandType.Text, CommandParams.ToArray());

                if (RV != null)
                {
                    return Convert.ToInt64(RV);
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        #endregion

        #region check if need to show notes sideBar

        public static bool CheckForSpecialNotes(long HierarchyId)
        {
            try
            {
                // Build The Query String
                System.Text.StringBuilder QryStr = new System.Text.StringBuilder();
                string status = NoteStatusTypes.D.ToString();

                //QryStr.Append("SELECT COUNT(Id) FROM PE_Note WHERE HierarchyId = " + HierarchyId + " AND SpecialInd = '" + true + "' ");

                QryStr.Append("SELECT '1' WHERE EXISTS (SELECT '1' FROM PE_Note WHERE HierarchyId = " + HierarchyId + " AND NoteStatus = '" + NoteStatusTypes.A + "' AND SpecialInd = 1)");

                // Fetch the DataTable from the database
                Int16 Count = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(QryStr.ToString(), System.Data.CommandType.Text, null)));

                if (Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region get list of used content versions

        public static void GetListOfSpecialNotesHierarchyIds()
        {
            try
            {
                string qry = "SELECT distinct HierarchyId" + 
                                " FROM PE_Note WHERE NoteStatus = '" + NoteStatusTypes.A + "' AND SpecialInd = 1";
                DataTable HierarchyIds = Domain.PersistenceLayer.FetchDataTable(qry.ToString(), CommandType.Text, null);
                if (HierarchyIds != null)
                {
                    foreach (DataRow DataRow in HierarchyIds.Rows)
                    {
                        int hID = Convert.ToInt32(DataRow.ItemArray[0]);
                        HierarchyBLL.listOfHierarchyIdsWithSpecialNotes.Add(hID);
                    }
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("Failed to get list of content versions");
            }
        }

        #endregion get list of used content versions

        #endregion

        #region Update Note Control Fields

        public static void UpdateControlFields(ref NotesModel note)
        {
            note.LastUpdateApplication = Domain.AppName;
            note.LastUpdateComputer = Domain.Workstn;
            note.LastUpdateUser = Domain.User;
            note.LastUpdateTime = DateTime.Now;
        }

        #endregion
    }
}