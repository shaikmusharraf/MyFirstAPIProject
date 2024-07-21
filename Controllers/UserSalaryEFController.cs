using DotnetAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controller;

[ApiController]
[Route("[controller]")]

public class UserSalaryEFController : ControllerBase
{
    DataContextEF _ef;
    IUserRepository _userRepository;
    public UserSalaryEFController(IConfiguration configuration, IUserRepository userRepository)
    {
        _ef = new DataContextEF(configuration);
        _userRepository = userRepository;
    }
    [HttpGet("GetUserSalary")]
    public IEnumerable<UserSalary> GetUserSalary()
    {
        IEnumerable<UserSalary> userSalaries = _ef.UserSalary.ToList<UserSalary>();
        return userSalaries;
    }
    [HttpGet("GetSingleUserSalary/{userId}")]
    public UserSalary GetSingleUserSalary(int userId)
    {
        UserSalary? userSalary = _ef.UserSalary.Where(o=>o.UserId == userId).FirstOrDefault<UserSalary>();
        if(userSalary != null)
        {
            return userSalary;
        }else {
            throw new Exception("Failed to return user salary");
        }
    }

    [HttpPut("EditUserSalary")]
    public IActionResult EditUserSalary(UserSalary usersalary)
    {
        UserSalary? userSalary = _ef.UserSalary.Where(o => o.UserId == usersalary.UserId).FirstOrDefault<UserSalary>();
        if (userSalary != null)
        {
            userSalary.Salary = usersalary.Salary;
        }
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }
        else
        {
            throw new Exception("Can't able to find it in Database");
        }
    }
    [HttpPost("AddUsersSalary")]
    public IActionResult AddUsersSalary(UserSalary userSalary)
    {
        UserSalary usersalary = new UserSalary()
        {
            UserId = userSalary.UserId,
            Salary = userSalary.Salary
        };
        _userRepository.AddEntity<UserSalary>(usersalary);
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }
        else
        {
            throw new Exception("Can't able to add uses job info");
        }
    }

    [HttpDelete("DeleteUserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        UserSalary? userSalary = _ef.UserSalary.Where(o => o.UserId == userId).FirstOrDefault<UserSalary>();
        if (userSalary != null)
        {
            // _ef.Remove(userSalary);
            _userRepository.RemoveEntity<UserSalary>(userSalary);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            else
            {
                throw new Exception("Failed to Delete user");
            }
        }
        else {
            throw new Exception("Failed to find user in DB.");
        }
    }


}

