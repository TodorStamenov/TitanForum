namespace TitaniumForum.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MinLength(DataConstants.CategoryConstants.MinNameLength)]
        [MaxLength(DataConstants.CategoryConstants.MaxNameLength)]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public virtual List<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
    }
}