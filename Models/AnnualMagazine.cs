using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace COMP1640.Models;

public partial class AnnualMagazine
{
    [Key]
    [Column("annualMagazineId")]
    public int AnnualMagazineId { get; set; }

    [Column("academicYear")]
    [StringLength(50)]
    public string? AcademicYear { get; set; }

    [Column("title")]
    [StringLength(200)]
    public string? Title { get; set; }

    [Column("submissionClosureDate")]
    public DateOnly? SubmissionClosureDate { get; set; }

    [Column("finalClosureDate")]
    public DateOnly? FinalClosureDate { get; set; }

    [InverseProperty("AnnualMagazine")]
    public virtual ICollection<Contribution> Contributions { get; set; } = new List<Contribution>();
}
