namespace TitaniumForum.Services.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Data;
    using Data.Models;
    using Infrastructure.Extensions;
    using Models.Answers;
    using Models.Comments;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AnswerService : IAnswerService
    {
        private readonly UnitOfWork db;

        public AnswerService(UnitOfWork db)
        {
            this.db = db;
        }

        public bool CanEdit(int id, int userId)
        {
            return this.db
                .Answers
                .Any(a => a.Id == id && a.AuthorId == userId);
        }

        public int TotalByQuestion(int questionId)
        {
            return this.db
                .Answers
                .Where(a => a.QuestionId == questionId)
                .Where(a => !a.IsDeleted)
                .Count();
        }

        public bool Create(int questionId, int authorId, string content)
        {
            var questionInfo = this.db
                .Questions
                .Where(q => q.Id == questionId)
                .Select(q => new
                {
                    q.IsDeleted
                })
                .FirstOrDefault();

            if (questionInfo == null
                || questionInfo.IsDeleted)
            {
                return false;
            }

            Answer answer = new Answer
            {
                Content = content,
                AuthorId = authorId,
                DateAdded = DateTime.UtcNow,
                QuestionId = questionId
            };

            this.db.Answers.Add(answer);
            this.db.Save();

            return true;
        }

        public bool Edit(int id, string content)
        {
            Answer answer = this.db.Answers.Find(id);

            if (answer == null
                || answer.IsDeleted)
            {
                return false;
            }

            answer.Content = content;

            this.db.Save();

            return true;
        }

        public bool Delete(int id)
        {
            Answer answer = this.db.Answers.Find(id);

            if (answer == null
                || answer.IsDeleted)
            {
                return false;
            }

            answer.IsDeleted = true;

            foreach (var comment in answer.Comments)
            {
                comment.IsDeleted = true;
            }

            this.db.Save();

            return true;
        }

        public AnswerFormServiceModel GetForm(int id)
        {
            return this.db
                .Answers
                .AllEntries()
                .Where(a => a.Id == id)
                .ProjectTo<AnswerFormServiceModel>()
                .FirstOrDefault();
        }

        public IEnumerable<ListAnswersServiceModel> ByQuestion(int questionId, int userId, int page, int pageSize)
        {
            return this.db
                .Answers
                .Where(a => a.QuestionId == questionId)
                .Where(a => !a.IsDeleted)
                .OrderBy(a => a.DateAdded)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new ListAnswersServiceModel
                {
                    Id = a.Id,
                    Content = a.Content,
                    DateAdded = a.DateAdded.ToLocalTime(),
                    AuthorUsername = a.Author.UserName,
                    AuthorProfileImage = a.Author.ProfileImage.ConvertImage(),
                    UpVotes = a.Votes.Count(v => v.Direction == Direction.Like),
                    DownVotes = a.Votes.Count(v => v.Direction == Direction.Dislike),
                    IsOwner = a.AuthorId == userId,
                    HasVoted = a.Votes.Any(v => v.UserId == userId),
                    Rating = a.Author.Rating,
                    Comments = a.Comments
                        .Where(c => !c.IsDeleted)
                        .OrderBy(c => c.DateAdded)
                        .Select(c => new ListCommentsServiceModel
                        {
                            Id = c.Id,
                            Content = c.Content,
                            DateAdded = c.DateAdded.ToLocalTime(),
                            AuthorUsername = c.Author.UserName,
                            AuthorProfileImage = c.Author.ProfileImage.ConvertImage(),
                            UpVotes = c.Votes.Count(v => v.Direction == Direction.Like),
                            DownVotes = c.Votes.Count(v => v.Direction == Direction.Dislike),
                            Rating = c.Author.Rating,
                            IsOwner = c.AuthorId == userId,
                            HasVoted = c.Votes.Any(v => v.UserId == userId)
                        })
                        .ToList()
                })
                .ToList();
        }
    }
}