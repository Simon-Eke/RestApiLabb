using Microsoft.EntityFrameworkCore;
using RestApiLabb.Models;

namespace RestApiLabb.Data
{
    public class RestApiLabbDbContext : DbContext
    {
        public RestApiLabbDbContext(DbContextOptions<RestApiLabbDbContext> options) : base(options)
        {
            
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Experience> Experiences { get; set; }
    }
}
