namespace TitaniumForum.Web.Areas.Moderator.Models.Comments
{
    using Infrastructure.Helpers;
    using Services.Areas.Moderator.Models.Comments;
    using System.Collections.Generic;

    public class ListCommetnsModeratorViewModel : BasePageViewModel
    {
        public string Search { get; set; }

        public IEnumerable<ListDeletedCommentsServiceModel> Comments { get; set; }
    }
}