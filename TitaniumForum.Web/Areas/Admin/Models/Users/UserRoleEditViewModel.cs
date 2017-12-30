namespace TitaniumForum.Web.Areas.Admin.Models.Users
{
    using Services.Areas.Admin.Models.Roles;
    using Services.Areas.Admin.Models.Users;
    using System.Collections.Generic;

    public class UserRoleEditViewModel
    {
        public bool IsUserLocked { get; set; }

        public UserRolesServiceModel User { get; set; }

        public IEnumerable<RoleServiceModel> Roles { get; set; }
    }
}