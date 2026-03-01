using System.Linq.Expressions;

namespace API.IServices
{
    public interface IBaseServicee<TEntity> where TEntity : class
    {
        // Create
        void Add(TEntity entity);

        void AddRange(IEnumerable<TEntity> entities);

        Task AddAsync(TEntity entity);

        Task AddRangeAsync(IEnumerable<TEntity> entities);


        // Update
        void Update(TEntity entity);

        void UpdateRange(IEnumerable<TEntity> entities);


        // Delete
        void Delete(TEntity entity);
        void DeleteRange(IEnumerable<TEntity> entities);

        //EXECUTE 

        void SaveChanges();

        Task SaveChangesAsync();


        // Transaction Management
        Task ExecuteInTransaction(Action action);

        Task ExecuteInTransactionAsync(Func<Task> action);


        // Read
        TEntity? GetByObjectId(object id);
        Task<TEntity?> GetByObjectIdAsync(object id);
        IQueryable<TEntity> GetByCondition(Expression<Func<TEntity, bool>> expression);
        IQueryable<TEntity> GetAll();
        Task<IQueryable<TEntity>> GetAllAsync();

        Task<List<TEntity>> GetListAllAsync();

    }
}
