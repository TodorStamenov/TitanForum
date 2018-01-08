namespace TitaniumForum.Web.Controllers
{
    using Common;
    using Data.Models;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Infrastructure.Filters;
    using Infrastructure.Helpers;
    using Microsoft.AspNet.Identity;
    using Models.Questions;
    using Services;
    using Services.Areas.Moderator;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    [Authorize]
    public class QuestionsController : BaseController
    {
        private const int AnswersPerPage = 5;
        private const int QuestionsPerPage = 5;
        private const string Question = "Question";
        private const string Questions = "Questions";

        private readonly IAnswerService answerService;
        private readonly ICategoryService categoryService;
        private readonly IQuestionService questionService;
        private readonly ISubCategoryService subCategoryService;
        private readonly ITagService tagService;

        public QuestionsController(
            IAnswerService answerService,
            ICategoryService categoryService,
            IQuestionService questionService,
            ISubCategoryService subCategoryService,
            ITagService tagService)
        {
            this.answerService = answerService;
            this.categoryService = categoryService;
            this.questionService = questionService;
            this.subCategoryService = subCategoryService;
            this.tagService = tagService;
        }

        public ActionResult Create()
        {
            QuestionFormViewModel model = new QuestionFormViewModel
            {
                SubCategories = this.GetSubCategories()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(QuestionFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.SubCategories = this.GetSubCategories();
                return View(model);
            }

            string title = model.Question.Title;

            if (this.questionService.TitleExists(title))
            {
                model.SubCategories = this.GetSubCategories();
                TempData.AddErrorMessage(string.Format(WebConstants.EntryExists, Question));
                return View(model);
            }

            int authorId = User.Identity.GetUserId<int>();

            int id = this.questionService.Create(
                authorId,
                title,
                model.Question.Content,
                model.Question.Tags,
                model.SubCategoryId);

            if (id < 1)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(string.Format(
                WebConstants.SuccessfullEntityOperation,
                Question,
                WebConstants.Added));

            return RedirectToAction(nameof(Details), new { id });
        }

        public ActionResult Edit(int? id)
        {
            if (id == null
                || this.questionService.IsLocked((int)id))
            {
                return BadRequest();
            }

            int userId = User.Identity.GetUserId<int>();

            bool canEdit = this.questionService.CanEdit((int)id, userId);

            if (!canEdit && !User.IsInRole(CommonConstants.ModeratorRole))
            {
                return new HttpUnauthorizedResult();
            }

            QuestionFormViewModel model = new QuestionFormViewModel
            {
                Question = this.questionService.GetForm((int)id),
                SubCategories = this.GetSubCategories()
            };

            if (model.Question == null)
            {
                return BadRequest();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, QuestionFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.SubCategories = this.GetSubCategories();
                return View(model);
            }

            int userId = User.Identity.GetUserId<int>();

            bool canEdit = this.questionService.CanEdit((int)id, userId);

            if (!canEdit && !User.IsInRole(CommonConstants.ModeratorRole))
            {
                return new HttpUnauthorizedResult();
            }

            string oldTitle = this.questionService.GetTitle((int)id);

            if (oldTitle == null)
            {
                return BadRequest();
            }

            string newTitle = model.Question.Title;

            if (this.questionService.TitleExists(newTitle)
                && newTitle != oldTitle)
            {
                model.SubCategories = this.GetSubCategories();
                TempData.AddErrorMessage(string.Format(WebConstants.EntryExists, Question));
                return View(model);
            }

            bool success = this.questionService.Edit(
                (int)id,
                newTitle,
                model.Question.Content,
                model.Question.Tags,
                model.SubCategoryId);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(string.Format(
                WebConstants.SuccessfullEntityOperation,
                Question,
                WebConstants.Edited));

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Report(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            bool success = this.questionService.Report((int)id);

            if (!success)
            {
                return BadRequest();
            }

            TempData.AddSuccessMessage(string.Format(
                WebConstants.SuccessfullEntityOperation,
                Question,
                WebConstants.Reported));

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Vote(int? id, int? page, string direction)
        {
            if (id == null || direction == null)
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

            bool success = this.questionService.Vote((int)id, userId, voteDirection);

            if (!success)
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(Details), new { id, page });
        }

        [AllowAnonymous]
        public ActionResult Details(int? id, int? page)
        {
            if (id == null)
            {
                return BadRequest();
            }

            if (page == null || page < 1)
            {
                page = 1;
            }

            int totalAnswers = this.answerService.TotalByQuestion((int)id);

            int userId = User.Identity.GetUserId<int>();

            QuestionDetailsViewModel model = new QuestionDetailsViewModel
            {
                CurrentPage = (int)page,
                TotalPages = ControllerHelpers.GetTotalPages(totalAnswers, AnswersPerPage),
                Question = this.questionService.Details((int)id, userId),
                Answers = this.answerService.ByQuestion((int)id, userId, (int)page, AnswersPerPage),
                Tags = this.tagService.ByQuestion((int)id)
            };

            if (model.Question == null)
            {
                return BadRequest();
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ByCategory(int? id, int? page)
        {
            if (id == null
                || !this.categoryService.Exists((int)id)
                || this.categoryService.IsDeleted((int)id)
                || !this.categoryService.HasQuestions((int)id))
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
                TagId = null,
                Questions = this.questionService.ByCategory((int)page, QuestionsPerPage, (int)id)
            };

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult BySubCategory(int? id, int? page)
        {
            if (id == null
                || !this.subCategoryService.Exists((int)id)
                || this.subCategoryService.IsDeleted((int)id)
                || !this.subCategoryService.HasQuestions((int)id))
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
                TagId = null,
                Questions = this.questionService.BySubCategory((int)page, QuestionsPerPage, (int)id)
            };

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ByTag(int? id, int? page)
        {
            if (id == null
               || !this.tagService.Exists((int)id))
            {
                return RedirectToAction(nameof(All));
            }

            if (page == null || page < 1)
            {
                page = 1;
            }

            int totalQuestions = this.questionService.TotalByTag((int)id);

            ListQuestionsViewModel model = new ListQuestionsViewModel
            {
                CurrentPage = (int)page,
                TotalPages = ControllerHelpers.GetTotalPages(totalQuestions, QuestionsPerPage),
                Search = null,
                CategoryId = null,
                SubCategoryId = null,
                TagId = (int)id,
                Questions = this.questionService.ByTag((int)page, QuestionsPerPage, (int)id)
            };

            return View(model);
        }

        [AllowAnonymous]
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
                TagId = null,
                Questions = this.questionService.All((int)page, QuestionsPerPage, search)
            };

            return View(model);
        }

        private IEnumerable<SelectListItem> GetSubCategories()
        {
            return this.subCategoryService
                .GetMenu()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
        }
    }
}