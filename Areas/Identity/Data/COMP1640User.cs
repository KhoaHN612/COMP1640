using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using COMP1640.Models;
using Microsoft.AspNetCore.Identity;

namespace COMP1640.Areas.Identity.Data;

public class COMP1640User : IdentityUser
{
    public string FullName { get; set; } = null!;
    public DateOnly DayOfBirth { get; set; }

    [StringLength(200)]
    public string Address { get; set; } = null!;

    [StringLength(100)]
    public string? ProfileImagePath { get; set; }
    public DateTime LastLogin { get; set; }
    public int? FacultyId { get; set; }

    [ForeignKey("FacultyId")]
    [InverseProperty("COMP1640User")]
    public virtual Faculty? Faculty { get; set; }
    [NotMapped]
    public IFormFile? ProfileImageFile { get; set; }

    public COMP1640User() {
        LastLogin = DateTime.MinValue;
    }
}