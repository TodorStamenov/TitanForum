namespace TitaniumForum.Services.Models.Answers
{
    using Data;
    using Questions;
    using System.ComponentModel.DataAnnotations;

    public class AnswerFormServiceModel
    {
        [Required]
        [StringLength(
            int.MaxValue,
            MinimumLength = DataConstants.AnswerConstants.MinContentLength)]
        public string Content { get; set; }

        public QuestionRedirectServiceModel RedirectInfo { get; set; } = new QuestionRedirectServiceModel();
    }
}