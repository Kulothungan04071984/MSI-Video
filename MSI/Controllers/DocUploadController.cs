//using Microsoft.AspNetCore.Mvc;

//namespace MSI.Controllers
//{
//    public class DocUploadController : Controller
//    {
//        public IActionResult ShowuploadDetails()
//        {
//            return View();
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using MSI.Models;
using static Azure.Core.HttpHeader;

namespace MSI.Controllers
{
    public class DocUploadController : Controller
    {
        private DataManagementcs _domainServices;
        private readonly long _fileSizeLimit = 10L * 1024L * 1024L * 1024L;
        // private readonly IWebHostEnvironment _webHostEnvironment;, IWebHostEnvironment webHostEnvironment
        public DocUploadController(DataManagementcs domainServices)
        {
            _domainServices = domainServices;
            //_webHostEnvironment = webHostEnvironment;
            // _iinsertDetails = iinsertDetails;
        }
        public IActionResult ShowuploadDetails()
        {
            Fileuploaddetails Fileuploaddetails = new Fileuploaddetails();
            var domainSid = _domainServices.GetDomainSid();
            //var systemName=_domainServices.GetAllConnectedSystemNames();
            ViewBag.DomainSid = domainSid;
            //ViewBag.computerName = systemName;
            Fileuploaddetails.lstcustomers = _domainServices.getcustomernames();
            Fileuploaddetails.lstfgnames = new List<SelectListItem>();
            Fileuploaddetails.lstdocVerifieds = _domainServices.getFileUploaddetails();
            return View(Fileuploaddetails);
        }

        [HttpPost]
        public async Task<IActionResult> ShowuploadDetails(IFormFileCollection files, Fileuploaddetails fileuploaddetails)
        {

            int result = 0;
            Fileuploaddetails objupload1 = new Fileuploaddetails();
            string Customer_name = fileuploaddetails.customerName;
            string Fg_Name = fileuploaddetails.FgName;
            try
            {
                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {

                        if (file.Length > _fileSizeLimit)
                        {
                            ViewBag.Message = "File size exceeds the limit.";
                            ViewBag.ThumbnailPath = "";
                        }
                        else
                        {
                            var path = "\\\\192.168.1.188\\MSI_Videos";
                            //var uploadVideoFile = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                            var uploadVideoFile = Path.Combine(path, "uploads");
                            writeErrorMessage(uploadVideoFile.ToString(), "File path combine successfully");
                            if (Directory.Exists(path))
                            {
                                //Directory.CreateDirectory(uploadVideoFile);
                             
                                var uploadDetails1 = new Fileuploaddetails();
                                //uploadDetails1.empId = string.IsNullOrEmpty(fileuploaddetails.empId.ToString()) ? "0" : fileuploaddetails.empId;
                                uploadDetails1.empId = "123";
                                uploadDetails1.empName = "70192";
                                uploadDetails1.docDateTime = DateTime.Today.ToString();
                                uploadDetails1.docName = "abc";
                                uploadDetails1.docType = "Reference copy";
                                //uploadDetails1.filepath=filePath;
                                uploadDetails1.filepath = string.Empty;
                                uploadDetails1.customerName = "abc";
                                uploadDetails1.FgName = "abc";

                                //Fileuploaddetails.systemname = Fileuploaddetails.lstSystem.Where(a => a.Value == Fileuploaddetails.systemid.ToString()).Select(a => a.Text.ToString()).FirstOrDefault();
                                //Fileuploaddetails.systemname = "";
                                result = _domainServices.uploaddatadetails(uploadDetails1); 
                                if (result > 0)
                                {
                                    //docName Created
                                    DateTime now = DateTime.Now;
                                    string dateOnly = now.ToString("yyyy-MM-dd");
                                    var docName = result + dateOnly;
                                    docName = docName.Replace("/", "");
                                    docName = docName.Replace("-", "");
                                    var pathname = uploadVideoFile;
                                    pathname = pathname + "\\" + docName+".pdf";
                                    var filepathUpdate=_domainServices.updateFilePath(result, pathname);
                                    writeErrorMessage(result.ToString(), "Document File Upload successfully");
                                    if (filepathUpdate > 0)
                                    {
                                        ViewBag.Message = "Video uploaded successfully";
                                        ViewBag.ThumbnailPath = $"/uploads/{Path.GetFileName(pathname)}";
                                        var filePath = Path.Combine(uploadVideoFile, docName);
                                        using (var filestream = new FileStream(filePath, FileMode.Create))
                                        {
                                            await file.CopyToAsync(filestream);
                                            writeErrorMessage(filePath.ToString(), "File Copy successfully");
                                        }
                                        var thumbnailPath = Path.Combine(uploadVideoFile, $"{Path.GetFileNameWithoutExtension(docName)}.jpg");
                                        ExtractThumbnail(filePath, thumbnailPath);
                                        writeErrorMessage(result.ToString(),
                                        "Video File Upload successfully");

                                        //ViewBag.Message = "Document uploaded successfully";
                                        //ViewBag.ThumbnailPath = $"/uploads/{Path.GetFileName(thumbnailPath)}";
                                        fileuploaddetails.lstcustomers = _domainServices.getcustomernames();
                                        fileuploaddetails.lstfgnames = new List<SelectListItem>();
                                        fileuploaddetails.lstdocVerifieds = _domainServices.getFileUploaddetails();
                                        objupload1 = fileuploaddetails;
                                    }
                                }
                                else
                                {
                                    ViewBag.Message = "Document Not uploaded ";
                                    ViewBag.ThumbnailPath = "";
                                }

                            }
                        }

                    }

                }

                
                else
                {
                    ViewBag.Message = "Please Give Proper Input";
                }

                return View(objupload1);

            }
            catch (Exception ex)
            {
                writeErrorMessage(ex.Message.ToString(), "MasterDetails Control Issue");
                ViewBag.Message = ex.Message;
                return View(objupload1);
            }

        }
        private void ExtractThumbnail(string videoPath, string thumbnailPath)
        {
            try
            {
                var inputFile = new MediaToolkit.Model.MediaFile { Filename = videoPath };
                var outputFile = new MediaToolkit.Model.MediaFile { Filename = thumbnailPath };

                using (var engine = new MediaToolkit.Engine())
                {
                    engine.GetMetadata(inputFile);
                    var options = new MediaToolkit.Options.ConversionOptions { Seek = TimeSpan.FromSeconds(1) };
                    engine.GetThumbnail(inputFile, outputFile, options);
                }
            }
            catch (Exception ex)
            {
            }
        }
        [HttpPost]
        public JsonResult deleteFileMapping(int documentId)
        {
            int resultdel = 0;
            try
            {
                resultdel = _domainServices.deleteFileMapping1(documentId);
            }

            catch (Exception ex)
            {
                resultdel = 0;
            }
            return Json(resultdel);
        }
        [HttpGet]
        public JsonResult GetFgNamesByCustomer(int customerId)
        {
            try
            {
                // Get the list of FG Names for the selected customer
                var fgNames = _domainServices.getfgnames(customerId);

                // Return the list of FG Names as JSON
                return Json(fgNames);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
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
    }
}


