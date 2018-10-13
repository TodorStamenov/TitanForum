namespace TitaniumForum.Services.Areas.Moderator.Implementations
{
    using Data.Contracts;
    using Data.Models;
    using Infrastructure.Extensions;
    using Models.Comments;
    using Services.Implementations;
    using System.Collections.Generic;
    using System.Linq;

    public class ModeratorCommentService : Service, IModeratorCommentService
    {
        public ModeratorCommentService(IDatabase database)
            : base(database)
        {
        }

        public int DeletedCount(string search)
        {
            return this.Database
                .Comments
                .Count(c => c.IsDeleted
                    && (!string.IsNullOrEmpty(search)
                        ? c.Content.ToLower().Contains(search.ToLower())
                        : true));
        }

        public bool Delete(int id)
        {
            Comment comment = this.Database.Comments.Find(id);

            if (comment == null
                || comment.IsDeleted)
            {
                return false;
            }

            comment.IsDeleted = true;

            this.Database.Save();

            return true;
        }

        public bool Restore(int id)
        {
            Comment comment = this.Database.Comments.Find(id);

            if (comment == null
                || !comment.IsDeleted
                || comment.Answer.IsDeleted)
            {
                return false;
            }

            comment.IsDeleted = false;

            this.Database.Save();

            return true;
        }

        public IEnumerable<ListDeletedCommentsServiceModel> Deleted(int page, int pageSize, string search)
        {
            return this.Database
                .Comments
                .Get(
                    filter: c => c.IsDeleted
                        && (!string.IsNullOrEmpty(search)
                            ? c.Content.ToLower().Contains(search.ToLower())
                            : true),
                    orderBy: e => e.OrderByDescending(c => c.DateAdded),
                    skip: (page - 1) * pageSize,
                    take: pageSize)
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