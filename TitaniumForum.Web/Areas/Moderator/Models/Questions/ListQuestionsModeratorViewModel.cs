namespace TitaniumForum.Web.Areas.Moderator.Models.Questions
{
    using Infrastructure.Helpers;
    using Services.Models.Questions;
    using System.Collections.Generic;

    public class ListQuestionsModeratorViewModel : BasePageViewModel
    {
        public string Search { get; set; }

        public IEnumerable<ListQuestionsServiceModel> Questions { get; set; }
    }
}