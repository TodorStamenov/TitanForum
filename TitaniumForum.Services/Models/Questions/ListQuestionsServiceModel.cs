namespace TitaniumForum.Services.Models.Questions
{
    using System;

    public class ListQuestionsServiceModel : BaseQuestionServiceModel
    {
        public int ViewCount { get; set; }

        public int AnswersCount { get; set; }

        public string LastUserUsername { get; set; }

        public string LastUserProfileImage { get; set; }

        public DateTime? LastAnswerDate { get; set; }
    }
}