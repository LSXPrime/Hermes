using System.Collections;
using System.Linq.Expressions;
using Hermes.Domain.Entities;
using Hermes.Domain.Interfaces;
using Hermes.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Hermes.Infrastructure.Repositories;

public abstract class GenericRepository<T>(HermesDbContext context) : IGenericRepository<T>
    where T : BaseEntity
{
    protected readonly HermesDbContext Context = context;

    /// <summary>
    /// Retrieves an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <returns>
    /// The entity with the specified ID, or null if no such entity exists.
    /// </returns>
    public async Task<T?> GetByIdAsync(int id)
    {
        return await Context.Set<T>().FindAsync(id);
    }
    
    /// <summary>
    /// Retrieves entities by their IDs.
    /// </summary>
    /// <param name="ids">The IDs of the entities to retrieve.</param>
    /// <returns>
    /// The entities with the specified IDs, or null if no such entities exist.
    /// </returns>
    public async Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return await Context.Set<T>().Where(x => ids.Contains(x.Id)).ToListAsync();
    }

    /// <summary>
    /// Retrieves an entity by its GUID.
    /// </summary>
    /// <param name="guid">The GUID of the entity to retrieve that generated on entity creation.</param>
    /// <returns>
    /// The entity with the specified GUID, or null if no such entity exists.
    /// </returns>
    public async Task<T?> GetByGuidAsync(Guid guid)
    {
        return await FindAsync(x => x.Guid == guid).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Retrieves all entities of the specified type.
    /// </summary>
    /// <returns>
    /// An IQueryable collection of all entities of the specified type.
    /// </returns>
    public IQueryable<T> GetAllAsync()
    {
        return Context.Set<T>().AsQueryable();
    }

    /// <summary>
    /// Retrieves entities that match the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter entities by.</param>
    /// <param name="includeProperties">The properties to include in the query.</param>
    /// <returns>
    /// An IQueryable collection of entities that satisfy the predicate.
    /// </returns>
    public IQueryable<T> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
    {
        var query = Context.Set<T>().Where(predicate);
        return includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
    }

    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    public async Task<T> AddAsync(T entity)
    {
        await Context.Set<T>().AddAsync(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    public async Task UpdateAsync(T entity)
    {
        Context.Set<T>().Update(entity);
        await Context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    public async Task DeleteAsync(T entity)
    {
        Context.Set<T>().Remove(entity);
        await Context.SaveChangesAsync();
    }

    /// <summary>
    /// Checks if an entity with the specified ID exists in the repository.
    /// </summary>
    /// <param name="id">The ID of the entity to check.</param>
    /// <returns>
    /// True if an entity with the specified ID exists in the repository, false otherwise.
    /// </returns>
    public async Task<bool> ExistsAsync(int id)
    {
        return await Context.Set<T>().FindAsync(id) != null;
    }

    /// <summary>
    /// Executes a LINQ query and returns the results as an IEnumerable.
    /// </summary>
    /// <param name="query">The LINQ query to execute.</param>
    /// <returns>
    /// An IEnumerable containing the results of the LINQ query.
    /// </returns>
    public async Task<IEnumerable<T>> ExecuteQueryAsync(IQueryable<T> query)
    {
        return await query.ToListAsync();
    }
    
    /// <summary>
    /// Executes a LINQ query and returns the results as an Entity.
    /// </summary>
    /// <param name="query">The LINQ query to execute.</param>
    /// <returns>
    /// An Entity containing the results of the LINQ query.
    /// </returns>
    public async Task<T?> ExecuteQuerySingleAsync(IQueryable<T> query)
    {
        return await query.FirstOrDefaultAsync();
    }

    /// <summary>
    /// Updates a property value of an entity in the database.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="predicate">The property to update.</param>
    /// <param name="newValue">The new value for the property.</param>
    /// <param name="originalValue">If true, sets the original value of the property. Otherwise, sets the current value.</param>
    public void EntryPropertyChange(T entity, Expression<Func<T, object>> predicate, object newValue, bool originalValue)
    {
        var propertyPath = predicate.Body.ToString();
        propertyPath = propertyPath.Substring(propertyPath.IndexOf('.') + 1).TrimEnd('.');

        if (originalValue)
            Context.Entry(entity).Property(propertyPath).OriginalValue = newValue;
        else
            Context.Entry(entity).Property(propertyPath).CurrentValue = newValue;
    }
}