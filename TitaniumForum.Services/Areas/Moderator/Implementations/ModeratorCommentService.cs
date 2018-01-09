namespace TitaniumForum.Services.Areas.Moderator.Implementations
{
    using Data;
    using Data.Models;

    public class ModeratorCommentService : IModeratorCommentService
    {
        private readonly UnitOfWork db;

        public ModeratorCommentService(UnitOfWork db)
        {
            this.db = db;
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
    }
}