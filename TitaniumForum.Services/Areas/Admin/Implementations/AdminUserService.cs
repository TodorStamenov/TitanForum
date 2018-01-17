namespace TitaniumForum.Services.Areas.Admin.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Data;
    using Data.Models;
    using Infrastructure.Extensions;
    using Models.Logs;
    using Models.Roles;
    using Models.Users;
    using Services.Models.Users;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AdminUserService : IAdminUserService
    {
        private readonly UnitOfWork db;

        public AdminUserService(UnitOfWork db)
        {
            this.db = db;
        }

        public string GetUsername(int id)
        {
            return this.db
                .Users
                .Get(filter: u => u.Id == id)
                .Select(u => u.UserName)
                .FirstOrDefault();
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

            this.db.Logs.Add(log);
            this.db.Save();
        }

        public bool AddToRole(int userId, string roleName)
        {
            var userRoleInfo = this.db
                .Roles
                .Get(filter: r => r.Name == roleName)
                .Select(r => new
                {
                    r.Id,
                    InRole = r.Users.Any(u => u.UserId == userId)
                })
                .FirstOrDefault();

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

            this.db.UserRoles.Add(userRole);
            this.db.Save();

            return true;
        }

        public bool RemoveFromRole(int userId, string roleName)
        {
            var userRoleInfo = this.db
                .Roles
                .Get(filter: r => r.Name == roleName)
                .Select(r => new
                {
                    r.Id,
                    InRole = r.Users.Any(u => u.UserId == userId)
                })
                .FirstOrDefault();

            if (userRoleInfo == null
                || !userRoleInfo.InRole)
            {
                return false;
            }

            UserRole userRole = this.db
                .UserRoles
                .Find(userId, userRoleInfo.Id);

            this.db.UserRoles.Delete(userRole);
            this.db.Save();

            return true;
        }

        public int Total(string search)
        {
            return this.db
                .Logs
                .Get()
                .Filter(search)
                .Count();
        }

        public int Total(string role, string search)
        {
            return this.db
                .Users
                .Get()
                .Filter(search)
                .InRole(role)
                .Count();
        }

        public UserRolesServiceModel Roles(int id)
        {
            return this.db
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
            return this.db
                .Logs
                .Get(
                    orderBy: q => q.OrderByDescending(l => l.TimeStamp),
                    skip: (page - 1) * pageSize,
                    take: pageSize)
                .Filter(search)
                .AsQueryable()
                .ProjectTo<ListLogsServiceModel>()
                .ToList();
        }

        public IEnumerable<RoleServiceModel> AllRoles()
        {
            return this.db
                .Roles
                .Get()
                .AsQueryable()
                .ProjectTo<RoleServiceModel>()
                .ToList();
        }

        public IEnumerable<ListUsersServiceModel> All(int page, string role, string search, int pageSize)
        {
            return this.db
                .Users
                .Get(
                    orderBy: q => q.OrderBy(u => u.UserName),
                    skip: (page - 1) * pageSize,
                    take: pageSize)
                .Filter(search)
                .InRole(role)
                .AsQueryable()
                .ProjectTo<ListUsersServiceModel>()
                .ToList();
        }
    }
}