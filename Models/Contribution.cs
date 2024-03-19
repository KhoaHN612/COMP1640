﻿using System;
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
    public String UserId { get; set; }

    [Column("annualMagazineId")]
    public int AnnualMagazineId { get; set; }

    [Column("title")]
    [StringLength(100)]
    public string Title { get; set; } = null!;

    [Column("submissionDate")]
    public DateTime SubmissionDate { get; set; }

    [Column("comment")]
    public string? Comment { get; set; }

    [Column("isPublished")]
    public bool IsPublished { get; set; }

    [Column("status")]
    [StringLength(10)]
    public string Status { get; set; } = null!;

    [ForeignKey("AnnualMagazineId")]
    [InverseProperty("Contributions")]
    public virtual AnnualMagazine AnnualMagazine { get; set; } = null!;

    [InverseProperty("Contribution")]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

// INSERT INTO [dbo].[Contributions]
//            ([contributionId]
//            ,[userId]
//            ,[annualMagazineId]
//            ,[title]
//            ,[submissionDate]
//            ,[comment]
//            ,[status])
// VALUES
//            (1
//            ,'4891e32b-3857-4d33-be0e-eb35fb34feb6'
//            ,1
//            ,'Sample Title 1'
//            ,GETDATE() -- Ngày hiện tại
//            ,'Sample comment 1'
//            ,'Pending'),

//            (2
//            ,'6be2b819-ac98-42e8-a697-c2508b32badc'
//            ,4
//            ,'Sample Title 2'
//            ,GETDATE() -- Ngày hiện tại
//            ,'Sample comment 2'
//            ,'Pending'),

//            (3
//            ,'8a265432-a08c-487a-af27-f8e49d783dc1'
//            ,1
//            ,'Sample Title 3'
//            ,GETDATE() -- Ngày hiện tại
//            ,'Sample comment 3'
//            ,'Pending')
// GO