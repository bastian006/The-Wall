using Microsoft.EntityFrameworkCore;

namespace models.Models
{
    public class MyContext : DbContext{
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<Comment> Comments {get;set;}
        public DbSet<Message> Messages {get;set;}
        public DbSet<User> Users {get;set;}
    }
}