namespace TitaniumForum.Data.Models
{
    public class TagQuestion
    {
        public int TagId { get; set; }

        public virtual Tag Tag { get; set; }

        public int QuestionId { get; set; }

        public virtual Question Question { get; set; }
    }
}