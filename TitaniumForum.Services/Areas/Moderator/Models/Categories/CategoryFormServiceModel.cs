namespace TitaniumForum.Services.Areas.Moderator.Models.Categories
{
    using Data;
    using System.ComponentModel.DataAnnotations;

    public class CategoryFormServiceModel
    {
        [Required]
        [StringLength(
            DataConstants.CategoryConstants.MaxNameLength,
            MinimumLength = DataConstants.CategoryConstants.MinNameLength)]
        public string Name { get; set; }
    }
}