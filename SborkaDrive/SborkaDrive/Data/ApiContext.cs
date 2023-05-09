using Microsoft.EntityFrameworkCore;
using SborkaDrive.Models;

namespace SborkaDrive.Data
{
    public class ApiContext : DbContext
    {
        public DbSet<LayoutFile> Files { get; set; }

        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {
            
        }
    }
}
