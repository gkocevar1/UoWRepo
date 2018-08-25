using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Architecture.URPCore.EF.Interfaces
{
    /// <summary>
    /// Unit of work interface.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TRepository">The type of the repository.</typeparam>
        /// <returns></returns>
        IRepository<TEntity> Repository<TEntity, TRepository>() where TEntity : class/*, IEntity*/;

        /// <summary>
        /// Saves the changes asynchronous.
        ///
        /// TODO: rename SaveChangesAsync to Commit
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Rejects the changes asynchronous.
        ///
        /// TODO: rename RejectChangesAsync to Rollback
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task RejectChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Executes the SQL command asynchronous.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<int> ExecuteSqlCommandAsync(string sql, IEnumerable<object> parameters, CancellationToken cancellationToken = default(CancellationToken));
    }
}
