using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using COMP1640.Areas.Identity.Data;

namespace COMP1640.Models;
public class PostComment
{
    public int Id { get; set; }

    [Required]
    public string Content { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Foreign key to UserId
    [ForeignKey("COMP1640UserId")]
    public string UserId { get; set; }
    public COMP1640User? User { get; set; }

    // Foreign key to Post
    [ForeignKey("PostId")]
    public int PostId { get; set; }
    public Post? Post { get; set; }
}