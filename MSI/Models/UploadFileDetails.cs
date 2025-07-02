using Microsoft.AspNetCore.Mvc.Rendering;

namespace MSI.Models
{
    public class UploadFileDetails
    {
        public string systemname { get; set; }

        public string fileid { get; set; }

        public string filepath { get; set; }

        public string Filename { get; set; }

        public string uploaddatetime { get; set; }

        public string uploadEmployee {  get; set; }

        public int systemid { get; set; }

        public DateTime VideoDate { get; set; }

        public TimeSpan VideoFromTime { get; set; }

        public TimeSpan VideoToTime { get; set; }

        public List<SelectListItem> lstSystem { get; set; }

        public int approvecustomid { get; set; }

        public int approvefgid { get; set; }

        public int approvefileid { get; set; }

        public int approvedcustname { get; set; }
        

        public int approvedfgname { get; set; }

        public IEnumerable<SelectListItem> lstapprovecustomers { get; set; }

        public IEnumerable<SelectListItem> lstapprovefgnames { get; set; }


        public List<SelectListItem> lstFile { get; set; }

        public List<FileMappingDetails> lstFileMappings { get; set; }
    }
}
