using System;
using System.Collections.Generic;
using COMP1640.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace COMP1640.Models;

public partial class Comp1640Context : IdentityDbContext<COMP1640User>
{
    public Comp1640Context()
    {
    }

    public Comp1640Context(DbContextOptions<Comp1640Context> options)
        : base(options)
    {
    }

    public virtual DbSet<AnnualMagazine> AnnualMagazines { get; set; }

    // public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    // public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    // public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    // public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    // public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    // public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Contribution> Contributions { get; set; }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<FileDetail> FileDetails { get; set; }
    
    public object COMP1640Users { get; internal set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:MyConnect");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<AnnualMagazine>(entity =>
        {
            entity.HasKey(e => e.AnnualMagazineId).HasName("PK__AnnualMa__B59FC27F5CBC5719");

            entity.Property(e => e.AnnualMagazineId).ValueGeneratedNever();
        });

        // modelBuilder.Entity<AspNetRole>(entity =>
        // {
        //     entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
        //         .IsUnique()
        //         .HasFilter("([NormalizedName] IS NOT NULL)");
        // });

        // modelBuilder.Entity<AspNetUser>(entity =>
        // {
        //     entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
        //         .IsUnique()
        //         .HasFilter("([NormalizedUserName] IS NOT NULL)");

        //     entity.HasOne(d => d.Faculty).WithMany(p => p.AspNetUsers).HasConstraintName("FK_AspNetUsers_Faculties");

        //     entity.HasMany(d => d.Roles).WithMany(p => p.Users)
        //         .UsingEntity<Dictionary<string, object>>(
        //             "AspNetUserRole",
        //             r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
        //             l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
        //             j =>
        //             {
        //                 j.HasKey("UserId", "RoleId");
        //                 j.ToTable("AspNetUserRoles");
        //                 j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
        //             });
        // });

        modelBuilder.Entity<Contribution>(entity =>
        {
            entity.HasKey(e => e.ContributionId).HasName("PK__Contribu__52B05C813D4CE088");

            entity.Property(e => e.ContributionId).ValueGeneratedNever();

            entity.HasOne(d => d.AnnualMagazine).WithMany(p => p.Contributions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Contribut__annua__656C112C");
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(e => e.FacultyId).HasName("PK__Facultie__DBBF6FD15ADF4128");

            entity.Property(e => e.FacultyId).ValueGeneratedNever();
        });

        modelBuilder.Entity<FileDetail>(entity =>
        {
            entity.HasKey(e => e.FileId).HasName("PK__FileDeta__C2C6FFDCE73A7F89");

            entity.Property(e => e.FileId).ValueGeneratedNever();

            entity.HasOne(d => d.Contribution).WithMany(p => p.FileDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FileDetai__contr__66603565");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
