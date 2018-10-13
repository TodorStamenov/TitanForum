namespace TitaniumForum.Services.Areas.Moderator.Implementations
{
    using Data.Contracts;
    using Data.Models;
    using Infrastructure.Extensions;
    using Models.Answers;
    using Services.Implementations;
    using System.Collections.Generic;
    using System.Linq;

    public class ModeratorAnswerService : Service, IModeratorAnswerService
    {
        public ModeratorAnswerService(IDatabase database)
            : base(database)
        {
        }

        public int DeletedCount(string search)
        {
            return this.Database
                .Answers
                .Count(a => a.IsDeleted
                    && (!string.IsNullOrEmpty(search)
                        ? a.Content.ToLower().Contains(search.ToLower())
                        : true));
        }

        public bool Delete(int id)
        {
            Answer answer = this.Database.Answers.Find(id);

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

            this.Database.Save();

            return true;
        }

        public bool Restore(int id)
        {
            Answer answer = this.Database.Answers.Find(id);

            if (answer == null
                || !answer.IsDeleted
                || answer.Question.IsDeleted)
            {
                return false;
            }

            answer.IsDeleted = false;

            foreach (var comment in answer.Comments)
            {
                comment.IsDeleted = false;
            }

            this.Database.Save();

            return true;
        }

        public IEnumerable<ListDeletedAnswersServiceModel> Deleted(int page, int pageSize, string search)
        {
            return this.Database
                .Answers
                .Get(
                    filter: a => a.IsDeleted
                        && (!string.IsNullOrEmpty(search)
                            ? a.Content.ToLower().Contains(search.ToLower())
                            : true),
                    orderBy: e => e.OrderByDescending(a => a.DateAdded),
                    skip: (page - 1) * pageSize,
                    take: pageSize)
                .Select(a => new ListDeletedAnswersServiceModel
                {
                    Id = a.Id,
                    Content = a.Content,
                    DateAdded = a.DateAdded.ToLocalTime(),
                    AuthorUsername = a.Author.UserName,
                    AuthorProfileImage = a.Author.ProfileImage.ConvertImage(),
                    UpVotes = a.Votes.Count(v => v.Direction == Direction.Like),
                    DownVotes = a.Votes.Count(v => v.Direction == Direction.Dislike),
                    IsDeleted = a.IsDeleted,
                    IsQuestionDeleted = a.Question.IsDeleted,
                    QuestionId = a.QuestionId,
                    Rating = a.Author.Rating
                })
                .ToList();
        }
    }
}