using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redis.Models
{
    public class RedisContext:DbContext
    {
        public RedisContext(DbContextOptions<RedisContext> options):base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
    }
}
