using Microsoft.EntityFrameworkCore;
using SiparisApps.Data.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SiparisApps.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        //Dependency Injection aracılığıyla applicationDbContextleri tanımlıcaz şimdi
        private readonly ApplicationDbContext _context;
        internal DbSet<T> _dbSet; //Dbseti benden kalıtım alan sınıflarda da kullanmak istiyorum.

        //Dependency Injection gerçekleşmesi için Constructoru oluşturalım.
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
                


        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }



        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            if(filter != null)
            {
                query = query.Where(filter);  
            }

            if(includeProperties != null)
            {
                foreach (var item in includeProperties.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            return query.ToList();
        }


        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);  /* burda gelen expression sorgusunu where sorgusuna yazdık */
            }

            if (includeProperties != null)
            {
                foreach (var item in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            return query.FirstOrDefault();
        }

        public void Remove(T entity)
        {
           _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities); /* Birden fazla kayıt silme işlemi. */
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);   
        }
    }

}
