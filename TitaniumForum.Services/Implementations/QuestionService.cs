namespace TitaniumForum.Services.Implementations
{
    using Areas.Moderator.Models.SubCategories;
    using Data;
    using Data.Contracts;
    using Data.Models;
    using Infrastructure.Extensions;
    using Models.Questions;
    using Models.Tags;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class QuestionService : Service, IQuestionService
    {
        public QuestionService(IDatabase database)
            : base(database)
        {
        }

        public bool CanEdit(int id, int userId)
        {
            return this.Database
                .Questions
                .Any(c => c.Id == id && c.AuthorId == userId);
        }

        public bool TitleExists(string title)
        {
            return this.Database
                .Questions
                .Any(q => q.Title == title);
        }

        public bool IsLocked(int id)
        {
            return this.Database
                .Questions
                .Any(q => q.Id == id && q.IsLocked);
        }

        public string GetTitle(int id)
        {
            return this.Database
                .Questions
                .ProjectSingle(
                    projection: q => q.Title,
                    filter: q => q.Id == id);
        }

        public int Total(string search)
        {
            return this.Database
                .Questions
                .Count(q => !q.IsDeleted
                    && (!string.IsNullOrEmpty(search)
                        ? (q.Title.ToLower().Contains(search.ToLower())
                            || q.Content.ToLower().Contains(search.ToLower())
                            || q.Tags.Any(t => t.Tag.Name.ToLower().Contains(search.ToLower())))
                        : true));
        }

        public int TotalByCategory(int categoryId)
        {
            return this.Database
                .Questions
                .Count(q => !q.IsDeleted
                    && q.SubCategory.CategoryId == categoryId);
        }

        public int TotalBySubCategory(int subCategoryId)
        {
            return this.Database
                .Questions
                .Count(q => !q.IsDeleted
                    && q.SubCategoryId == subCategoryId);
        }

        public int TotalByTag(int tagId)
        {
            return this.Database
                .Questions
                .Count(q => !q.IsDeleted
                    && q.Tags.Any(t => t.TagId == tagId));
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

            this.Database.Questions.Add(question);
            this.Database.Save();

            return question.Id;
        }

        public bool Edit(int id, string title, string content, string tags, int subCategoryId)
        {
            Question question = this.Database.Questions.Find(id);

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

            this.Database.Save();

            return true;
        }

        public bool Report(int id)
        {
            Question question = this.Database.Questions.Find(id);

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

            this.Database.Save();

            return true;
        }

        public bool Vote(int id, int userId, Direction voteDirection)
        {
            Question question = this.Database.Questions.Find(id);

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

            this.Database.Save();

            return true;
        }

        public QuestionFormServiceModel GetForm(int id)
        {
            return this.Database
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
            Question question = this.Database
                .Questions
                .Find(id);

            if (question == null)
            {
                return null;
            }

            question.ViewCount++;

            QuestionDetailsServiceModel model = Enumerable.Repeat(question, 1)
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

            this.Database.Save();

            return model;
        }

        public IEnumerable<ListQuestionsServiceModel> ByCategory(int page, int pageSize, int categoryId)
        {
            return this.Database
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
            return this.Database
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
            return this.Database
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
            return this.Database
                .Questions
                .Get(
                    skip: (page - 1) * pageSize,
                    take: pageSize,
                    orderBy: e => e.OrderByDescending(q => q.DateAdded),
                    filter: q => !q.IsDeleted
                        && (!string.IsNullOrEmpty(search)
                            ? (q.Title.ToLower().Contains(search.ToLower())
                                || q.Content.ToLower().Contains(search.ToLower())
                                || q.Tags.Any(t => t.Tag.Name.ToLower().Contains(search.ToLower())))
                            : true))
                .ProjectToListModel()
                .ToList();
        }

        private SubCategoryInfoServiceModel GetSubCategoryInfo(int subCategoryId)
        {
            return this.Database
                .SubCategories
                .ProjectSingle(
                    projection: c => new SubCategoryInfoServiceModel { IsDeleted = c.IsDeleted },
                    filter: c => c.Id == subCategoryId);
        }

        private List<ListTagsServiceModel> GetTagInfo()
        {
            return this.Database
                .Tags
                .Project(t => new ListTagsServiceModel { Id = t.Id, Name = t.Name })
                .ToList();
        }

        private List<string> GetTags(string tags)
        {
            return tags
                .Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .Where(t => t.Length >= DataConstants.TagConstants.MinNameLength
                    && t.Length <= DataConstants.TagConstants.MaxNameLength)
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

                    this.Database.Tags.Add(tag);
                    this.Database.Save();
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