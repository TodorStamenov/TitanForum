﻿namespace TitaniumForum.Data.Models
{
    using IdentityModels;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class User : IdentityUser<int, UserLogin, UserRole, UserClaim>
    {
        public int PostsCount { get; set; }

        public int Rating { get; set; }

        [MaxLength(DataConstants.UserConstants.MaxProfileImageSize)]
        public byte[] ProfileImage { get; set; }

        public List<Question> Questions { get; set; } = new List<Question>();

        public List<Answer> Answers { get; set; } = new List<Answer>();

        public List<Comment> Comments { get; set; } = new List<Comment>();

        public List<UserQuestionVote> QuestionVotes { get; set; } = new List<UserQuestionVote>();

        public List<UserAnswerVote> AnswerVotes { get; set; } = new List<UserAnswerVote>();

        public List<UserCommentVote> CommentVotes { get; set; } = new List<UserCommentVote>();

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}