namespace TitaniumForum.Services.Implementations
{
    using Data.Contracts;
    using Models.Tags;
    using System.Collections.Generic;
    using System.Linq;

    public class TagService : Service, ITagService
    {
        public TagService(IDatabase database)
            : base(database)
        {
        }

        public bool Exists(int id)
        {
            return this.Database
                .Tags
                .Any(t => t.Id == id);
        }

        public IEnumerable<ListTagsServiceModel> ByQuestion(int questionId)
        {
            return this.Database
                .Tags
                .Project(
                    projection: t => new ListTagsServiceModel { Id = t.Id, Name = t.Name },
                    filter: t => t.Questions.Any(q => q.QuestionId == questionId));
        }
    }
}