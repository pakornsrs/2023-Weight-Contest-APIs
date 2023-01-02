using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeightContestService.RequestModels;
using WeightContestService.Services;

namespace WeightContestService.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class LoginAndSessionController : ControllerBase
    {
        private WeightContestDBContext _context;
        public LoginAndSessionController(WeightContestDBContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult Login(LoginRequestModel request)
        {
            try
            {
                LoginAndSessionService loginService = new LoginAndSessionService(_context);

                var Result = loginService.Login(request);

                return Result.ActionResult == Enum.ActionResult.Success ? StatusCode(200, Result) : StatusCode(400, Result);

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("logout")]
        public ActionResult CheckSession(LogOutRequestModel request)
        {
            try
            {
                LoginAndSessionService loginService = new LoginAndSessionService(_context);

                var Result = loginService.LogOut(request);

                return Result.ActionResult == Enum.ActionResult.Success ? StatusCode(200, Result) : StatusCode(400, Result);

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("session/check")]
        public ActionResult CheckSession(CheckSessionRequestModel request)
        {
            try
            {
                LoginAndSessionService loginService = new LoginAndSessionService(_context);

                var Result = loginService.CehckSession(request);

                return Result.ActionResult == Enum.ActionResult.Success ? StatusCode(200, Result) : StatusCode(400, Result);

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
