namespace TitaniumForum.Services.Models.Answers
{
    using Common.Mapping;
    using Data;
    using Data.Models;
    using Questions;
    using System.ComponentModel.DataAnnotations;

    public class AnswerFormServiceModel : IMapFrom<Answer>
    {
        [Required]
        [StringLength(
            int.MaxValue,
            MinimumLength = DataConstants.AnswerConstants.MinContentLength)]
        public string Content { get; set; }

        public QuestionRedirectServiceModel RedirectInfo { get; set; } = new QuestionRedirectServiceModel();
    }
}