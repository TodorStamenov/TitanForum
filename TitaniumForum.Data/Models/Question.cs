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

        public int SubCategoryId { get; set; }

        public SubCategory SubCategory { get; set; }

        public int AuthorId { get; set; }

        public User Author { get; set; }

        public List<Answer> Answers { get; set; } = new List<Answer>();

        public List<UserQuestionVote> Votes { get; set; } = new List<UserQuestionVote>();

        public List<TagQuestion> Tags { get; set; } = new List<TagQuestion>();
    }
}