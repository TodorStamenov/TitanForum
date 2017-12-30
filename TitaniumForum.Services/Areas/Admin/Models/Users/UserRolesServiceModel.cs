namespace TitaniumForum.Services.Areas.Admin.Models.Users
{
    using AutoMapper;
    using Common.Mapping;
    using Data.Models;
    using Infrastructure;
    using Roles;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UserRolesServiceModel : IMapFrom<User>, ICustomMapping
    {
        public int Id { get; set; }

        public string ProfileImage { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public IEnumerable<RoleServiceModel> Roles { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper.CreateMap<User, UserRolesServiceModel>()
                .ForMember(u => u.Roles,
                    cfg => cfg.MapFrom(
                        u => u.Roles.Select(r => new RoleServiceModel { Name = r.Role.Name })))
                .ForMember(u => u.ProfileImage,
                    cfg => cfg.MapFrom(
                        u => ServiceConstants.DataImage + Convert.ToBase64String(u.ProfileImage)));
        }
    }
}