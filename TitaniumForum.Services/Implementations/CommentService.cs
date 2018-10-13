namespace TitaniumForum.Services.Implementations
{
    using Data.Contracts;
    using Data.Models;
    using Models.Comments;
    using System;
    using System.Linq;

    public class CommentService : Service, ICommentService
    {
        public CommentService(IDatabase database)
            : base(database)
        {
        }

        public bool CanEdit(int id, int userId)
        {
            return this.Database
                .Comments
                .Any(c => c.Id == id
                    && c.AuthorId == userId);
        }

        public bool Create(int answerId, int authorId, string content)
        {
            var answerInfo = this.Database
                .Answers
                .ProjectSingle(
                    projection: a => new
                    {
                        a.IsDeleted,
                        IsQuestionDeleted = a.Question.IsDeleted,
                        IsQuestionLocked = a.Question.IsLocked
                    },
                    filter: a => a.Id == answerId);

            if (answerInfo == null
                || answerInfo.IsDeleted
                || answerInfo.IsQuestionDeleted
                || answerInfo.IsQuestionLocked)
            {
                return false;
            }

            Comment comment = new Comment
            {
                Content = content,
                AuthorId = authorId,
                AnswerId = answerId,
                DateAdded = DateTime.UtcNow
            };

            this.Database.Comments.Add(comment);
            this.Database.Save();

            return true;
        }

        public bool Edit(int id, string content)
        {
            Comment comment = this.Database.Comments.Find(id);

            if (comment == null
                || comment.Answer.IsDeleted
                || comment.Answer.Question.IsDeleted
                || comment.Answer.Question.IsLocked)
            {
                return false;
            }

            comment.Content = content;

            this.Database.Save();

            return true;
        }

        public bool Vote(int id, int userId, Direction voteDirection)
        {
            Comment comment = this.Database.Comments.Find(id);

            if (comment == null
                || comment.IsDeleted
                || comment.Votes.Any(v => v.UserId == userId))
            {
                return false;
            }

            User user = comment.Author;

            comment.Votes.Add(new UserCommentVote
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

        public CommentFormServiceModel GetForm(int id)
        {
            return this.Database
                .Comments
                .ProjectSingle(
                    projection: c => new CommentFormServiceModel { AnswerId = c.AnswerId, Content = c.Content },
                    filter: c => c.Id == id
                        && !c.Answer.IsDeleted
                        && !c.Answer.Question.IsDeleted
                        && !c.Answer.Question.IsLocked);
        }
    }
}