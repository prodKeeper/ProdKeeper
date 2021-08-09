using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProdKeeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdKeeper.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public virtual DbSet<Item> Item { get; set; }
        public virtual DbSet<ItemMetadata> ItemMetadata { get; set; }
        public virtual DbSet<MetadataKey> MetadataKey { get; set; }
        public virtual DbSet<MetadataValues> MetadataValues { get; set; }
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Item>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.FileContent).IsRequired();

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(55);
            });

            modelBuilder.Entity<ItemMetadata>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Iditem).HasColumnName("IDItem");

                entity.Property(e => e.IdmetadataValue).HasColumnName("IDMetadataValue");

                entity.HasOne(d => d.IditemNavigation)
                    .WithMany(p => p.ItemMetadata)
                    .HasForeignKey(d => d.Iditem)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ItemMetadata_Item");

                entity.HasOne(d => d.IdmetadataValueNavigation)
                    .WithMany(p => p.ItemMetadata)
                    .HasForeignKey(d => d.IdmetadataValue)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ItemMetadata_MetadataValues");
            });

            modelBuilder.Entity<MetadataKey>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(55);
            });

            modelBuilder.Entity<MetadataValues>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Idkey).HasColumnName("IDKey");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(55);

                entity.HasOne(d => d.IdkeyNavigation)
                    .WithMany(p => p.MetadataValues)
                    .HasForeignKey(d => d.Idkey)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MetadataValues_MetadataKey");
            });
        }
    }
}
