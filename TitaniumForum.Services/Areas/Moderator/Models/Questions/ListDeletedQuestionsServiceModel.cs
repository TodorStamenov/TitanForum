namespace TitaniumForum.Services.Areas.Moderator.Models.Questions
{
    using Services.Models.Questions;

    public class ListDeletedQuestionsServiceModel : ListQuestionsServiceModel
    {
        public bool IsDeleted { get; set; }

        public bool IsSubCategoryDeleted { get; set; }
    }
}