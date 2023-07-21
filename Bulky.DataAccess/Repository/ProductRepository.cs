using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class ProdcutRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProdcutRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Product product)
        {
            var productFromDB = _db.Products.FirstOrDefault(p => p.Id == product.Id);
            if (productFromDB != null)
            {
                productFromDB.Title = product.Title;
                productFromDB.ISBN = product.ISBN;
                productFromDB.Price = product.Price;
                productFromDB.Price50 = product.Price50;
                productFromDB.ListPrice = product.ListPrice;
                productFromDB.Price100 = product.Price100;
                productFromDB.Description = product.Description;
                productFromDB.CategoryId = product.CategoryId;
                productFromDB.Author = product.Author;
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    productFromDB.ImageUrl = product.ImageUrl;
                }
            }            
        }
    }
}
