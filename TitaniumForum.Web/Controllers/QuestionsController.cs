namespace TitaniumForum.Web.Controllers
{
    using Common;
    using Data.Models;
    using Infrastructure;
    using Infrastructure.Extensions;
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
            if (id == null)
            {
                return BadRequest();
            }

            int userId = User.Identity.GetUserId<int>();

            if (!this.questionService.CanEdit((int)id, userId)
                && !User.IsInRole(CommonConstants.ModeratorRole))
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

            if (!this.questionService.CanEdit((int)id, userId)
                && !User.IsInRole(CommonConstants.ModeratorRole))
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
                return HttpNotFound();
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
                TotalEntries = totalAnswers,
                EntriesPerPage = AnswersPerPage,
                Question = this.questionService.Details((int)id, userId),
                Answers = this.answerService.ByQuestion((int)id, userId, (int)page, AnswersPerPage),
                Tags = this.tagService.ByQuestion((int)id)
            };

            if (model.Question == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Index(int? page, string search, int? tagId, int? categoryId, int? subCategoryId)
        {
            if (page == null || page < 1)
            {
                page = 1;
            }

            ListQuestionsViewModel model = new ListQuestionsViewModel
            {
                CurrentPage = (int)page,
                EntriesPerPage = QuestionsPerPage
            };

            if (categoryId != null
                && this.categoryService.Exists((int)categoryId)
                && !this.categoryService.IsDeleted((int)categoryId)
                && this.categoryService.HasQuestions((int)categoryId))
            {
                model.CategoryId = categoryId;
                model.TotalEntries = this.questionService.TotalByCategory((int)categoryId);
                model.Questions = this.questionService.ByCategory((int)page, QuestionsPerPage, (int)categoryId);
            }
            else if (subCategoryId != null
                && this.subCategoryService.Exists((int)subCategoryId)
                && !this.subCategoryService.IsDeleted((int)subCategoryId)
                && this.subCategoryService.HasQuestions((int)subCategoryId))
            {
                model.SubCategoryId = subCategoryId;
                model.TotalEntries = this.questionService.TotalBySubCategory((int)subCategoryId);
                model.Questions = this.questionService.BySubCategory((int)page, QuestionsPerPage, (int)subCategoryId);
            }
            else if (tagId != null
                && this.tagService.Exists((int)tagId))
            {
                model.TagId = tagId;
                model.TotalEntries = this.questionService.TotalByTag((int)tagId);
                model.Questions = this.questionService.ByTag((int)page, QuestionsPerPage, (int)tagId);
            }
            else
            {
                model.TotalEntries = this.questionService.Total(search);
                model.Search = search;
                model.Questions = this.questionService.All((int)page, QuestionsPerPage, search);
            }

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