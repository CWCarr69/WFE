using Microsoft.EntityFrameworkCore;
using Timesheet.Domain;
using Timesheet.Domain.Repositories;

namespace Timesheet.Infrastructure.Persistence.Repositories
{
    internal class WriteRepository<TEntity> : IWriteRepository<TEntity> where TEntity : Entity
    {
        protected DbContext _context;
        private DbSet<TEntity> _entities;

        public WriteRepository(TimesheetDbContext dbContext)
        {
            this._context = dbContext;
        }

        public DbSet<TEntity> Entities
        {
            get
            {
                if (_entities is null)
                {
                    _entities = this._context.Set<TEntity>();
                }
                return _entities;
            }
        }

        public async Task Add(TEntity entity) => await this.Entities.AddAsync(entity);

        public void Delete(string id) {
            var entity = this.Entities.FirstOrDefault(e => e.Id == id);
            if (entity is not null)
            {
                this.Entities.Remove(entity);
            }
        }
    }
}
