namespace TitaniumForum.Data.Models
{
    public class UserQuestionVote
    {
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public int QuestionId { get; set; }

        public virtual Question Question { get; set; }

        public Direction Direction { get; set; }
    }
}