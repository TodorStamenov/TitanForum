namespace TitaniumForum.Services.Models.Tags
{
    using Common.Mapping;
    using Data.Models;

    public class ListTagsServiceModel : IMapFrom<Tag>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}