namespace COMP1640.Models;

public partial class ContributionAcademicYearModel
{
    public Contribution contribution { get; set; }
    public IEnumerable<AnnualMagazine> annualMagazines {get;set;}
}