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

        public int Rating { get; set; }

        public DateTime DateAdded { get; set; }

        public bool IsDeleted { get; set; }

        public int QuestionId { get; set; }

        public virtual Question Question { get; set; }

        public int AuthorId { get; set; }

        public virtual User Author { get; set; }

        public virtual List<Comment> Comments { get; set; } = new List<Comment>();

        public virtual List<UserAnswerVote> Votes { get; set; } = new List<UserAnswerVote>();
    }
}