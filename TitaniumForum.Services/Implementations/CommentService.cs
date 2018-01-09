namespace TitaniumForum.Services.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Data;
    using Data.Models;
    using Models.Comments;
    using System;
    using System.Linq;

    public class CommentService : ICommentService
    {
        private readonly UnitOfWork db;

        public CommentService(UnitOfWork db)
        {
            this.db = db;
        }

        public bool CanEdit(int id, int userId)
        {
            return this.db
                .Comments
                .Any(c => c.Id == id && c.AuthorId == userId);
        }

        public bool Create(int answerId, int authorId, string content)
        {
            var answerInfo = this.db
                .Answers
                .Where(a => a.Id == answerId)
                .Select(a => new
                {
                    a.IsDeleted,
                    IsQuestionDeleted = a.Question.IsDeleted,
                    IsQuestionLocked = a.Question.IsLocked
                })
                .FirstOrDefault();

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

            this.db.Comments.Add(comment);
            this.db.Save();

            return true;
        }

        public bool Edit(int id, string content)
        {
            Comment comment = this.db.Comments.Find(id);

            if (comment == null
                || comment.Answer.IsDeleted
                || comment.Answer.Question.IsDeleted
                || comment.Answer.Question.IsLocked)
            {
                return false;
            }

            comment.Content = content;

            this.db.Save();

            return true;
        }

        public bool Vote(int id, int userId, Direction voteDirection)
        {
            Comment comment = this.db.Comments.Find(id);

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

            this.db.Save();

            return true;
        }

        public CommentFormServiceModel GetForm(int id)
        {
            return this.db
                .Comments
                .AllEntries()
                .Where(c => c.Id == id)
                .Where(c => !c.Answer.IsDeleted)
                .Where(c => !c.Answer.Question.IsDeleted)
                .Where(c => !c.Answer.Question.IsLocked)
                .ProjectTo<CommentFormServiceModel>()
                .FirstOrDefault();
        }
    }
}