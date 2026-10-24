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

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=2000");

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
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
