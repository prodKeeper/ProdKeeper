using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProdKeeper.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdKeeper.Entity.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public virtual DbSet<Item> Item { get; set; }
        public virtual DbSet<ItemMetadata> ItemMetadata { get; set; }
        public virtual DbSet<ItemVersion> ItemVersion { get; set; }
        public virtual DbSet<MetadataKey> MetadataKey { get; set; }
        public virtual DbSet<MetadataValues> MetadataValues { get; set; }
        public virtual DbSet<PatternsRepository> PatternsRepository { get; set; }
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

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateLastAccess)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(55);
            });

            modelBuilder.Entity<ItemMetadata>(entity =>
            {
                entity.HasIndex(e => new { e.IditemVersion, e.IdmetadataValue })
                    .HasName("IX_ItemMetadata")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.IditemVersion).HasColumnName("IDItemVersion");

                entity.Property(e => e.IdmetadataValue).HasColumnName("IDMetadataValue");

                entity.HasOne(d => d.IditemVersionNavigation)
                    .WithMany(p => p.ItemMetadata)
                    .HasForeignKey(d => d.IditemVersion)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ItemMetadata_ItemVersion");

                entity.HasOne(d => d.IdmetadataValueNavigation)
                    .WithMany(p => p.ItemMetadata)
                    .HasForeignKey(d => d.IdmetadataValue)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ItemMetadata_MetadataValues");
            });

            modelBuilder.Entity<ItemVersion>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FilePath).IsRequired();

                entity.Property(e => e.Iditem).HasColumnName("IDItem");

                entity.HasOne(d => d.IditemNavigation)
                    .WithMany(p => p.ItemVersion)
                    .HasForeignKey(d => d.Iditem)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ItemVersion_Item");
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
                entity.HasIndex(e => new { e.Idkey, e.Libelle })
                    .HasName("IX_MetadataValues")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Idkey).HasColumnName("IDKey");

                entity.Property(e => e.Idparent).HasColumnName("IDParent");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(55);

                entity.HasOne(d => d.IdkeyNavigation)
                    .WithMany(p => p.MetadataValues)
                    .HasForeignKey(d => d.Idkey)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MetadataValues_MetadataKey");

                entity.HasOne(d => d.IdparentNavigation)
                    .WithMany(p => p.InverseIdparentNavigation)
                    .HasForeignKey(d => d.Idparent)
                    .HasConstraintName("FK_MetadataValues_MetadataValues");
            });

            modelBuilder.Entity<PatternsRepository>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Libelle)
                    .IsRequired()
                    .HasMaxLength(55);

                entity.Property(e => e.Patterns)
                    .IsRequired()
                    .HasColumnName("patterns");
            });


        }
    }
}
