using Microsoft.AspNetCore.Mvc.Rendering;

namespace MSI.Models
{
    public class Fileuploaddetails
    {
        public int docId { get; set; }
        public string empId { get; set; }
        public string filepath { get; set; }
        public string empName { get; set; }
        public string docName { get; set; }
        public string docDateTime { get; set; }
        public string docType { get; set; }
        public string docStatus { get; set; }
        public bool isActive { get; set; }
        public string Reject_reason { get; set; }
        public List<DocVerified> lstdocVerifieds {  get; set; }
        public List<SelectListItem>lstcustomers { get; set; }
        public List<SelectListItem> lstfgnames {  get; set; }

    }
}
