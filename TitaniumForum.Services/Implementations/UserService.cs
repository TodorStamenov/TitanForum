namespace TitaniumForum.Services.Implementations
{
    using Data;
    using Data.Models;
    using Infrastructure.Extensions;
    using Models.Users;
    using System.Collections.Generic;
    using System.Linq;

    public class UserService : IUserService
    {
        private readonly UnitOfWork db;

        public UserService(UnitOfWork db)
        {
            this.db = db;
        }

        public int Total()
        {
            return this.db
                .Users
                .Count();
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

        public IEnumerable<ListUserRankingServiceModel> Ranking(int page, int pageSize)
        {
            return this.db
                .Users
                .OrderByDescending(u => u.Rating)
                .ThenByDescending(u => this.GetPostsCount(u))
                .ThenBy(u => u.UserName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new ListUserRankingServiceModel
                {
                    Username = u.UserName,
                    Rating = u.Rating,
                    PostsCount = this.GetPostsCount(u),
                    ProfileImage = u.ProfileImage.ConvertImage()
                })
                .ToList();
        }

        private int GetPostsCount(User user)
        {
            return user.Questions.Count
                + user.Answers.Count
                + user.Comments.Count;
        }
    }
}