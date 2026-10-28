using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DbClassLibrary.Models;

[Table("PostComment")]
public partial class PostComment
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int PostId { get; set; }

    [StringLength(3000)]
    public string Content { get; set; } = null!;

    [Column(TypeName = "time with time zone")]
    public DateTimeOffset? CreatedAt { get; set; }

    [ForeignKey("PostId")]
    [InverseProperty("PostComments")]
    public virtual Post Post { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("PostComments")]
    public virtual User User { get; set; } = null!;
}
