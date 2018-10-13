namespace TitaniumForum.Services.Areas.Moderator.Implementations
{
    using Data.Contracts;
    using Data.Models;
    using Infrastructure.Extensions;
    using Models.Questions;
    using Models.SubCategories;
    using Services.Implementations;
    using Services.Models.Questions;
    using System.Collections.Generic;
    using System.Linq;

    public class ModeratorQuestionService : Service, IModeratorQuestionService
    {
        public ModeratorQuestionService(IDatabase database)
            : base(database)
        {
        }

        public int DeletedCount(string search)
        {
            return this.Database
                .Questions
                .Count(q => q.IsDeleted
                    && !string.IsNullOrEmpty(search)
                        ? (q.Title.ToLower().Contains(search.ToLower())
                            || q.Content.ToLower().Contains(search.ToLower())
                            || q.Tags.Any(t => t.Tag.Name.ToLower().Contains(search.ToLower())))
                        : true);
        }

        public bool Delete(int id)
        {
            Question question = this.Database.Questions.Find(id);

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

            this.Database.Save();

            return true;
        }

        public bool Restore(int id)
        {
            Question question = this.Database.Questions.Find(id);
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

            this.Database.Save();

            return true;
        }

        public bool Lock(int id)
        {
            Question question = this.Database.Questions.Find(id);

            if (question == null
                || question.IsLocked
                || question.IsDeleted)
            {
                return false;
            }

            question.IsLocked = true;
            question.IsReported = false;

            this.Database.Save();

            return true;
        }

        public bool Unlock(int id)
        {
            Question question = this.Database.Questions.Find(id);

            if (question == null
                || !question.IsLocked
                || question.IsDeleted)
            {
                return false;
            }

            question.IsLocked = false;
            question.IsReported = false;

            this.Database.Save();

            return true;
        }

        public bool Conceal(int id)
        {
            Question question = this.Database.Questions.Find(id);

            if (question == null
                || !question.IsReported)
            {
                return false;
            }

            question.IsReported = false;

            this.Database.Save();

            return true;
        }

        private SubCategoryInfoServiceModel GetSubCategoryInfo(int subCategoryId)
        {
            return this.Database
                .SubCategories
                .ProjectSingle(
                    projection: c => new SubCategoryInfoServiceModel { IsDeleted = c.IsDeleted },
                    filter: c => c.Id == subCategoryId);
        }

        public IEnumerable<ListQuestionsServiceModel> Reported(int questionsCount)
        {
            return this.Database
                .Questions
                .Get(
                    filter: q => q.IsReported,
                    orderBy: q => q.OrderByDescending(question => question.DateAdded),
                    take: questionsCount)
                .ProjectToListModel()
                .ToList();
        }

        public IEnumerable<ListDeletedQuestionsServiceModel> Deleted(int page, int pageSize, string search)
        {
            return this.Database
                .Questions
                .Get(
                    filter: q => q.IsDeleted
                        && (!string.IsNullOrEmpty(search)
                            ? (q.Title.ToLower().Contains(search.ToLower())
                                || q.Content.ToLower().Contains(search.ToLower())
                                || q.Tags.Any(t => t.Tag.Name.ToLower().Contains(search.ToLower())))
                            : true),
                    orderBy: e => e.OrderByDescending(q => q.DateAdded),
                    skip: (page - 1) * pageSize,
                    take: pageSize)
                .ProjectToDeletedListModel()
                .ToList();
        }
    }
}