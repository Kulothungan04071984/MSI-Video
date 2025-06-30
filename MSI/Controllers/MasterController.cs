using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using MSI.Models;

namespace MSI.Controllers
{
	public class MasterController : Controller
	{
		private DataManagementcs _domainServices;
        private readonly long _fileSizeLimit = 10L * 1024L * 1024L * 1024L;
        // private readonly IWebHostEnvironment _webHostEnvironment;, IWebHostEnvironment webHostEnvironment
        public MasterController(DataManagementcs domainServices)
        {
            _domainServices = domainServices;
            //_webHostEnvironment = webHostEnvironment;
           // _iinsertDetails = iinsertDetails;
        }
        public IActionResult MasterDetails()
		{
            UploadFileDetails uploadFileDetails = new UploadFileDetails();
			var domainSid = _domainServices.GetDomainSid();
            
			//var systemName=_domainServices.GetAllConnectedSystemNames();
			ViewBag.DomainSid = domainSid;
            //ViewBag.computerName = systemName;

            uploadFileDetails.lstapprovecustomers = _domainServices.approvedGetCustomer();
            uploadFileDetails.lstapprovefgnames = new List<SelectListItem>();
            uploadFileDetails.lstFile = new List<SelectListItem>();
            uploadFileDetails.lstSystem = _domainServices.getSystemNames();
            //uploadFileDetails.lstFile= _domainServices.GetfileName();
            uploadFileDetails.lstFileMappings = _domainServices.getFileMappingDetails();
            return View(uploadFileDetails);
		}


        [HttpGet]
        public JsonResult GetFgNamesByCustomer(int customerId)
        {
            try
            {
                // Get the list of FG Names for the selected customer
                var approvefgNames = _domainServices.approveGetFgnames(customerId);

                // Return the list of FG Names as JSON
                return Json(approvefgNames);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }
        [HttpGet]
        public JsonResult GetFgNames(string customerId, string fgid)
        {
            UploadFileDetails uploadFileDetails = new UploadFileDetails();
            try
            {
                uploadFileDetails.lstFile = _domainServices.GetfileName(customerId, fgid);
                return Json(uploadFileDetails.lstFile);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return Json(uploadFileDetails.lstFile); // or return Json(null);
            }
        }

        public JsonResult getPdfFileName(string fileid)
        {
            var resultfilename = _domainServices.pdfFileCopyfromServerview(fileid);
            if (string.IsNullOrEmpty(resultfilename))
            {
                ViewBag.ErrorMessage = "File Not Found";
                return Json("File Not Found");
            }
            return Json(resultfilename);
        }


        [HttpPost]
        public async Task<IActionResult> MasterDetails(IFormFile file,UploadFileDetails uploadFileDetails)
        {

            int result = 0;
            UploadFileDetails objupload =new UploadFileDetails();
            try
            {
                if (file != null && file.Length > 0)
                {
         
                    if (file.Length > _fileSizeLimit)
                    {
                        ViewBag.Message = "File size exceeds the limit.";
                        ViewBag.ThumbnailPath = "";
                    }
                    else
                    {
                        //var path = "\\\\192.168.1.188\\MSI_Videos";
                        var path = "\\\\192.168.1.121\\MSI_Applications";
                        //var uploadVideoFile = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                        var uploadVideoFile = Path.Combine(path, "upload");
                        writeErrorMessage(uploadVideoFile.ToString(), "File path combine successfully");
                        if (Directory.Exists(path))
                        {
                            //Directory.CreateDirectory(uploadVideoFile);
                            var filePath = Path.Combine(uploadVideoFile, file.FileName);
                            using (var filestream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(filestream);
                                writeErrorMessage(filePath.ToString(), "File Copy successfully");
                            }
                            var thumbnailPath = Path.Combine(uploadVideoFile, $"{Path.GetFileNameWithoutExtension(file.FileName)}.jpg");
                            ExtractThumbnail(filePath, thumbnailPath);
                            var uploadDetails = new UploadFileDetails();
                            uploadDetails.systemid = string.IsNullOrEmpty(uploadFileDetails.systemid.ToString()) ? 0 : uploadFileDetails.systemid;
                            uploadDetails.filepath = filePath;
                            uploadDetails.uploaddatetime = DateTime.Today.ToString();
                            uploadDetails.uploadEmployee = "70192";
                            uploadDetails.VideoFromTime= uploadFileDetails.VideoFromTime;
                            uploadDetails.VideoToTime = uploadFileDetails.VideoToTime;
                            uploadDetails.VideoDate = uploadFileDetails.VideoDate;
                            //uploadFileDetails.systemname = uploadFileDetails.lstSystem.Where(a => a.Value == uploadFileDetails.systemid.ToString()).Select(a => a.Text.ToString()).FirstOrDefault();
                            //uploadFileDetails.systemname = "";
                            result = _domainServices.uploaddatainserted(uploadDetails);
                            writeErrorMessage(result.ToString(),
                                "Video File Upload successfully");
                            if (result > 0)
                            {
                                ViewBag.Message = "Video uploaded successfully";
                                ViewBag.ThumbnailPath = $"/upload/{Path.GetFileName(thumbnailPath)}";
                                uploadFileDetails.lstSystem = _domainServices.getSystemNames();
                                uploadFileDetails.lstFileMappings = _domainServices.getFileMappingDetails();
                                objupload = uploadFileDetails;
                            }
                            else
                            {
                                ViewBag.Message = "Video Not uploaded ";
                                ViewBag.ThumbnailPath = "";
                            }

                        }
                    }

                }
                else
                {
                    ViewBag.Message = "Please Give Proper Input";                
                }

                return View(objupload);
               
            }
            catch(Exception ex)
            {
                writeErrorMessage(ex.Message.ToString(), "MasterDetails Control Issue");
                ViewBag.Message = ex.Message;
                return View(objupload);
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
            catch (Exception ex) {
                writeErrorMessage(ex.Message.ToString(), "ExtractThumbnail");
            }
        }
        [HttpPost]
        public JsonResult deleteFileMapping(int systemid,string videoDate, string fromtime, string totime)
        {
            int resultdel = 0;
            try
            {
                 resultdel = _domainServices.deleteFileMapping(systemid, videoDate, fromtime, totime);
               }
             
            catch(Exception ex)
            {
                writeErrorMessage(ex.Message.ToString(), "deleteFileMapping");
                resultdel = 0;
            }
            return Json(resultdel);
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

