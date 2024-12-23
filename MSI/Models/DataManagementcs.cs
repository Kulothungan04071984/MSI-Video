
using System.Data;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
namespace MSI.Models
{

    public class DataManagementcs
    {
        private readonly string ConnectionString;

        public DataManagementcs(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("conn");
        }
        public int uploaddatainserted(UploadFileDetails objFileDetails)
        {
            int result = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("pro_InsertUploadVideoDetails", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@systemid", objFileDetails.systemid);
                        //cmd.Parameters.AddWithValue("@systemName", objFileDetails.systemname);
                        cmd.Parameters.AddWithValue("@uploadVideoPath", objFileDetails.filepath);
                        cmd.Parameters.AddWithValue("@uploadDateTime", objFileDetails.uploaddatetime);
                        cmd.Parameters.AddWithValue("@userid", objFileDetails.uploadEmployee);
                        cmd.Parameters.AddWithValue("@fromTime", objFileDetails.VideoFromTime);
                        cmd.Parameters.AddWithValue("@toTime",objFileDetails.VideoToTime);
                        cmd.Parameters.AddWithValue("@videodate",objFileDetails.VideoDate);
                        conn.Open();
                        result = cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                writeErrorMessage(ex.Message.ToString(), "uploaddatainserted");
                return result;
            }
        }
        public string GetDomainSid()
        {
            try
            {
                // Get the domain name of the current machine
                string domainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                if (string.IsNullOrEmpty(domainName))
                {
                    return "This machine is not part of a domain.";
                }

                // Use DirectoryServices to find the domain SID
                System.DirectoryServices.DirectoryEntry entry = new System.DirectoryServices.DirectoryEntry($"LDAP://{domainName}");
                byte[] sidBytes = (byte[])entry.Properties["objectSid"].Value;
                SecurityIdentifier sid = new SecurityIdentifier(sidBytes, 0);
                return sid.Value;
            }
            catch (Exception ex)
            {
                return $"An error occurred while retrieving the domain SID: {ex.Message}";
            }
        }

        public List<string> GetAllConnectedSystemNames()
        {
            var computerNames = new List<string>();

            try
            {
                // Get the domain context
                using (var context = new PrincipalContext(ContextType.Domain))
                {
                    using (var searcher = new PrincipalSearcher(new ComputerPrincipal(context)))
                    {
                        foreach (var result in searcher.FindAll())
                        {
                            using (var directoryEntry = result.GetUnderlyingObject() as DirectoryEntry)
                            {
                                if (directoryEntry != null)
                                {
                                    computerNames.Add(directoryEntry.Properties["name"].Value.ToString());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                //computerNames.Add($"An error occurred: {ex.Message}");
                writeErrorMessage(ex.Message.ToString(), "GetAllConnectedSystemNames");
            }

            return computerNames;
        }

        public List<SelectListItem> getSystemNames()
        {
            var list = new List<SelectListItem>();
            try
            {
                DataTable dtGetValue = new DataTable();

                using (SqlConnection conGetValue = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmdSetValue = new SqlCommand("pro_getSystemName", conGetValue))
                    {
                        cmdSetValue.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter daGetValue = new SqlDataAdapter(cmdSetValue))
                        {
                            daGetValue.Fill(dtGetValue);
                            if (dtGetValue.Rows.Count > 0)
                            {
                                foreach (DataRow row in dtGetValue.Rows)
                                {
                                    list.Add(new SelectListItem { Value = row["system_id"].ToString(), Text = row["system_name"].ToString() });
                                }
                            }
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                writeErrorMessage(ex.Message.ToString(), "getSystemNames");
                return list;
            }
        }

        public List<FileMappingDetails> getFileMappingDetails()
        {
            var lstFileMapping = new List<FileMappingDetails>();
            FileMappingDetails objFileMapping;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("pro_getFilemappingDetails", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            using (DataTable dataTable = new DataTable())
                            {
                                da.Fill(dataTable);
                                if (dataTable.Rows.Count > 0)
                                {
                                    foreach (DataRow row in dataTable.Rows)
                                    {
                                        objFileMapping = new FileMappingDetails();
                                        objFileMapping.systemid = Convert.ToInt32(row["systemid"].ToString());
                                        objFileMapping.systemname = row["system_name"].ToString();
                                        objFileMapping.filepath = row["File_Path"].ToString();
                                        objFileMapping.videoDate = row["VideoDate"].ToString();
                                        objFileMapping.fromtime = row["FromTime"].ToString();
                                        objFileMapping.totime = row["Totime"].ToString();
                                        lstFileMapping.Add(objFileMapping);
                                    }
                                }
                            }

                        }
                    }
                }
                return lstFileMapping;
            }

            catch (Exception ex)
            {
                writeErrorMessage(ex.Message.ToString(), "getFileMappingDetails");
                return lstFileMapping;
            }
        }

        public string getfilepath(string device_name)
        {
            
            DataTable dt = new DataTable();
            try
            {
                // Open a new SQL connection
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    // Create the SQL command object for the stored procedure
                    using (SqlCommand cmd = new SqlCommand("pro_getfilepath", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        writeErrorMessage(device_name, "Device Name Details");
                        // Add parameters for the stored procedure
                        string dname= string.IsNullOrEmpty(device_name) ? "10.10.120.234" : device_name;
                        cmd.Parameters.AddWithValue("@device_name", dname);

                        // Use SqlDataAdapter to fill the DataTable
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                }

                // Check if the DataTable has rows and extract the file path
                if (dt.Rows.Count > 0)
                {
                    // Assuming 'File_Path' is the name of the column in your SQL result set
                    string filePath = dt.Rows[0]["File_Path"].ToString();
                    writeErrorMessage(filePath, "getfilepath");
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        return filePath;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (you might want to log this somewhere, e.g., in a log file)
                // Console.WriteLine("Error fetching file path: " + ex.Message);
                // You can also use a logger instead of Console.WriteLine in production
                writeErrorMessage(ex.Message.ToString(), "getfilepath");
            }

            // Return null if no file path was found or an error occurred
            return null;
        }

        public int deleteFileMapping(int fileMappingId)
        {
            int resultDelete = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("pro_deleteFileMapping", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@systemid", fileMappingId);
                        con.Open();
                        resultDelete = command.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return resultDelete;

            }
            catch (Exception ex)
            {
                writeErrorMessage(ex.Message.ToString(), "deleteFileMapping");
                return 0;
            }
        }

        DataTable dt;
        SqlDataAdapter da;

        public DataTable getLoginDetails(string userid, string password)
        {

            dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("pro_getLoginDetails", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@empid", userid);
                        cmd.Parameters.AddWithValue("@pwd", password);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }

                    return dt;
                }
            }
            catch (Exception ex)
            {
                return dt;
            }
        }

        public int UpdatePassword(string empid, string password)
        {
            int updateResult = 0;
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand com_password = new SqlCommand("pro_UpdatePassword", sqlConnection))
                    {
                        com_password.CommandType = CommandType.StoredProcedure;
                        com_password.Parameters.AddWithValue("@empid", empid);
                        com_password.Parameters.AddWithValue("@Password", password);
                        sqlConnection.Open();
                        updateResult = com_password.ExecuteNonQuery();
                        sqlConnection.Close();
                        return updateResult;
                    }
                }
            }
            catch (Exception ex)
            {
                return updateResult;
            }
        }

