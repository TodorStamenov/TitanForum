namespace TitaniumForum.Web.Areas.Moderator.Models.Answers
{
    using Infrastructure.Helpers;
    using Services.Areas.Moderator.Models.Answers;
    using System.Collections.Generic;

    public class ListAnswersModeratorViewModel : BasePageViewModel
    {
        public string Search { get; set; }

        public IEnumerable<ListDeletedAnswersServiceModel> Answers { get; set; }
    }
}