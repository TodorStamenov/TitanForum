namespace TitaniumForum.Services.Models.Questions
{
    using Common.Mapping;
    using Data.Models;

    public class QuestionInfoServiceModel : IMapFrom<Question>
    {
        public bool IsDeleted { get; set; }

        public bool IsLocked { get; set; }
    }
}