namespace TitaniumForum.Web.Controllers
{
    using Infrastructure.Helpers;
    using Models.Questions;
    using Services;
    using System.Web.Mvc;

    public class QuestionsController : BaseController
    {
        private const int QuestionsPerPage = 10;

        private readonly IQuestionService questionService;

        public QuestionsController(IQuestionService questionService)
        {
            this.questionService = questionService;
        }

        public ActionResult ByCategory(int? id, int? page)
        {
            if (id == null)
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
            if (id == null)
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