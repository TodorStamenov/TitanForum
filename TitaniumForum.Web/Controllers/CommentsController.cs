namespace TitaniumForum.Web.Controllers
{
    using Common;
    using Data.Models;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Infrastructure.Filters;
    using Microsoft.AspNet.Identity;
    using Services;
    using Services.Models.Comments;
    using System.Web.Mvc;
    using TitaniumForum.Services.Models.Questions;

    [Authorize]
    public class CommentsController : BaseController
    {
        private const string Comment = "Comment";
        private const string Comments = "Comments";
        private const string Questions = "Questions";

        private readonly ICommentService commentService;

        public CommentsController(ICommentService commentService)
        {
            this.commentService = commentService;
        }

        public ActionResult Create(int? answerId, int? questionId, int? page)
        {
            if (answerId == null || questionId == null)
            {
                return BadRequest();
            }

            if (page == null || page < 1)
            {
                page = 1;
            }

            CommentFormServiceModel model = new CommentFormServiceModel();
            model.AnswerId = (int)answerId;
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

            bool canEdit = this.commentService.CanEdit((int)id, userId);

            if (!canEdit && !User.IsInRole(CommonConstants.ModeratorRole))
            {
                return new HttpUnauthorizedResult();
            }

            CommentFormServiceModel model = this.commentService.GetForm((int)id);
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

            bool canEdit = this.commentService.CanEdit((int)id, userId);

            if (!canEdit && !User.IsInRole(CommonConstants.ModeratorRole))
            {
                return new HttpUnauthorizedResult();
            }

            bool success = this.commentService.Edit((int)id, model.Content);

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
        [Log(LogType.Delete, Comments)]
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

            bool success = this.commentService.Delete((int)id);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(string.Format(
                WebConstants.SuccessfullEntityOperation,
                Comment,
                WebConstants.Deleted));

            return RedirectToAction(
                nameof(QuestionsController.Details),
                Questions,
                new { id = model.QuestionId, model.Page });
        }
    }
}