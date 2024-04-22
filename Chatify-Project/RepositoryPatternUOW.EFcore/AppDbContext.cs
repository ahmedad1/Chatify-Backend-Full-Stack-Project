using Microsoft.EntityFrameworkCore;
using RepositoryPattern.Core.Models;
using RepositoryPatternUOW.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPatternUOW.EFcore
{
    public class AppDbContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserConnection> UserConnections { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<VerificationCode> VerifcationCodes { get; set; }
        public AppDbContext(DbContextOptions options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(x =>
            {
                x.HasMany(e => e.UserConnections).WithOne(e => e.User).HasForeignKey(f => f.UserId);
                x.HasMany(e => e.RefreshTokens).WithOne(e => e.User).HasForeignKey(f => f.UserId);
                x.HasMany(e => e.Groups).WithMany(e => e.Users).UsingEntity<UserGroup>();
                x.Property(p => p.FirstName).HasMaxLength(100);
                x.Property(p => p.LastName).HasMaxLength(100);
                x.Property(p => p.UserName).HasMaxLength(100).IsUnicode(false);
                x.Property(p => p.Email).HasMaxLength(100).IsUnicode(false);
                x.Property(p => p.Password).HasMaxLength(100);
                x.HasIndex(p => p.Email);
                x.HasIndex(p => p.UserName);

            });
            modelBuilder.Entity<RefreshToken>(x =>
            {
                x.HasKey(k => new { k.UserId,k.Token});
                x.Property(p => p.Token).HasMaxLength(44);

            });
            modelBuilder.Entity<UserConnection>(x =>
            {
                x.Property(p => p.ConnectionId).HasMaxLength(255);
                x.HasKey(k => new { k.UserId, k.ConnectionId });
            }); 
            modelBuilder.Entity<Group>(x =>
            {
                x.Property(x => x.Id).HasMaxLength(255);
                
            });
            modelBuilder.Entity<VerificationCode>(x =>
            {
                x.HasKey(k => new {k.Code,k.UserId});
            }) ;
        }

    }
}