        public void writeErrorMessage(string errorMessage, string functionName)
        {
            var systemPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Syrma_Training_Errors" + "\\" + DateTime.Now.ToString("dd-MM-yyyy");

            if (!Directory.Exists(systemPath))
            {
                Directory.CreateDirectory(systemPath);
            }

            string WrErrorLog = String.Format(@"{0}\{1}.txt", systemPath, "ErrorLogInRFIDTag");
            using (StreamWriter errLogs = new StreamWriter(WrErrorLog, true))
            {
                errLogs.WriteLine("--------------------------------------------------------------------------------------------------------------------" + Environment.NewLine);
                errLogs.WriteLine("---------------------------------------------------" + DateTime.Now + "----------------------------------------------" + Environment.NewLine);
                errLogs.WriteLine(errorMessage + Environment.NewLine + "-----" + functionName);
                errLogs.Close();
            }
        }
        public List<insert_stat> SaveDataToDatabase(string SystemId, string Usertype)
        {
            // var dt = DateTime.Today;
            var insertlist = new List<insert_stat>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("pro_Insert_SystemId", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SystemId", SystemId);
                    cmd.Parameters.AddWithValue("@Usertype", Usertype);
                    connection.Open();
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                insertlist.Add(new insert_stat
                                {
                                    Insert_status = reader.GetInt32(0)
                                });
                            }
                        }

                    }
                    connection.Close();
                }


            }
            return insertlist;
        }
        public List<Systemid> GetData()
        {
            var dataList = new List<Systemid>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("pro_GetSystem_Id", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    connection.Open();
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dataList.Add(new Systemid
                                {
                                    Id = reader.GetString(0),
                                    SystemId = reader.GetString(1),
                                });
                            }
                        }
                    }
                    connection.Close();
                }
            }
            return dataList;
        }

        public int deleteSystemid(string deletesystemId)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("pro_delete_ipaddress", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    connection.Open();
                    cmd.Parameters.AddWithValue("@SystemId", deletesystemId);
                    result = cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return result;
        }
    }
}
