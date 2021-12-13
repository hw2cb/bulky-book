using BulkyBook.DataAcces.Data;
using BulkyBook.DataAcces.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAcces.Repository
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            CoverTypes = new CoverTypeRepository(_db);
            Products = new ProductRepository(_db);
            Companies = new CompanyRepository(_db);
            ApplicationUsers = new ApplicationUserRepository(_db);
            SP_CALL = new SP_Call(_db);
        }
        public ICategoryRepository Category { get; private set; }
        public ICoverTypeRepository CoverTypes { get; private set; }
        public IProductRepository Products { get; private set; }
        public ICompanyRepository Companies { get; private set; }
        public ISP_Call SP_CALL { get; private set; }

        public IApplicationUserRepository ApplicationUsers { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
