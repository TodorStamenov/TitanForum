namespace TitaniumForum.Services.Areas.Admin.Implementations
{
    using Data.Contracts;
    using Data.Models;
    using Infrastructure.Extensions;
    using Models.Logs;
    using Models.Roles;
    using Models.Users;
    using Services.Implementations;
    using Services.Models.Users;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AdminUserService : Service, IAdminUserService
    {
        public AdminUserService(IDatabase database)
            : base(database)
        {
        }

        public string GetUsername(int id)
        {
            return this.Database
                .Users
                .ProjectSingle(
                    projection: u => u.UserName,
                    filter: u => u.Id == id);
        }

        public void Log(string username, LogType action, string tableName)
        {
            Log log = new Log
            {
                Username = username,
                LogType = action,
                TableName = tableName,
                TimeStamp = DateTime.UtcNow
            };

            this.Database.Logs.Add(log);
            this.Database.Save();
        }

        public bool AddToRole(int userId, string roleName)
        {
            var userRoleInfo = this.Database
                .Roles
                .ProjectSingle(
                    projection: r => new
                    {
                        r.Id,
                        InRole = r.Users.Any(u => u.UserId == userId)
                    },
                    filter: r => r.Name == roleName);

            if (userRoleInfo == null
                || userRoleInfo.InRole)
            {
                return false;
            }

            UserRole userRole = new UserRole
            {
                RoleId = userRoleInfo.Id,
                UserId = userId
            };

            this.Database.UserRoles.Add(userRole);
            this.Database.Save();

            return true;
        }

        public bool RemoveFromRole(int userId, string roleName)
        {
            var userRoleInfo = this.Database
                .Roles
                .ProjectSingle(
                    projection: r => new
                    {
                        r.Id,
                        InRole = r.Users.Any(u => u.UserId == userId)
                    },
                    filter: r => r.Name == roleName);

            if (userRoleInfo == null
                || !userRoleInfo.InRole)
            {
                return false;
            }

            UserRole userRole = this.Database
                .UserRoles
                .Find(userId, userRoleInfo.Id);

            this.Database.UserRoles.Delete(userRole);
            this.Database.Save();

            return true;
        }

        public int Total(string search)
        {
            return this.Database
                .Logs
                .Count(l =>
                    !string.IsNullOrEmpty(search)
                        ? l.Username.ToLower().Contains(search.ToLower())
                        : true);
        }

        public int Total(string role, string search)
        {
            return this.Database
                .Users
                .Count(u =>
                    !string.IsNullOrEmpty(search)
                        ? u.UserName.ToLower().Contains(search.ToLower())
                        : true
                    && !string.IsNullOrEmpty(role)
                        ? u.Roles.Any(r => r.Role.Name.ToLower() == role.ToLower())
                        : true);
        }

        public UserRolesServiceModel Roles(int id)
        {
            return this.Database
                .Users
                .Get(filter: u => u.Id == id)
                .Select(u => new UserRolesServiceModel
                {
                    Id = u.Id,
                    Username = u.UserName,
                    Email = u.Email,
                    ProfileImage = u.ProfileImage.ConvertImage(),
                    Roles = u.Roles
                        .Select(r => new RoleServiceModel
                        {
                            Id = r.RoleId,
                            Name = r.Role.Name
                        })
                })
                .FirstOrDefault();
        }

        public IEnumerable<ListLogsServiceModel> Logs(int page, int pageSize, string search)
        {
            return this.Database
                .Logs
                .Project(
                    projection: l => new ListLogsServiceModel
                    {
                        Username = l.Username,
                        TableName = l.TableName,
                        LogType = l.LogType,
                        TimeStamp = l.TimeStamp
                    },
                    filter: l =>
                        !string.IsNullOrEmpty(search)
                            ? l.Username.ToLower().Contains(search.ToLower())
                            : true,
                    orderBy: q => q.OrderByDescending(l => l.TimeStamp),
                    skip: (page - 1) * pageSize,
                    take: pageSize);
        }

        public IEnumerable<RoleServiceModel> AllRoles()
        {
            return this.Database
                .Roles
                .Project(projection: r => new RoleServiceModel { Id = r.Id, Name = r.Name });
        }

        public IEnumerable<ListUsersServiceModel> All(int page, string role, string search, int pageSize)
        {
            return this.Database
                .Users
                .Project(
                    projection: u => new ListUsersServiceModel
                    {
                        Id = u.Id,
                        Email = u.Email,
                        Username = u.UserName
                    },
                    filter: u =>
                        !string.IsNullOrEmpty(search)
                            ? u.UserName.ToLower().Contains(search.ToLower())
                            : true
                        && !string.IsNullOrEmpty(role)
                            ? u.Roles.Any(r => r.Role.Name.ToLower() == role.ToLower())
                            : true,
                    orderBy: q => q.OrderBy(u => u.UserName),
                    skip: (page - 1) * pageSize,
                    take: pageSize);
        }
    }
}