﻿namespace TitaniumForum.Web.Controllers
{
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
                CurrentPage = page.Value,
                TotalEntries = totalUsers,
                EntriesPerPage = UsersPerPage,
                Users = this.userService.Ranking(page.Value, UsersPerPage)
            };

            return View(model);
        }
    }
}