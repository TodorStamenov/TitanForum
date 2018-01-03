namespace TitaniumForum.Services.Areas.Moderator.Models.SubCategories
{
    using Common.Mapping;
    using Data;
    using Data.Models;
    using System.ComponentModel.DataAnnotations;

    public class SubCategoryFormServiceModel : IMapFrom<SubCategory>
    {
        [Required]
        [StringLength(
            DataConstants.SubCategotyConstants.MaxNameLength,
            MinimumLength = DataConstants.SubCategotyConstants.MinNameLength)]
        public string Name { get; set; }
    }
}