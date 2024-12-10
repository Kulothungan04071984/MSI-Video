namespace MSI.Models
{
    interface iUploadVideos
    {
        int uploaddatainserted(UploadFileDetails objFileDetails);
        List<string> GetAllConnectedSystemNames();
        string GetDomainSid();
       
    }

  
}
