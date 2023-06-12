using BookyBook.DataAccess.Data;
using BookyBook.DataAccess.Repository.IRepository;
using BookyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookyBook.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
	{
		public ShoppingCartRepository(ApplicationDbContext ctx) : base(ctx) {}

		public void Update(ShoppingCart obj)
		{
			_db.Update(obj);
		}
	}
}
