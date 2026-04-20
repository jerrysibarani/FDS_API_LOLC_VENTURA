using API.Data;
using API.IServices;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API.Services
{
    public class BaseService<TEntity> : IBaseServicee<TEntity> where TEntity : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public BaseService(AppDbContext DBContext)
        {
            _context = DBContext;
            _dbSet = _context.Set<TEntity>();
        }


        // Create
        public void Add(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }


        // Update
        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            _dbSet.UpdateRange(entities);
            _context.Entry(entities).State = EntityState.Modified;
        }


        // Delete
        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
            _context.Entry(entities).State = EntityState.Modified;
        }

        //EXECUTE 

        public void SaveChanges()
        {
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
        }


        // Transaction Management
        public async Task ExecuteInTransaction(Action action)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                action();
                await _context.SaveChangesAsync();
                transaction.Commit();
                _context.ChangeTracker.Clear();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }


        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await action();
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                _context.ChangeTracker.Clear();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        // Read
        public TEntity? GetByObjectId(object id)
        {
            return _dbSet.Find(id);
        }

        public async Task<TEntity?> GetByObjectIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public IQueryable<TEntity> GetByCondition(Expression<Func<TEntity, bool>> expression)
        {
            return _dbSet.Where(expression);
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbSet.AsQueryable().AsNoTracking();
        }
        public async Task<IQueryable<TEntity>> GetAllAsync()
        {
            return await Task.FromResult(_dbSet.AsQueryable().AsNoTracking());
        }

        public async Task<List<TEntity>> GetListAllAsync()
        {
            return await _dbSet.AsQueryable().AsNoTracking().ToListAsync();
        }

    }
}
