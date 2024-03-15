using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using COMP1640.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace COMP1640.Models
{
    [Index("ContributionId", Name = "IX_Comment_contributionId")]
    public partial class Comment
    {
        [Key]
        [Column("comment_id")]
        public int CommentId { get; set; }

        [ForeignKey("Id")]
        [Column("userId")]
        public string UserId { get; set; }

        [Column("contributionId")]
        public int ContributionId { get; set; }

        [Column("comment_Field")]
        [StringLength(1000)]
        public string CommentField { get; set; } = null!;

        [Column("comment_date")]
        public DateTime CommentDate { get; set; }

        [ForeignKey("ContributionId")]
        [InverseProperty("Comments")]
        public virtual Contribution Contribution { get; set; } = null!;

        
        [ForeignKey("UserId")]
        public virtual COMP1640User User { get; set; }
    }
}
