using Azure.Identity;

namespace WeightContestService.RequestModels
{
    public class RegisterRequestModel
    {
        public string UserName {get;set;}
        public string Password { get;set;}
        public string Name { get; set; }
        public decimal InitialWeight { get; set; }
        public decimal TargetWeight { get; set; }
        public bool? IsAcceptCondition { get; set; }
    }
}
