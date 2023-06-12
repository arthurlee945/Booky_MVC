﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookyBook.DataAccess.Repository.IRepository
{
	public interface IRepository<T> where T: class
	{
		//T - Category or generic model
		IEnumerable<T> GetAll(Expression<Func<T, bool>>? expression = null, string? includeProperties = null);
		T Get(Expression<Func<T,bool>> expression, string? includeProperties = null, bool tracked = false);
		void Add(T entity);
		void Remove(T entity);
		void RemoveRange(IEnumerable<T> entities);
	}
}
