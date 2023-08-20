using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq.Expressions;

namespace GhumGham_Nepal.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        ITransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Snapshot);
        void Commit();
        Task CommitAsync(bool isSoftDelete = true);
        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>Repository</returns>
        IRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class;
    }


    public interface IRepository<T> : IUnitOfWork where T : class
    {


        // Marks an entity as new
        void Add(T entity);
        Task AddAsync(T entity);
        void AddRange(List<T> itemList);
        // Marks an entity as modified
        void Update(T entity);
        // Marks an entity to be removed
        void UpdateRange(List<T> entity);

        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> where);
        // Get an entity by int id
        T GetById(int id);
        T FirstOrDefault();
        Task<T> GetByIdAsync(int id);
        Task<T> GetByCompositeKeyAsync(int id1, int id2);
        Task<T> GetByIdAsync(Guid id);
        Task<T> GetByIdAsync(long id);
        // Get an entity by int id as no tracking
        Task<T> GetByIdAsNoTrackingAsync(int id);

        // Get an entity using delegate
        T Get(Expression<Func<T, bool>> where);
        Task<T> GetAsNoTrackingAsync(Expression<Func<T, bool>> where);
        Task<T> GetAsync(Expression<Func<T, bool>> where);
        void DeleteList(IQueryable<T> itemList);

        void DeleteRange(List<T> itemList);
        // Gets all entities of type T
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> GetAll(Expression<Func<T, bool>> where);
        // Gets entities using delegate

        int CountAll(Expression<Func<T, bool>> where);
        IEnumerable<T> GetMany(Expression<Func<T, bool>> where);
        IEnumerable<T> GetManyAsNoTracking(Expression<Func<T, bool>> where);

        Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> where);

        bool IsExist(Expression<Func<T, bool>> where);


        IQueryable<T> Table { get; }

        IQueryable<T> TableNoTracking { get; }

        /// <summary>
        /// Generic find by predicate and option to include child entity
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="includeProperties">The include sub-entity.</param>
        /// <returns>Entity</returns>     
        Task<T> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        // 1. SqlQuery approach
        //ICollection<T> ExcuteSqlQuery(string sqlQuery, CommandType commandType, SqlParameter[] parameters = null);

        //ICollection<TOutput> ExcuteSqlQuery<TOutput>(string sqlQuery, CommandType commandType,
        //    SqlParameter[] parameters = null);
        // 2. SqlCommand approach
        // void ExecuteNonQuery(string commandText, CommandType commandType, SqlParameter[] parameters = null);

        //  ICollection<T> ExecuteReader(string commandText, CommandType commandType, SqlParameter[] parameters = null);

        // ICollection<TOutput> ExecuteReader<TOutput>(string commandText, CommandType commandType, SqlParameter[] parameters = null) where TOutput : class;
        Task<ICollection<TOutput>> ExecuteReaderAsync<TOutput>(string commandText, CommandType commandType, SqlParameter[] parameters = null) where TOutput : class;

        /// <summary>
        /// Get all entities with selected properties only
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="select"></param>
        /// <param name="asNoTracking"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        Task<List<TType>> GetAllAsync<TType>(Expression<Func<T, TType>> select, bool asNoTracking = false, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);

    }
}
