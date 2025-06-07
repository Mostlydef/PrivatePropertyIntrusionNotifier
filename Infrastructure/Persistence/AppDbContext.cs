using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            :base(options)
        {

        }

        public DbSet<UserChat> UserChats { get; set; } = null!;
        public DbSet<UserDevices> UserDevices { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserChat>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.ChatId)
                    .IsRequired();
                entity.HasIndex(u => u.ChatId);
            });

            modelBuilder.Entity<UserDevices>(entity =>
            {
                entity.HasKey(d => d.Id);

                entity.Property(d => d.MacAdress)
                .IsRequired()
                .HasMaxLength(17);

                entity.HasIndex(d => d.MacAdress)
                .IsUnique();

                entity.Property(d => d.Location)
                .IsRequired()
                .HasMaxLength(128);

                entity.HasOne(d => d.UserChat)
                .WithMany(u => u.UserDevices)
                .HasForeignKey(d => d.UserChatId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            
        }
    }
}
