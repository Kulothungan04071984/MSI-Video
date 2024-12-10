﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using MSI.Models;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;


namespace MSI.Controllers
{
    public class PlayVideoController : Controller
    {
          private DataManagementcs _domainServices;
        //private readonly string _videoDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/videos");
        //private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<PlayVideoController> _logger;

        public PlayVideoController(DataManagementcs domainServices, ILogger<PlayVideoController> logger,IWebHostEnvironment webHostEnvironment)
        {
            _domainServices = domainServices;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }
        
        public async Task<IActionResult> VideoPlaying()
        {
            writeErrorMessage("PlayVideoController", "VideoPlaying Enter");
            // Get the device name (machine name)
            string deviceName =await Process_systemname();
           // string deviceName = "STPLPC903";

            // Fetch file path using DataAccess
            string filePath1 = _domainServices.getfilepath(deviceName);
           // string filePath = $@"{filePath1}";
           
           

             string networkPath = $@"{filePath1}";

            string wwwrootPath = _webHostEnvironment.WebRootPath;
            writeErrorMessage(wwwrootPath, "WWW root path Enter");
            string videoFolderPath = Path.Combine(wwwrootPath, "videos");
            string dname = deviceName.Replace(".", "");
            writeErrorMessage(dname, "DeviceName");
            string videoFilePath = Path.Combine(videoFolderPath, dname + ".mp4");
            writeErrorMessage(videoFilePath, "VideoFilePath Enter");
            try
            {
                if (!Directory.Exists(videoFolderPath))
                {
                    Directory.CreateDirectory(videoFolderPath);
                }

                // Log the attempt to copy the file
                _logger.LogInformation($"Attempting to copy video from {networkPath} to {videoFilePath}");

                // Use file I/O to copy the file
                //if (Directory.Exists(videoFilePath))
                //{
                var npath=string.IsNullOrEmpty(networkPath) ? "\\\\192.168.1.188\\MSI_Videos\\uploads\\WIRE ROUTING.mp4" : networkPath;
                writeErrorMessage(networkPath, "Strat Copy the video File");
                await Task.Run(() => System.IO.File.Copy(npath, videoFilePath, true));
                //}
                writeErrorMessage(networkPath, "End Copy the video File");

                _logger.LogInformation($"Video copied successfully to {videoFilePath}");
                ViewBag.VideoFolderPath = dname + ".mp4";
                return View();
            }
            catch (Exception ex)
            {
                string errorMessage = $"An error occurred while copying the video: {ex.Message}";
                writeErrorMessage(ex.Message.ToString(), "End Copy the video File");
                _logger.LogError(ex, errorMessage);
                return StatusCode(500, errorMessage);
            }
        }

      

        // Error handling for the app
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Get the machine's system name
        private async Task<string> Process_systemname()
        {
            try
            {
                // Query to get system information (device name)
                //string machineName = Environment.MachineName;
                //Console.WriteLine("Machine Name: " + machineName);

                var clientIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                writeErrorMessage(clientIpAddress.ToString(), "Client IP Address");
                //string clientHostname = null;
                //if (!string.IsNullOrEmpty(clientIpAddress))
                //{

                //    //try
                //    //{
                //    //    var hostEntry = await GetHostNameAsync(clientIpAddress);
                //    //    if (hostEntry != null)
                //    //    {
                //    //        clientHostname = hostEntry.ToString();
                //    //        writeErrorMessage(clientHostname, "Client Host Name");
                //    //    }
                //    //    else
                //    //        writeErrorMessage(clientHostname, "ClientHostName Empty");

                //    //}
                //    //catch (Exception ex)
                //    //{
                //    //    // Handle exceptions (e.g., DNS lookup failure)
                //    //    clientHostname = "Hostname could not be resolved: " + ex.Message;
                //    //}
                //}
                return clientIpAddress;
            }
            catch (Exception ex)
            {
                writeErrorMessage(ex.Message.ToString(), "Process_systemname");
                // Console.WriteLine("Error retrieving data: " + me.Message);
                return string.Empty;
            }
        }

        private async Task<string> GetHostNameAsync(string ipAddress)
        {
            try
            {
                var ip = IPAddress.Parse(ipAddress);
                var hostEntry = await Dns.GetHostEntryAsync(ip);
                return hostEntry.HostName;
            }
            catch (SocketException)
            {
                return "Hostname could not be resolved";
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