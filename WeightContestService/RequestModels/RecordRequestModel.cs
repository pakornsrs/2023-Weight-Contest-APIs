namespace WeightContestService.RequestModels
{
    public class RecordRequestModel
    {
        public string OwnerId { get; set; }
        public string Name { get; set; }
        public decimal Weight { get; set; }
        public DateTime SubmitSchdule { get; set; }
        public string ImagePath { get; set; }
    }
}
