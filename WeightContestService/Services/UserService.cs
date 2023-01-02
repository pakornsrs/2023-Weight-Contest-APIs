using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WeightContestService.Entities;
using WeightContestService.RequestModels;
using WeightContestService.ResponseModels;
using WeightContestService.Utilities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;

namespace WeightContestService.Services
{
    public class UserService
    {
        private WeightContestDBContext _context;
        SetupResponseModel SetupResponse = new SetupResponseModel();
        public UserService(WeightContestDBContext context)
        {
            _context = context;
        }

        // Register
        public ServiceResponse<RegisterResponseModel> Register(RegisterRequestModel RequestModel)
        {
            UserEntity ApplicationForm = new UserEntity();
            RegisterResponseModel ResponseModel = new RegisterResponseModel();

            try
            {
                #region Validation and Model-Entity mapper
                
                // ID
                ApplicationForm.ID = Guid.NewGuid().ToString().Replace("-",String.Empty);
                
                // Username
                if(string.IsNullOrEmpty(RequestModel.UserName) || string.IsNullOrWhiteSpace(RequestModel.UserName))
                {
                    return SetupResponse.GetErrorResponse<RegisterResponseModel>("Username must not be null", Enum.ActionResult.Failed);
                }
                else
                {
                    //Check duplicate username
                    var query = from item in _context.user
                                 where item.USERNAME == RequestModel.UserName
                                 select item;

                    var member = query.FirstOrDefault<UserEntity>();

                    if(member != null) return SetupResponse.GetErrorResponse<RegisterResponseModel>("Username was already used", Enum.ActionResult.Failed);

                    ApplicationForm.USERNAME = RequestModel.UserName;
                    ResponseModel.UserName = RequestModel.UserName;
                }

                // Password
                if (string.IsNullOrEmpty(RequestModel.Password) || string.IsNullOrWhiteSpace(RequestModel.Password))
                {
                    return SetupResponse.GetErrorResponse<RegisterResponseModel>("Password must not be null", Enum.ActionResult.Failed);
                }
                else
                {
                    StringHashing util = new StringHashing();
                    var hashPassword = util.GetHashString(RequestModel.Password);
                    ApplicationForm.PASSWORD = hashPassword;

                    // update request password
                    RequestModel.Password = hashPassword;
                }

                // Display name
                if (string.IsNullOrEmpty(RequestModel.Name) || string.IsNullOrWhiteSpace(RequestModel.Name))
                {
                    return SetupResponse.GetErrorResponse<RegisterResponseModel>("Name must not be null or empty", Enum.ActionResult.Failed);
                }
                else
                {
                    ApplicationForm.NAME = RequestModel.Name;
                    ResponseModel.Name = RequestModel.Name;
                }

                if (RequestModel.InitialWeight <= 0)
                {
                    return SetupResponse.GetErrorResponse<RegisterResponseModel>("Initial weight must not be null and value", Enum.ActionResult.Failed);
                }
                else
                {
                    ApplicationForm.INIT_WEIGHT = RequestModel.InitialWeight;
                    ResponseModel.InitialWeight = RequestModel.InitialWeight;
                }

                if (RequestModel.TargetWeight <= 0)
                {
                    return SetupResponse.GetErrorResponse<RegisterResponseModel>("Target weight must not be null and value", Enum.ActionResult.Failed);
                }
                else
                {
                    ApplicationForm.TARGET_WEIGHT = RequestModel.TargetWeight;
                    ResponseModel.TargetWeight = RequestModel.TargetWeight;
                }

                if (RequestModel.IsAcceptCondition == null)
                {
                    return SetupResponse.GetErrorResponse<RegisterResponseModel>("Please accept contest condition", Enum.ActionResult.Failed);
                }
                else
                {
                    ApplicationForm.IS_ACCEPT_CONDITION = RequestModel.IsAcceptCondition.Value;
                    ResponseModel.IsAcceptCondition = RequestModel.IsAcceptCondition.Value;
                }

                ApplicationForm.CREATE_DATE = new DateTime();
                ApplicationForm.UPDATE_DATE = new DateTime();
                ApplicationForm.UPDATE_BY = "System";

                #endregion

                _context.user.Add(ApplicationForm);
                _context.SaveChanges();

                return SetupResponse.GetResponse(ResponseModel, "Success", Enum.ActionResult.Success);
            }
            catch (Exception ex)
            {
                return SetupResponse.GetResponse(ResponseModel, "Service Exception : " + ex, Enum.ActionResult.Exception);
            }
        }

