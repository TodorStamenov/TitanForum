namespace TitaniumForum.Services.Areas.Moderator.Implementations
{
    using Data;
    using Data.Models;

    public class ModeratorAnswerService : IModeratorAnswerService
    {
        private readonly UnitOfWork db;

        public ModeratorAnswerService(UnitOfWork db)
        {
            this.db = db;
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
    }
}