using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace UdemyRealWordUnitTest.Web.Model
{
    public partial class UdemyUnitTestContext : DbContext
    {
        public virtual DbSet<Product> Products { get; set; } = null!;

        public UdemyUnitTestContext()
        {
        }

        public UdemyUnitTestContext(DbContextOptions<UdemyUnitTestContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Color).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Price).HasColumnType("money");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
