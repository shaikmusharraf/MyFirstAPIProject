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
        string sql = @"EXEC  [TutorialAppSchema].[spUsers_Get]";

        string parameters = "";

        if (userId != 0)
        {
            parameters = parameters + " @UserId = " + userId.ToString();
        }
        if (isActive)
        {
            parameters = parameters + " ,@Active = " + isActive.ToString();
        }
        sql += parameters.Substring(1);
        System.Console.WriteLine(sql);
        IEnumerable<UserComplete> users = _dapper.LoadData<UserComplete>(sql);
        return users;
    }

    [HttpPut("UpsertUser")]
    public IActionResult EditUser(UserComplete user)
    {
        string sql = @"
            EXEC [TutorialAppSchema].[spUser_Upsert]
                @FirstName ='" + user.FirstName +
                "', @LastName ='" + user.LastName +
                "', @Email ='" + user.Email +
                "', @Gender ='" + user.Gender +
                "', @Active = '" + user.Active +
                "', @JobTitle = '" + user.JobTitle +
                "', @Department = '" + user.Department +
                "', @Salary = '" + user.Salary +
                "', @UserId =" + user.UserId;
        System.Console.WriteLine(sql);
        if (_dapper.ExecuteSQL(sql))
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
        string sql = @" EXEC [TutorialAppSchema].[spUser_Delete] @UserId = " + userId.ToString();
        System.Console.WriteLine(sql);
        if (_dapper.ExecuteSQL(sql))
        {
            return Ok();
        }
        else
        {
            throw new Exception("Failed to Delete the user");
        }
    }
}