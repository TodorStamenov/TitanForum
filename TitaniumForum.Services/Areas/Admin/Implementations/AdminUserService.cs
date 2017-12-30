namespace TitaniumForum.Services.Areas.Admin.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Data;
    using Data.Models;
    using Infrastructure.Extensions;
    using Models.Roles;
    using Models.Users;
    using Services.Models.Users;
    using System.Collections.Generic;
    using System.Data.Entity;
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
                .All()
                .Where(u => u.Id == id)
                .Select(u => u.UserName)
                .FirstOrDefault();
        }

        //public void Log(string username, LogType logType, string tableName)
        //{
        //    Log log = new Log
        //    {
        //        User = username,
        //        LogType = logType,
        //        TableName = tableName,
        //        TimeStamp = DateTime.UtcNow
        //    };

        //    this.db.Logs.Add(log);
        //    this.db.SaveChanges();
        //}

        public bool AddToRole(int userId, string roleName)
        {
            var userRoleInfo = this.db
                .Roles
                .All()
                .Where(r => r.Name == roleName)
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
                .All()
                .Where(r => r.Name == roleName)
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

        //public int Total(string search)
        //{
        //    return this.db
        //        .Logs
        //        .Where(l => l.User.ToLower().Contains(search.ToLower()))
        //        .Count();
        //}

        public int Total(string role, string search)
        {
            return this.db
                .Users
                .All()
                .Filter(search)
                .InRole(role)
                .Count();
        }

        public UserRolesServiceModel Roles(int id)
        {
            return this.db
                .Users
                .All()
                .Where(u => u.Id == id)
                .ProjectTo<UserRolesServiceModel>()
                .FirstOrDefault();
        }

        //public IEnumerable<ListLogsServiceModel> Logs(int page, int itemsPerPage, string search)
        //{
        //    return this.db
        //        .Logs
        //        .Where(l => l.User.ToLower().Contains(search.ToLower()))
        //        .OrderByDescending(l => l.TimeStamp)
        //        .Skip((page - 1) * itemsPerPage)
        //        .Take(itemsPerPage)
        //        .ProjectTo<ListLogsServiceModel>()
        //        .ToList();
        //}

        public IEnumerable<RoleServiceModel> AllRoles()
        {
            return this.db
                .Roles
                .All()
                .ProjectTo<RoleServiceModel>()
                .ToList();
        }

        public IEnumerable<ListUsersServiceModel> All(int page, string role, string search, int usersPerPage)
        {
            return this.db
                .Users
                .All()
                .Include(u => u.Roles)
                .Filter(search)
                .InRole(role)
                .ProjectTo<ListUsersServiceModel>()
                .OrderBy(u => u.Username)
                .Skip((page - 1) * usersPerPage)
                .Take(usersPerPage)
                .ToList();
        }
    }
}