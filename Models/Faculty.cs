using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using COMP1640.Areas.Identity.Data;
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
    public ICollection<AspNetUser> AspNetUsers { get; set; }
    [InverseProperty("Faculty")]
    public virtual ICollection<COMP1640User> COMP1640User { get; set; } = new List<COMP1640User>();
}

// INSERT INTO [dbo].[Faculties]
//            ([facultyID]
//            ,[name]
//            ,[description]
//            ,[deanName])
//      VALUES
//            (4
//            ,'Engineering'
//            ,'The Faculty of Engineering offers a wide range of undergraduate and postgraduate programs in various engineering disciplines.'
//            ,'John Smith'),
//            (5
//            ,'Business Administration'
//            ,'The Faculty of Business Administration provides high-quality education in business management, finance, marketing, and more.'
//            ,'Emily Johnson'),
//            (6
//            ,'Arts and Humanities'
//            ,'The Faculty of Arts and Humanities offers diverse programs in literature, history, philosophy, and languages.'
//            ,'Michael Brown'),
//            (7
//            ,'Medicine'
//            ,'The Faculty of Medicine focuses on training future healthcare professionals and conducting cutting-edge research in medical sciences.'
//            ,'Sarah Wilson'),
//            (8
//            ,'Science'
//            ,'The Faculty of Science offers programs in biology, chemistry, physics, and other scientific disciplines, promoting scientific inquiry and discovery.'
//            ,'David Martinez'),
//            (9
//            ,'Law'
//            ,'The Faculty of Law provides comprehensive legal education and fosters critical thinking and analytical skills in its students.'
//            ,'Jessica Thompson'),
//            (10
//            ,'Education'
//            ,'The Faculty of Education prepares educators and administrators to meet the challenges of today’s educational landscape.'
//            ,'Daniel Garcia'),
//            (11
//            ,'Social Sciences'
//            ,'The Faculty of Social Sciences offers interdisciplinary programs in sociology, psychology, anthropology, and more.'
//            ,'Olivia Rodriguez'),
//            (12
//            ,'Architecture and Design'
//            ,'The Faculty of Architecture and Design focuses on nurturing creativity and innovation in architectural and design practices.'
//            ,'Matthew Taylor'),
//            (13
//            ,'Information Technology'
//            ,'The Faculty of Information Technology offers programs in computer science, software engineering, and information systems.'
//            ,'Sophia Clark')
// GO
