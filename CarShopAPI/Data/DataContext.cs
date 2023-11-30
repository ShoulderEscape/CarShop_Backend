using Entites;
using Microsoft.EntityFrameworkCore;

namespace CarShopAPI.Data
{
    public class DataContext : DbContext
    { 
        public DataContext(DbContextOptions<DataContext>options) : base(options) 
        {



        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<User> Users { get; set; }

    }




}
