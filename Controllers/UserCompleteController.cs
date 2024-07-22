using System.Data;
using Dapper;
using DotnetAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controller;

[ApiController]
[Route("[controller]")]

public class UserCompleteController : ControllerBase
{
    DataContextDapper _dapper;
    public UserCompleteController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        // System.Console.WriteLine(config.GetConnectionString("DefaultConnection"));
    }
    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("Select GetDate()");
    }


    [HttpGet("GetUsers/{userId}/{isActive}")]
    // public IActionResult Test()
    public IEnumerable<UserComplete> GetUsers(int userId, bool isActive)
    {
        DynamicParameters sqlParameters = new DynamicParameters();
        string sql = @"EXEC  [TutorialAppSchema].[spUsers_Get]";

        string parameters = "";

        if (userId != 0)
        {
            parameters = parameters + " @UserId =@UserIdParameter";
            sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
        }
        if (isActive)
        {
            parameters = parameters + " ,@Active = @ActiveParameter";
            sqlParameters.Add("@ActiveParameter", isActive, DbType.Boolean);
        }
        if (parameters.Length > 0)
        {
            sql += parameters.Substring(1);
        }
        System.Console.WriteLine(sql);
        IEnumerable<UserComplete> users = _dapper.LoadDatawithParameters<UserComplete>(sql, sqlParameters);
        return users;
    }

    [HttpPut("UpsertUser")]
    public IActionResult UpsertUser(UserComplete user)
    {
        DynamicParameters sqlParameters = new DynamicParameters();
        string sql = @"
            EXEC [TutorialAppSchema].[spUser_Upsert]
                @FirstName =@FirstNameParameter
                , @LastName =@LastNameParameter
                , @Email =@EmailParameter
                , @Gender =@GenderParameter
                , @Active = @ActiveParameter
                , @JobTitle =@JobTitleParameter  
                , @Department =@DepartmentParameter 
                , @Salary = @SalaryParameter
                , @UserId =@UserIdParameter";
        System.Console.WriteLine(sql);
        sqlParameters.Add("@FirstNameParameter", user.FirstName, DbType.String);
        sqlParameters.Add("@LastNameParameter", user.LastName, DbType.String);
        sqlParameters.Add("@EmailParameter", user.Email, DbType.String);
        sqlParameters.Add("@GenderParameter", user.Gender, DbType.String);
        sqlParameters.Add("@ActiveParameter", user.Active, DbType.Boolean);
        sqlParameters.Add("@JobTitleParameter", user.JobTitle, DbType.String);
        sqlParameters.Add("@DepartmentParameter", user.Department, DbType.String);
        sqlParameters.Add("@SalaryParameter", user.Salary, DbType.Decimal);
        sqlParameters.Add("@UserIdParameter", user.UserId, DbType.Int32);


        if (_dapper.ExecuteSQLwithParameters(sql,sqlParameters))
        {
            return Ok();
        }
        else
        {
            throw new Exception("Failed to update the user");
        }
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @" EXEC [TutorialAppSchema].[spUser_Delete] @UserId = @UserIdParameter";

        DynamicParameters sqlParameters = new DynamicParameters();
        sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);

        System.Console.WriteLine(sql);
        if (_dapper.ExecuteSQLwithParameters(sql,sqlParameters))
        {
            return Ok();
        }
        else
        {
            throw new Exception("Failed to Delete the user");
        }
    }
}