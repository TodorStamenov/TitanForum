namespace TitaniumForum.Data.Contracts
{
    using System;
    using System.Threading.Tasks;

    public interface IUnitOfWork : IDisposable
    {
        void Save();

        Task SaveAsync();
    }
}