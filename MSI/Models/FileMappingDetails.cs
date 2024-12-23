namespace MSI.Models
{
    public class FileMappingDetails
    {
        public int systemid { get; set; }
        public string systemname { get; set; }
        public string filepath { get; set; }

        public string filename { get; set; }

        public TimeOnly FromTime { get; set; }

        public TimeOnly ToTime { get; set; }

        public DateOnly Date { get; set; }
    }

}
