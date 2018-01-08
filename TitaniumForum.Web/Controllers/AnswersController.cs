namespace TitaniumForum.Web.Controllers
{
    using Common;
    using Data.Models;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Infrastructure.Filters;
    using Microsoft.AspNet.Identity;
    using Services;
    using Services.Models.Answers;
    using Services.Models.Questions;
    using System;
    using System.Web.Mvc;

    [Authorize]
    public class AnswersController : BaseController
    {
        private const string Answer = "Answer";
        private const string Answers = "Answers";
        private const string Questions = "Questions";

        private readonly IAnswerService answerService;

        public AnswersController(IAnswerService answerService)
        {
            this.answerService = answerService;
        }

        public ActionResult Create(int? questionId, int? page)
        {
            if (questionId == null)
            {
                return BadRequest();
            }

            if (page == null || page < 1)
            {
                page = 1;
            }

            AnswerFormServiceModel model = new AnswerFormServiceModel();
            model.RedirectInfo.Page = page;
            model.RedirectInfo.QuestionId = questionId;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AnswerFormServiceModel model)
        {
            if (model.RedirectInfo.QuestionId == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.RedirectInfo.Page == null
                || model.RedirectInfo.Page < 1)
            {
                model.RedirectInfo.Page = 1;
            }

            int authorId = User.Identity.GetUserId<int>();

            bool success = this.answerService.Create(
                (int)model.RedirectInfo.QuestionId,
                authorId,
                model.Content);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(string.Format(
                WebConstants.SuccessfullEntityOperation,
                Answer,
                WebConstants.Added));

            return RedirectToAction(
                nameof(QuestionsController.Details),
                Questions,
                new { id = model.RedirectInfo.QuestionId, model.RedirectInfo.Page });
        }

        public ActionResult Edit(int? id, int? questionId, int? page)
        {
            if (id == null || questionId == null)
            {
                return BadRequest();
            }

            if (page == null || page < 1)
            {
                page = 1;
            }

            int userId = User.Identity.GetUserId<int>();

            bool canEdit = this.answerService.CanEdit((int)id, userId);

            if (!canEdit && !User.IsInRole(CommonConstants.ModeratorRole))
            {
                return new HttpUnauthorizedResult();
            }

            AnswerFormServiceModel model = this.answerService.GetForm((int)id);
            model.RedirectInfo.Page = page;
            model.RedirectInfo.QuestionId = questionId;

            if (model == null)
            {
                return BadRequest();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, AnswerFormServiceModel model)
        {
            if (id == null || model.RedirectInfo.QuestionId == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.RedirectInfo.Page == null
                || model.RedirectInfo.Page < 1)
            {
                model.RedirectInfo.Page = 1;
            }

            int userId = User.Identity.GetUserId<int>();

            bool canEdit = this.answerService.CanEdit((int)id, userId);

            if (!canEdit && !User.IsInRole(CommonConstants.ModeratorRole))
            {
                return new HttpUnauthorizedResult();
            }

            bool success = this.answerService.Edit((int)id, model.Content);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(string.Format(
                WebConstants.SuccessfullEntityOperation,
                Answer,
                WebConstants.Edited));

            return RedirectToAction(
                nameof(QuestionsController.Details),
                Questions,
                new { id = model.RedirectInfo.QuestionId, model.RedirectInfo.Page });
        }

        [Authorize(Roles = CommonConstants.ModeratorRole)]
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

            QuestionRedirectServiceModel model = new QuestionRedirectServiceModel
            {
                Page = page,
                QuestionId = questionId
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = CommonConstants.ModeratorRole)]
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
                nameof(QuestionsController.Details),
                Questions,
                new { id = model.QuestionId, model.Page });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Vote(int? id, int? answerId, int? page, string direction)
        {
            if (id == null
                || answerId == null
                || direction == null)
            {
                return BadRequest();
            }

            if (page == null || page < 1)
            {
                page = 1;
            }

            bool parsed = Enum.TryParse(direction, true, out Direction voteDirection);

            if (!parsed)
            {
                return BadRequest();
            }

            int userId = User.Identity.GetUserId<int>();

            bool success = this.answerService.Vote((int)answerId, userId, voteDirection);

            if (!success)
            {
                return BadRequest();
            }

            return RedirectToAction(
                nameof(QuestionsController.Details),
                Questions,
                new { id, page });
        }
    }
}