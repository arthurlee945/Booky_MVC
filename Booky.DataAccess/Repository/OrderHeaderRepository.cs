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
	public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
	{
		public OrderHeaderRepository(ApplicationDbContext ctx) : base(ctx) {}

		public void Update(OrderHeader obj)
		{
			_db.OrderHeaders.Update(obj);
		}
		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
			OrderHeader oh = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
			if(oh != null)
			{
				oh.OrderStatus = orderStatus;
				if(!string.IsNullOrEmpty(paymentStatus))
				{
					oh.PaymentStatus = paymentStatus;
				}
			}
		}

		public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
		{
			OrderHeader oh = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
			if(!string.IsNullOrEmpty(sessionId))
			{
				oh.SessionId = sessionId;
			}
			if (!string.IsNullOrEmpty(paymentIntentId))
			{
				oh.PaymentIntentId = paymentIntentId;
				oh.PaymentDate = DateTime.Now;
			}
		}

	}
}
