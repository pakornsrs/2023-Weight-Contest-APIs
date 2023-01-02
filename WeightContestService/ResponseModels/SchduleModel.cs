namespace WeightContestService.ResponseModels
{
    public class SchduleModel
    {
        public DateTime SchduleDate { get; set; }
        public string DisplaySchduleDate { get; set; }
        public DateTime StartSubmitDate { get; set; }
        public DateTime EndSubmitDate { get; set; }
        public int Phase { get; set; }
    }
}
