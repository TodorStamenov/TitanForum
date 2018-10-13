namespace TitaniumForum.Services.Implementations
{
    using Data.Contracts;
    using Data.Models;
    using Infrastructure.Extensions;
    using Models.Users;
    using System.Collections.Generic;
    using System.Linq;

    public class UserService : Service, IUserService
    {
        public UserService(IDatabase database)
            : base(database)
        {
        }

        public int Total()
        {
            return this.Database
                .Users
                .Count();
        }

        public bool AddProfileImage(int userId, byte[] imageContent)
        {
            User user = this.Database.Users.Find(userId);

            if (user == null)
            {
                return false;
            }

            user.ProfileImage = imageContent;

            this.Database.Save();

            return true;
        }

        public IEnumerable<ListUserRankingServiceModel> Ranking(int page, int pageSize)
        {
            return this.Database
                .Users
                .Get(
                    orderBy: q => q.OrderByDescending(u => u.Rating)
                        .ThenByDescending(u => u.Questions.Count + u.Answers.Count + u.Comments.Count)
                        .ThenBy(u => u.UserName),
                    skip: (page - 1) * pageSize,
                    take: pageSize)
                .Select(u => new ListUserRankingServiceModel
                {
                    Username = u.UserName,
                    Rating = u.Rating,
                    PostsCount = this.GetPostsCount(u),
                    ProfileImage = u.ProfileImage.ConvertImage()
                });
        }

        private int GetPostsCount(User user)
        {
            return user.Questions.Count
                + user.Answers.Count
                + user.Comments.Count;
        }
    }
}