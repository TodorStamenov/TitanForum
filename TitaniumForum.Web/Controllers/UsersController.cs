namespace TitaniumForum.Web.Controllers
{
    using Infrastructure.Helpers;
    using Models.Users;
    using Services;
    using System.Web.Mvc;

    public class UsersController : BaseController
    {
        private const int UsersPerPage = 6;

        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        public ActionResult Ranking(int? page)
        {
            if (page == null || page < 1)
            {
                page = 1;
            }

            int totalUsers = this.userService.Total();

            ListUserRankingViewModel model = new ListUserRankingViewModel
            {
                CurrentPage = (int)page,
                TotalPages = ControllerHelpers.GetTotalPages(totalUsers, UsersPerPage),
                Users = this.userService.Ranking((int)page, UsersPerPage)
            };

            return View(model);
        }
    }
}