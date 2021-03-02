using DAL.Entity;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class Context: DbContext
    {
        public DbSet<Animal> Animals { get; set; }
        public DbSet<Coordinate> Coordinates { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RegisteredAnimal> RegisteredAnimals { get; set; }

        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }
    }
}
