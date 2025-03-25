namespace MSI.Models
{
    public class DocVerified
    {
        public int docId {  get; set; }
        public string filepath { get; set; }
        public int empId { get; set; }
        public string customer_name { get; set; }
        public string Fg_Name { get; set; }
        public string docName { get; set; }
        public string docDateTime { get; set; }
        public string docType { get; set; }
        public string docStatus { get; set; }
        public bool isActive { get; set; }
        public string Reject_reason { get; set; }


    }
}
