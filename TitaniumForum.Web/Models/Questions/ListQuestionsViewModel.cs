namespace TitaniumForum.Web.Models.Questions
{
    using Areas.Moderator.Models.Questions;

    public class ListQuestionsViewModel : ListQuestionsModeratorViewModel
    {
        public int? CategoryId { get; set; }

        public int? SubCategoryId { get; set; }

        public int? TagId { get; set; }
    }
}