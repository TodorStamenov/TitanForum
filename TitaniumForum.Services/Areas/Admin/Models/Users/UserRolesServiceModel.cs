namespace TitaniumForum.Services.Areas.Admin.Models.Users
{
    using Roles;
    using System.Collections.Generic;

    public class UserRolesServiceModel
    {
        public int Id { get; set; }

        public string ProfileImage { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public IEnumerable<RoleServiceModel> Roles { get; set; }
    }
}