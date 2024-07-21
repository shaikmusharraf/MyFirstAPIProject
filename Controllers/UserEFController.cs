using AutoMapper;
using DotnetAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controller;

[ApiController]
[Route("[controller]")]

public class UserEFController : ControllerBase
{
    IUserRepository _userRepository;
    IMapper _mapper;
    public UserEFController(IConfiguration config, IUserRepository userRepository)
    {
        _userRepository = userRepository;

        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserDto, User>();
        }));
    }
    [HttpGet("GetUsers")]

    public IEnumerable<User> GetUsers()
    {
        IEnumerable<User> users = _userRepository.GetUsers();
        return users;
    }
    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        return _userRepository.GetSingleUser(userId);
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        User? userDb = _userRepository.GetSingleUser(user.UserId);
        if (userDb != null)
        {
            userDb.FirstName = user.FirstName;
            userDb.LastName = user.LastName;
            userDb.Active = user.Active;
            userDb.Gender = user.Gender;
            userDb.Email = user.Email;
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            else
            {
                throw new Exception("Failed to Update user");
            }
        }
        else throw new Exception("Can't able to find it in Database");
    }
    [HttpPost("AddUser")]
    public IActionResult AddUser(UserDto user)
    {
        User userDb = _mapper.Map<User>(user);

        _userRepository.AddEntity<User>(userDb);
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }
        else
        {
            throw new Exception("Failed to Add user");
        }
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        User? userDb = _userRepository.GetSingleUser(userId);
        if (userDb != null)
        {
            _userRepository.RemoveEntity<User>(userDb);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            else
            {
                throw new Exception("Failed to Delete user");
            }
        }
        else throw new Exception("Failed to find user in DB.");
    }
}


























/*
using AutoMapper;
using DotnetAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controller;

[ApiController]
[Route("[controller]")]

public class UserEFController : ControllerBase
{
    // DataContextEF _entityframework;

    IUserRepository _userRepository;

    IMapper _mapper;

    public UserEFController(IConfiguration config, IUserRepository userRepository)
    {
        // _entityframework = new DataContextEF(config);

        _userRepository = userRepository;

        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserDto, User>();
        }));
        // System.Console.WriteLine(config.GetConnectionString("DefaultConnection"));
    }

    // [HttpGet("TestConnection")]
    // public DateTime TestConnection()
    // {
    //     return _entityframework.LoadDataSingle<DateTime>("Select GetDate()");
    // }


    [HttpGet("GetUsers")]
    // public IActionResult Test()
    public IEnumerable<User> GetUsers()
    {
        // string sql = @"SELECT * FROM [TutorialAppSchema].[Users]";
        IEnumerable<User> users = _userRepository.GetUsers();
        return users;
    }
    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        // string sql = @"SELECT * FROM [TutorialAppSchema].[Users] WHERE UserId = " + userId.ToString();
        // User? user = _entityframework.Users.Where(o => o.UserId == userId).FirstOrDefault<User>();
        // if (user != null)
        // {
        //     return user;
        // }
        // else
        // {
        //     throw new Exception("Failed to return user");
        // }
        return _userRepository.GetSingleUser(userId);
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        User? userDb = _userRepository.GetSingleUser(user.UserId);
        if (userDb != null)
        {
            userDb.FirstName = user.FirstName;
            userDb.LastName = user.LastName;
            userDb.Active = user.Active;
            userDb.Gender = user.Gender;
            userDb.Email = user.Email;
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            else
            {
                throw new Exception("Failed to Update user");
            }
        }
        else throw new Exception("Can't able to find it in Database");
    }
    [HttpPost("AddUser")]
    public IActionResult AddUser(UserDto user)
    {
        User userDb = _mapper.Map<User>(user);
        // User userDb = new User
        // {
        //     FirstName = user.FirstName,
        //     LastName = user.LastName,
        //     Active = user.Active,
        //     Gender = user.Gender,
        //     Email = user.Email
        // };
        // _entityframework.Add(userDb);
        _userRepository.AddEntity<User>(userDb);
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }
        else
        {
            throw new Exception("Failed to Add user");
        }
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        User? userDb =_userRepository.GetSingleUser(userId);
        if (userDb != null)
        {
            // _entityframework.Remove<User>(userDb);
            _userRepository.RemoveEntity<User>(userDb);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            else
            {
                throw new Exception("Failed to Delete user");
            }
        }
        else throw new Exception("Failed to find user in DB.");
    }
}



*/