using DotnetAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controller;

[ApiController]
[Route("[controller]")]

public class UserController : ControllerBase
{
    DataContextDapper _dapper;
    public UserController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        // System.Console.WriteLine(config.GetConnectionString("DefaultConnection"));
    }
    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("Select GetDate()");
    }


    [HttpGet("GetUsers")]
    // public IActionResult Test()
    public IEnumerable<User> GetUsers()
    {
        string sql = @"SELECT * FROM [TutorialAppSchema].[Users]";
        IEnumerable<User> users = _dapper.LoadData<User>(sql);
        return users;
    }
    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        string sql = @"SELECT * FROM [TutorialAppSchema].[Users] WHERE UserId = " + userId.ToString();
        return _dapper.LoadDataSingle<User>(sql);
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string sql = @"
            UPDATE [TutorialAppSchema].[Users]
                SET [FirstName]='" + user.FirstName +
                "',[LastName]='" + user.LastName +
                "',[Email]='" + user.Email +
                "',[Gender]='" + user.Gender +
                "',[Active]= '" + user.Active +
                "' WHERE [UserId] =" + user.UserId;
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
    [HttpPost("AddUser")]
    public IActionResult AddUser(UserDto user)
    {
        string sql = @"
        INSERT INTO  [TutorialAppSchema].[Users] (
            [FirstName]
            ,[LastName]
            ,[Email]
            ,[Gender]
            ,[Active]
        ) VALUES(
            '" + user.FirstName +
            "', '" + user.LastName +
            "', '" + user.Email +
            "', '" + user.Gender +
            "', '" + user.Active +
            "')";

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
        string sql = @"
        Delete from [TutorialAppSchema].[Users] where [UserId] = " + userId.ToString();
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

