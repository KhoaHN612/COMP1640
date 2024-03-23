namespace COMP1640.Models.MultiModels;

public partial class ContributionFiledetails
{
    public Contribution? Contributions { get; set; }
    public IEnumerable<FileDetail>? FileDetails {get;set;}
}