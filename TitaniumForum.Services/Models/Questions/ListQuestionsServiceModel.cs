namespace TitaniumForum.Services.Models.Questions
{
    using System;

    public class ListQuestionsServiceModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int SubCategoryId { get; set; }

        public string SubCategoryName { get; set; }

        public string AuthorUsername { get; set; }

        public string AuthorProfileImage { get; set; }

        public int ViewCount { get; set; }

        public int AnswersCount { get; set; }

        public int UpVotes { get; set; }

        public int DownVotes { get; set; }

        public string LastUserUsername { get; set; }

        public string LastUserProfileImage { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime? LastAnswerDate { get; set; }
    }
}