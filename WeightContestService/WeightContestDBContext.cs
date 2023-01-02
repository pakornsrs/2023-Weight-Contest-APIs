using Microsoft.EntityFrameworkCore;
using WeightContestService.Entities;

namespace WeightContestService
{
    public class WeightContestDBContext : DbContext
    {
        public WeightContestDBContext(DbContextOptions<WeightContestDBContext> options) : base(options)
        {
                
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<RecordEntity>()
        //    .Property(record => record.ID)
        //    .ValueGeneratedOnAdd();
        //}

        public DbSet<UserEntity> user { get; set; }
        public DbSet<RecordEntity> record { get; set; }
        public DbSet<SessionEntity> session { get; set; }
        public DbSet<ContestSchdule> schdule { get; set; }
    }
}
