using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace COMP1640.Models;

[Index("ContributionId", Name = "IX_FileDetails_contributionId")]
public partial class FileDetail
{
    [Key]
    [Column("fileId")]
    public int FileId { get; set; }

    [Column("contributionId")]
    public int ContributionId { get; set; }

    [Column("type")]
    [StringLength(10)]
    public string Type { get; set; } = null!;

    [Column("filePath")]
    public string FilePath { get; set; } = null!;

    [ForeignKey("ContributionId")]
    [InverseProperty("FileDetails")]
    public virtual Contribution Contribution { get; set; } = null!;

    [NotMapped]
    public IFormFile ContributionFile { get; set; }
}
