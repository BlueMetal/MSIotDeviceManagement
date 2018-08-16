using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MS.IoT.Domain.Interface
{
    public interface ICosmosDBRepository<T>
    {
        Task<T> GetItemAsync(string id);
        Task<bool> IsItemExistsByIdAsync(string Id);
        Task<bool> IsItemExistsByNonIdAsync(Expression<Func<T, bool>> where);
        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> select);
        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetItemsAsync();
        Task<string> CreateItemAsync(T item);
        Task<bool> UpdateItemAsync(string id, T item);
        Task<bool> DeleteItemAsync(string id);
        Task Initialize();
    }
}
