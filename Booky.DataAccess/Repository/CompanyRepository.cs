﻿using BookyBook.DataAccess.Data;
using BookyBook.DataAccess.Repository.IRepository;
using BookyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookyBook.DataAccess.Repository
{
    public class CompanyRepository: Repository<Company>,ICompanyRepository
    {
        public CompanyRepository(ApplicationDbContext ctx) : base(ctx) { }

        public void Update(Company obj)
        {
            _db.Update(obj);
        }
    }
}
