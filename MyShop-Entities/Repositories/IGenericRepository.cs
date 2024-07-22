using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyShop_Entities.Repositories
{
    public interface IGenericRepository<T> where T : class
    {   
        // Include & Where . to list()
        IEnumerable<T> GetAll(Expression<Func<T,bool>>? perdicat = null, string? includeWord = null);
        // Include & Where . to single or defuat
        T GetFirstOrDefualt(Expression<Func<T, bool>>? perdicat = null, string? includeWord = null);
        T GetById(int id);
        T GetByName(string name);
        void Delete(T entity);
        void Add(T entity);
        void RemoveRange(IEnumerable<T> entities);


    }
}
