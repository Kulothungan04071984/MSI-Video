using Microsoft.AspNetCore.Mvc.Rendering;

namespace MSI.Models
{
    public class Fileuploaddetails
    {
        public int customid { get; set; }
        public string customerName { get; set; }
        public string FgName {  get; set; }
        public int fgid { get; set; }
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
        public IEnumerable<SelectListItem>lstcustomers { get; set; }
        public IEnumerable<SelectListItem> lstfgnames {  get; set; }

    }
}
