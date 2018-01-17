namespace TitaniumForum.Services.Areas.Moderator.Implementations
{
    using Data;
    using Data.Models;
    using Infrastructure.Extensions;
    using Models.Comments;
    using System.Collections.Generic;
    using System.Linq;

    public class ModeratorCommentService : IModeratorCommentService
    {
        private readonly UnitOfWork db;

        public ModeratorCommentService(UnitOfWork db)
        {
            this.db = db;
        }

        public int DeletedCount(string search)
        {
            return this.db
                .Comments
                .Get(filter: c => c.IsDeleted)
                .Filter(search)
                .Count();
        }

        public bool Delete(int id)
        {
            Comment comment = this.db.Comments.Find(id);

            if (comment == null
                || comment.IsDeleted)
            {
                return false;
            }

            comment.IsDeleted = true;

            this.db.Save();

            return true;
        }

        public bool Restore(int id)
        {
            Comment comment = this.db.Comments.Find(id);

            if (comment == null
                || !comment.IsDeleted
                || comment.Answer.IsDeleted)
            {
                return false;
            }

            comment.IsDeleted = false;

            this.db.Save();

            return true;
        }

        public IEnumerable<ListDeletedCommentsServiceModel> Deleted(int page, int pageSize, string search)
        {
            return this.db
                .Comments
                .Get(filter: c => c.IsDeleted)
                .Filter(search)
                .OrderByDescending(c => c.DateAdded)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsEnumerable()
                .Select(c => new ListDeletedCommentsServiceModel
                {
                    Id = c.Id,
                    Content = c.Content,
                    DateAdded = c.DateAdded.ToLocalTime(),
                    AuthorUsername = c.Author.UserName,
                    AuthorProfileImage = c.Author.ProfileImage.ConvertImage(),
                    UpVotes = c.Votes.Count(v => v.Direction == Direction.Like),
                    DownVotes = c.Votes.Count(v => v.Direction == Direction.Dislike),
                    IsDeleted = c.IsDeleted,
                    IsAnswerDeleted = c.Answer.IsDeleted,
                    QuestionId = c.Answer.QuestionId,
                    Rating = c.Author.Rating
                })
                .ToList();
        }
    }
}