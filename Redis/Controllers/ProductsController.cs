using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Redis.Models;
using Redis.Service;
using StackExchange.Redis;

namespace Redis.Controllers
{
    public class ProductsController : Controller
    {
        private readonly RedisService _service;
        private readonly IDatabase _db;
        private readonly RedisContext _context;
        private string _listKey = "products";

        public ProductsController(RedisService service, RedisContext context)
        {
            _service = service;
            _db = _service.GetDb(0);
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> plist;
            if (!_db.KeyExists(_listKey))
            {
                plist = await _context.Products.ToListAsync();
                string jsonSerialize = JsonConvert.SerializeObject(plist);
                await _db.StringSetAsync(_listKey, jsonSerialize, TimeSpan.FromMinutes(10));
                return View(plist);
            }
            string jsonDeserialize = await _db.StringGetAsync(_listKey);
            plist = JsonConvert.DeserializeObject<List<Product>>(jsonDeserialize);
            return View(plist);
        }

        public async Task SaveChange()
        {
            await _context.SaveChangesAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Product p)
        {
            await _context.Products.AddAsync(p);
            await SaveChange();
            await _db.KeyDeleteAsync(_listKey);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            Product p = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (p != null) _context.Products.Remove(p);
            await SaveChange();
            await _db.KeyDeleteAsync(_listKey);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int id)
        {
            Product p = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            return View(p);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Product product)
        {
            Product p = await _context.Products.FirstOrDefaultAsync(x => x.Id == product.Id);
            if (p != null) _context.Products.Update(p);
            
            p.Name = product.Name;
            p.Price = product.Price;
            await SaveChange();
            await _db.KeyDeleteAsync(_listKey);
            return RedirectToAction("Index");
        }
    }
}