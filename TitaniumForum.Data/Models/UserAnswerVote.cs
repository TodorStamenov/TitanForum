namespace TitaniumForum.Data.Models
{
    public class UserAnswerVote
    {
        public int UserId { get; set; }

        public User User { get; set; }

        public int AnswerId { get; set; }

        public Answer Answer { get; set; }

        public Direction Direction { get; set; }
    }
}