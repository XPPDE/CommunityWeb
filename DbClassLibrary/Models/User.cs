using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbClassLibrary.Models;

[Table("User")]
public partial class User
{
    [Key]
    public int Id { get; set; }

    [StringLength(30)]
    public string Name { get; set; } = null!;

    [StringLength(100)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(1000)]
    public string? AvatarUrl { get; set; }

    [StringLength(300)]
    public string? Bio { get; set; }

    [StringLength(50)]
    public string Role { get; set; } = null!;

    [StringLength(30)]
    public string? Username { get; set; }

    [InverseProperty("Author")]
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    [InverseProperty("Author")]
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
