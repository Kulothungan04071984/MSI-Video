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
        public IActionResult MasterDetails(UploadFileDetails uploadFileDetails)
        {
            UploadFileDetails objupload = new UploadFileDetails();
            int result = 0;

            try
            {
                if (uploadFileDetails.approvefileid == 0 || uploadFileDetails.systemid == 0)
                {
                    ViewBag.Message = "Please select System and File.";
                    return View(uploadFileDetails);
                }

                // Resolve file path from file ID
                string filePath = _domainServices.getfilepathforview(uploadFileDetails.approvefileid); // This must be implemented in your domain service

                if (string.IsNullOrEmpty(filePath))
                {
                    ViewBag.Message = "File not found for the selected file ID.";
                    return View(uploadFileDetails);
                }

                var uploadDetails = new UploadFileDetails
                {
                    systemid = uploadFileDetails.systemid,
                    filepath = filePath,
                    uploaddatetime = DateTime.Now.ToString(),
                    uploadEmployee = "70192",
                    VideoDate = uploadFileDetails.VideoDate,
                    VideoFromTime = uploadFileDetails.VideoFromTime,
                    VideoToTime = uploadFileDetails.VideoToTime
                };

                result = _domainServices.uploaddatainserted(uploadDetails);
                writeErrorMessage(result.ToString(), "Video File Mapping inserted");

                if (result > 0)
                {
                    ViewBag.Message = "Video mapping saved successfully.";
                }
                else
                {
                    ViewBag.Message = "Failed to save video mapping.";
                }
            }
            catch (Exception ex)
            {
                writeErrorMessage(ex.Message, "MasterDetails POST");
                ViewBag.Message = ex.Message;
            }

            // Reload dropdowns and mappings
            objupload.lstapprovecustomers = _domainServices.approvedGetCustomer();
            objupload.lstapprovefgnames = new List<SelectListItem>();
            objupload.lstFile = new List<SelectListItem>();
            objupload.lstSystem = _domainServices.getSystemNames();
            objupload.lstFileMappings = _domainServices.getFileMappingDetails();

            return View(objupload);
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

