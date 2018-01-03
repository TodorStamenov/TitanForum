namespace TitaniumForum.Services.Areas.Moderator.Models.Categories
{
    using Common.Mapping;
    using Data;
    using Data.Models;
    using System.ComponentModel.DataAnnotations;

    public class CategoryFormServiceModel : IMapFrom<Category>
    {
        [Required]
        [StringLength(
            DataConstants.CategoryConstants.MaxNameLength,
            MinimumLength = DataConstants.CategoryConstants.MinNameLength)]
        public string Name { get; set; }
    }
}