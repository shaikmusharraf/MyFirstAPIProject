// using System.Security.Cryptography;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;


namespace DotnetAPI.Controller
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly ReusableSql _reusableSql;
        // private readonly IConfiguration _config;
        private readonly AuthHelper _authHelper;
        private readonly IMapper _mapper;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            // _config = config;
            _authHelper = new AuthHelper(config);
            _reusableSql = new ReusableSql(config);
            _mapper = new Mapper(new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<UserForRegistrationDto, UserComplete>();
                }
            ));
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sqlCheckUserExists = @" SELECT Email FROM [TutorialAppSchema].[Auth] WHERE Email = '" + userForRegistration.Email + "'";
                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
                if (existingUsers.Count() == 0)
                {

                    // byte[] passwordSalt = new byte[128 / 8];
                    // using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    // {
                    //     rng.GetNonZeroBytes(passwordSalt);
                    // }

                    // byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);


                    // string sqlAddAuth = @"EXEC [TutorialAppSchema].[spRegistration_Upsert]
                    //                     @Email=@EmailParam ,@PasswordHash=@PasswordHashParam, @PasswordSalt=@PasswordSaltParam";

                    // List<SqlParameter> sqlParameters = new List<SqlParameter>();

                    // SqlParameter emailParameter = new SqlParameter("@EmailParam", SqlDbType.VarChar);
                    // emailParameter.Value = userForRegistration.Email;
                    // sqlParameters.Add(emailParameter);

                    // SqlParameter passwordHashParameter = new SqlParameter("@PasswordHashParam", SqlDbType.VarBinary);
                    // passwordHashParameter.Value = passwordHash;
                    // sqlParameters.Add(passwordHashParameter);

                    // SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSaltParam", SqlDbType.VarBinary);
                    // passwordSaltParameter.Value = passwordSalt;
                    // sqlParameters.Add(passwordSaltParameter);

                    UserForLoginDto userForSetPwd = new UserForLoginDto()
                    {
                        Email = userForRegistration.Email,
                        Password = userForRegistration.Password
                    };

                    if (_authHelper.SetPassword(userForSetPwd))
                    {
                        UserComplete userComplete = _mapper.Map<UserComplete>(userForRegistration);
                        userComplete.Active= true;


                    //     string sqladdUser = @"EXEC [TutorialAppSchema].[spUser_Upsert]
                    // @FirstName ='" + userForRegistration.FirstName +
                    // "', @LastName ='" + userForRegistration.LastName +
                    // "', @Email ='" + userForRegistration.Email +
                    // "', @Gender ='" + userForRegistration.Gender +
                    // "', @Active = 1" +
                    // ", @JobTitle = '" + userForRegistration.JobTitle +
                    // "', @Department = '" + userForRegistration.Department +
                    // "', @Salary = '" + userForRegistration.Salary + "'";


                        // string sqladdUser = @"
                        // INSERT INTO  [TutorialAppSchema].[Users] (
                        //     [FirstName]
                        //     ,[LastName]
                        //     ,[Email]
                        //     ,[Gender]
                        //     ,[Active]
                        // ) VALUES(
                        //     '" + userForRegistration.FirstName +
                        //     "', '" + userForRegistration.LastName +
                        //     "', '" + userForRegistration.Email +
                        //     "', '" + userForRegistration.Gender +
                        //     "',1)";

                        if (_reusableSql.UpsertUser(userComplete))
                        {
                            return Ok();
                        }
                        throw new Exception("Failed to add user");
                    }
                    throw new Exception("Failed to register user");
                }
                throw new Exception("User already exists");
            }
            throw new Exception("password and confirm password doesn't match");
        }

        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(UserForLoginDto userForSetPwd)
        {
            if (_authHelper.SetPassword(userForSetPwd))
            {
                return Ok();
            }
            throw new Exception("Failed to update password");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlForHashandSalt = @"EXEC [TutorialAppSchema].[spLoginConfirmation_Get] 
                    @Email = @EmailParam";

            DynamicParameters sqlParameters = new DynamicParameters();
            // SqlParameter emailParameter = new SqlParameter("@EmailParam", SqlDbType.VarChar);
            // emailParameter.Value = userForLogin.Email;
            // sqlParameters.Add(emailParameter);
            sqlParameters.Add("@EmailParam", userForLogin.Email, DbType.String);


            // string sqlForHashandSalt = @"SELECT
            //                             [PasswordHash]
            //                             ,[PasswordSalt] FROM [TutorialAppSchema].[Auth] WHERE Email = '" + userForLogin.Email + "'";

            UserForLoginConfirmationDto userForLoginConfirmation = _dapper.LoadDataSinglewithParameters<UserForLoginConfirmationDto>(sqlForHashandSalt, sqlParameters);

            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForLoginConfirmation.PasswordSalt);
            for (int i = 0; i < passwordHash.Length; i++)
            {
                if (passwordHash[i] != userForLoginConfirmation.PasswordHash[i])
                {
                    return StatusCode(401, "Incorrect Password!");
                }
            }
            string userIdSql = @"SELECT UserId from [TutorialAppSchema].[Users] WHERE Email = '" + userForLogin.Email + "'";
            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string>{
                {"token",_authHelper.CreateToken(userId)}
            });
        }


        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            string userId = User.FindFirst("userId")?.Value + "";
            string userIdSql = "SELECT UserId from [TutorialAppSchema].[Users] where UserId = " + userId;

            int userIdFromDB = _dapper.LoadDataSingle<int>(userIdSql);
            return Ok(new Dictionary<string, string>{
                {"token",_authHelper.CreateToken(userIdFromDB)}
            });
        }
    }
}