using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Data;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;
using GhumGhamNepal.Core.ApplicationDbContext;

namespace GhumGham_Nepal.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        protected ProjectContext _dbContext;

        public UnitOfWork(ProjectContext dbContext)
        {
            _dbContext = dbContext;
        }

        private Dictionary<Type, object> repositories;


        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        public IRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class
        {
            if (repositories == null)
            {
                repositories = new Dictionary<Type, object>();
            }

            var type = typeof(TEntity);
            if (!repositories.ContainsKey(type))
            {
                repositories[type] = new Repository<TEntity>(_dbContext);
            }

            return (IRepository<TEntity>)repositories[type];
        }

        public ITransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            return new DbTransaction(_dbContext.Database.CurrentTransaction ?? _dbContext.Database.BeginTransaction());
        }

        public void Commit()
        {
            _dbContext.SaveChanges();
        }

        public async Task CommitAsync(bool isSoftDelete = true)
        {
            try
            {
                await _dbContext.SaveChangesAsync(isSoftDelete).ConfigureAwait(false);
            }
            catch (Exception ex)
            { throw; }
        }
        /// <summary>
        /// Disposes the current object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes all external resources.
        /// </summary>
        /// <param name="disposing">The dispose indicator.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_dbContext != null)
                {
                    _dbContext.Dispose();
                    _dbContext = null;
                }
            }
        }

    }
    public class Repository<T> : UnitOfWork, IRepository<T> where T : class
    {
        #region Properties

        private readonly DbSet<T> _dbSet;


        #endregion

        public Repository(ProjectContext dbContext) : base(dbContext)
        {
            _dbSet = dbContext.Set<T>();
        }

        #region Implementation
        public virtual void Add(T entity)
        {
            _dbSet.Add(entity);
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual void Update(T entity)
        {
            try
            {
                _dbSet.Attach(entity);
                _dbContext.Entry(entity).State = EntityState.Modified;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual void UpdateRange(List<T> itemList)
        {
            foreach (T obj in itemList)
            {
                _dbSet.Attach(obj);
                _dbContext.Entry(obj).State = EntityState.Modified;
            }
        }
        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> objects = _dbSet.Where(where).AsEnumerable();
            foreach (T obj in objects)
                _dbSet.Remove(obj);
        }
        public virtual void DeleteList(IQueryable<T> itemList)
        {
            foreach (T obj in itemList)
                _dbSet.Remove(obj);
        }
        public virtual void DeleteRange(List<T> itemList)

        {
            _dbSet.RemoveRange(itemList);
        }
        public virtual void AddRange(List<T> itemList)

        {
            _dbSet.AddRange(itemList);
        }

        public virtual T GetById(int id)
        {
            return _dbSet.Find(id);
        }
        public virtual T FirstOrDefault()
        {
            return _dbSet.FirstOrDefault();
        }
        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        public virtual async Task<T> GetByCompositeKeyAsync(int id1, int id2)
        {
            return await _dbSet.FindAsync(id1, id2);
        }
        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }
        public virtual async Task<T> GetByIdAsync(long id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <inheritdoc/>
        public async Task<T> GetAsync(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = _dbSet.Where(where);
            foreach (var include in includeProperties)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync();
        }


        public virtual async Task<T> GetByIdAsNoTrackingAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
                _dbContext.Entry(entity).State = EntityState.Detached;
            return entity;
        }
        public virtual async Task<T> GetAsNoTrackingAsync(Expression<Func<T, bool>> where)
        {
            var entity = await _dbSet.Where(where).FirstOrDefaultAsync();

            if (entity != null)
                _dbContext.Entry(entity).State = EntityState.Detached;

            return entity;

        }
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync().ConfigureAwait(false);
        }

        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return _dbSet.Where(where).ToList();
        }
        public virtual IEnumerable<T> GetManyAsNoTracking(Expression<Func<T, bool>> where)
        {
            return _dbSet.AsNoTracking().Where(where).ToList();
        }


        public virtual async Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> where)
        {
            return await _dbSet.Where(where).ToListAsync().ConfigureAwait(false);
        }
        public T Get(Expression<Func<T, bool>> where)
        {
            return _dbSet.Where(where).FirstOrDefault();
        }
        public async Task<T> GetAsync(Expression<Func<T, bool>> where)
        {
            return await _dbSet.Where(where).FirstOrDefaultAsync();
        }
        public IQueryable<T> GetAll(Expression<Func<T, bool>> where)
        {
            return _dbSet.Where(where);
        }
        public int CountAll(Expression<Func<T, bool>> where)
        {
            return _dbSet.Count(where);
        }
        public bool IsExist(Expression<Func<T, bool>> where)
        {
            return _dbSet.Any(where);
        }
        /// <summary>
        /// Gets a table
        /// </summary>
        public virtual IQueryable<T> Table
        {
            get
            {
                return _dbSet;
            }
        }

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        public virtual IQueryable<T> TableNoTracking
        {
            get
            {
                return _dbSet.AsNoTracking();
            }
        }


        public async Task ExecuteNonQueryAsync(string commandText, CommandType commandType, SqlParameter[] parameters = null)
        {
            if (_dbContext.Database.GetDbConnection().State == ConnectionState.Closed)
            {
                await _dbContext.Database.OpenConnectionAsync();
            }
            var command = _dbContext.Database.GetDbConnection().CreateCommand();

            if (_dbContext.Database.CurrentTransaction != null)
                command.Transaction = _dbContext.Database.CurrentTransaction.GetDbTransaction();

            command.CommandText = commandText;
            command.CommandType = commandType;

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        public ICollection<T> ExecuteReader(string commandText, CommandType commandType, SqlParameter[] parameters = null)
        {
            if (_dbContext.Database.GetDbConnection().State == ConnectionState.Closed)
            {
                _dbContext.Database.OpenConnection();
            }

            var command = _dbContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = commandText;
            command.CommandType = commandType;

            if (_dbContext.Database.CurrentTransaction != null)
                command.Transaction = _dbContext.Database.CurrentTransaction.GetDbTransaction();

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }

            using var reader = command.ExecuteReader();
            if (!reader.HasRows)
                while (reader.NextResult()) { }

            var mapper = new DataReaderMapper<T>();
            return mapper.MapToList(reader);
        }
        public async Task<ICollection<TOutput>> ExecuteReaderAsync<TOutput>(string commandText, CommandType commandType, SqlParameter[] parameters = null) where TOutput : class
        {
            if (_dbContext.Database.GetDbConnection().State == ConnectionState.Closed)
            {
                _dbContext.Database.OpenConnection();
            }

            var command = _dbContext.Database.GetDbConnection().CreateCommand();

            if (_dbContext.Database.CurrentTransaction != null)
                command.Transaction = _dbContext.Database.CurrentTransaction.GetDbTransaction();

            command.CommandText = commandText;
            command.CommandType = commandType;

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }

            using DbDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            if (!reader.HasRows)
                while (await reader.NextResultAsync()) { }

            var mapper = new DataReaderMapper<TOutput>();
            return mapper.MapToList(reader);
        }
        public async Task ExecuteReaderAsync(string commandText, CommandType commandType, SqlParameter[] parameters = null)
        {
            if (_dbContext.Database.GetDbConnection().State == ConnectionState.Closed)
            {
                _dbContext.Database.OpenConnection();
            }

            var command = _dbContext.Database.GetDbConnection().CreateCommand();

            if (_dbContext.Database.CurrentTransaction != null)
                command.Transaction = _dbContext.Database.CurrentTransaction.GetDbTransaction();

            command.CommandText = commandText;
            command.CommandType = commandType;

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }

            using DbDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            if (!reader.HasRows)
                while (await reader.NextResultAsync()) { }


        }

        public ICollection<TOutput> ExecuteReader<TOutput>(string commandText, CommandType commandType, SqlParameter[] parameters = null) where TOutput : class
        {
            if (_dbContext.Database.GetDbConnection().State == ConnectionState.Closed)
            {
                _dbContext.Database.OpenConnection();
            }

            var command = _dbContext.Database.GetDbConnection().CreateCommand();

            if (_dbContext.Database.CurrentTransaction != null)
                command.Transaction = _dbContext.Database.CurrentTransaction.GetDbTransaction();

            command.CommandText = commandText;
            command.CommandType = commandType;

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }

            using DbDataReader reader = command.ExecuteReader();

            if (!reader.HasRows)
                while (reader.NextResult()) { }

            var mapper = new DataReaderMapper<TOutput>();
            return mapper.MapToList(reader);
        }
        public async Task<object> ExecuteScalarAsync(string commandText, CommandType commandType, SqlParameter[] parameters = null)
        {
            if (_dbContext.Database.GetDbConnection().State == ConnectionState.Closed)
            {
                _dbContext.Database.OpenConnection();
            }

            var command = _dbContext.Database.GetDbConnection().CreateCommand();

            if (_dbContext.Database.CurrentTransaction != null)
                command.Transaction = _dbContext.Database.CurrentTransaction.GetDbTransaction();

            command.CommandText = commandText;
            command.CommandType = commandType;

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }

            var reader = await command.ExecuteScalarAsync().ConfigureAwait(false);
            return reader;

        }
        protected virtual async Task<T> ExecuteReaderAsync<T>(Func<DbDataReader, T> mapEntities, string exec, SqlParameter[] parameters = null)
        {

            if (_dbContext.Database.GetDbConnection().State == ConnectionState.Closed)
            {
                _dbContext.Database.OpenConnection();
            }

            var command = _dbContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = exec;

            if (_dbContext.Database.CurrentTransaction != null)
                command.Transaction = _dbContext.Database.CurrentTransaction.GetDbTransaction();

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }

            using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);


            //NOTE Removed this because data didnot bind in multiset object if 1st result set is null.
            //TODO But this could result in error.
            //if (!reader.HasRows)
            //    while (await reader.NextResultAsync()) { } 

            T data = mapEntities(reader);
            return data;
        }
        protected virtual async Task<T> ExecuteReaderAsync<T>(Func<DbDataReader, T> mapEntities, string commandText, CommandType commandType, SqlParameter[] parameters = null)
        {

            if (_dbContext.Database.GetDbConnection().State == ConnectionState.Closed)
            {
                _dbContext.Database.OpenConnection();
            }

            var command = _dbContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = commandText;
            command.CommandType = commandType;

            if (_dbContext.Database.CurrentTransaction != null)
                command.Transaction = _dbContext.Database.CurrentTransaction.GetDbTransaction();

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }

            using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);


            //NOTE Removed this because data didnot bind in multiset object if 1st result set is null.
            //TODO But this could result in error.
            //if (!reader.HasRows)
            //    while (await reader.NextResultAsync()) { } 

            T data = mapEntities(reader);
            return data;
        }
        ///<inheritdoc/>
        public async Task<List<TType>> GetAllAsync<TType>(Expression<Func<T, TType>> select, bool asNoTracking = false, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            IQueryable<T> query = _dbSet;

            if (asNoTracking)
                query = query.AsNoTracking();

            if (orderBy != null)
                query = orderBy(query);

            return await query.Select(select).ToListAsync().ConfigureAwait(false);
        }
        #endregion

    }
}
