namespace TitaniumForum.Web.Infrastructure.Filters
{
    using Ninject;
    using Services.Areas.Admin;
    using System.Web.Mvc;

    public class LogAttribute : ActionFilterAttribute
    {
        //private readonly LogType logType;
        //private readonly string tableName;

        //public LogAttribute(LogType logType, string tableName)
        //{
        //    this.logType = logType;
        //    this.tableName = tableName;
        //}

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            IKernel kernel = NinjectWebCommon.CreateKernel();

            IAdminUserService logService = kernel.Get<IAdminUserService>();

            string username = context
                .HttpContext
                .User
                .Identity
                .Name;

            //logService.Log(username, this.logType, this.tableName);
        }
    }
}