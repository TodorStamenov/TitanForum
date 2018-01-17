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
                .Get()
                .Any(c => c.Id == id && c.AuthorId == userId);
        }

        public bool TitleExists(string title)
        {
            return this.db
                .Questions
                .Get()
                .Any(q => q.Title == title);
        }

        public bool IsLocked(int id)
        {
            return this.db
                .Questions
                .Get(filter: q => q.Id == id)
                .Select(q => q.IsLocked)
                .FirstOrDefault();
        }

        public string GetTitle(int id)
        {
            return this.db
                .Questions
                .Get(filter: q => q.Id == id)
                .Select(q => q.Title)
                .FirstOrDefault();
        }

        public int Total(string search)
        {
            return this.db
                .Questions
                .Get(filter: q => !q.IsDeleted)
                .Filter(search)
                .Count();
        }

        public int TotalByCategory(int categoryId)
        {
            return this.db
                .Questions
                .Get(
                    filter: q => q.SubCategory.CategoryId == categoryId
                        && !q.IsDeleted)
                .Count();
        }

        public int TotalBySubCategory(int subCategoryId)
        {
            return this.db
                .Questions
                .Get(
                    filter: q => q.SubCategoryId == subCategoryId
                        && !q.IsDeleted)
                .Count();
        }

        public int TotalByTag(int tagId)
        {
            return this.db
                .Questions
                .Get(
                    filter: q => q.Tags.Any(t => t.TagId == tagId)
                        && !q.IsDeleted)
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
                || question.IsLocked
                || question.SubCategory.IsDeleted
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

        public bool Report(int id)
        {
            Question question = this.db.Questions.Find(id);

            if (question == null
                || question.IsLocked
                || question.IsDeleted)
            {
                return false;
            }

            if (question.IsReported)
            {
                return true;
            }

            question.IsReported = true;

            this.db.Save();

            return true;
        }

        public bool Vote(int id, int userId, Direction voteDirection)
        {
            Question question = this.db.Questions.Find(id);

            if (question == null
                || question.IsDeleted
                || question.IsLocked
                || question.Votes.Any(v => v.UserId == userId))
            {
                return false;
            }

            User user = question.Author;

            question.Votes.Add(new UserQuestionVote
            {
                UserId = userId,
                Direction = voteDirection
            });

            if (voteDirection == Direction.Like)
            {
                user.Rating++;
            }
            else if (voteDirection == Direction.Dislike)
            {
                user.Rating--;
            }

            this.db.Save();

            return true;
        }

        public QuestionFormServiceModel GetForm(int id)
        {
            return this.db
                .Questions
                .Get(
                    filter: q => q.Id == id
                        && !q.IsDeleted
                        && !q.IsLocked
                        && !q.SubCategory.IsDeleted)
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
                .Get(
                    filter: q => q.Id == id
                        && !q.IsDeleted)
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
                    IsLocked = q.IsLocked,
                    IsReported = q.IsReported,
                    HasVoted = q.Votes.Any(v => v.UserId == userId)
                })
                .FirstOrDefault();

            if (model == null)
            {
                return null;
            }

            Question question = this.db
                .Questions
                .Get()
                .FirstOrDefault(q => q.Id == id);

            question.ViewCount++;

            this.db.Save();

            return model;
        }

        public IEnumerable<ListQuestionsServiceModel> ByCategory(int page, int pageSize, int categoryId)
        {
            return this.db
                 .Questions
                 .Get(
                    filter: q => !q.IsDeleted
                        && q.SubCategory.CategoryId == categoryId,
                    orderBy: q => q.OrderByDescending(question => question.DateAdded),
                    skip: (page - 1) * pageSize,
                    take: pageSize)
                 .ProjectToListModel()
                 .ToList();
        }

        public IEnumerable<ListQuestionsServiceModel> BySubCategory(int page, int pageSize, int subCategoryId)
        {
            return this.db
                .Questions
                .Get(
                    filter: q => !q.IsDeleted
                        && q.SubCategoryId == subCategoryId,
                    orderBy: q => q.OrderByDescending(question => question.DateAdded),
                    skip: (page - 1) * pageSize,
                    take: pageSize)
                .ProjectToListModel()
                .ToList();
        }

        public IEnumerable<ListQuestionsServiceModel> ByTag(int page, int pageSize, int tagId)
        {
            return this.db
                .Questions
                .Get(
                    filter: q => !q.IsDeleted
                        && q.Tags.Any(t => t.TagId == tagId),
                    orderBy: q => q.OrderByDescending(question => question.DateAdded),
                    skip: (page - 1) * pageSize,
                    take: pageSize)
                .ProjectToListModel()
                .ToList();
        }

        public IEnumerable<ListQuestionsServiceModel> All(int page, int pageSize, string search)
        {
            return this.db
                .Questions
                .Get(filter: q => !q.IsDeleted)
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
                .SubCategories
                .Get(filter: c => c.Id == subCategoryId)
                .AsQueryable()
                .ProjectTo<SubCategoryInfoServiceModel>()
                .FirstOrDefault();
        }

        private List<ListTagsServiceModel> GetTagInfo()
        {
            return this.db
                .Tags
                .Get()
                .AsQueryable()
                .ProjectTo<ListTagsServiceModel>()
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