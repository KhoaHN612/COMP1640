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



//     USE [COMP1640]
// GO

// INSERT INTO [dbo].[AnnualMagazines]
//            ([annualMagazineId]
//            ,[academicYear]
//            ,[title]
//            ,[submissionClosureDate]
//            ,[finalClosureDate])
//      VALUES
//            (1, N'2022-2023', N'Annual Magazine 2022', '2023-05-31', '2023-06-30'),
//            (2, N'2022-2023', N'Annual Magazine 2023', '2024-05-31', '2024-06-30'),
//            (3, N'2023-2024', N'Annual Magazine 2024', '2025-05-31', '2025-06-30'),
//            (4, N'2023-2024', N'Annual Magazine 2025', '2026-05-31', '2026-06-30'),
//            (5, N'2024-2025', N'Annual Magazine 2026', '2027-05-31', '2027-06-30'),
//            (6, N'2024-2025', N'Annual Magazine 2027', '2028-05-31', '2028-06-30'),
//            (7, N'2025-2026', N'Annual Magazine 2028', '2029-05-31', '2029-06-30'),
//            (8, N'2025-2026', N'Annual Magazine 2029', '2030-05-31', '2030-06-30'),
//            (9, N'2026-2027', N'Annual Magazine 2030', '2031-05-31', '2031-06-30'),
//            (10, N'2026-2027', N'Annual Magazine 2031', '2032-05-31', '2032-06-30')
// GO

}
