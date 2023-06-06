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
	public class ProductRepository : Repository<Product>, IProductRepository
	{
		public ProductRepository(ApplicationDbContext db): base(db) { }

		public void Update(Product obj)
		{
			_db.Update(obj);
		}
	}
}
