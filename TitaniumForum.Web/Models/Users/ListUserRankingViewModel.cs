namespace TitaniumForum.Web.Models.Users
{
    using Infrastructure.Helpers;
    using Services.Models.Users;
    using System.Collections.Generic;

    public class ListUserRankingViewModel : BasePageViewModel
    {
        public IEnumerable<ListUserRankingServiceModel> Users { get; set; }
    }
}