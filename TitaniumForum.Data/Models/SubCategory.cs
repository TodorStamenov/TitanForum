namespace TitaniumForum.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class SubCategory
    {
        public int Id { get; set; }

        [Required]
        [MinLength(DataConstants.SubCategotyConstants.MinNameLength)]
        [MaxLength(DataConstants.SubCategotyConstants.MaxNameLength)]
        public string Name { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public List<Question> Questions { get; set; } = new List<Question>();
    }
}