namespace TitaniumForum.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Question
    {
        public int Id { get; set; }

        [Required]
        [MinLength(DataConstants.QuestionConstants.MinTitleLength)]
        [MaxLength(DataConstants.QuestionConstants.MaxTitleLength)]
        public string Title { get; set; }

        [Required]
        [MinLength(DataConstants.QuestionConstants.MinContentLength)]
        public string Content { get; set; }

        public int Rating { get; set; }

        public DateTime DateAdded { get; set; }

        public int ViewCount { get; set; }

        public bool IsDeleted { get; set; }

        public int SubCategoryId { get; set; }

        public virtual SubCategory SubCategory { get; set; }

        public int AuthorId { get; set; }

        public virtual User Author { get; set; }

        public virtual List<Answer> Answers { get; set; } = new List<Answer>();

        public virtual List<UserQuestionVote> Votes { get; set; } = new List<UserQuestionVote>();

        public virtual List<TagQuestion> Tags { get; set; } = new List<TagQuestion>();
    }
}