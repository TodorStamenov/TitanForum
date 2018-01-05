namespace TitaniumForum.Services.Implementations
{
    using Data;
    using Infrastructure.Extensions;
    using Models.Questions;
    using System.Collections.Generic;
    using System.Linq;

    public class QuestionService : IQuestionService
    {
        private readonly UnitOfWork db;

        public QuestionService(UnitOfWork db)
        {
            this.db = db;
        }

        public int Total(string search)
        {
            return this.db
                .Questions
                .AllEntries()
                .Where(q => !q.IsDeleted)
                .Filter(search)
                .Count();
        }

        public int TotalByCategory(int id)
        {
            return this.db
                .Questions
                .Where(q => q.SubCategory.CategoryId == id)
                .Where(q => !q.IsDeleted)
                .Count();
        }

        public int TotalBySubCategory(int id)
        {
            return this.db
                .Questions
                .Where(q => q.SubCategoryId == id)
                .Where(q => !q.IsDeleted)
                .Count();
        }

        public IEnumerable<ListQuestionsServiceModel> ByCategory(int page, int pageSize, int categoryId)
        {
            return this.db
                 .Questions
                 .AllEntries()
                 .Where(q => !q.IsDeleted)
                 .Where(q => q.SubCategory.CategoryId == categoryId)
                 .OrderByDescending(q => q.DateAdded)
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .ProjectToListModel()
                 .ToList();
        }

        public IEnumerable<ListQuestionsServiceModel> BySubCategory(int page, int pageSize, int subCategoryId)
        {
            return this.db
                .Questions
                .AllEntries()
                .Where(q => !q.IsDeleted)
                .Where(q => q.SubCategoryId == subCategoryId)
                .OrderByDescending(q => q.DateAdded)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectToListModel()
                .ToList();
        }

        public IEnumerable<ListQuestionsServiceModel> All(int page, int pageSize, string search)
        {
            return this.db
                .Questions
                .AllEntries()
                .Where(q => !q.IsDeleted)
                .Filter(search)
                .OrderByDescending(q => q.DateAdded)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectToListModel()
                .ToList();
        }
    }
}