using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbClassLibrary.Models;

[Table("Project")]
public partial class Project
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Title { get; set; } = null!;

    [StringLength(300)]
    public string Description { get; set; } = null!;

    [Column(TypeName = "time with time zone")]
    public DateTimeOffset? CreatedAt { get; set; }

    public int? AuthorId { get; set; }

    [StringLength(300)]
    public string? LiveUrl { get; set; }

    [ForeignKey("AuthorId")]
    [InverseProperty("Projects")]
    public virtual User? Author { get; set; }
}
