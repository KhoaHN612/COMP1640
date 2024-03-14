namespace COMP1640.Models.MultiModels
{
    public class ContributionWithoutComment
    {
        public DateTime Date { get; set; }
        public int ContributionsWithoutComments { get; set; }
        public int ContributionsWithoutCommentsAfter14Days { get; set; }
    }
}