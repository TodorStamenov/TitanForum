namespace TitaniumForum.Web.Infrastructure.Filters
{
    using Data.Models;
    using Services.Areas.Admin;
    using System.Web.Mvc;

    public class LogAttribute : ActionFilterAttribute
    {
        private readonly LogType logType;
        private readonly string tableName;

        public LogAttribute(LogType logType, string tableName)
        {
            this.logType = logType;
            this.tableName = tableName;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            IAdminUserService adminService = DependencyResolver.Current.GetService<IAdminUserService>();

            string username = context
                .HttpContext
                .User
                .Identity
                .Name;

            adminService.Log(username, this.logType, this.tableName);
        }
    }
}