namespace TitaniumForum.Web.Areas.Moderator.Controllers
{
    using Data.Models;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Infrastructure.Filters;
    using Models.Comments;
    using Services.Areas.Moderator;
    using Services.Models.Questions;
    using System.Web.Mvc;

    public class CommentsController : BaseModeratorController
    {
        private const int CommentsPerPage = 5;
        private const string Comment = "Comment";
        private const string Comments = "Comments";
        private const string Details = "Details";
        private const string Questions = "Questions";

        private readonly IModeratorCommentService commentService;

        public CommentsController(IModeratorCommentService commentService)
        {
            this.commentService = commentService;
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

            bool success = this.commentService.Delete(id.Value);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(string.Format(
                WebConstants.SuccessfullEntityOperation,
                Comment,
                WebConstants.Deleted));

            return RedirectToAction(
                Details,
                Questions,
                new { id = model.QuestionId, model.Page, area = string.Empty });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Log(LogType.Restore, Comments)]
        public ActionResult Restore(int? id, int? questionId)
        {
            if (id == null || questionId == null)
            {
                return BadRequest();
            }

            bool success = this.commentService.Restore(id.Value);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(string.Format(
                WebConstants.SuccessfullEntityOperation,
                Comment,
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

            int questionsCount = this.commentService.DeletedCount(search);

            ListCommetnsModeratorViewModel model = new ListCommetnsModeratorViewModel
            {
                CurrentPage = page.Value,
                Search = search,
                TotalEntries = questionsCount,
                EntriesPerPage = CommentsPerPage,
                Comments = this.commentService.Deleted(page.Value, CommentsPerPage, search)
            };

            return View(model);
        }
    }
}