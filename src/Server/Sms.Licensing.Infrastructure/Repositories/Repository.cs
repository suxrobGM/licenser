using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sms.Licensing.Core.Entities.Abstractions;
using Sms.Licensing.Core.Repositories.Abstractions;
using Sms.Licensing.Infrastructure.Data;

namespace Sms.Licensing.Infrastructure.Repositories
{
    /// <summary>
    /// Generic repository.
    /// </summary>
    /// <typeparam name="TEntity">Class that implements IEntityBase interface</typeparam>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntityBase
    {
        private readonly ApplicationDbContext _context;

        // ReSharper disable once MemberCanBeProtected.Global
        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get entity object by its ID.
        /// </summary>
        /// <param name="id">Entity primary key</param>
        /// <returns>Entity object</returns>
        public Task<TEntity> GetByIdAsync(object id)
        {
            return _context.Set<TEntity>().FindAsync(id).AsTask();
        }

        /// <summary>
        /// Get entity object by predicate.
        /// </summary>
        /// <param name="predicate">Predicate to filter query</param>
        /// <returns>Entity object</returns>
        public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Get list of entity objects
        /// </summary>
        /// <param name="predicate">Predicate to filter query</param>
        /// <returns>List of entity objects</returns>
        public Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            return predicate == null ? _context.Set<TEntity>().ToListAsync() : _context.Set<TEntity>().Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Get IQueryable entity objects.
        /// </summary>
        /// <param name="predicate">Predicate to filter query</param>
        /// <returns>IQueryable entity objects</returns>
        public IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> predicate = null)
        {
            return predicate != null ? _context.Set<TEntity>().Where(predicate) : _context.Set<TEntity>();
        }

        /// <summary>
        /// Add new entry to database.
        /// </summary>
        /// <param name="entity">Entity object</param>
        /// <returns>Task</returns>
        public Task AddAsync(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            return _context.SaveChangesAsync();
        }

        /// <summary>
        /// Update existing entry.
        /// </summary>
        /// <param name="entity">Entity object</param>
        /// <returns>Task</returns>
        public Task UpdateAsync(TEntity entity)
        {
            _context.Set<TEntity>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return _context.SaveChangesAsync();
        }

        /// <summary>
        /// Delete entry from database by its ID.
        /// </summary>
        /// <param name="id">Primary key of entity</param>
        /// <returns>Task</returns>
        public Task DeleteByIdAsync(object id)
        {
            if (id == null)
            {
                return Task.CompletedTask;
            }

            var entity = _context.Set<TEntity>().Find(id);
            return DeleteAsync(entity);
        }

        /// <summary>
        /// Delete entity object from database.
        /// </summary>
        /// <param name="entity">Entity object</param>
        /// <returns>Task</returns>
        public Task DeleteAsync(TEntity entity)
        {
            if (entity == null)
            {
                return Task.CompletedTask;
            }

            _context.Set<TEntity>().Remove(entity);
            return _context.SaveChangesAsync();
        }
    }
}