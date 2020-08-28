using NLog.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookstore_API.Contracts
{
    public interface IBaseRepository<T> where T: class
    {
        Task<IList<T>> FindAll();

        Task<T> FindById(int id);

        Task<bool> Exists(int id);

        Task<bool> Create(T entity);
        
        Task<bool> Update(T entity);
        
        Task<bool> Delete(T entity);

        Task<bool> Save();

    }
}
