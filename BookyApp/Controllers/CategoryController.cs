﻿using BookyApp.Data;
using BookyApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookyApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> objCatList = _db.Categories.ToList();
            return View(objCatList);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            _db.Categories.Add(obj);
            _db.SaveChanges();
            //_db.Categories.Where(x => x.Name == "MOnkey");
            //return RedirectToAction("Index", "Category");
            return RedirectToAction("Index");
        }
    }
}
