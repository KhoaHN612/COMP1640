using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace COMP1640.Models;

public partial class Comp1640Context : DbContext
{
    public Comp1640Context()
    {
    }

    public Comp1640Context(DbContextOptions<Comp1640Context> options)
        : base(options)
    {
    }

    public virtual DbSet<AnnualMagazine> AnnualMagazines { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Contribution> Contributions { get; set; }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<FileDetail> FileDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:MyConnect");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnnualMagazine>(entity =>
        {
            entity.HasKey(e => e.AnnualMagazineId).HasName("PK__AnnualMa__B59FC27F3FBADA5A");

            entity.Property(e => e.AnnualMagazineId).ValueGeneratedNever();
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<Contribution>(entity =>
        {
            entity.HasKey(e => e.ContributionId).HasName("PK__Contribu__52B05C813DC3ECD4");

            entity.Property(e => e.ContributionId).ValueGeneratedNever();

            entity.HasOne(d => d.AnnualMagazine).WithMany(p => p.Contributions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Contribut__annua__4F7CD00D");
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(e => e.FacultyId).HasName("PK__Facultie__DBBF6FD164BB68D2");

            entity.Property(e => e.FacultyId).ValueGeneratedNever();
        });

        modelBuilder.Entity<FileDetail>(entity =>
        {
            entity.HasKey(e => e.FileId).HasName("PK__FileDeta__C2C6FFDC917CB1B3");

            entity.Property(e => e.FileId).ValueGeneratedNever();

            entity.HasOne(d => d.Contribution).WithMany(p => p.FileDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FileDetai__contr__5070F446");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
