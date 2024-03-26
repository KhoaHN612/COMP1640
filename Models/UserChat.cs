using System.ComponentModel.DataAnnotations;
using COMP1640.Areas.Identity.Data;
using COMP1640.Models;
using Microsoft.EntityFrameworkCore;

namespace COMP1640.Models;
[PrimaryKey(nameof(UserId), nameof(ChatId))]
public class UserChat
    {
        public string UserId { get; set; }
        public int ChatId { get; set; }

        // Navigation properties
        public COMP1640User? User { get; set; }
        public Chat? Chat { get; set; }
    }