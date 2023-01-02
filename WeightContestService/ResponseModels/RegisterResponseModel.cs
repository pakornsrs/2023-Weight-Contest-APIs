namespace WeightContestService.ResponseModels
{
    public class RegisterResponseModel
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public decimal InitialWeight { get; set; }
        public decimal TargetWeight { get; set; }
        public bool? IsAcceptCondition { get; set; }
    }
}
