using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace COMP1640.Models;

public partial class FileDetail
{
    [Key]
    [Column("fileId")]
    public int FileId { get; set; }

    [Column("contributionId")]
    public int? ContributionId { get; set; }

    [Column("type")]
    [StringLength(10)]
    public string Type { get; set; } = null!;

    [Column("filePath")]
    public string FilePath { get; set; } = null!;

    [NotMapped]
    public List<IFormFile>? ContributionFile { get; set; }
}
