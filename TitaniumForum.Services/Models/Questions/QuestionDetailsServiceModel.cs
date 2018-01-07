namespace TitaniumForum.Services.Models.Questions
{
    public class QuestionDetailsServiceModel : BaseQuestionServiceModel
    {
        public string Content { get; set; }

        public int Rating { get; set; }

        public bool IsOwner { get; set; }

        public bool HasVoted { get; set; }
    }
}