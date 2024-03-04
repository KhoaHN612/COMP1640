using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace COMP1640.Models;

[Index("AnnualMagazineId", Name = "IX_Contributions_annualMagazineId")]
public partial class Contribution
{
    [Key]
    [Column("contributionId")]
    public int ContributionId { get; set; }

    [Column("userId")]
    public int UserId { get; set; }

    [Column("annualMagazineId")]
    public int AnnualMagazineId { get; set; }

    [Column("title")]
    [StringLength(100)]
    public string Title { get; set; } = null!;

    [Column("submissionDate")]
    public DateOnly SubmissionDate { get; set; }

    [Column("comment")]
    public string? Comment { get; set; }

    [Column("status")]
    [StringLength(10)]
    public string Status { get; set; } = null!;

    [ForeignKey("AnnualMagazineId")]
    [InverseProperty("Contributions")]
    public virtual AnnualMagazine AnnualMagazine { get; set; } = null!;

    [InverseProperty("Contribution")]
    public virtual ICollection<FileDetail> FileDetails { get; set; } = new List<FileDetail>();
}
