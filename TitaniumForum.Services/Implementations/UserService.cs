namespace TitaniumForum.Services.Implementations
{
    using Data;
    using Data.Models;

    public class UserService : IUserService
    {
        private readonly UnitOfWork db;

        public UserService(UnitOfWork db)
        {
            this.db = db;
        }

        public bool AddProfileImage(int userId, byte[] imageContent)
        {
            User user = this.db.Users.Find(userId);

            if (user == null)
            {
                return false;
            }

            user.ProfileImage = imageContent;

            this.db.Save();

            return true;
        }
    }
}