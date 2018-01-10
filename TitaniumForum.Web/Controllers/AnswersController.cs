namespace TitaniumForum.Web.Controllers
{
    using Common;
    using Data.Models;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Microsoft.AspNet.Identity;
    using Services;
    using Services.Models.Answers;
    using System;
    using System.Web.Mvc;

    [Authorize]
    public class AnswersController : BaseController
    {
        private const string Answer = "Answer";
        private const string Answers = "Answers";
        private const string Questions = "Questions";

        private readonly IAnswerService answerService;
        private readonly IQuestionService questionService;

        public AnswersController(IAnswerService answerService, IQuestionService questionService)
        {
            this.answerService = answerService;
            this.questionService = questionService;
        }

        public ActionResult Create(int? questionId, int? page)
        {
            if (questionId == null
                || this.questionService.IsLocked(questionId.Value))
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
                model.RedirectInfo.QuestionId.Value,
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
            if (id == null
                || questionId == null)
            {
                return BadRequest();
            }

            if (page == null || page < 1)
            {
                page = 1;
            }

            int userId = User.Identity.GetUserId<int>();

            bool canEdit = this.answerService.CanEdit(id.Value, userId);

            if (!canEdit && !User.IsInRole(CommonConstants.ModeratorRole))
            {
                return new HttpUnauthorizedResult();
            }

            AnswerFormServiceModel model = this.answerService.GetForm(id.Value);

            if (model == null)
            {
                return BadRequest();
            }

            model.RedirectInfo.Page = page;
            model.RedirectInfo.QuestionId = questionId;

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

            bool canEdit = this.answerService.CanEdit(id.Value, userId);

            if (!canEdit && !User.IsInRole(CommonConstants.ModeratorRole))
            {
                return new HttpUnauthorizedResult();
            }

            bool success = this.answerService.Edit(id.Value, model.Content);

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

            bool success = this.answerService.Vote(answerId.Value, userId, voteDirection);

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