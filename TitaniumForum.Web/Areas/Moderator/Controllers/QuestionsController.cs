namespace TitaniumForum.Web.Areas.Moderator.Controllers
{
    using Common;
    using Data.Models;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Infrastructure.Filters;
    using Services.Areas.Moderator;
    using System.Web.Mvc;

    public class QuestionsController : BaseModeratorController
    {
        private const string All = "All";
        private const string Details = "Details";
        private const string Question = "Question";
        private const string Questions = "Questions";

        private readonly IModeratorQuestionService questionService;

        public QuestionsController(IModeratorQuestionService questionService)
        {
            this.questionService = questionService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Log(LogType.Lock, Questions)]
        public ActionResult Lock(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            bool success = this.questionService.Lock((int)id);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(string.Format(
                WebConstants.SuccessfullEntityOperation,
                Question,
                WebConstants.Locked));

            return RedirectToAction(
                Details,
                Questions,
                new { id, area = string.Empty });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Log(LogType.Unlock, Questions)]
        public ActionResult Unlock(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            bool success = this.questionService.Unlock((int)id);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(string.Format(
                WebConstants.SuccessfullEntityOperation,
                Question,
                WebConstants.Unlocked));

            return RedirectToAction(
                Details,
                Questions,
                new { id, area = string.Empty });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Log(LogType.Conceal, Questions)]
        public ActionResult Conceal(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            bool success = this.questionService.Conceal((int)id);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(string.Format(
                WebConstants.SuccessfullEntityOperation,
                Question,
                WebConstants.Concealed));

            return RedirectToAction(
                All,
                Questions,
                new { id, area = string.Empty });
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
        [ValidateAntiForgeryToken]
        [Log(LogType.Delete, Questions)]
        [ActionName(nameof(Delete))]
        public ActionResult DeletePost(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            bool success = this.questionService.Delete((int)id);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(string.Format(
                WebConstants.SuccessfullEntityOperation,
                Question,
                WebConstants.Deleted));

            return RedirectToAction(
                All,
                Questions,
                new { area = string.Empty });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Log(LogType.Restore, Questions)]
        public ActionResult Restore(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            bool success = this.questionService.Restore((int)id);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(string.Format(
                WebConstants.SuccessfullEntityOperation,
                Question,
                WebConstants.Restored));

            return RedirectToAction(
                All,
                Questions,
                new { area = string.Empty });
        }
    }
}