using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DbClassLibrary.Models;

[Table("User")]
[Index("Username", Name = "User_Username_key", IsUnique = true)]
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
    public string Username { get; set; } = null!;

    [StringLength(100)]
    public string? TelegramChannel { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();

    [InverseProperty("Author")]
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    [InverseProperty("Author")]
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
