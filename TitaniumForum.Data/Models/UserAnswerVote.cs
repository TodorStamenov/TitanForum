namespace TitaniumForum.Data.Models
{
    public class UserAnswerVote
    {
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public int AnswerId { get; set; }

        public virtual Answer Answer { get; set; }

        public Direction Direction { get; set; }
    }
}