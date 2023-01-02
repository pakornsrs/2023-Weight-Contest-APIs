namespace WeightContestService.ResponseModels
{
    public class LoginResponseModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public decimal InitWeight { get; set; }
        public decimal TargetWeight { get; set; }
        public string[] allApplicantName { get; set; }
        public int applicantNo => allApplicantName.Count();
        public DateTime StartSession { get; set; }
        public DateTime EndSession { get; set; }
        public List<Record> Records { get; set; }
        public List<SchduleModel> Schdule { get; set; }
    }
}
