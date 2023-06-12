using BookyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookyBook.DataAccess.Repository.IRepository
{
	public interface IOrderDetailRepository : IRepository<OrderDetail>
	{
		public void Update(OrderDetail obj);
	}
}
