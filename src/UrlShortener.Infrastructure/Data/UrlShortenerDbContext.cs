using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Domain.Common;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Data
{
    public class UrlShortenerDbContext : DbContext
    {
        public UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options)
            : base(options)
        {

        }

        public DbSet<ShortUrl> ShortUrls { get; set; } = null!;
        //public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //// Nếu bạn dùng Fluent API tách riêng config:
            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(UrlShortenerDbContext).Assembly)
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ShortUrl>(entity =>
            {
                entity.ToTable("ShortUrls"); // optional, table name convention

                entity.HasKey(s => s.Id);

                entity.Property(s => s.Id)
                      .ValueGeneratedOnAdd();

                entity.Property(s => s.OriginalUrl)
                      .IsRequired()
                      .HasMaxLength(2048); // long enough for most URLs

                entity.Property(s => s.ShortCode)
                      .IsRequired()
                      .HasMaxLength(MaxLengthConfig.SHORT_CODE);

                entity.Property(s => s.UserId)
                      .IsRequired(false);

                entity.Property(s => s.CreatedAt)
                      .IsRequired()
                      .HasDefaultValueSql("GETUTCDATE()"); // SQL Server default UTC

                entity.Property(s => s.ExpiryDate)
                      .IsRequired(false);

                entity.Property(s => s.ClickCount)
                      .HasDefaultValue(0);

                // relationships
                entity.HasOne(s => s.User)
                      .WithMany(u => u.ShortUrls) // assuming User has ICollection<ShortUrl>
                      .HasForeignKey(s => s.UserId)
                      .OnDelete(DeleteBehavior.SetNull); // if user is deleted, keep short url
            });
        }
       
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder != null)
            {
                //optionsBuilder.UseSqlServer(GetConnectionString("DefaultConnection")).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }
        }
    }
}
