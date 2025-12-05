using FloodRelief.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FloodRelief.Api.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<User> Users => Set<User>();
		public DbSet<CollectionPoint> CollectionPoints => Set<CollectionPoint>();
		public DbSet<Donation> Donations => Set<Donation>();


        public DbSet<DonationFeedback> DonationFeedbacks { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Map to existing table names if necessary
			modelBuilder.Entity<User>().ToTable("Users");
			modelBuilder.Entity<CollectionPoint>().ToTable("CollectionPoints");
			modelBuilder.Entity<Donation>().ToTable("Donations");

			// Relationships
			modelBuilder.Entity<User>()
				.HasOne(u => u.CollectionPoint)
				.WithMany(cp => cp.Users)
				.HasForeignKey(u => u.CollectionPointId);

			modelBuilder.Entity<Donation>()
				.HasOne(d => d.CollectionPoint)
				.WithMany(cp => cp.Donations)
				.HasForeignKey(d => d.CollectionPointId);

			modelBuilder.Entity<Donation>()
				.HasOne(d => d.CollectedByUser)
				.WithMany()
				.HasForeignKey(d => d.CollectedByUserId);


            //modelBuilder.Entity<DonationFeedback>();

            modelBuilder.Entity<DonationFeedback>(entity =>
            {
                entity.ToTable("DonationFeedbacks");

                entity.Property(f => f.FullName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(f => f.Email)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(f => f.Phone)
                    .HasMaxLength(50);

                entity.Property(f => f.AddressLine1)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(f => f.AddressLine2)
                    .HasMaxLength(200);

                entity.Property(f => f.Postcode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(f => f.ItemsDescription)
                    .IsRequired();

                entity.Property(f => f.CreatedDate)
                    .IsRequired();
                entity.Property(f => f.Weight)
                    .IsRequired();
            });





        }

    }
}
