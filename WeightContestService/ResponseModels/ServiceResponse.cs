using WeightContestService.Enum;

namespace WeightContestService.ResponseModels
{
    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public int ErrorCode { get; set; }
        public ActionResult ActionResult { get; set; }
        public string Message { get; set; }
    }
}
