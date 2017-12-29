namespace TitaniumForum.Data.Models
{
    public class UserQuestionVote
    {
        public int UserId { get; set; }

        public User User { get; set; }

        public int QuestionId { get; set; }

        public Question Question { get; set; }

        public Direction Direction { get; set; }
    }
}