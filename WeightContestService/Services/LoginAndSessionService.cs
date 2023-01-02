using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WeightContestService.Entities;
using WeightContestService.RequestModels;
using WeightContestService.ResponseModels;
using WeightContestService.Utilities;

namespace WeightContestService.Services
{
    public class LoginAndSessionService
    {
        private WeightContestDBContext _context;
        SetupResponseModel SetupResponse = new SetupResponseModel();
        StringHashing util = new StringHashing();

        public LoginAndSessionService(WeightContestDBContext context)
        {
            _context = context;
        }

        // Login to system
        public ServiceResponse<LoginResponseModel> Login(LoginRequestModel RequestModel)
        {
            LoginResponseModel ResponseModel = new LoginResponseModel();

            try
            {
                // Get user ID

                string getUserIdSql = $"SELECT * FROM USER_DATA WHERE USERNAME = @UserName AND PASSWORD = @Password";

                List<SqlParameter> userParameterList = new List<SqlParameter>();
                userParameterList.Add(new SqlParameter("@UserName", RequestModel.UserName));
                userParameterList.Add(new SqlParameter("@Password", util.GetHashString(RequestModel.Password)));

                SqlParameter[] userParam = userParameterList.ToArray();

                var User = _context.user.FromSqlRaw(getUserIdSql, userParam).FirstOrDefault();

                if (User == null)
                {
                    return SetupResponse.GetErrorResponse<LoginResponseModel>("Username or Password in incorrect", Enum.ActionResult.Failed);
                }

                // Check already in session table

                string checkSessionSql = $"SELECT * FROM SESSION WHERE USER_ID = @UserId";
                var Param = new SqlParameter("@UserId", User.ID);

                var Session = _context.session.FromSqlRaw(checkSessionSql, Param).FirstOrDefault();

                if (Session == null)
                {
                    string sql = "INSERT INTO SESSION (USER_ID, START_SESSION, END_SESSION, IP_ADDRESS)" +
                                 "VALUES (@UserId,@Start,@End,@Ip)";

                    List<SqlParameter> newSessionParameterList = new List<SqlParameter>();
                    newSessionParameterList.Add(new SqlParameter("@UserId", User.ID));
                    newSessionParameterList.Add(new SqlParameter("@Start", DateTime.Now));
                    newSessionParameterList.Add(new SqlParameter("@End", DateTime.Now.AddMinutes(10)));
                    newSessionParameterList.Add(new SqlParameter("@Ip", RequestModel.IPAddress));

                    SqlParameter[] newSessionParam = newSessionParameterList.ToArray();

                    _context.Database.ExecuteSqlRaw(sql, newSessionParam);
                }
                else
                {
                    // update session

                    Session.START_SESSION = DateTime.Now;
                    Session.END_SESSION = DateTime.Now.AddMinutes(10);
                    if (Session.IP_ADDRESS != RequestModel.IPAddress) Session.IP_ADDRESS = RequestModel.IPAddress;

                    _context.SaveChanges();
                }

                // Get user record and summary all applicant

                //string getRecordsSql = $"SELECT * FROM WEIGHT_RECORDS WHERE OWNER_ID = @OwnerId";
                //var getRecordsParam = new SqlParameter("@OwnerId", User.ID);

                //var getRecordResult = _context.record.FromSqlRaw(getRecordsSql, getRecordsParam).ToList<RecordEntity>();

                var getRecordResult = _context.record.ToList();

                if (getRecordResult == null)
                {
                    return SetupResponse.GetErrorResponse<LoginResponseModel>("Cannot query data", Enum.ActionResult.Failed);
                }

                var applicant = getRecordResult.Select(x => x.NAME).Distinct().ToList();

                ResponseModel.allApplicantName = new string[applicant.Count];

                for(int i = 0; i < applicant.Count; i++)
                {
                    ResponseModel.allApplicantName[i] = applicant[i];
                }


                // Entity - DTO mapping

                ResponseModel.Records = new List<Record>();

                foreach (var item in getRecordResult)
                {
                    Record record = new Record()
                    {
                        Name = item.NAME,
                        Weight = item.WEIGHT,
                        SubmitDate = item.SUBMIT_DATE.ToString("dd/MM/yyyy - HH:mm:ss"),
                        ScheduleDate = item.SUBMIT_SCHEDULE == null ? "" : item.SUBMIT_SCHEDULE.Value.ToString("dd/MM/yyyy - HH:mm:ss"),
                        ImagePath = item.IMAGE_PATH == null ? "" : item.IMAGE_PATH
                    };

                    ResponseModel.Records.Add(record);
                }

                // Get schdule

                var schduleList = _context.schdule.ToList();

                if (schduleList == null)
                {
                    return SetupResponse.GetErrorResponse<LoginResponseModel>("Cannot query data", Enum.ActionResult.Failed);
                }

                ResponseModel.Schdule = new List<SchduleModel>();

                foreach (var item in schduleList)
                {
                    SchduleModel schdule = new SchduleModel()
                    {
                        SchduleDate = item.SCHDULE_DATA,
                        DisplaySchduleDate = item.SCHDULE_DATA.ToString("dd-MMMM-yyyy"),
                        StartSubmitDate = item.START_SUBMIT_DATE,
                        EndSubmitDate = item.END_SUBMIT_DATE,
                        Phase = item.PHASE
                    };

                    ResponseModel.Schdule.Add(schdule);
                }

                // Build response model

                ResponseModel.UserId = User.ID;
                ResponseModel.UserName = User.USERNAME;
                ResponseModel.Name = User.NAME;
                ResponseModel.InitWeight = User.INIT_WEIGHT;
                ResponseModel.TargetWeight = User.TARGET_WEIGHT;
                ResponseModel.StartSession = DateTime.Now;
                ResponseModel.EndSession = DateTime.Now.AddMinutes(10);

                return SetupResponse.GetResponse(ResponseModel, "Log in Success", Enum.ActionResult.Success);

            }
            catch (Exception ex)
            {
                return SetupResponse.GetResponse(ResponseModel, "Service Exception : " + ex, Enum.ActionResult.Exception);
            }
        }


