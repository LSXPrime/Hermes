using System.Linq.Expressions;

namespace Hermes.Domain.Interfaces;

public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Retrieves an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <returns>
    /// The entity with the specified ID, or null if no such entity exists.
    /// </returns>
    Task<T?> GetByIdAsync(int id);
    
    /// <summary>
    /// Retrieves entities by their IDs.
    /// </summary>
    /// <param name="ids">The IDs of the entities to retrieve.</param>
    /// <returns>
    /// The entities with the specified IDs, or null if no such entities exist.
    /// </returns>
    Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<int> ids);
    
    /// <summary>
    /// Retrieves an entity by its GUID.
    /// </summary>
    /// <param name="guid">The GUID of the entity to retrieve that generated on entity creation.</param>
    /// <returns>
    /// The entity with the specified GUID, or null if no such entity exists.
    /// </returns>
    Task<T?> GetByGuidAsync(Guid guid);
    
    /// <summary>
    /// Retrieves all entities of the specified type.
    /// </summary>
    /// <returns>
    /// An IQueryable collection of all entities of the specified type.
    /// </returns>
    IQueryable<T> GetAllAsync();

    /// <summary>
    /// Retrieves entities that match the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter entities by.</param>
    /// <param name="includeProperties">The properties to include in the query.</param>
    /// <returns>
    /// An IQueryable collection of entities that satisfy the predicate.
    /// </returns>
    IQueryable<T> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
    
    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    Task<T> AddAsync(T entity);
    
    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    Task UpdateAsync(T entity);
    
    /// <summary>
    /// Deletes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    Task DeleteAsync(T entity);
    
    /// <summary>
    /// Checks if an entity with the specified ID exists in the repository.
    /// </summary>
    /// <param name="id">The ID of the entity to check.</param>
    /// <returns>
    /// A task representing the asynchronous operation that returns true if the entity exists, false otherwise.
    /// </returns>
    Task<bool> ExistsAsync(int id);
    
    /// <summary>
    /// Executes a LINQ query and returns the results as an IEnumerable.
    /// </summary>
    /// <param name="query">The LINQ query to execute.</param>
    /// <returns>
    /// An IEnumerable containing the results of the LINQ query.
    /// </returns>
    Task<IEnumerable<T>> ExecuteQueryAsync(IQueryable<T> query);

    /// <summary>
    /// Executes a LINQ query and returns the results as an Entity.
    /// </summary>
    /// <param name="query">The LINQ query to execute.</param>
    /// <returns>
    /// An Entity containing the results of the LINQ query.
    /// </returns>
    Task<T?> ExecuteQuerySingleAsync(IQueryable<T> query);
    
    /// <summary>
    /// Updates a property value of an entity in the database.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="predicate">The property to update.</param>
    /// <param name="newValue">The new value for the property.</param>
    /// <param name="originalValue">If true, sets the original value of the property. Otherwise, sets the current value.</param>
    void EntryPropertyChange(T entity, Expression<Func<T, object>> predicate, object newValue, bool originalValue);
}