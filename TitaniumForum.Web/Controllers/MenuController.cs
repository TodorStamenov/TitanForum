namespace TitaniumForum.Web.Controllers
{
    using Services.Areas.Moderator;
    using Services.Models.Categories;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class MenuController : Controller
    {
        private readonly ICategoryService categoryService;

        public MenuController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public ActionResult Invoke(int? categoryId, int? subCategoryId)
        {
            IEnumerable<MenuCategoryServiceModel> model = this.categoryService.GetMenu();

            ViewBag.CategoryId = categoryId;
            ViewBag.SubCategoryId = subCategoryId;

            return PartialView("_CategoryMenu", model);
        }
    }
}