using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MSI.Models
{
    public class Systemid
    {

        public int addsystemustomid { get; set; }

        public int addsystemfgid { get; set; }

        public int addsystemcustname { get; set; }

        public int addsystemfgname { get; set; }
        public string Id { get; set; }

        public string SystemId { get; set; }
        public string cutomerName { get; set; }
        public string fgNo { get; set; }

        public string Usertype { get; set; } 
        
        public string Updatesystemid { get; set; }  

        public string Updateusertype    { get; set; }

        public string StageName { get; set; }  
        
        public string UpdateStageName    { get; set; }

        public List<Systemid> lstaddsystem { get; set; }

        public IEnumerable<SelectListItem> lstcustomname { get; set; }
        public IEnumerable<SelectListItem> lstfgno { get; set; }
    }

}