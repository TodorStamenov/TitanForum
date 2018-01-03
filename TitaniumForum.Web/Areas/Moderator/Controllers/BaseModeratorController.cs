namespace TitaniumForum.Web.Areas.Moderator.Controllers
{
    using Common;
    using Infrastructure;
    using System.Web.Mvc;
    using Web.Controllers;

    [RouteArea(WebConstants.ModeratorArea)]
    [Authorize(Roles = CommonConstants.ModeratorRole)]
    public class BaseModeratorController : BaseController
    {
    }
}