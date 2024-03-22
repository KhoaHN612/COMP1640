using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using COMP1640.Areas.Identity.Data;

namespace COMP1640.Models;
public class Post
{
    [Key]
    [Column("PostId")]
    public int PostId { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    public DateTime PostedAt { get; set; }

    // Foreign key for User
    [ForeignKey("COMP1640UserId")]
    public string UserId { get; set; }
    public COMP1640User? User { get; set; }

    [InverseProperty("Post")]
    public virtual ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();

    [NotMapped]
    public IFormFile? ImageFile { get; set; }

    [StringLength(100)]
    public string? ImagePath { get; set; }
}