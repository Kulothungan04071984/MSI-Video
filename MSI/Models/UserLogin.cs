using Microsoft.AspNetCore.Mvc.Rendering;

namespace MSI.Models;
public partial class UserLogin
{
    public int Userloginid { get; set; }

    public string? Userid { get; set; }

    public string? Userpassword { get; set; }

    public string? Usertype { get; set; }

    public int? Createid { get; set; }

    public DateTime? Createddate { get; set; }

    public int? Updateid { get; set; }

    public DateTime? Updatedate { get; set; }

    public bool? Isactive { get; set; }
}