using System.ComponentModel.DataAnnotations;

namespace MSI.Models
{
    public class Systemid
    {

        public string Id { get; set; }

        [Required]
        public string SystemId { get; set; }

        public string Usertype { get; set; } 
        
        public string Updatesystemid { get; set; }  

        public string Updateusertype    { get; set; }

        public string StageName { get; set; }  
        
        public string UpdateStageName    { get; set; }   

    
    
    }
    public class insert_stat
    {
        public int Insert_status { get; set; }

    }
    public class update_stat
    {
        public int Update_status { get; set; }
    }
}