using Microsoft.AspNetCore.Mvc.Rendering;

namespace MSI.Models
{
    public class FileApprovedData
    {
        public int approvecustomid { get; set; }

        public int approvefgid { get; set; }

        public int approvedcustname { get; set; }

        public int approvedfgname { get; set; }

        public string CustomerName { get; set; }

        public string FgNo { get; set; }

        public string DocumentName { get; set; }

        public string DocumentStatus { get; set; }

        public List<DataAprovel> lstapprovedata { get; set; }
        public IEnumerable<SelectListItem> lstapprovecustomers { get; set; }
        public IEnumerable<SelectListItem> lstapprovefgnames { get; set; }

    }
}
