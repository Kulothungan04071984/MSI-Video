using System.ComponentModel.DataAnnotations;

namespace MSI.Models
{
    public class Systemid
    {

        public string Id { get; set; }

        [Required]
        public string SystemId { get; set; }

        public string Usertype { get; set; }    

    }
    public class insert_stat
    {
        public int Insert_status { get; set; }

    }
}