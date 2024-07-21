using AutoMapper;
using DotnetAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI;

[ApiController]
[Route("[controller]")]

public class UserJobInfoEFController : ControllerBase
{
    DataContextEF _ef;
    IUserRepository _userRepository;
    public UserJobInfoEFController(IConfiguration configuration, IUserRepository userRepository)
    {
        _ef = new DataContextEF(configuration);
        _userRepository = userRepository;
    }

    [HttpGet("GetAllUsersJobInfo")]
    public IEnumerable<UserJobInfo> GetAllUsersJobInfo()
    {
        IEnumerable<UserJobInfo> userJobInfos = _ef.UserJobInfo.ToList<UserJobInfo>();
        return userJobInfos;
    }
    [HttpGet("GetSingleUsersJobInfo/{userId}")]
    public UserJobInfo GetSingleUsersJobInfo(int userId)
    {
        UserJobInfo? userJobInfo = _ef.UserJobInfo.Where(o => o.UserId == userId).FirstOrDefault<UserJobInfo>();
        if (userJobInfo != null)
        {
            return userJobInfo;
        }
        else
        {
            throw new Exception("Failed to return userJobInfo");
        }
    }

    [HttpPut("EditUsersJobInfo")]
    public IActionResult EditUsersJobInfo(UserJobInfo userJobInfo)
    {
        UserJobInfo? userJob = _ef.UserJobInfo.Where(o => o.UserId == userJobInfo.UserId).FirstOrDefault<UserJobInfo>();
        if (userJob != null)
        {
            userJob.JobTitle = userJobInfo.JobTitle;
            userJob.Department = userJobInfo.Department;
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
    [HttpPost("AddUsersJobInfo")]
    public IActionResult AddUsersJobInfo(UserJobInfo userJobInfo)
    {
        UserJobInfo userJob = new UserJobInfo()
        {
            UserId = userJobInfo.UserId,
            Department = userJobInfo.Department,
            JobTitle = userJobInfo.JobTitle
        };
        _userRepository.AddEntity<UserJobInfo>(userJob);
        // _ef.Add(userJob);
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }
        else
        {
            throw new Exception("Can't able to add uses job info");
        }
    }

    [HttpDelete("DeleteUsersJobInfo/{userId}")]
    public IActionResult DeleteUsersJobInfo(int userId)
    {
        UserJobInfo? userJobInfo = _ef.UserJobInfo.Where(o => o.UserId == userId).FirstOrDefault<UserJobInfo>();
        if (userJobInfo != null)
        {
            _userRepository.RemoveEntity<UserJobInfo>(userJobInfo);
            // _ef.Remove(userJobInfo);
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
