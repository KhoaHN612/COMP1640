using COMP1640.Areas.Identity.Data;

namespace COMP1640.Models;

public partial class ContributionUsersModel
{
    public Contribution contribution { get; set; }
    public COMP1640User user {get;set;}
}