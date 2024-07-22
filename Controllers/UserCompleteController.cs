using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controller;
[Authorize]
[ApiController]
[Route("[controller]")]

public class UserCompleteController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly ReusableSql _reusableSql;
    public UserCompleteController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _reusableSql = new ReusableSql(config);
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
        if (_reusableSql.UpsertUser(user))
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
        if (_dapper.ExecuteSQLwithParameters(sql, sqlParameters))
        {
            return Ok();
        }
        else
        {
            throw new Exception("Failed to Delete the user");
        }
    }
}