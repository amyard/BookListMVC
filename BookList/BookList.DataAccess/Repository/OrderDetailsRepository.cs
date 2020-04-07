using BookList.DataAccess.Data;
using BookList.DataAccess.Repository.IRepository;
using BookList.Models;
using System.Linq;

namespace BookList.DataAccess.Repository
{
    public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderDetailsRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderDetails obj)
        {
            _db.Update(obj);          
        }
    }
}
