using DotnetAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controller;

[ApiController]
[Route("[controller]")]

public class UserSalaryController : ControllerBase
{
    DataContextDapper _dapper;
    public UserSalaryController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        // System.Console.WriteLine(config.GetConnectionString("DefaultConnection"));
    }
    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("Select GetDate()");
    }


    [HttpGet("GetUserSalary")]
    // public IActionResult Test()
    public IEnumerable<UserSalary> GetUserSalary()
    {
        string sql = @"SELECT * FROM [TutorialAppSchema].[UserSalary]";
        IEnumerable<UserSalary> userSalaries = _dapper.LoadData<UserSalary>(sql);
        return userSalaries;
    }
    [HttpGet("GetSingleUserSalary/{userId}")]
    public UserSalary GetSingleUserSalary(int userId)
    {
        string sql = @"SELECT * FROM [TutorialAppSchema].[UserSalary] WHERE UserId = " + userId.ToString();
        return _dapper.LoadDataSingle<UserSalary>(sql);
    }

    [HttpPut("EditUserSalary")]
    public IActionResult EditUserSalary(UserSalary userSalary)
    {
        string sql = @"
            UPDATE [TutorialAppSchema].[UserSalary]
                SET [UserId]='" + userSalary.UserId +
                "',[Salary]='" + userSalary.Salary +
                "' WHERE [UserId] =" + userSalary.UserId;
        System.Console.WriteLine(sql);
        if (_dapper.ExecuteSQL(sql))
        {
            return Ok();
        }
        else
        {
            throw new Exception("Failed to update the userSalary");
        }

    }
    [HttpPost("AddUserSalary")]
    public IActionResult AddUserSalary(UserSalary userSalary)
    {
        string sql = @"
        INSERT INTO  [TutorialAppSchema].[UserSalary] (
            [UserId]
            ,[Salary]
        ) VALUES(
            '" + userSalary.UserId +
            "', '" + userSalary.Salary +
            "')";

        System.Console.WriteLine(sql);
        if (_dapper.ExecuteSQL(sql))
        {
            return Ok();
        }
        else
        {
            throw new Exception("Failed to Add the user salary");
        }
    }

    [HttpDelete("DeleteUserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        string sql = @"
        Delete from [TutorialAppSchema].[UserSalary] where [UserId] = " + userId.ToString();
        System.Console.WriteLine(sql);
        if (_dapper.ExecuteSQL(sql))
        {
            return Ok();
        }
        else
        {
            throw new Exception("Failed to Delete the user Salary");
        }
    }

}

