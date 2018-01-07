namespace TitaniumForum.Web.Controllers
{
    using Data;
    using Data.Models;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;
    using Models.Manage;
    using Services;
    using System;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;

    [Authorize]
    public class ManageController : BaseController
    {
        private ApplicationSignInManager signInManager;
        private ApplicationUserManager userManager;
        private readonly IUserService userService;

        public ManageController()
        {
        }

        public ManageController(
            ApplicationUserManager userManager,
            ApplicationSignInManager signInManager,
            IUserService userService)
        {
            this.UserManager = userManager;
            this.SignInManager = signInManager;
            this.userService = userService;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return this.signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }

            private set
            {
                this.signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return this.userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }

            private set
            {
                this.userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";

            User user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());

            IndexViewModel model = new IndexViewModel
            {
                ProfileImage = this.ConvertUserImage(user.ProfileImage)
            };

            return View(model);
        }

        //
        // GET: /Manage/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(IndexViewModel model)
        {
            int userId = User.Identity.GetUserId<int>();

            bool success = true;

            if (model.Image != null)
            {
                if (!model.Image.ContentType.Contains("image")
                        || model.Image.ContentLength > DataConstants.UserConstants.MaxProfileImageSize)
                {
                    TempData.AddErrorMessage("Uploaded image file must be less then 100 KB");
                    return RedirectToAction(nameof(Index));
                }

                success = this.userService.AddProfileImage(userId, model.Image.ToByteArray());

                if (!success)
                {
                    return BadRequest();
                }
            }

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage("Your profile has been updated");
            return RedirectToAction(nameof(Index));
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId<int>(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && userManager != null)
            {
                userManager.Dispose();
                userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private string ConvertUserImage(byte[] profileImage)
        {
            return WebConstants.DataImage + Convert.ToBase64String(profileImage);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            Error
        }

        #endregion Helpers
    }
}