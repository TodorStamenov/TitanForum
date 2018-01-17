namespace TitaniumForum.Services.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Data;
    using Models.Tags;
    using System.Collections.Generic;
    using System.Linq;

    public class TagService : ITagService
    {
        private readonly UnitOfWork db;

        public TagService(UnitOfWork db)
        {
            this.db = db;
        }

        public bool Exists(int id)
        {
            return this.db
                .Tags
                .Get()
                .Any(t => t.Id == id);
        }

        public IEnumerable<ListTagsServiceModel> ByQuestion(int questionId)
        {
            return this.db
                .Tags
                .Get(filter: t => t.Questions.Any(q => q.QuestionId == questionId))
                .AsQueryable()
                .ProjectTo<ListTagsServiceModel>()
                .ToList();
        }
    }
}