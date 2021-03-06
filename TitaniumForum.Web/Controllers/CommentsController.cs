﻿namespace TitaniumForum.Web.Controllers
{
    using Common;
    using Data.Models;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Microsoft.AspNet.Identity;
    using Services;
    using Services.Models.Comments;
    using System;
    using System.Web.Mvc;

    [Authorize]
    public class CommentsController : BaseController
    {
        private const string Comment = "Comment";
        private const string Comments = "Comments";
        private const string Questions = "Questions";

        private readonly ICommentService commentService;
        private readonly IQuestionService questionService;

        public CommentsController(ICommentService commentService, IQuestionService questionService)
        {
            this.commentService = commentService;
            this.questionService = questionService;
        }

        public ActionResult Create(int? answerId, int? questionId, int? page)
        {
            if (answerId == null
                || questionId == null
                || this.questionService.IsLocked(questionId.Value))
            {
                return BadRequest();
            }

            if (page == null || page < 1)
            {
                page = 1;
            }

            CommentFormServiceModel model = new CommentFormServiceModel();
            model.AnswerId = answerId.Value;
            model.RedirectInfo.Page = page;
            model.RedirectInfo.QuestionId = questionId;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CommentFormServiceModel model)
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

            bool success = this.commentService.Create(model.AnswerId, authorId, model.Content);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(string.Format(
                WebConstants.SuccessfullEntityOperation,
                Comment,
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

            if (!this.commentService.CanEdit(id.Value, userId)
                && !User.IsInRole(CommonConstants.ModeratorRole))
            {
                return new HttpUnauthorizedResult();
            }

            CommentFormServiceModel model = this.commentService.GetForm(id.Value);

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
        public ActionResult Edit(int? id, CommentFormServiceModel model)
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

            if (!this.commentService.CanEdit(id.Value, userId)
                && !User.IsInRole(CommonConstants.ModeratorRole))
            {
                return new HttpUnauthorizedResult();
            }

            bool success = this.commentService.Edit(id.Value, model.Content);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(string.Format(
                WebConstants.SuccessfullEntityOperation,
                Comment,
                WebConstants.Edited));

            return RedirectToAction(
                nameof(QuestionsController.Details),
                Questions,
                new { id = model.RedirectInfo.QuestionId, model.RedirectInfo.Page });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Vote(int? id, int? commentId, int? page, string direction)
        {
            if (id == null
                || commentId == null
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

            bool success = this.commentService.Vote(commentId.Value, userId, voteDirection);

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