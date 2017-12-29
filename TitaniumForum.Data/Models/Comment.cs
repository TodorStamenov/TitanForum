namespace TitaniumForum.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [MinLength(DataConstants.CommentConstants.MinContentLength)]
        public string Content { get; set; }

        public int Rating { get; set; }

        public DateTime DateAdded { get; set; }

        public int AnswerId { get; set; }

        public Answer Answer { get; set; }

        public int AuthorId { get; set; }

        public User Author { get; set; }

        public List<UserCommentVote> Votes { get; set; } = new List<UserCommentVote>();
    }
}