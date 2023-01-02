using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WeightContestService.RequestModels;
using WeightContestService.Services;

namespace WeightContestService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private WeightContestDBContext _context;
        public UserController(WeightContestDBContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public ActionResult Register(RegisterRequestModel request)
        {
            try
            {
                UserService userService = new UserService(_context);

                var Result = userService.Register(request);

                return Result.ActionResult == Enum.ActionResult.Success ? StatusCode(200,Result) : StatusCode(400,Result);

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("record")]
        public ActionResult Record(RecordRequestModel request)
        {
            try
            {
                UserService userService = new UserService(_context);

                var Result = userService.RequestRecord(request);

                return Result.ActionResult == Enum.ActionResult.Success ? StatusCode(200, Result) : StatusCode(400, Result);

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("record/get")]
        public ActionResult GetMyRecord(GetRecordRequestModel request)
        {
            try
            {
                UserService userService = new UserService(_context);

                var Result = userService.GetMyRecord(request);

                return Result.ActionResult == Enum.ActionResult.Success ? StatusCode(200, Result) : StatusCode(400, Result);

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("record/get/all")]
        public ActionResult GetAllRecord()
        {
            try
            {
                UserService userService = new UserService(_context);

                var Result = userService.GetAllRecord();

                return Result.ActionResult == Enum.ActionResult.Success ? StatusCode(200, Result) : StatusCode(400, Result);

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
