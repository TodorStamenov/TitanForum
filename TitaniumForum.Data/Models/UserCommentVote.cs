namespace TitaniumForum.Data.Models
{
    public class UserCommentVote
    {
        public int UserId { get; set; }

        public User User { get; set; }

        public int CommentId { get; set; }

        public Comment Comment { get; set; }

        public Direction Direction { get; set; }
    }
}