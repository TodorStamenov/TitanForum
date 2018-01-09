namespace TitaniumForum.Web.Areas.Admin.Controllers
{
    using Common;
    using Data.Models;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Infrastructure.Filters;
    using Models.Logs;
    using Models.Users;
    using Services.Areas.Admin;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Web.Controllers;

    [RouteArea(WebConstants.AdminArea)]
    [Authorize(Roles = CommonConstants.AdminRole)]
    public class UsersController : BaseController
    {
        private const int UsersPerPage = 10;
        private const int LogsPerPage = 10;
        private const string UsersTable = "Users";

        private readonly IAdminUserService userService;
        private readonly ApplicationUserManager userManager;

        public UsersController(IAdminUserService userService, ApplicationUserManager userManager)
        {
            this.userService = userService;
            this.userManager = userManager;
        }

        public async Task<ActionResult> EditRoles(int id)
        {
            User user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                return BadRequest();
            }

            UserRoleEditViewModel model = new UserRoleEditViewModel
            {
                IsUserLocked = await userManager.IsLockedOutAsync(id),
                User = this.userService.Roles(id),
                Roles = this.userService.AllRoles()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRole(int userId, string roleName)
        {
            string username = this.userService.GetUsername(userId);

            if (username == null)
            {
                return BadRequest();
            }

            bool success = this.userService.AddToRole(userId, roleName);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(string.Format(WebConstants.UserAddedToRole, username, roleName));

            return RedirectToAction(nameof(EditRoles), new { id = userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveRole(int userId, string roleName)
        {
            string username = this.userService.GetUsername(userId);

            if (username == null)
            {
                return BadRequest();
            }

            bool success = this.userService.RemoveFromRole(userId, roleName);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(string.Format(WebConstants.UserRemovedFormRole, username, roleName));

            return RedirectToAction(nameof(EditRoles), new { id = userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Log(LogType.Lock, UsersTable)]
        public async Task<ActionResult> Lock(int userId)
        {
            User user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest();
            }

            if (await this.userManager.IsLockedOutAsync(userId))
            {
                return BadRequest();
            }

            await userManager.SetLockoutEnabledAsync(userId, true);
            await userManager.SetLockoutEndDateAsync(userId, DateTime.UtcNow.AddYears(10));

            TempData.AddSuccessMessage(string.Format(WebConstants.UserLocked, user.UserName));

            return RedirectToAction(nameof(EditRoles), new { id = userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Log(LogType.Unlock, UsersTable)]
        public async Task<ActionResult> Unlock(int userId)
        {
            User user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest();
            }

            if (!await this.userManager.IsLockedOutAsync(userId))
            {
                return BadRequest();
            }

            await userManager.SetLockoutEnabledAsync(userId, false);

            TempData.AddSuccessMessage(string.Format(WebConstants.UserUnlocked, user.UserName));

            return RedirectToAction(nameof(EditRoles), new { id = userId });
        }

        public ActionResult Logs(int? page, string search)
        {
            if (page == null || page <= 0)
            {
                page = 1;
            }

            search = search ?? string.Empty;

            int totalLogs = this.userService.Total(search);

            ListLogsViewModel model = new ListLogsViewModel
            {
                Search = search,
                CurrentPage = (int)page,
                TotalEntries = totalLogs,
                EntriesPerPage = LogsPerPage,
                Logs = this.userService.Logs((int)page, LogsPerPage, search),
            };

            return View(model);
        }

        public ActionResult Users(int? page, string userRole, string search)
        {
            if (page == null || page <= 0)
            {
                page = 1;
            }

            int totalUsers = this.userService.Total(userRole, search);

            ListUsersViewModel model = new ListUsersViewModel
            {
                Search = search,
                CurrentPage = (int)page,
                UserRole = userRole,
                TotalEntries = totalUsers,
                EntriesPerPage = UsersPerPage,
                Users = this.userService.All((int)page, userRole, search, UsersPerPage),
                Roles = this.userService.AllRoles(),
            };

            return View(model);
        }
    }
}