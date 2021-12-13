using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAcces.Repository.IRepository
{
    public interface IUnitOfWork: IDisposable
    {
        ICategoryRepository Category { get; }
        ICoverTypeRepository CoverTypes { get; }
        IProductRepository Products { get; }
        ICompanyRepository Companies { get; }
        IApplicationUserRepository ApplicationUsers { get; }
        ISP_Call SP_CALL { get; }
        void Save();
    }
}
