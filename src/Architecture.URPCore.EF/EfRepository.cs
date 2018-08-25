using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Architecture.URPCore.EF.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Architecture.URPCore.EF
{
    /// <summary>
    /// Entity framework repository implementation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IntegrationServices.Files.Core.Interfaces.IRepository{T}" />
    public abstract class EfRepository<T> : IRepository<T> where T : class/*, IEntity*/
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}" /> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <exception cref="ArgumentNullException">unitOfWork</exception>
        public EfRepository(DbContext context)
        {
            _context = context;

            if (_context != null)
            {
                _dbSet = _context.Set<T>();
            }
        }

        /// <summary>
        /// Adds the specific entity asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public async Task<T> AddAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Added;
            await _dbSet.AddAsync(entity).ConfigureAwait(false);

            return entity;
        }

        /// <summary>
        /// Deletes the specific entity asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }

            _context.Entry(entity).State = EntityState.Deleted;

            return await Task.FromResult(1).ConfigureAwait(false);
        }

        /// <summary>
        /// Finds the entity by specified predicate (optionally: order by columns and include properties).
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="include">The include.</param>
        /// <returns></returns>
        public async Task<T> FindAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            IQueryable<T> query = _dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                return await orderBy(query).SingleOrDefaultAsync().ConfigureAwait(false);
            }

            return await query.SingleOrDefaultAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// List the entities by specified predicate (optionally: order by columns and include properties).
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="include">The include.</param>
        /// <returns></returns>
        public async Task<ICollection<T>> ListAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            IQueryable<T> query = _dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync().ConfigureAwait(false);
            }

            return await query.ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the entity by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<T> GetAsync(object id)
        {
            return await _dbSet.FindAsync(id).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the specific entity asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;

            return await Task.FromResult(entity).ConfigureAwait(false);
        }

        #region Dispose
        private bool _disposed = false;
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                this._disposed = true;
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;
    }
}
