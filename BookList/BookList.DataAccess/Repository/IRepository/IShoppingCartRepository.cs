using BookList.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookList.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        void Update(ShoppingCart obj);
    }
}
