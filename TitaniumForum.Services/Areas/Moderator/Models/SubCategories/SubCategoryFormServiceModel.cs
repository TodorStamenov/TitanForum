namespace TitaniumForum.Services.Areas.Moderator.Models.SubCategories
{
    using Data;
    using System.ComponentModel.DataAnnotations;

    public class SubCategoryFormServiceModel
    {
        [Required]
        [StringLength(
            DataConstants.SubCategotyConstants.MaxNameLength,
            MinimumLength = DataConstants.SubCategotyConstants.MinNameLength)]
        public string Name { get; set; }
    }
}