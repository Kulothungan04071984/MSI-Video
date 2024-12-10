namespace MSI.Models
{
    public class MSI_Wo_Display_transactions
    {

      public int  system_id { get; set; }
      public string  File_Path { get; set; }
      public string  Upload_time { get; set; }
      public int user_id { get; set; }
      public int isActive { get; set; }

    }

    public class MSI_Wo_Display
    {

        public int system_id { get; set; }
        public string system_name { get; set; }
        public int isActive { get; set; }

    }


}
