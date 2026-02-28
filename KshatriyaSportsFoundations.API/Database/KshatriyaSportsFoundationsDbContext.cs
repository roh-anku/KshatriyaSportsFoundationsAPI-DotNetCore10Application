using KshatriyaSportsFoundations.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace KshatriyaSportsFoundations.API.Database
{
    public class KshatriyaSportsFoundationsDbContext:DbContext
    {
        public KshatriyaSportsFoundationsDbContext(DbContextOptions<KshatriyaSportsFoundationsDbContext> options):base(options)
        {         
        }

        public DbSet<EnquiryDomain> Enquiries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //
        }
    }
}
