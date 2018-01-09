namespace TitaniumForum.Services.Areas.Moderator.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Data;
    using Data.Models;
    using Infrastructure.Extensions;
    using Models.Questions;
    using Models.SubCategories;
    using Services.Models.Questions;
    using System.Collections.Generic;
    using System.Linq;

    public class ModeratorQuestionService : IModeratorQuestionService
    {
        private readonly UnitOfWork db;

        public ModeratorQuestionService(UnitOfWork db)
        {
            this.db = db;
        }

        public int DeletedCount(string search)
        {
            return this.db
                .Questions
                .AllEntries()
                .Where(q => q.IsDeleted)
                .Filter(search)
                .Count();
        }

        public bool Delete(int id)
        {
            Question question = this.db.Questions.Find(id);

            if (question == null
                || question.IsDeleted)
            {
                return false;
            }

            question.IsDeleted = true;

            foreach (var answer in question.Answers)
            {
                foreach (var comment in answer.Comments)
                {
                    comment.IsDeleted = true;
                }

                answer.IsDeleted = true;
            }

            this.db.Save();

            return true;
        }

        public bool Restore(int id)
        {
            Question question = this.db.Questions.Find(id);
            SubCategoryInfoServiceModel subCategoryInfo = this.GetSubCategoryInfo(question.SubCategoryId);

            if (question == null
                || !question.IsDeleted
                || subCategoryInfo == null
                || subCategoryInfo.IsDeleted)
            {
                return false;
            }

            question.IsDeleted = false;

            foreach (var answer in question.Answers)
            {
                foreach (var comment in answer.Comments)
                {
                    comment.IsDeleted = false;
                }

                answer.IsDeleted = false;
            }

            this.db.Save();

            return true;
        }

        public bool Lock(int id)
        {
            Question question = this.db.Questions.Find(id);

            if (question == null
                || question.IsLocked
                || question.IsDeleted)
            {
                return false;
            }

            question.IsLocked = true;
            question.IsReported = false;

            this.db.Save();

            return true;
        }

        public bool Unlock(int id)
        {
            Question question = this.db.Questions.Find(id);

            if (question == null
                || !question.IsLocked
                || question.IsDeleted)
            {
                return false;
            }

            question.IsLocked = false;
            question.IsReported = false;

            this.db.Save();

            return true;
        }

        public bool Conceal(int id)
        {
            Question question = this.db.Questions.Find(id);

            if (question == null
                || !question.IsReported)
            {
                return false;
            }

            question.IsReported = false;

            this.db.Save();

            return true;
        }

        private SubCategoryInfoServiceModel GetSubCategoryInfo(int subCategoryId)
        {
            return this.db
                .SubCategories
                .AllEntries()
                .Where(c => c.Id == subCategoryId)
                .ProjectTo<SubCategoryInfoServiceModel>()
                .FirstOrDefault();
        }

        public IEnumerable<ListQuestionsServiceModel> Reported(int questionsCount)
        {
            return this.db
                .Questions
                .AllEntries()
                .Where(q => q.IsReported)
                .OrderByDescending(q => q.DateAdded)
                .Take(questionsCount)
                .ProjectToListModel()
                .ToList();
        }

        public IEnumerable<ListDeletedQuestionsServiceModel> Deleted(int page, int pageSize, string search)
        {
            return this.db
                .Questions
                .AllEntries()
                .Where(q => q.IsDeleted)
                .Filter(search)
                .OrderByDescending(q => q.DateAdded)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectToDeletedListModel()
                .ToList();
        }
    }
}