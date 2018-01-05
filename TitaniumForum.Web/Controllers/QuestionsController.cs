namespace TitaniumForum.Web.Controllers
{
    using Infrastructure.Helpers;
    using Models.Questions;
    using Services;
    using Services.Areas.Moderator;
    using System.Web.Mvc;

    public class QuestionsController : BaseController
    {
        private const int QuestionsPerPage = 5;

        private readonly IQuestionService questionService;
        private readonly ICategoryService categoryService;
        private readonly ISubCategoryService subCategoryService;

        public QuestionsController(
            IQuestionService questionService,
            ICategoryService categoryService,
            ISubCategoryService subCategoryService)
        {
            this.questionService = questionService;
            this.categoryService = categoryService;
            this.subCategoryService = subCategoryService;
        }

        public ActionResult ByCategory(int? id, int? page)
        {
            if (id == null
                || !this.categoryService.Exists((int)id)
                || this.categoryService.IsDeleted((int)id))
            {
                return RedirectToAction(nameof(All));
            }

            if (page == null || page < 1)
            {
                page = 1;
            }

            int totalQuestions = this.questionService.TotalByCategory((int)id);

            ListQuestionsViewModel model = new ListQuestionsViewModel
            {
                CurrentPage = (int)page,
                TotalPages = ControllerHelpers.GetTotalPages(totalQuestions, QuestionsPerPage),
                Search = null,
                CategoryId = id,
                SubCategoryId = null,
                Questions = this.questionService.ByCategory((int)page, QuestionsPerPage, (int)id)
            };

            return View(model);
        }

        public ActionResult BySubCategory(int? id, int? page)
        {
            if (id == null
                || !this.subCategoryService.Exists((int)id)
                || this.subCategoryService.IsDeleted((int)id))
            {
                return RedirectToAction(nameof(All));
            }

            if (page == null || page < 1)
            {
                page = 1;
            }

            int totalQuestions = this.questionService.TotalBySubCategory((int)id);

            ListQuestionsViewModel model = new ListQuestionsViewModel
            {
                CurrentPage = (int)page,
                TotalPages = ControllerHelpers.GetTotalPages(totalQuestions, QuestionsPerPage),
                Search = null,
                CategoryId = null,
                SubCategoryId = id,
                Questions = this.questionService.BySubCategory((int)page, QuestionsPerPage, (int)id)
            };

            return View(model);
        }

        public ActionResult All(int? page, string search)
        {
            if (page == null || page < 1)
            {
                page = 1;
            }

            int totalQuestions = this.questionService.Total(search);

            ListQuestionsViewModel model = new ListQuestionsViewModel
            {
                CurrentPage = (int)page,
                TotalPages = ControllerHelpers.GetTotalPages(totalQuestions, QuestionsPerPage),
                Search = search,
                CategoryId = null,
                SubCategoryId = null,
                Questions = this.questionService.All((int)page, QuestionsPerPage, search)
            };

            return View(model);
        }
    }
}