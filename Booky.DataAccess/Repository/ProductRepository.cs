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
			var objFromDb = _db.Products.FirstOrDefault(x => x.Id == obj.Id);
			if(objFromDb != null)
			{
				objFromDb.Title = obj.Title;
				objFromDb.Description = obj.Description;
				objFromDb.Price = obj.Price;
				objFromDb.Price50 = obj.Price50;
				objFromDb.Price100 = obj.Price100;
				objFromDb.ListPrice = obj.ListPrice;
				objFromDb.Author = obj.Author;
				objFromDb.ProductImages = obj.ProductImages;
				_db.Update(objFromDb);
			}
		}
	}
}
