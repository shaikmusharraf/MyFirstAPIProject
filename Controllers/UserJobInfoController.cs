using DotnetAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controller;

[ApiController]
[Route("[controller]")]

public class UserJobInfoController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    public UserJobInfoController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("TestConnection")]
    public DateTime GetDateTime()
    {
        string sql = @"SELECT GETDATE()";
        System.Console.WriteLine(sql);
        return _dapper.LoadDataSingle<DateTime>(sql);
    }

    [HttpGet("GetAllUsersJobInfo")]
    public IEnumerable<UserJobInfo> GetAllUsersJobInfo()
    {
        string sql = @"SELECT * FROM [TutorialAppSchema].[UserJobInfo]";

        System.Console.WriteLine(sql);

        return _dapper.LoadData<UserJobInfo>(sql);

    }

    [HttpGet("GetSingleUsersJobInfo/{userId}")]
    public UserJobInfo GetSingleUsersJobInfo(int userId)
    {
        string sql = @"SELECT * FROM [TutorialAppSchema].[UserJobInfo] WHERE [UserId] = " + userId.ToString();

        System.Console.WriteLine(sql);

        return _dapper.LoadDataSingle<UserJobInfo>(sql);
    }

    [HttpPut("EditUsersJobInfo")]
    public IActionResult EditUsersJobInfo(UserJobInfo userJobInfo)
    {
        string sql = @"UPDATE  [TutorialAppSchema].[UserJobInfo]
                        SET [JobTitle] ='" + userJobInfo.JobTitle +
                        "', [Department] = '" + userJobInfo.Department +
                        "' WHERE UserId = " + userJobInfo.UserId;

        System.Console.WriteLine(sql);
        if (_dapper.ExecuteSQL(sql))
        {
            return Ok();
        }
        else
        {
            throw new Exception("Failed to update the UserJobInfo");
        }

    }
    [HttpPost("AddUserJobInfo")]
    public IActionResult AddUserJobInfo(UserJobInfo userJobInfo)
    {
        string sql = @"INSERT INTO [TutorialAppSchema].[UserJobInfo] (
                    UserId,
                    JobTitle,
                    Department
                    ) VALUES(
                        '" + userJobInfo.UserId +
                        "','" + userJobInfo.JobTitle +
                        "' , '" + userJobInfo.Department +
                        "')";
        System.Console.WriteLine(sql);
        if (_dapper.ExecuteSQL(sql))
        {
            return Ok();
        }
        else
        {
            throw new Exception("Failed to add the UserJobInfo");
        }

    }
    [HttpDelete("DeleteUserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfo(int userId)
    {
        string sql = @"DELETE FROM  [TutorialAppSchema].[UserJobInfo] WHERE userId = " + userId.ToString();
        System.Console.WriteLine(sql);
        if (_dapper.ExecuteSQL(sql))
        {
            return Ok();
        }
        else
        {
            throw new Exception("Failed to Delete the UserJobInfo");
        }
    }
}
