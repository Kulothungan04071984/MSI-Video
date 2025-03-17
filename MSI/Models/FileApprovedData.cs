<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Mvc.Rendering;

namespace MSI.Models
{
    public class FileApprovedData
    {
        public int approvecustomid { get; set; }

        public int approvefgid { get; set; }

=======
﻿namespace MSI.Models
{
    public class FileApprovedData
    {
>>>>>>> 90074adabb6fbd97863e312541321423ab235aa8
        public string CustomerName { get; set; }

        public string FgNo { get; set; }

<<<<<<< HEAD
        public string DocumentName { get; set; }

        public string DocumentStatus { get; set; }

        public List<DataAprovel> lstapprovedata { get; set; }
        public IEnumerable<SelectListItem> lstapprovecustomers { get; set; }
        public IEnumerable<SelectListItem> lstapprovefgnames { get; set; }
=======
        public string DocumentName { get; set;}

        public string DocumentStatus { get; set;}
>>>>>>> 90074adabb6fbd97863e312541321423ab235aa8

    }
}
