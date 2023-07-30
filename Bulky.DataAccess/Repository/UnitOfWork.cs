using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository category { get; private set; }
        public IProductRepository product { get; private set; }
        public ICompanyRepository company { get; private set; }
        public IShoppingCartRepository shoppingCart { get; private set; }
        public IApplicationUserRepository applicationUser { get; private set; }
        public IOrderHeaderRepository orderHeader { get; private set; }
        public IOrderDeatilsRepository orderDeatils { get; private set; }

        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            applicationUser = new ApplicationUserRepository(_db);
            shoppingCart = new ShoppingCartRepository(_db);
            category = new CategoryRepository(_db);
            product = new ProdcutRepository(_db);
            company=new CompanyRepository(_db);
            orderHeader=new OrderHeaderRepository(_db);
            orderDeatils=new OrderDeatilsRepository(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
