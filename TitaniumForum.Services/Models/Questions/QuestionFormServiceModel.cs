namespace TitaniumForum.Services.Models.Questions
{
    using Data;
    using System.ComponentModel.DataAnnotations;

    public class QuestionFormServiceModel
    {
        [Required]
        [StringLength(
            DataConstants.QuestionConstants.MaxTitleLength,
            MinimumLength = DataConstants.QuestionConstants.MinTitleLength)]
        public string Title { get; set; }

        [Required]
        [StringLength(
            int.MaxValue,
            MinimumLength = DataConstants.QuestionConstants.MinContentLength)]
        public string Content { get; set; }

        [Required]
        [StringLength(
            int.MaxValue,
            MinimumLength = DataConstants.TagConstants.MinNameLength)]
        public string Tags { get; set; }
    }
}