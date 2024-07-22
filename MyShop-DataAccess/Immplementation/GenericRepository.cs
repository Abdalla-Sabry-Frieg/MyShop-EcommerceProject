using Microsoft.EntityFrameworkCore;
using MyShop_DataAccess.Data;
using MyShop_Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyShop_DataAccess.Immplementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
           _context = context;
            _dbSet = _context.Set<T>();
        }

        public void Add(T entity)
        {
           _dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> perdicat, string? includeWord)
        {
            IQueryable<T> query = _dbSet;
            if(perdicat != null) // Where
            {
                query = query.Where(perdicat);
            }
            if(includeWord != null) // Include
            {
                foreach (var item in includeWord.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            return query.ToList();
        }

        public T GetById(int id)
        {
           return _dbSet.Find(id);
        }

        public T GetByName(string name)
        {
            return _dbSet.Find(name);
        }

        public T GetFirstOrDefualt(Expression<Func<T, bool>>? perdicat, string? includeWord)
        {
            IQueryable<T> query = _dbSet;
            if (perdicat != null) // Where
            {
                query = query.Where(perdicat);
            }
            if (includeWord != null) // Include
            {
                foreach (var item in includeWord.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            return query.SingleOrDefault();
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
           _dbSet.RemoveRange(entities);
        }
    }
}
