using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DbClassLibrary.Models;

[Table("Post")]
public partial class Post
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "character varying")]
    public string Title { get; set; } = null!;

    [StringLength(3000)]
    public string Content { get; set; } = null!;

    [Column(TypeName = "time with time zone")]
    public DateTimeOffset? CreatedAt { get; set; }

    public int? AuthorId { get; set; }

    [ForeignKey("AuthorId")]
    [InverseProperty("Posts")]
    public virtual User? Author { get; set; }
}
