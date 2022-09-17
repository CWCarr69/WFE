using Timesheet.Domain;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Repositories;

namespace Timesheet.Infrastructure.Persistence.Queries
{
    internal abstract class ReadRepository<TEntity> : IReadRepository<TEntity> where TEntity : Entity
    {
        protected readonly TimesheetDbContext _context;

        public ReadRepository(TimesheetDbContext context)
        {
            this._context = context;
        }

        public async Task<TEntity?> Get(string identifier)
        {
            var entity = await _context.Set<TEntity>().FindAsync(identifier);
            if(entity == null)
            {
                throw new EntityNotFoundException<TEntity>(identifier);
            }
            return entity;
        }

        public IEnumerable<TEntity> Get() => _context.Set<TEntity>();
    }
}
