namespace TitaniumForum.Web.Areas.Moderator.Controllers
{
    using Data.Models;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Infrastructure.Filters;
    using Models.SubCategories;
    using Services.Areas.Moderator;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class SubCategoriesController : BaseModeratorController
    {
        private const string Categories = "Categories";
        private const string SubCategory = "Sub Category";
        private const string SubCategories = "SubCategries";

        private readonly ICategoryService categoryService;
        private readonly ISubCategoryService subCategoryService;

        public SubCategoriesController(ICategoryService categoryServive, ISubCategoryService subCategoryService)
        {
            this.categoryService = categoryServive;
            this.subCategoryService = subCategoryService;
        }

        public ActionResult Create()
        {
            SubCategoryFormViewModel model = new SubCategoryFormViewModel
            {
                Categories = this.GetCategories()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Log(LogType.Create, SubCategories)]
        public ActionResult Create(SubCategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = this.GetCategories();
                return View(model);
            }

            string name = model.SubCategory.Name;

            if (this.subCategoryService.NameExists(name))
            {
                TempData.AddErrorMessage(string.Format(WebConstants.EntryExists, name));
                model.Categories = this.GetCategories();
                return View(model);
            }

            bool success = this.subCategoryService.Create(model.CategoryId, name);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(string.Format(
                WebConstants.SuccessfullEntityOperation,
                SubCategory,
                WebConstants.Added));

            return RedirectToAction(
                nameof(CategoriesController.All),
                Categories,
                new { area = WebConstants.ModeratorArea });
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            SubCategoryFormViewModel model = new SubCategoryFormViewModel
            {
                SubCategory = this.subCategoryService.GetForm(id.Value),
                Categories = this.GetCategories()
            };

            if (model.SubCategory == null)
            {
                return BadRequest();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Log(LogType.Edit, SubCategories)]
        public ActionResult Edit(int? id, SubCategoryFormViewModel model)
        {
            if (id == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                model.Categories = this.GetCategories();
                return View(model);
            }

            string oldName = this.subCategoryService.GetName(id.Value);

            if (oldName == null)
            {
                return BadRequest();
            }

            string newName = model.SubCategory.Name;

            if (this.subCategoryService.NameExists(newName)
                && oldName != newName)
            {
                model.Categories = this.GetCategories();
                TempData.AddErrorMessage(string.Format(WebConstants.EntryExists, newName));
                return View(model);
            }

            bool success = this.subCategoryService.Edit(id.Value, model.CategoryId, newName);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(
                string.Format(WebConstants.SuccessfullEntityOperation,
                SubCategory,
                WebConstants.Edited));

            return RedirectToAction(
                nameof(CategoriesController.All),
                Categories,
                new { area = WebConstants.ModeratorArea });
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            return View();
        }

        [HttpPost]
        [ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        [Log(LogType.Delete, SubCategories)]
        public ActionResult DeletePost(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            bool success = this.subCategoryService.Delete(id.Value);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(
                string.Format(WebConstants.SuccessfullEntityOperation,
                SubCategory,
                WebConstants.Deleted));

            return RedirectToAction(
                nameof(CategoriesController.All),
                Categories,
                new { area = WebConstants.ModeratorArea });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Log(LogType.Restore, SubCategories)]
        public ActionResult Restore(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            bool success = this.subCategoryService.Restore(id.Value);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(
                string.Format(WebConstants.SuccessfullEntityOperation,
                SubCategory,
                WebConstants.Restored));

            return RedirectToAction(
                nameof(CategoriesController.All),
                Categories,
                new { area = WebConstants.ModeratorArea });
        }

        private IEnumerable<SelectListItem> GetCategories()
        {
            return this.categoryService
                .All()
                .Where(c => !c.IsDeleted)
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
        }
    }
}