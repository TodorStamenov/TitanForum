namespace TitaniumForum.Web.Areas.Admin.Models.Users
{
    using Infrastructure.Helpers;
    using Services.Areas.Admin.Models.Roles;
    using Services.Models.Users;
    using System.Collections.Generic;

    public class ListUsersViewModel : BasePageViewModel
    {
        public string Search { get; set; }

        public string UserRole { get; set; }

        public IEnumerable<ListUsersServiceModel> Users { get; set; }

        public IEnumerable<RoleServiceModel> Roles { get; set; }
    }
}