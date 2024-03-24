using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using COMP1640.Areas.Identity.Data;

namespace COMP1640.Models;
public class WebAccessLog
{
        [Key]
        public int Id { get; set; }

        [Required]
        public string BrowserName { get; set; }

        [Required]
        public int AccessCount { get; set; }

        [Required]
        public DateTime AccessDate { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual AspNetUser User { get; set; }
}