namespace TitaniumForum.Web.Models.Questions
{
    using Infrastructure.Helpers;
    using Services.Models.Questions;
    using System.Collections.Generic;

    public class ListQuestionsViewModel : BasePageViewModel
    {
        public int? CategoryId { get; set; }

        public int? SubCategoryId { get; set; }

        public int? TagId { get; set; }

        public string Search { get; set; }

        public IEnumerable<ListQuestionsServiceModel> Questions { get; set; }
    }
}