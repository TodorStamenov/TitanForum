namespace TitaniumForum.Web.Models.Questions
{
    using Infrastructure.Helpers;
    using Services.Models.Answers;
    using Services.Models.Questions;
    using Services.Models.Tags;
    using System.Collections.Generic;

    public class QuestionDetailsViewModel : BasePageViewModel
    {
        public QuestionDetailsServiceModel Question { get; set; }

        public IEnumerable<ListAnswersServiceModel> Answers { get; set; }

        public IEnumerable<ListTagsServiceModel> Tags { get; set; }
    }
}