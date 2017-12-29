namespace TitaniumForum.Data.Models
{
    public class TagQuestion
    {
        public int TagId { get; set; }

        public Tag Tag { get; set; }

        public int QuestionId { get; set; }

        public Question Question { get; set; }
    }
}