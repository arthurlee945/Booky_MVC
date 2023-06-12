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
	public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
	{
		public OrderDetailRepository(ApplicationDbContext ctx) : base(ctx) {}

		public void Update(OrderDetail obj)
		{
			_db.Update(obj);
		}
	}
}
