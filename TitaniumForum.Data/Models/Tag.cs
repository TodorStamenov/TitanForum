namespace TitaniumForum.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Tag
    {
        public int Id { get; set; }

        [MinLength(DataConstants.TagConstants.MinNameLength)]
        [MaxLength(DataConstants.TagConstants.MaxNameLength)]
        public string Name { get; set; }

        public virtual List<TagQuestion> Questions { get; set; } = new List<TagQuestion>();
    }
}