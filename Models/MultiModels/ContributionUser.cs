namespace COMP1640.Models.MultiModels
{
    public class ContributionUser
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Faculty { get; set; }
        public int TotalContribution { get; set; }
        public int TotalAccept { get; set; }
        public int TotalReject { get; set; }
        public int TotalPending { get; set; }
        public int Year { get; set; }
    }
}