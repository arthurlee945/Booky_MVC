using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BookyBook.DataAccess.Data;
using BookyBook.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BookyBook.DataAccess.Repository
{
	public class Repository<T> : IRepository<T> where T : class
	{
		protected readonly ApplicationDbContext _db;
		internal DbSet<T> dbSet;
		public Repository(ApplicationDbContext db)
		{
			_db = db;
			this.dbSet = _db.Set<T>();
			//_db.Products.Include(u => u.Category);
		}
		public void Add(T entity)
		{
			dbSet.Add(entity);
		}

		public T Get(System.Linq.Expressions.Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
		{

			IQueryable<T> query = tracked? dbSet : dbSet.AsNoTracking();
			if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach (string includeProp in includeProperties
					.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includeProp);
				}
			}
			return query.Where(filter).FirstOrDefault();
		}

		public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
		{

			IQueryable<T> query = dbSet;
            query = filter == null ? query : query.Where(filter);
			if(!string.IsNullOrEmpty(includeProperties))
			{
				foreach(string includeProp in includeProperties
					.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includeProp);
				}
			}
			return query.ToList();
		}

		public void Remove(T entity)
		{
			dbSet.Remove(entity);
		}

		public void RemoveRange(IEnumerable<T> entities)
		{
			dbSet.RemoveRange(entities);
		}
	}
}
