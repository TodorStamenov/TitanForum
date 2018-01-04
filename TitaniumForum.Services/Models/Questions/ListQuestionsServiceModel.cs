namespace TitaniumForum.Services.Models.Questions
{
    using System;

    public class ListQuestionsServiceModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string SubCategoryName { get; set; }

        public string AuthorUsername { get; set; }

        public string AuthorProfileImage { get; set; }

        public int ViewCount { get; set; }

        public int AnswersCount { get; set; }

        public int UpVotes { get; set; }

        public int DownVotes { get; set; }

        public DateTime DateAdded { get; set; }
    }
}