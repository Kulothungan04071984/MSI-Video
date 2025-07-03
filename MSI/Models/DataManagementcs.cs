
using System.Data;
using System.Security.Principal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
namespace MSI.Models
{

    public class DataManagementcs
    {
        private readonly string ConnectionString;
        private readonly string ConnectionString1;

        public DataManagementcs(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("conn");
            ConnectionString1 = configuration.GetConnectionString("conn2");
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

        public int uploaddatadetails(Fileuploaddetails objFileDetails1)
        {
            int result = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("pro_InsertUploaddataDetails", conn))
                    {

                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@empid", objFileDetails1.empId);
                        //cmd.Parameters.AddWithValue("@empName", objFileDetails1.empName);
                        cmd.Parameters.AddWithValue("@docDateTime", objFileDetails1.docDateTime);
                        cmd.Parameters.AddWithValue("@docname", objFileDetails1.docName);
                        cmd.Parameters.AddWithValue("@doctype", objFileDetails1.docType);
                        cmd.Parameters.AddWithValue("@docfilepath", objFileDetails1.filepath);
                        cmd.Parameters.AddWithValue("@Customer_Name", objFileDetails1.customerName);
                        cmd.Parameters.AddWithValue("@Fg_Name", objFileDetails1.FgName);

                        SqlParameter outputparam = new SqlParameter("@InsertedId", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputparam);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        result = Convert.ToInt32(outputparam.Value);
                        conn.Close();

                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                writeErrorMessage(ex.Message.ToString(), "uploaddocinserted");
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



        //public List<string> GetAllConnectedSystemNames()
        //{
        //    var computerNames = new List<string>();

        //    try
        //    {
        //        // Get the domain context
        //        using (var context = new PrincipalContext(ContextType.Domain))
        //        {
        //            using (var searcher = new PrincipalSearcher(new ComputerPrincipal(context)))
        //            {
        //                foreach (var result in searcher.FindAll())
        //                {
        //                    using (var directoryEntry = result.GetUnderlyingObject() as DirectoryEntry)
        //                    {
        //                        if (directoryEntry != null)
        //                        {
        //                            computerNames.Add(directoryEntry.Properties["name"].Value.ToString());
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions as needed
        //        //computerNames.Add($"An error occurred: {ex.Message}");
        //        writeErrorMessage(ex.Message.ToString(), "GetAllConnectedSystemNames");
        //    }

        //    return computerNames;
        //}
        public List<DataAprovel> GetApprovedData()
        {
            var approvedlist = new List<DataAprovel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString1))
                {
                    using (SqlCommand cmd = new SqlCommand("pro_GetApprovedDetails", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        connection.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                approvedlist.Add(new DataAprovel
                                {
                                    CustomerName = reader["customer_name"].ToString(),
                                    FgNo = reader["Fg_name"].ToString(),
                                    DocumentName = reader["Doc_name"].ToString(),
                                    DocumentStatus = reader["Doc_status"].ToString(),
                                    FileName = reader["File_Path"].ToString()
                                    
                                });
                            }
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                writeErrorMessage(ex.Message.ToString(), "ApprovedGetdata");
            }

            return approvedlist;
        }
        public List<DataAprovel> GetApprovedDatabyfg(string customername,string fgname)
        {
            var approvedlist = new List<DataAprovel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("pro_GetApprovedData", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        // Explicitly defining parameter types and sizes
                        cmd.Parameters.Add("@Customer_Name", SqlDbType.NVarChar, 255).Value = customername;
                        cmd.Parameters.Add("@Fg_Name", SqlDbType.NVarChar, 255).Value = fgname;

                        connection.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                approvedlist.Add(new DataAprovel
                                {
                                    CustomerName = reader["customer_name"] == DBNull.Value ? null : reader["customer_name"].ToString(),
                                    FgNo = reader["Fg_name"] == DBNull.Value ? null : reader["Fg_name"].ToString(),
                                    DocumentName = reader["Doc_name"] == DBNull.Value ? null : reader["Doc_name"].ToString(),
                                    DocumentStatus = reader["Doc_status"] == DBNull.Value ? null : reader["Doc_status"].ToString(),
                                    FileName = reader["File_Path"] == DBNull.Value ? null : reader["File_Path"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                writeErrorMessage(ex.Message.ToString(), "ApprovedGetdata");
            }

            return approvedlist;
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
                                    list.Add(new SelectListItem { Value = row["system_id"].ToString(), Text = row["Stage_Name"].ToString() });
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

        public List<SelectListItem> approvedGetCustomer()
        {
            var aprovedcustomlist = new List<SelectListItem>();
            try
            {
                DataTable dtaprcustomValue = new DataTable();

                using (SqlConnection customValue = new SqlConnection(ConnectionString1))
                {
                    using (SqlCommand cmdSetValue = new SqlCommand("Get_customer_Name", customValue))
                    {
                        cmdSetValue.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter daGetValue = new SqlDataAdapter(cmdSetValue))
                        {
                            daGetValue.Fill(dtaprcustomValue);
                            if (dtaprcustomValue.Rows.Count > 0)
                            {
                                foreach (DataRow row in dtaprcustomValue.Rows)
                                {
                                    aprovedcustomlist.Add(new SelectListItem { Value = row["cutomer_id"].ToString(), Text = row["customer_name"].ToString() });
                                }
                            }
                        }
                    }
                }
                return aprovedcustomlist;
            }
            catch (Exception ex)
            {
                writeErrorMessage(ex.Message.ToString(), "getcustomernames");
                return aprovedcustomlist;
            }


        }
        public List<SelectListItem> getcustomernames()
        {
            var customlist = new List<SelectListItem>();
            try
            {
                DataTable dtcustomValue = new DataTable();

                using (SqlConnection customValue = new SqlConnection(ConnectionString1))
                {
                    using (SqlCommand cmdSetValue = new SqlCommand("pro_getCustomerName", customValue))
                    {
                        cmdSetValue.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter daGetValue = new SqlDataAdapter(cmdSetValue))
                        {
                            daGetValue.Fill(dtcustomValue);
                            if (dtcustomValue.Rows.Count > 0)
                            {
                                foreach (DataRow row in dtcustomValue.Rows)
                                {
                                    customlist.Add(new SelectListItem { Value = row["cutomer_id"].ToString(), Text = row["customer_name"].ToString() });
                                }
                            }
                        }
                    }
                }
                return  customlist;
            }
            catch (Exception ex)
            {
                writeErrorMessage(ex.Message.ToString(), "getcustomernames");
                return customlist;
            }
        }

        public List<SelectListItem> getfgnames(int customerId)
        {
            var Fglist = new List<SelectListItem>();
            try
            {
                DataTable dtFgValue = new DataTable();

                using (SqlConnection FgValue = new SqlConnection(ConnectionString1))
                {
                    using (SqlCommand cmdFgValue = new SqlCommand("pro_getFgName", FgValue))
                    {
                        
                        using (SqlDataAdapter daGetValue = new SqlDataAdapter(cmdFgValue))
                        {
                            cmdFgValue.CommandType = CommandType.StoredProcedure;
                            cmdFgValue.Parameters.AddWithValue("@Customer_id", customerId);

                            daGetValue.Fill(dtFgValue);
                            if (dtFgValue.Rows.Count > 0)
                            {
                                foreach (DataRow row in dtFgValue.Rows)
                                {
                                    Fglist.Add(new SelectListItem { Value = row["Fg_id"].ToString(), Text = row["Fg_Name"].ToString() });
                                }
                            }
                        }
                    }
                }
                return Fglist;
            }
            catch (Exception ex)
            {
                writeErrorMessage(ex.Message.ToString(), "getfgnames");
                return Fglist;
            }
        }
        public List<SelectListItem> approveGetFgnames(int customerId)
        {
            var aproveFglist = new List<SelectListItem>();
            try
            {
                DataTable dtFgValue = new DataTable();

                using (SqlConnection approveFgValue = new SqlConnection(ConnectionString1))
                {
                    using (SqlCommand cmdaproveFgValue = new SqlCommand("Get_Fg_Name", approveFgValue))
                    {

                        using (SqlDataAdapter daGetValue = new SqlDataAdapter(cmdaproveFgValue))
                        {
                            cmdaproveFgValue.CommandType = CommandType.StoredProcedure;
                            cmdaproveFgValue.Parameters.AddWithValue("@Customer_id", customerId);

                            daGetValue.Fill(dtFgValue);
                            if (dtFgValue.Rows.Count > 0)
                            {
                                foreach (DataRow row in dtFgValue.Rows)
                                {
                                    aproveFglist.Add(new SelectListItem { Value = row["Fg_id"].ToString(), Text = row["Fg_Name"].ToString() });
                                }
                            }
                        }
                    }
                }
                return aproveFglist;
            }
            catch (Exception ex)
            {
                writeErrorMessage(ex.Message.ToString(), "approveGetFgnames");
                return aproveFglist;
            }
        }
        //public List<SelectListItem> approvedFgnames(int customerId,int fgid)
        //{
        //    var aprovedFg = new List<SelectListItem>();
        //    try
        //    {
        //        DataTable dtFgValue = new DataTable();

        //        using (SqlConnection approveFgValue = new SqlConnection(ConnectionString))
        //        {
        //            using (SqlCommand cmdaproveFgValue = new SqlCommand("Get_ApprovedFg_Name", approveFgValue))
        //            {

        //                using (SqlDataAdapter daGetValue = new SqlDataAdapter(cmdaproveFgValue))
        //                {
        //                    cmdaproveFgValue.CommandType = CommandType.StoredProcedure;
        //                    cmdaproveFgValue.Parameters.AddWithValue("@Customer_id", customerId);
        //                    cmdaproveFgValue.Parameters.AddWithValue("@FG_id", fgid);
        //                    daGetValue.Fill(dtFgValue);
        //                    if (dtFgValue.Rows.Count > 0) 
        //                    {
        //                        foreach (DataRow row in dtFgValue.Rows)
        //                        {
        //                            aprovedFg.Add(new SelectListItem { Value = row["customer_id"].ToString(), Text = row["customer_name"].ToString() });
        //                            aprovedFg.Add(new SelectListItem { Value = row["Fg_id"].ToString(), Text = row["Fg_Name"].ToString() });
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        return aprovedFg;
        //    }
        //    catch (Exception ex)
        //    {
        //        writeErrorMessage(ex.Message.ToString(), "approvedFg");
        //        return aprovedFg;
        //    }
        //}

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

        public List<SelectListItem> GetfileName(string customername, string fgname)
        {
            var filelist = new List<SelectListItem>();
            try
            {
                DataTable dtGetValue = new DataTable();

                using (SqlConnection conGetValue = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmdSetValue = new SqlCommand("pro_GetfileName", conGetValue))
                    {
                        cmdSetValue.CommandType = CommandType.StoredProcedure;
                        cmdSetValue.Parameters.AddWithValue("@Customer_name", customername);
                        cmdSetValue.Parameters.AddWithValue("@fg_name", fgname);
                        using (SqlDataAdapter daGetValue = new SqlDataAdapter(cmdSetValue))
                        {
                            daGetValue.Fill(dtGetValue);
                            if (dtGetValue.Rows.Count > 0)
                            {
                                foreach (DataRow row in dtGetValue.Rows)
                                {
                                    filelist.Add(new SelectListItem { Value = row["File_id"].ToString(), Text = row["File_Name"].ToString() });
                                }
                            }
                        }
                    }
                }
                return filelist;
            }
            catch (Exception ex)
            {
                writeErrorMessage(ex.Message.ToString(), "GetfileNames");
                return filelist;
            }
        }



        public List<DocVerified> getFileUploaddetails()
        {
            var lstFileuploading = new List<DocVerified>();
            DocVerified objFileupload;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))

                {
                    using (SqlCommand cmd = new SqlCommand("pro_getFileuploadDetails", con))
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
                                        objFileupload = new DocVerified();
                                        objFileupload.docId = Convert.ToInt32(row["docId"].ToString());
                                        objFileupload.empId = Convert.ToInt32(row["Emp_id"].ToString());
                                        objFileupload.customer_name = row["customer_name"].ToString();
                                        objFileupload.Fg_Name = row["Fg_name"].ToString();
                                        objFileupload.docName = row["Doc_name"].ToString();
                                        objFileupload.docDateTime = row["Upload_time"].ToString();
                                        objFileupload.docType = row["Doc_Type"].ToString();
                                        objFileupload.docStatus = row["Doc_status"].ToString();
                                        objFileupload.Reject_reason = row["Reject_reason"].ToString();
                                        lstFileuploading.Add(objFileupload);
                                    }
                                }
                            }
                        }
                    }
                }
                return lstFileuploading;
            }

            catch (Exception ex)
            {
                writeErrorMessage(ex.Message.ToString(), "getFileMappingDetails");
                return lstFileuploading;
            }
        }

        public string getfilepath(string device_name,string currentTime , string currentDate)
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
                        cmd.Parameters.AddWithValue("@timedetails", currentTime);
                        cmd.Parameters.AddWithValue("@date", currentDate);
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

        public int deleteFileMapping(int fileMappingId,string videoDate, string fromtime, string totime)
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
                        command.Parameters.AddWithValue("@videodate", videoDate);
                        command.Parameters.AddWithValue("@fromTime", fromtime);
                        command.Parameters.AddWithValue("@toTime", totime);
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

        public int deleteFileMapping1(int fileMappingId)
        {
            int resultDelete = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("pro_deleteFileMapping1", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@docId", fileMappingId);
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
                writeErrorMessage(ex.Message.ToString(), "getLoginDetails");
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
                writeErrorMessage(ex.Message.ToString(), "UpdatePassword");
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
        public int SaveDataToDatabase(string SystemId, string Usertype,string StageName,string Customername,string Fgno)
        {
            // var dt = DateTime.Today;
            var insertlist = 0;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("pro_Insert_SystemId", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SystemId", SystemId);
                    cmd.Parameters.AddWithValue("@Usertype", Usertype);
                    cmd.Parameters.AddWithValue("@StageName", StageName);            
                    cmd.Parameters.AddWithValue("@customername", Customername);            
                    cmd.Parameters.AddWithValue("@fgname", Fgno);            
                    connection.Open();
                    insertlist = cmd.ExecuteNonQuery();
                    connection.Close();
                }

            }
            return insertlist;
        }
         public int UpdateDataToDatabase(string SystemDetailsid, string Updateystemid,string Updateusertype,string UpdateStageName)
        {
            // var dt = DateTime.Today;
            var upresult = 0;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("pro_Update_SystemId", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SystemId", SystemDetailsid);
                    //cmd.Parameters.AddWithValue("@Usertype", Usertype);                  
                    //cmd.Parameters.AddWithValue("@StageName", StageName);                  
                    cmd.Parameters.AddWithValue("@UpdateSystemId", Updateystemid);                  
                    cmd.Parameters.AddWithValue("@UpdateUserType", Updateusertype);                  
                    cmd.Parameters.AddWithValue("@UpdateStageName", UpdateStageName);                  
                    connection.Open();                    
                    upresult=cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return upresult;
        }
        public List<Systemid> GetData()
        {
            var dataList = new List<Systemid>();
            try
            {
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
                                        Id = reader.IsDBNull(reader.GetOrdinal("systemid")) ? null : Convert.ToString(reader.GetInt32(reader.GetOrdinal("systemid"))),
                                        SystemId = reader.IsDBNull(reader.GetOrdinal("systemIP")) ? null : reader.GetString(reader.GetOrdinal("systemIP")),
                                        Usertype = reader.IsDBNull(reader.GetOrdinal("usertype")) ? null : reader.GetString(reader.GetOrdinal("usertype")),
                                        StageName = reader.IsDBNull(reader.GetOrdinal("stagename")) ? null : reader.GetString(reader.GetOrdinal("stagename")),
                                        cutomerName = reader.IsDBNull(reader.GetOrdinal("customer_name")) ? null : reader.GetString(reader.GetOrdinal("customer_name")),
                                        fgNo = reader.IsDBNull(reader.GetOrdinal("fg_name")) ? null : reader.GetString(reader.GetOrdinal("fg_name")),
                                    });
                                }
                            }
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex) 
            {
                writeErrorMessage(ex.Message.ToString(), "GetData");          
            }
            return dataList;
        }

        public int deleteSystemid(string deletesystemId)
        {

            int result = 0;
            try
            {
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
            }
            catch (Exception ex) {
                writeErrorMessage(ex.Message.ToString(), "deleteSystemid");
                return result;
            }
            return result;
        }

        public List<DocVerified> getDocVerifiedList()
        {
            var lstDocVerified = new List<DocVerified>();
            var objDocVerified = new DocVerified();
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                using (var cmd = new SqlCommand("pro_getDocDetails", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            objDocVerified = new DocVerified
                            {
                                docId = Convert.ToInt32(reader["id"]),
                                empId = Convert.ToInt32(reader["Emp_id"]),
                                customer_name = reader["customer_name"].ToString(),
                                Fg_Name = reader["Fg_name"].ToString(),
                                docName = reader["Doc_name"].ToString(),
                                docDateTime = reader["Upload_time"].ToString(),
                                docType = reader["Doc_Type"].ToString(),
                                docStatus = reader["Doc_status"].ToString(),
                                filepath = reader["File_Path"].ToString(),
                                isActive = Convert.ToBoolean(reader["IsActive"].ToString())
                            };

                            lstDocVerified.Add(objDocVerified);
                        }
                    }
                    connection.Close();
                }
            }
            catch(Exception ex)
            {
                writeErrorMessage(ex.Message.ToString(), "getDocVerifiedList");
                return lstDocVerified;
            }
            return lstDocVerified;
        }

        public int updateDocStatus(int docId,string path)
        {
            int updateStatus = 0;
            string watermark = string.Empty;
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                using (var cmd = new SqlCommand("pro_DocStatusUpdated", conn))
                {
                    cmd.CommandType= CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@docid", docId);
                    conn.Open();
                    updateStatus = cmd.ExecuteNonQuery();
                    conn.Close();
                    watermark = AddWatermarkcaption(path,path,"APPROVED");
                }
            }
            catch (Exception ex) {
                writeErrorMessage(ex.Message.ToString(), "updateDocStatus");
                return updateStatus;
            }
            return updateStatus;
        }

        public int updateDocRejectDetails(int docId, string rejectReason,string path)
        {
            int updateResult = 0;
            string watermark = string.Empty;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                using (var cmd = new SqlCommand("pro_DocRejectStatus", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@docid", docId);
                    cmd.Parameters.AddWithValue("@rejectReason", rejectReason);
                    con.Open();
                    updateResult = cmd.ExecuteNonQuery();
                    con.Close();
                    //watermark = AddWatermarkcaption(path, path, "REJECT");
                }
            }
            catch (Exception ex)
            {
                writeErrorMessage(ex.Message.ToString(), "updateDocRejectDetails");
                return updateResult;
            }
            return updateResult;
        }

        public string AddWatermarkcaption(string path, string outpath, string waterMark)
        {
            string result = string.Empty;
            try
            {
                string inputFile = path;
                string outputFile = outpath;
                string watermarkText = waterMark;
                

                // Open the file
                PdfDocument document = PdfReader.Open(inputFile, PdfDocumentOpenMode.Modify);

                foreach (PdfPage page in document.Pages)
                {
                    //XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Prepend);
                    //Indise page water mark
                    //var gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append);
                    //XFont font = new XFont("Arial", 60, XFontStyleEx.Bold);
                    //XSize size = gfx.MeasureString(watermarkText, font);

                    //// Center coordinates and angle
                    //double x = (page.Width - size.Width) / 2;
                    //double y = (page.Height - size.Height) / 2;

                    //gfx.TranslateTransform(x + size.Width / 2, y + size.Height / 2);
                    //gfx.RotateTransform(-45);
                    //gfx.TranslateTransform(-(x + size.Width / 2), -(y + size.Height / 2));

                    //gfx.DrawString(watermarkText, font, new XSolidBrush(XColor.FromArgb(128, 200, 200, 200)), x, y);

                    //gfx.Dispose();

                    var gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append);
                    XFont font = new XFont("Arial", 20, XFontStyleEx.Bold); // Smaller font for stamp-style
                    XSize size = gfx.MeasureString(watermarkText, font);

                    // Margin from the edge
                    double marginX = 20; // distance from the right edge
                    double marginY = 20; // distance from the top edge

                    // Top-right position
                    double x = page.Width - size.Width - marginX;
                    double y = marginY;

                    gfx.DrawString(watermarkText, font, new XSolidBrush(XColor.FromArgb(180, 200, 0, 0)), x, y);

                    gfx.Dispose();
                }

                // Save the document
                document.Save(outputFile);
               return result = "Success";
            }
            catch (Exception ex) {
                return result = ex.Message.ToString();
            }
        }
        public int updateFilePath(int docid, string filepath)
        {
            var resultPathUpdate = 0;
            try
            {
                using(SqlConnection con = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand("Pro_updateFileName", con))
                {
                    cmd.CommandType= CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@docid",docid);
                    cmd.Parameters.AddWithValue("@filepath", filepath);
                    con.Open();
                    resultPathUpdate = cmd.ExecuteNonQuery();
                    con.Close();
                }

            }
            catch (Exception ex) {
                writeErrorMessage(ex.Message.ToString(), "updateDocRejectDetails");
                return resultPathUpdate;
            }

            return resultPathUpdate;
        }

        public string pdfFileCopyfromServer(string pdfFile)
        {
            var resultfilename = string.Empty;
            try
            {
                string sourcePath = pdfFile; // Network path or local server path
                string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos");

                if (!Directory.Exists(wwwrootPath))
                {
                    Directory.CreateDirectory(wwwrootPath);
                }
                string fileName = Path.GetFileName(pdfFile);
                string destinationPath = Path.Combine(wwwrootPath, fileName);

                File.Copy(sourcePath, destinationPath, true); // Overwrite if exists
                resultfilename = fileName;
            }
            catch (Exception ex)
            {
               return resultfilename;
            }

            return resultfilename;
        }


        public string pdfFileCopyfromServerview(string fileid)
        {
            var resultfilename = string.Empty;
            try
            {
                var Fileid = int.Parse(fileid);
                string sourcePath = getfilepathforview(Fileid); // Network path or local server path
                string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos");

                if (!Directory.Exists(wwwrootPath))
                {
                    Directory.CreateDirectory(wwwrootPath);
                }
                string fileName = Path.GetFileName(sourcePath);
                string destinationPath = Path.Combine(wwwrootPath, fileName);

                File.Copy(sourcePath, destinationPath, true); // Overwrite if exists
                resultfilename = fileName;
            }
            catch (Exception ex)
            {
                return resultfilename;
            }

            return resultfilename;
        }

        public string getfilepathforview(int fileid)
        {
            string filepath = null; // changed type from SqlDataReader to string
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                using (var cmd = new SqlCommand("pro_getfilePathforview", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@docid", fileid);

                    con.Open();
                    var result = cmd.ExecuteScalar(); // returns object
                    filepath = result != null ? result.ToString() : null;
                }
            }
            catch (Exception ex)
            {
                writeErrorMessage(ex.Message.ToString(), "FileViewforProdAdmin");
                return filepath;
            }

            return filepath;
        }

    }
}
