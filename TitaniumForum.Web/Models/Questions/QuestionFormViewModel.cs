namespace TitaniumForum.Web.Models.Questions
{
    using Services.Models.Questions;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class QuestionFormViewModel
    {
        public QuestionFormServiceModel Question { get; set; }

        [Display(Name = "Sub Category")]
        public int SubCategoryId { get; set; }

        public IEnumerable<SelectListItem> SubCategories { get; set; }
    }
}