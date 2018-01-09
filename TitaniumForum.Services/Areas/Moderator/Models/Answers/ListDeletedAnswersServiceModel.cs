namespace TitaniumForum.Services.Areas.Moderator.Models.Answers
{
    using System;

    public class ListDeletedAnswersServiceModel
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public string AuthorUsername { get; set; }

        public string AuthorProfileImage { get; set; }

        public int Rating { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsQuestionDeleted { get; set; }

        public int QuestionId { get; set; }

        public int UpVotes { get; set; }

        public int DownVotes { get; set; }

        public DateTime DateAdded { get; set; }
    }
}