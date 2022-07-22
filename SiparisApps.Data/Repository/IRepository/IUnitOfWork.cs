using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiparisApps.Data.Repository.IRepository
{
    public interface IUnitOfWork /*: IDisposable *///Disposable iş bitince  ramde tutulmasını engeller
    {
        IAppUserRepository AppUser { get; }
        ICartRepository Cart { get; }
        ICategoryRepository Category { get; } 
        IOrderDetailsRepository OrderDetails { get; }
        IOrderProductRepository OrderProduct { get; }
        IProductRepository Product { get; }


        void Save();


    }
}