        // Save record
        public ServiceResponse<RecordResponseModel> RequestRecord(RecordRequestModel RequestModel)
        {
            RecordEntity RequestRecord = new RecordEntity();
            RecordResponseModel ResponseModel = new RecordResponseModel();

            try
            {
                #region Validation and Model-Entity mapper

                // OwnerId
                if (String.IsNullOrEmpty(RequestModel.OwnerId) || String.IsNullOrWhiteSpace(RequestModel.OwnerId))
                {
                    return SetupResponse.GetErrorResponse<RecordResponseModel>("OwnnerId must not be null", Enum.ActionResult.Failed);
                }
                else
                {
                    RequestRecord.OWNER_ID = RequestModel.OwnerId;
                }

                // Name
                if (String.IsNullOrEmpty(RequestModel.Name) || String.IsNullOrWhiteSpace(RequestModel.Name))
                {
                    return SetupResponse.GetErrorResponse<RecordResponseModel>("Display name must not be null", Enum.ActionResult.Failed);
                }
                else
                {
                    RequestRecord.NAME = RequestModel.Name;
                    ResponseModel.Name = RequestModel.Name;
                }

                // Weight
                if (RequestModel.Weight <= 0)
                {
                    return SetupResponse.GetErrorResponse<RecordResponseModel>("Input weight must be positive and not be null", Enum.ActionResult.Failed);
                }
                else
                {
                    RequestRecord.WEIGHT = RequestModel.Weight;
                    ResponseModel.Weight = RequestModel.Weight;
                }

                // Image path
                if (RequestModel.ImagePath == null || RequestModel.ImagePath == "None"|| RequestModel.ImagePath == "none")
                {
                    RequestRecord.IMAGE_PATH = RequestModel.ImagePath;
                }
                else
                {
                    var imgPath = LoadAndSaveImage(RequestModel.ImagePath);
                    RequestRecord.IMAGE_PATH = imgPath;
                    ResponseModel.ImagePath = imgPath;
                }

                // Schedule
                if((RequestModel.SubmitSchdule.AddDays(3) > DateTime.Now) && (RequestModel.SubmitSchdule.AddDays(-3) < DateTime.Now))
                {
                    RequestRecord.SUBMIT_SCHEDULE = RequestModel.SubmitSchdule;
                }
                else
                {
                    return SetupResponse.GetErrorResponse<RecordResponseModel>("Out of submit schedule bound.", Enum.ActionResult.Failed);
                }

                RequestRecord.IS_ACTIVE = true;
                RequestRecord.SUBMIT_DATE = DateTime.Now;

                #endregion

                #region Save to DB

                string sql = "INSERT INTO WEIGHT_RECORDS (OWNER_ID, NAME, WEIGHT, SUBMIT_DATE,SUBMIT_SCHEDULE,IMAGE_PATH,IS_ACTIVE )" +
                             "VALUES (@ownerId,@name,@weight,@date,@scehdule,@image,@isActive)";

                List<SqlParameter> parameterList = new List<SqlParameter>();
                parameterList.Add(new SqlParameter("@ownerId", RequestRecord.OWNER_ID));
                parameterList.Add(new SqlParameter("@name", RequestRecord.NAME));
                parameterList.Add(new SqlParameter("@weight", RequestRecord.WEIGHT));
                parameterList.Add(new SqlParameter("@date", RequestRecord.SUBMIT_DATE));
                parameterList.Add(new SqlParameter("@scehdule", RequestRecord.SUBMIT_SCHEDULE));
                parameterList.Add(new SqlParameter("@image", RequestRecord.IMAGE_PATH));
                parameterList.Add(new SqlParameter("@isActive", RequestRecord.IS_ACTIVE));

                SqlParameter[] Param = parameterList.ToArray();

                _context.Database.ExecuteSqlRaw(sql, Param);

                #endregion

                return SetupResponse.GetResponse(ResponseModel, "Success", Enum.ActionResult.Success);
            }
            catch (Exception ex)
            {
                return SetupResponse.GetResponse(ResponseModel, "Service Exception : " + ex, Enum.ActionResult.Exception);
            }
        }

