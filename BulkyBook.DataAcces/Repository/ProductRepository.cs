using BulkyBook.DataAcces.Data;
using BulkyBook.DataAcces.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAcces.Repository
{
    public class ProductRepository :Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            var objFromDb = _db.Products.FirstOrDefault(p => p.Id == product.Id);
            if(objFromDb != null)
            {
                if(product.ImageUrl != null)
                {
                    objFromDb.ImageUrl = product.ImageUrl;
                }
                objFromDb.ISBN = product.ISBN;
                objFromDb.Title = product.Title;
                objFromDb.Price = product.Price;
                objFromDb.LastPrice = product.LastPrice;
                objFromDb.Price50 = product.Price50;
                objFromDb.Price100 = product.Price100;
                objFromDb.Description = product.Description;
                objFromDb.CategoryId = product.CategoryId;
                objFromDb.CoverTypeId = product.CoverTypeId;
                objFromDb.Author = product.Author;
            }
        }
    }
}
