namespace WeightContestService.ResponseModels
{
    public class GetRecordResponseModel
    {
        public List<Record> Records { get; set; }
    }

    public class Record
    {
        public string Name { get; set; }
        public decimal Weight { get; set; }
        public string SubmitDate { get; set; }
        public string ScheduleDate { get; set; }
        public string ImagePath { get; set; }
    }
}
