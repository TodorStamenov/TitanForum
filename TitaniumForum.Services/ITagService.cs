namespace TitaniumForum.Services
{
    using Models.Tags;
    using System.Collections.Generic;

    public interface ITagService
    {
        bool Exists(int id);

        IEnumerable<ListTagsServiceModel> ByQuestion(int questionId);
    }
}