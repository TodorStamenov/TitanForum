namespace TitaniumForum.Data.Contracts
{
    using System;

    public interface IUnitOfWork : IDisposable
    {
        void Save();
    }
}