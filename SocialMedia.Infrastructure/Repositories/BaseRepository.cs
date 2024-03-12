using Microsoft.EntityFrameworkCore;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Interfaces;
using SocialMedia.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.Infrastructure.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T: BaseEntity
    {
        private readonly SocialMediaContext _context;
        protected readonly DbSet<T> _entities;
        public BaseRepository(SocialMediaContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }
        //public async Task<IEnumerable<T>> GetAll()
        public IEnumerable<T> GetAll()
        {
            //return await _entities.ToListAsync();
            return _entities.AsEnumerable();
        }

        public async Task<T> GetById(int id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task Add(T entity)
        {
            await _entities.AddAsync(entity);
            //El SaveChanges ahora lo maneja el UnitOfWork
            //await _context.SaveChangesAsync();
        }

        //Se quitó la asincronía porque no hay un método Update asíncrono
        //public async Task Update(T entity)
        public void Update(T entity)
        {
            _entities.Update(entity);
            //El SaveChanges ahora lo maneja el UnitOfWork
            //await _context.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            T entity = await GetById(id);
            _entities.Remove(entity);
            //El SaveChanges ahora lo maneja el UnitOfWork
            //await _context.SaveChangesAsync();
        }
    }
}
