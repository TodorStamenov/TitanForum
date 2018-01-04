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

        public DateTime DateAdded { get; set; }

        public bool IsDeleted { get; set; }

        public int AnswerId { get; set; }

        public virtual Answer Answer { get; set; }

        public int AuthorId { get; set; }

        public virtual User Author { get; set; }

        public virtual List<UserCommentVote> Votes { get; set; } = new List<UserCommentVote>();
    }
}