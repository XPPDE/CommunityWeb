using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DbClassLibrary.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostComment> PostComments { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pgagent", "pgagent");

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Post_pkey");

            entity.Property(e => e.Content).HasDefaultValueSql("'Text'::character varying");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.Title).HasDefaultValueSql("'Post'::character varying");

            entity.HasOne(d => d.Author).WithMany(p => p.Posts).HasConstraintName("AuthorId");
        });

        modelBuilder.Entity<PostComment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Id");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Post).WithMany(p => p.PostComments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Post");

            entity.HasOne(d => d.User).WithMany(p => p.PostComments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("User");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Project_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.LiveUrl).HasDefaultValueSql("''::character varying");
            entity.Property(e => e.Title).HasDefaultValueSql("'Project'::character varying");

            entity.HasOne(d => d.Author).WithMany(p => p.Projects).HasConstraintName("AuthorId");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_pkey");

            entity.Property(e => e.Name).HasDefaultValueSql("'User'::character varying");
            entity.Property(e => e.Role).HasDefaultValueSql("'Member'::character varying");
            entity.Property(e => e.Username).HasDefaultValueSql("'user'::character varying");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
