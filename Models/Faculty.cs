using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace COMP1640.Models;

public partial class Faculty
{
    [Key]
    [Column("facultyID")]
    public int FacultyId { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string? Name { get; set; }

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

    [Column("deanName")]
    [StringLength(255)]
    public string? DeanName { get; set; }

    [InverseProperty("Faculty")]
    public virtual ICollection<AspNetUser> AspNetUsers { get; set; } = new List<AspNetUser>();
}
