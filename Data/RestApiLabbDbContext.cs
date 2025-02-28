using Microsoft.EntityFrameworkCore;

namespace RestApiLabb.Data
{
    public class RestApiLabbDbContext : DbContext
    {
        public RestApiLabbDbContext(DbContextOptions<RestApiLabbDbContext> options) : base(options)
        {
            
        }
    }
}
