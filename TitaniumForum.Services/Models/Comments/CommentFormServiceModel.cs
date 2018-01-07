namespace TitaniumForum.Services.Models.Comments
{
    using Common.Mapping;
    using Data;
    using Data.Models;
    using Questions;
    using System.ComponentModel.DataAnnotations;

    public class CommentFormServiceModel : IMapFrom<Comment>
    {
        [Required]
        [StringLength(
            int.MaxValue,
            MinimumLength = DataConstants.CommentConstants.MinContentLength)]
        public string Content { get; set; }

        public int AnswerId { get; set; }

        public QuestionRedirectServiceModel RedirectInfo { get; set; } = new QuestionRedirectServiceModel();
    }
}