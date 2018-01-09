namespace TitaniumForum.Web.Areas.Moderator.Controllers
{
    using Data.Models;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Infrastructure.Filters;
    using Models.Answers;
    using Services.Areas.Moderator;
    using Services.Models.Questions;
    using System.Web.Mvc;

    public class AnswersController : BaseModeratorController
    {
        private const int AnswersPerPage = 5;
        private const string Answer = "Answer";
        private const string Answers = "Answers";
        private const string Details = "Details";
        private const string Questions = "Questions";

        private readonly IModeratorAnswerService answerService;

        public AnswersController(IModeratorAnswerService answerService)
        {
            this.answerService = answerService;
        }

        public ActionResult Delete(int? id, int? questionId, int? page)
        {
            if (id == null || questionId == null)
            {
                return BadRequest();
            }

            if (page == null || page < 1)
            {
                page = 1;
            }

            QuestionRedirectServiceModel model = new QuestionRedirectServiceModel();

            model.Page = page;
            model.QuestionId = questionId;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        [Log(LogType.Delete, Answers)]
        public ActionResult DeletePost(int? id, QuestionRedirectServiceModel model)
        {
            if (id == null || model.QuestionId == null)
            {
                return BadRequest();
            }

            if (model.Page == null || model.Page < 1)
            {
                model.Page = 1;
            }

            bool success = this.answerService.Delete((int)id);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(string.Format(
                WebConstants.SuccessfullEntityOperation,
                Answer,
                WebConstants.Deleted));

            return RedirectToAction(
                Details,
                Questions,
                new { id = model.QuestionId, model.Page, area = string.Empty });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Log(LogType.Restore, Answers)]
        public ActionResult Restore(int? id, int? questionId)
        {
            if (id == null || questionId == null)
            {
                return BadRequest();
            }

            bool success = this.answerService.Restore((int)id);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(string.Format(
                WebConstants.SuccessfullEntityOperation,
                Answer,
                WebConstants.Restored));

            return RedirectToAction(
                Details,
                Questions,
                new { id = questionId, area = string.Empty });
        }

        public ActionResult Deleted(int? page, string search)
        {
            if (page == null || page < 1)
            {
                page = 1;
            }

            int questionsCount = this.answerService.DeletedCount(search);

            ListAnswersModeratorViewModel model = new ListAnswersModeratorViewModel
            {
                CurrentPage = (int)page,
                Search = search,
                TotalEntries = questionsCount,
                EntriesPerPage = AnswersPerPage,
                Answers = this.answerService.Deleted((int)page, AnswersPerPage, search)
            };

            return View(model);
        }
    }
}