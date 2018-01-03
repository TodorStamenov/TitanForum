namespace TitaniumForum.Data.Models
{
    public class UserCommentVote
    {
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public int CommentId { get; set; }

        public virtual Comment Comment { get; set; }

        public Direction Direction { get; set; }
    }
}