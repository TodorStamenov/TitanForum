namespace TitaniumForum.Services.Models.Comments
{
    using System;

    public class ListCommentsServiceModel
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public string AuthorUsername { get; set; }

        public string AuthorProfileImage { get; set; }

        public int Rating { get; set; }

        public bool IsOwner { get; set; }

        public bool HasVoted { get; set; }

        public int UpVotes { get; set; }

        public int DownVotes { get; set; }

        public DateTime DateAdded { get; set; }
    }
}