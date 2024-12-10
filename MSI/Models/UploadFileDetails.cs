using Microsoft.AspNetCore.Mvc.Rendering;

namespace MSI.Models
{
    public class UploadFileDetails
    {
        public string systemname { get; set; }

        public string filepath { get; set; }

        public string uploaddatetime { get; set; }

        public string uploadEmployee {  get; set; }

        public int systemid { get; set; }

        public List<SelectListItem> lstSystem { get; set; }

        public List<FileMappingDetails> lstFileMappings { get; set; }
    }
}
