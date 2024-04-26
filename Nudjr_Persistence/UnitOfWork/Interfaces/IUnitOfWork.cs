using Nudjr_Persistence.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_Persistence.UnitOfWork.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepositoryExtension<TEntity> GetRepository<TEntity>() where TEntity : class;

        int SaveChanges();

        Task<int> SaveChangesAsync();
    }

}
