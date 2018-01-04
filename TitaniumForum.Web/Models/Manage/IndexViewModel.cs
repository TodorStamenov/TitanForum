namespace TitaniumForum.Web.Models.Manage
{
    using System.Web;

    public class IndexViewModel
    {
        public string ProfileImage { get; set; }

        public HttpPostedFileBase Image { get; set; }
    }
}