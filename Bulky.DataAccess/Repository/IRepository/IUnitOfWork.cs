﻿using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository category { get;}
        IProductRepository product { get;}
        ICompanyRepository company { get;}
        IShoppingCartRepository shoppingCart { get;}
        IApplicationUserRepository applicationUser { get;}
        IOrderHeaderRepository orderHeader { get;}
        IOrderDeatilsRepository orderDeatils { get;}
        void Save();
    }
}
