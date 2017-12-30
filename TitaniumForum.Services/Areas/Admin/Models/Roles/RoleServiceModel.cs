namespace TitaniumForum.Services.Areas.Admin.Models.Roles
{
    using Common.Mapping;
    using Data.Models;

    public class RoleServiceModel : IMapFrom<Role>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}