        public ServiceResponse<LoginResponseModel> CehckSession(CheckSessionRequestModel RequestModel)
        {
            LoginResponseModel ResponseModel = new LoginResponseModel();

            try
            {
                // Query Session

                string getSessionSql = $"SELECT * FROM SESSION WHERE IP_ADDRESS = @IP";
                var getSessionParam = new SqlParameter("@IP", RequestModel.IPAddress);

                var getSessionResult = _context.session.FromSqlRaw(getSessionSql, getSessionParam).FirstOrDefault();

                if (getSessionResult == null || getSessionResult.END_SESSION <= DateTime.Now)
                {
                    return SetupResponse.GetErrorResponse<LoginResponseModel>("Session not found or session expired", Enum.ActionResult.SessionExpire);
                }

                // Get user by userId

                string getUserSql = $"SELECT * FROM USER_DATA WHERE ID = @ID";
                var getUSerParam = new SqlParameter("@ID", getSessionResult.USER_ID);

                var getUserResult = _context.user.FromSqlRaw(getUserSql, getUSerParam).FirstOrDefault();

                if (getUserResult != null)
                {
                    // Get user record

                    //string getRecordsSql = $"SELECT * FROM WEIGHT_RECORDS WHERE OWNER_ID = @OwnerId";
                    //var getRecordsParam = new SqlParameter("@OwnerId", getUserResult.ID);

                    //var getRecordResult = _context.record.FromSqlRaw(getRecordsSql, getRecordsParam).ToList<RecordEntity>();

                    //if (getRecordResult == null)
                    //{
                    //    return SetupResponse.GetErrorResponse<LoginResponseModel>("Cannot query data", Enum.ActionResult.Failed);
                    //}

                    var getRecordResult = _context.record.ToList();

                    if (getRecordResult == null)
                    {
                        return SetupResponse.GetErrorResponse<LoginResponseModel>("Cannot query data", Enum.ActionResult.Failed);
                    }

                    var applicant = getRecordResult.Select(x => x.NAME).Distinct().ToList();

                    ResponseModel.allApplicantName = new string[applicant.Count];

                    for (int i = 0; i < applicant.Count; i++)
                    {
                        ResponseModel.allApplicantName[i] = applicant[i];
                    }

                    // Entity - DTO mapping

                    ResponseModel.Records = new List<Record>();

                    foreach (var item in getRecordResult)
                    {
                        Record record = new Record()
                        {
                            Name = item.NAME,
                            Weight = item.WEIGHT,
                            SubmitDate = item.SUBMIT_DATE.ToString("dd/MM/yyyy - HH:mm:ss"),
                            ScheduleDate = item.SUBMIT_SCHEDULE == null ? "" : item.SUBMIT_SCHEDULE.Value.ToString("dd/MM/yyyy - HH:mm:ss"),
                            ImagePath = item.IMAGE_PATH == null ? "" : item.IMAGE_PATH
                        };

                        ResponseModel.Records.Add(record);
                    }

                    // Get schdule

                    var schduleList = _context.schdule.ToList();

                    if (schduleList == null)
                    {
                        return SetupResponse.GetErrorResponse<LoginResponseModel>("Cannot query data", Enum.ActionResult.Failed);
                    }

                    ResponseModel.Schdule = new List<SchduleModel>();

                    foreach (var item in schduleList)
                    {
                        SchduleModel schdule = new SchduleModel()
                        {
                            SchduleDate = item.SCHDULE_DATA,
                            DisplaySchduleDate = item.SCHDULE_DATA.ToString("dd-MMMM-yyyy"),
                            StartSubmitDate = item.START_SUBMIT_DATE,
                            EndSubmitDate = item.END_SUBMIT_DATE,
                            Phase = item.PHASE
                        };

                        ResponseModel.Schdule.Add(schdule);
                    }


                    ResponseModel.UserId = getUserResult.ID;
                    ResponseModel.UserName = getUserResult.USERNAME;
                    ResponseModel.InitWeight = getUserResult.INIT_WEIGHT;
                    ResponseModel.TargetWeight = getUserResult.TARGET_WEIGHT;
                    ResponseModel.Name = getUserResult.NAME;
                    ResponseModel.StartSession = getSessionResult.START_SESSION;
                    ResponseModel.EndSession = getSessionResult.END_SESSION;

                    return SetupResponse.GetResponse(ResponseModel, "Success", Enum.ActionResult.Success);
                }
                else
                {
                    return SetupResponse.GetResponse(ResponseModel, "No session found or expired", Enum.ActionResult.Failed);
                }
            }
            catch (Exception ex)
            {

                return SetupResponse.GetResponse(ResponseModel, "Service Exception : " + ex, Enum.ActionResult.Exception);
            }
        }

        public ServiceResponse<string> LogOut(LogOutRequestModel RequestModel)
        {
            try
            {
                string getSessionSql = $"SELECT * FROM SESSION WHERE USER_ID = @ID";
                var getSessionParam = new SqlParameter("@ID", RequestModel.UserId);

                var getSessionResult = _context.session.FromSqlRaw(getSessionSql, getSessionParam).FirstOrDefault();

                if (getSessionResult == null || getSessionResult.END_SESSION <= DateTime.Now)
                {
                    return SetupResponse.GetErrorResponse<string>("Session not found or session expired", Enum.ActionResult.SessionExpire);
                }

                getSessionResult.END_SESSION = DateTime.Now;
                _context.SaveChanges();

                return SetupResponse.GetResponse("Success", "Success" , Enum.ActionResult.Success);
            }
            catch (Exception ex)
            {
                return SetupResponse.GetResponse("Exception", "Service Exception : " + ex, Enum.ActionResult.Exception);
            }
        }
    }
}
