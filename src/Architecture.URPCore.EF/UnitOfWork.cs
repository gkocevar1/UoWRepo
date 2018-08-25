using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Architecture.URPCore.EF.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Architecture.URPCore.EF
{
    /// <summary>
    /// Unit of work implementation.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public UnitOfWork(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Gets the database context.
        /// </summary>
        /// <value>
        /// The database context.
        /// </value>
        internal DbContext Context => _context;

        /// <summary>
        /// Saves the changes asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<int> SaveChangesAsync() =>
            await this.SaveChangesAsync(CancellationToken.None).ConfigureAwait(false);

        /// <summary>
        /// Saves the changes asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var changesAsync = await Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return changesAsync;
        }

        /// <summary>
        /// Rejects the changes asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task RejectChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var entry in Context.ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged))
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;

                    case EntityState.Modified:
                    case EntityState.Deleted:
                        await entry.ReloadAsync(cancellationToken).ConfigureAwait(false);
                        break;
                }
            }
        }

        /// <summary>
        /// Executes the SQL command asynchronous.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public virtual async Task<int> ExecuteSqlCommandAsync(string sql, IEnumerable<object> parameters, CancellationToken cancellationToken = default(CancellationToken))
            => await Context.Database.ExecuteSqlCommandAsync(sql, parameters, cancellationToken);

        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TRepository">The type of the repository.</typeparam>
        /// <returns></returns>
        public IRepository<T> Repository<T, TRepository>() where T : class/*, IEntity*/
        {
            if (_repositories == null)
            {
                _repositories = new Dictionary<string, dynamic>();
            }

            var type = typeof(T).Name;

            if (_repositories.ContainsKey(type))
            {
                return (IRepository<T>)_repositories[type];
            }

            var repositoryType = typeof(TRepository);

            // to instantiate generic repository
            //_repositories.Add(type, Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), new object[] { this }));
            _repositories.Add(type, Activator.CreateInstance(repositoryType, new object[] { this }));
            return _repositories[type];
        }

        #region Dispose
        private bool disposed = false;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (_context != null)
                    {
                        _context.Dispose();
                        _context = null;
                    }
                }
            }
            this.disposed = true;
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

        private DbContext _context;
        private Dictionary<string, dynamic> _repositories;
    }
}