        // Get private record
        public ServiceResponse<GetRecordResponseModel> GetMyRecord(GetRecordRequestModel RequestModel)
        {
            var RequestRecord = new GetRecordResponseModel();

            try
            {
                // Validate request

                if(string.IsNullOrEmpty(RequestModel.UserId) || string.IsNullOrWhiteSpace(RequestModel.UserId))
                {
                    return SetupResponse.GetErrorResponse<GetRecordResponseModel>("OwnerId name must not be null", Enum.ActionResult.Failed);
                }

                // Query records

                string sql = $"SELECT * FROM WEIGHT_RECORDS WHERE OWNER_ID = @OwnerId";
                var Param = new SqlParameter("@OwnerId", RequestModel.UserId);

                var result = _context.record.FromSqlRaw(sql, Param).ToList<RecordEntity>();
                       
                if (result == null || result.Count == 0)
                {
                    return SetupResponse.GetErrorResponse<GetRecordResponseModel>("Cannot query recird or record not found", Enum.ActionResult.Failed);
                }

                // Entity - DTO mapping

                RequestRecord.Records = new List<Record>();

                foreach (var item in result)
                {
                    Record record = new Record()
                    {
                        Name = item.NAME,
                        Weight = item.WEIGHT,
                        SubmitDate = item.SUBMIT_DATE.ToString("dd/MM/yyyy - HH:mm:ss"),
                        ScheduleDate = item.SUBMIT_SCHEDULE == null ? "" : item.SUBMIT_SCHEDULE.Value.ToString("dd/MM/yyyy - HH:mm:ss"),
                        ImagePath = item.IMAGE_PATH == null ? "" : item.IMAGE_PATH
                    };

                    RequestRecord.Records.Add(record);
                }

                return SetupResponse.GetResponse(RequestRecord, "Success", Enum.ActionResult.Success);
            }
            catch (Exception ex)
            {
                return SetupResponse.GetResponse(RequestRecord, "Service Exception : " + ex, Enum.ActionResult.Exception);
            }
        }

        // Get compare record
        public ServiceResponse<GetRecordResponseModel> GetAllRecord()
        {
            GetRecordResponseModel RequestAllRecord = new GetRecordResponseModel();

            try
            {
                // Select all
                var recordList = _context.record.ToList();

                if(recordList.Count > 0)
                {
                    RequestAllRecord.Records = new List<Record>();

                    foreach (var item in recordList)
                    {
                        Record record = new Record()
                        {
                            Name = item.NAME,
                            Weight = item.WEIGHT,
                            SubmitDate = item.SUBMIT_DATE.ToString("dd/MM/yyyy - HH:mm:ss"),
                            ScheduleDate = item.SUBMIT_SCHEDULE == null ? "" : item.SUBMIT_SCHEDULE.Value.ToString("dd/MM/yyyy - HH:mm:ss"),
                        };

                        RequestAllRecord.Records.Add(record);
                    }

                    return SetupResponse.GetResponse(RequestAllRecord, "Success", Enum.ActionResult.Success);
                }
                else
                {
                    return SetupResponse.GetResponse(RequestAllRecord, "Not found record", Enum.ActionResult.Success);
                }
            }
            catch (Exception ex)
            {
                return SetupResponse.GetResponse(RequestAllRecord, "Service Exception : " + ex, Enum.ActionResult.Exception);
            }
        }

        private string LoadAndSaveImage(string base64Image)
        {
            try
            {
                Random _rnd = new Random();

                string imgName = _rnd.Next(int.MaxValue).ToString("x");
                string filePath = @$"ImgRecord/{imgName}.jpg";
                File.WriteAllBytes(filePath, Convert.FromBase64String(base64Image));

                return filePath;
            }
            catch
            {
                return "None";
            }


        }
    }
}
