namespace TitaniumForum.Web.Controllers
{
    using System.Net;
    using System.Web.Mvc;

    public class BaseController : Controller
    {
        protected HttpStatusCodeResult BadRequest()
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
    }
}