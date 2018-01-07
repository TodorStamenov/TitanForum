namespace TitaniumForum.Services.Implementations
{
    using Areas.Moderator.Models.SubCategories;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Data.Models;
    using Infrastructure.Extensions;
    using Models.Questions;
    using Models.Tags;
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

        public bool CanEdit(int id, int userId)
        {
            return this.db
                .Questions
                .Any(c => c.Id == id && c.AuthorId == userId);
        }

        public bool TitleExists(string title)
        {
            return this.db
                .Questions
                .Any(q => q.Title == title);
        }

        public string GetTitle(int id)
        {
            return this.db
                .Questions
                .Where(q => q.Id == id)
                .Select(q => q.Title)
                .FirstOrDefault();
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

        public int TotalByCategory(int categoryId)
        {
            return this.db
                .Questions
                .Where(q => q.SubCategory.CategoryId == categoryId)
                .Where(q => !q.IsDeleted)
                .Count();
        }

        public int TotalBySubCategory(int subCategoryId)
        {
            return this.db
                .Questions
                .Where(q => q.SubCategoryId == subCategoryId)
                .Where(q => !q.IsDeleted)
                .Count();
        }

        public int TotalByTag(int tagId)
        {
            return this.db
                .Questions
                .Where(q => q.Tags.Any(t => t.TagId == tagId))
                .Where(q => !q.IsDeleted)
                .Count();
        }

        public int Create(int authorId, string title, string content, string tags, int subCategoryId)
        {
            if (this.TitleExists(title))
            {
                return -1;
            }

            List<string> tagTokens = this.GetTags(tags);

            if (!tagTokens.Any())
            {
                return -1;
            }

            SubCategoryInfoServiceModel subCategoryInfo = this.GetSubCategoryInfo(subCategoryId);

            if (subCategoryInfo == null
                || subCategoryInfo.IsDeleted)
            {
                return -1;
            }

            Question question = new Question
            {
                Title = title,
                Content = content,
                DateAdded = DateTime.UtcNow,
                AuthorId = authorId,
                SubCategoryId = subCategoryId
            };

            AddTagsToQuestion(tagTokens, question);

            this.db.Questions.Add(question);
            this.db.Save();

            return question.Id;
        }

        public bool Edit(int id, string title, string content, string tags, int subCategoryId)
        {
            Question question = this.db.Questions.Find(id);

            if (question == null
                || question.IsDeleted
                || (question.Title != title
                    && this.TitleExists(title)))
            {
                return false;
            }

            List<string> tagTokens = this.GetTags(tags);

            if (!tagTokens.Any())
            {
                return false;
            }

            SubCategoryInfoServiceModel subCategoryInfo = this.GetSubCategoryInfo(subCategoryId);

            if (subCategoryInfo == null
                || subCategoryInfo.IsDeleted)
            {
                return false;
            }

            question.Title = title;
            question.Content = content;
            question.SubCategoryId = subCategoryId;
            question.Tags.Clear();

            AddTagsToQuestion(tagTokens, question);

            this.db.Save();

            return true;
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
            throw new NotImplementedException();
        }

        public bool Lock(int id)
        {
            throw new NotImplementedException();
        }

        public bool Unlock(int id)
        {
            throw new NotImplementedException();
        }

        public bool Report(int id)
        {
            throw new NotImplementedException();
        }

        public QuestionFormServiceModel GetForm(int id)
        {
            return this.db
                .Questions
                .Where(q => q.Id == id)
                .Select(q => new QuestionFormServiceModel
                {
                    Title = q.Title,
                    Content = q.Content,
                    Tags = string.Join(", ", q.Tags.Select(t => t.Tag.Name))
                })
                .FirstOrDefault();
        }

        public QuestionDetailsServiceModel Details(int id, int userId)
        {
            QuestionDetailsServiceModel model = this.db
               .Questions
               .Where(q => q.Id == id)
               .Where(q => !q.IsDeleted)
               .Select(q => new QuestionDetailsServiceModel
               {
                   Id = q.Id,
                   Title = q.Title,
                   Content = q.Content,
                   DateAdded = q.DateAdded.ToLocalTime(),
                   AuthorUsername = q.Author.UserName,
                   AuthorProfileImage = q.Author.ProfileImage.ConvertImage(),
                   Rating = q.Author.Rating,
                   SubCategoryId = q.SubCategoryId,
                   SubCategoryName = q.SubCategory.Name,
                   UpVotes = q.Votes.Count(v => v.Direction == Direction.Like),
                   DownVotes = q.Votes.Count(v => v.Direction == Direction.Dislike),
                   IsOwner = q.AuthorId == userId,
                   HasVoted = q.Votes.Any(v => v.UserId == userId)
               })
               .FirstOrDefault();

            if (model == null)
            {
                return null;
            }

            Question question = this.db
                .Questions
                .FirstOrDefault(q => q.Id == id);

            question.ViewCount++;

            this.db.Save();

            return model;
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

        public IEnumerable<ListQuestionsServiceModel> ByTag(int page, int pageSize, int tagId)
        {
            return this.db
                .Questions
                .AllEntries()
                .Where(q => !q.IsDeleted)
                .Where(q => q.Tags.Any(t => t.TagId == tagId))
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

        private SubCategoryInfoServiceModel GetSubCategoryInfo(int subCategoryId)
        {
            return this.db
                .Categories
                .AllEntries()
                .Where(c => c.Id == subCategoryId)
                .ProjectTo<SubCategoryInfoServiceModel>()
                .FirstOrDefault();
        }

        private List<ListTagsServiceModel> GetTagInfo()
        {
            return this.db
                .Tags
                .Select(t => new ListTagsServiceModel
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToList();
        }

        private List<string> GetTags(string tags)
        {
            return tags
                .Split(
                    new[] { ' ', ',' },
                    StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .Where(t => t.Length >= DataConstants.TagConstants.MinNameLength)
                .Where(t => t.Length <= DataConstants.TagConstants.MaxNameLength)
                .ToList();
        }

        private void AddTagsToQuestion(List<string> tagTokens, Question question)
        {
            List<ListTagsServiceModel> tagInfo = this.GetTagInfo();

            foreach (var tagName in tagTokens)
            {
                if (!tagInfo.Any(t => t.Name == tagName))
                {
                    Tag tag = new Tag
                    {
                        Name = tagName
                    };

                    this.db.Tags.Add(tag);
                    this.db.Save();
                    tagInfo.Add(new ListTagsServiceModel
                    {
                        Id = tag.Id,
                        Name = tagName
                    });
                }

                int tagId = tagInfo
                    .FirstOrDefault(t => t.Name == tagName)
                    .Id;

                question.Tags.Add(new TagQuestion
                {
                    TagId = tagId
                });
            }
        }
    }
}