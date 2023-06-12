using BookyBook.DataAccess.Data;
using BookyBook.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookyBook.DataAccess.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		public ICategoryRepository Category { get; private set; }
		public IProductRepository Product { get; private set; }
		public ICompanyRepository Company { get; private set; }
		public IShoppingCartRepository ShoppingCart { get; private set; }
		public IApplicationUserRepository ApplicationUser { get; private set; }
		public IOrderHeaderRepository OrderHeader { get; private set; }
		public IOrderDetailRepository OrderDetail { get; private set; }

		protected readonly ApplicationDbContext _db;
		public UnitOfWork(ApplicationDbContext db)
		{
			_db = db;
			Category = new CategoryRepository(db);
			Product = new ProductRepository(db);
			Company = new CompanyRepository(db);
			ShoppingCart = new ShoppingCartRepository(db);
			ApplicationUser = new ApplicationUserRepository(db);
			OrderHeader = new OrderHeaderRepository(db);
			OrderDetail = new OrderDetailRepository(db);
		}
		public void Save()
		{
			_db.SaveChanges();
		}
	}
}
