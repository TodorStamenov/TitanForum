namespace TitaniumForum.Services.Areas.Moderator.Implementations
{
    using Data;
    using Data.Models;
    using Infrastructure.Extensions;
    using Models.Answers;
    using System.Collections.Generic;
    using System.Linq;

    public class ModeratorAnswerService : IModeratorAnswerService
    {
        private readonly UnitOfWork db;

        public ModeratorAnswerService(UnitOfWork db)
        {
            this.db = db;
        }

        public int DeletedCount(string search)
        {
            return this.db
                .Answers
                .AllEntries()
                .Where(a => a.IsDeleted)
                .Filter(search)
                .Count();
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

        public bool Restore(int id)
        {
            Answer answer = this.db.Answers.Find(id);

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

            this.db.Save();

            return true;
        }

        public IEnumerable<ListDeletedAnswersServiceModel> Deleted(int page, int pageSize, string search)
        {
            return this.db
                .Answers
                .AllEntries()
                .Where(a => a.IsDeleted)
                .Filter(search)
                .OrderByDescending(a => a.DateAdded)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsEnumerable()
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