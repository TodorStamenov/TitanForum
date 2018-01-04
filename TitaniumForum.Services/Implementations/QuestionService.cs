namespace TitaniumForum.Services.Implementations
{
    using Data;
    using Data.Models;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Models.Questions;
    using System;
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
                .Filter(search)
                .Count();
        }

        public int TotalByCategory(int id)
        {
            return this.db
                .Questions
                .Count(q => q.SubCategory.CategoryId == id);
        }

        public int TotalBySubCategory(int id)
        {
            return this.db
                .Questions
                .Count(q => q.SubCategoryId == id);
        }

        public IEnumerable<ListQuestionsServiceModel> ByCategory(int page, int pageSize, int categoryId)
        {
            return this.db
                 .Questions
                 .AllEntries()
                 .Where(q => !q.IsDeleted)
                 .Where(q => !q.SubCategory.IsDeleted)
                 .Where(q => q.SubCategory.CategoryId == categoryId)
                 .OrderByDescending(q => q.DateAdded)
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .AsEnumerable()
                 .Select(q => new ListQuestionsServiceModel
                 {
                     Id = q.Id,
                     Title = q.Title,
                     DateAdded = q.DateAdded.ToLocalTime(),
                     AnswersCount = q.Answers.Count,
                     SubCategoryName = q.SubCategory.Name,
                     AuthorUsername = q.Author.UserName,
                     AuthorProfileImage = ServiceConstants.DataImage + Convert.ToBase64String(q.Author.ProfileImage),
                     ViewCount = q.ViewCount,
                     UpVotes = q.Votes.Count(v => v.Direction == Direction.Like),
                     DownVotes = q.Votes.Count(v => v.Direction == Direction.Dislike)
                 })
                 .ToList();
        }

        public IEnumerable<ListQuestionsServiceModel> BySubCategory(int page, int pageSize, int subCategoryId)
        {
            return this.db
                .Questions
                .AllEntries()
                .Where(q => !q.IsDeleted)
                .Where(q => !q.SubCategory.IsDeleted)
                .Where(q => q.SubCategoryId == subCategoryId)
                .OrderByDescending(q => q.DateAdded)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsEnumerable()
                .Select(q => new ListQuestionsServiceModel
                {
                    Id = q.Id,
                    Title = q.Title,
                    DateAdded = q.DateAdded.ToLocalTime(),
                    AnswersCount = q.Answers.Count,
                    SubCategoryName = q.SubCategory.Name,
                    AuthorUsername = q.Author.UserName,
                    AuthorProfileImage = ServiceConstants.DataImage + Convert.ToBase64String(q.Author.ProfileImage),
                    ViewCount = q.ViewCount,
                    UpVotes = q.Votes.Count(v => v.Direction == Direction.Like),
                    DownVotes = q.Votes.Count(v => v.Direction == Direction.Dislike)
                })
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
                .AsEnumerable()
                .Select(q => new ListQuestionsServiceModel
                {
                    Id = q.Id,
                    Title = q.Title,
                    DateAdded = q.DateAdded.ToLocalTime(),
                    AnswersCount = q.Answers.Count,
                    SubCategoryName = q.SubCategory.Name,
                    AuthorUsername = q.Author.UserName,
                    AuthorProfileImage = ServiceConstants.DataImage + Convert.ToBase64String(q.Author.ProfileImage),
                    ViewCount = q.ViewCount,
                    UpVotes = q.Votes.Count(v => v.Direction == Direction.Like),
                    DownVotes = q.Votes.Count(v => v.Direction == Direction.Dislike)
                })
                .ToList();
        }
    }
}