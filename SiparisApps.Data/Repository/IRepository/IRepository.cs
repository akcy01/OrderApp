using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SiparisApps.Data.Repository.IRepository
{
    public interface IRepository<T> where T : class //T buraya gelecek olan modeli temsil eder category,orderproduct vs vs.
    {
        void Add(T entity);
        T GetFirstOrDefault(Expression<Func<T,bool>> filter,string? includeProperties = null); /* sorgu vs yapmak için oldu bu link sorgusuna olanak sağlayan bir yapı bu.*/
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null , string? includeProperties = null);
        void Update(T entity); 
        void Remove(T entity); //Crud işlemleri 
        void RemoveRange(IEnumerable<T> entities);
    }

}
