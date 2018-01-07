namespace TitaniumForum.Web.Areas.Moderator.Controllers
{
    using Data.Models;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Infrastructure.Filters;
    using Services.Areas.Moderator;
    using Services.Areas.Moderator.Models.Categories;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class CategoriesController : BaseModeratorController
    {
        private const string Category = "Category";
        private const string Categories = "Categries";

        private readonly ICategoryService categoryServive;

        public CategoriesController(ICategoryService categoryServive)
        {
            this.categoryServive = categoryServive;
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Log(LogType.Create, Categories)]
        public ActionResult Create(CategoryFormServiceModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string name = model.Name;

            if (this.categoryServive.NameExists(name))
            {
                TempData.AddErrorMessage(string.Format(WebConstants.EntryExists, name));
                return View(model);
            }

            bool success = this.categoryServive.Create(name);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(string.Format(
                WebConstants.SuccessfullEntityOperation,
                Category,
                WebConstants.Added));

            return RedirectToAction(nameof(All));
        }

        public ActionResult Edit(int id)
        {
            CategoryFormServiceModel model = this.categoryServive.GetForm(id);

            if (model == null)
            {
                return BadRequest();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Log(LogType.Edit, Categories)]
        public ActionResult Edit(int id, CategoryFormServiceModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string oldName = this.categoryServive.GetName(id);

            if (oldName == null)
            {
                return BadRequest();
            }

            string newName = model.Name;

            if (this.categoryServive.NameExists(newName)
                && oldName != newName)
            {
                TempData.AddErrorMessage(string.Format(WebConstants.EntryExists, newName));
                return View(model);
            }

            bool success = this.categoryServive.Edit(id, newName);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(string.Format(
                WebConstants.SuccessfullEntityOperation,
                Category,
                WebConstants.Edited));

            return RedirectToAction(nameof(All));
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        [ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        [Log(LogType.Delete, Categories)]
        public ActionResult DeletePost(int id)
        {
            bool success = this.categoryServive.Delete(id);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(string.Format(
                WebConstants.SuccessfullEntityOperation,
                Category,
                WebConstants.Deleted));

            return RedirectToAction(nameof(All));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Log(LogType.Restore, Categories)]
        public ActionResult Restore(int id)
        {
            bool success = this.categoryServive.Restore(id);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(string.Format(
                WebConstants.SuccessfullEntityOperation,
                Category,
                WebConstants.Restored));

            return RedirectToAction(nameof(All));
        }

        public ActionResult All()
        {
            IEnumerable<ListCategoriesServiceModel> model = this.categoryServive.All();

            return View(model);
        }
    }
}