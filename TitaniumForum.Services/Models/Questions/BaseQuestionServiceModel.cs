namespace TitaniumForum.Services.Models.Questions
{
    using System;

    public class BaseQuestionServiceModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int SubCategoryId { get; set; }

        public string SubCategoryName { get; set; }

        public string AuthorUsername { get; set; }

        public string AuthorProfileImage { get; set; }

        public int UpVotes { get; set; }

        public int DownVotes { get; set; }

        public DateTime DateAdded { get; set; }
    }
}