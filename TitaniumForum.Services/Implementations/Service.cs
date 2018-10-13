namespace TitaniumForum.Services.Implementations
{
    using TitaniumForum.Data.Contracts;

    public abstract class Service
    {
        protected Service(IDatabase database)
        {
            this.Database = database;
        }

        protected IDatabase Database { get; }
    }
}