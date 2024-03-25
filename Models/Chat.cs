using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using COMP1640.Areas.Identity.Data;

namespace COMP1640.Models;
public class Chat
{
    [Key]
    public int Id { get; set; }
    public DateTime UpdateAt {get; set;}
    public ICollection<UserChat> Users { get; set; }
    public ICollection<Message> Messages { get; set; }

}