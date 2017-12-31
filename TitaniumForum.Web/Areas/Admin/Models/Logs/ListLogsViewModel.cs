namespace TitaniumForum.Web.Areas.Admin.Models.Logs
{
    using Infrastructure.Helpers;
    using Services.Areas.Admin.Models.Logs;
    using System.Collections.Generic;

    public class ListLogsViewModel : BasePageViewModel
    {
        public string Search { get; set; }

        public IEnumerable<ListLogsServiceModel> Logs { get; set; }
    }
}