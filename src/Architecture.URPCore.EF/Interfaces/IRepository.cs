using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;

namespace Architecture.URPCore.EF.Interfaces
{
    /// <summary>
    /// Repository interface
    /// </summary>
    public interface IRepository<T> where T : class/*, IEntity*/
    {
        /// <summary>
        /// Gets the entity by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<T> GetAsync(object id);

        /// <summary>
        /// Finds the entity by specified predicate (optionally: order by columns and include properties).
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="include">The include.</param>
        /// <returns></returns>
        Task<T> FindAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);

        /// <summary>
        /// List the entities by specified predicate (optionally: order by columns and include properties).
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="include">The include.</param>
        /// <returns></returns>
        Task<ICollection<T>> ListAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);

        /// <summary>
        /// Adds the specific entity asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Deletes the specific entity asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        Task<int> DeleteAsync(T entity);

        /// <summary>
        /// Updates the specific entity asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        Task<T> UpdateAsync(T entity);
    }
}
