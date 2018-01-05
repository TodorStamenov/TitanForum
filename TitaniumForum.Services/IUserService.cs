namespace TitaniumForum.Services
{
    using Models.Users;
    using System.Collections.Generic;

    public interface IUserService
    {
        int Total();

        bool AddProfileImage(int userId, byte[] imageContent);

        IEnumerable<ListUserRankingServiceModel> Ranking(int page, int pageSize);
    }
}