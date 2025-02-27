namespace MSI.Models
{
    public class DocVerified
    {
        public int docId {  get; set; }
        public string empId { get; set; }
        public string empName { get; set; }
        public string docName { get; set; }
        public string docDateTime { get; set; }
        public string docType { get; set; }
        public string docStatus { get; set; }
        public bool isActive { get; set; }
    }
}
