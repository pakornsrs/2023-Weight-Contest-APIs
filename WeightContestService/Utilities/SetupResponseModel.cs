using Azure;
using WeightContestService.Enum;
using WeightContestService.ResponseModels;

namespace WeightContestService.Utilities
{
    public class SetupResponseModel
    {
        public ServiceResponse<T> GetResponse<T>(T model, string errorMessage, ActionResult actionResult)
        {
            var response = new ServiceResponse<T>();

            response.Data = model;
            response.ActionResult = actionResult;
            response.Message = errorMessage;

            if (actionResult == Enum.ActionResult.Success) response.ErrorCode = 200;
            else if(actionResult == Enum.ActionResult.Failed) response.ErrorCode = 400;
            else if (actionResult == Enum.ActionResult.SessionExpire) response.ErrorCode = 400;
            else if (actionResult == Enum.ActionResult.Exception) response.ErrorCode = 500;

            return response;
        }
        public ServiceResponse<T> GetErrorResponse<T>(string errorMessage,ActionResult actionResult)
        {
            var response = new ServiceResponse<T>();

            response.ActionResult = actionResult;
            response.Message = errorMessage;

            if (actionResult == Enum.ActionResult.Success) response.ErrorCode = 200;
            else if (actionResult == Enum.ActionResult.Failed) response.ErrorCode = 400;
            else if (actionResult == Enum.ActionResult.SessionExpire) response.ErrorCode = 401;
            else if (actionResult == Enum.ActionResult.Exception) response.ErrorCode = 500;

            return response;
        }
    }
}
