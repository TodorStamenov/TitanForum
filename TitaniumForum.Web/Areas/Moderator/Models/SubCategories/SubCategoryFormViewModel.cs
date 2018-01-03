namespace TitaniumForum.Web.Areas.Moderator.Models.SubCategories
{
    using Services.Areas.Moderator.Models.SubCategories;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class SubCategoryFormViewModel
    {
        public SubCategoryFormServiceModel SubCategory { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}