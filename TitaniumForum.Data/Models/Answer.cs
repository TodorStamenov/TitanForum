namespace TitaniumForum.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Answer
    {
        public int Id { get; set; }

        [Required]
        [MinLength(DataConstants.AnswerConstants.MinContentLength)]
        public string Content { get; set; }

        public DateTime DateAdded { get; set; }

        public int QuestionId { get; set; }

        public Question Question { get; set; }

        public int AuthorId { get; set; }

        public User Author { get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();

        public List<UserAnswerVote> Votes { get; set; } = new List<UserAnswerVote>();
    }
}