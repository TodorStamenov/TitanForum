namespace TitaniumForum.Services.Implementations
{
    using Data.Contracts;
    using Data.Models;
    using Infrastructure.Extensions;
    using Models.Answers;
    using Models.Comments;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AnswerService : Service, IAnswerService
    {
        public AnswerService(IDatabase database)
            : base(database)
        {
        }

        public bool CanEdit(int id, int userId)
        {
            return this.Database
                .Answers
                .Any(a => a.Id == id && a.AuthorId == userId);
        }

        public int TotalByQuestion(int questionId)
        {
            return this.Database
                .Answers
                .Count(a => a.QuestionId == questionId && !a.IsDeleted);
        }

        public bool Create(int questionId, int authorId, string content)
        {
            var questionInfo = this.Database
                .Questions
                .ProjectSingle(
                    projection: q => new { q.IsLocked, q.IsDeleted },
                    filter: q => q.Id == questionId);

            if (questionInfo == null
                || questionInfo.IsLocked
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

            this.Database.Answers.Add(answer);
            this.Database.Save();

            return true;
        }

        public bool Edit(int id, string content)
        {
            Answer answer = this.Database.Answers.Find(id);

            if (answer == null
                || answer.IsDeleted
                || answer.Question.IsDeleted
                || answer.Question.IsLocked)
            {
                return false;
            }

            answer.Content = content;

            this.Database.Save();

            return true;
        }

        public bool Vote(int id, int userId, Direction voteDirection)
        {
            Answer answer = this.Database.Answers.Find(id);

            if (answer == null
                || answer.IsDeleted
                || answer.Question.IsDeleted
                || answer.Question.IsLocked
                || answer.Votes.Any(v => v.UserId == userId))
            {
                return false;
            }

            User user = answer.Author;

            answer.Votes.Add(new UserAnswerVote
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

        public AnswerFormServiceModel GetForm(int id)
        {
            return this.Database
                .Answers
                .ProjectSingle(
                    projection: a => new AnswerFormServiceModel { Content = a.Content },
                    filter: a => a.Id == id
                        && !a.Question.IsDeleted
                        && !a.Question.IsLocked);
        }

        public IEnumerable<ListAnswersServiceModel> ByQuestion(int questionId, int userId, int page, int pageSize)
        {
            return this.Database
                .Answers
                .Get(
                    filter: a => !a.IsDeleted
                        && a.QuestionId == questionId,
                    orderBy: q => q.OrderBy(a => a.DateAdded),
                    skip: (page - 1) * pageSize,
                    take: pageSize)
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
                });
        }
    }
}