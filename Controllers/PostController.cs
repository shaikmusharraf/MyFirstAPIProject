using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controller
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public PostController(IConfiguration configuration)
        {
            _dapper = new DataContextDapper(configuration);
        }

        [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
        public IEnumerable<Post> GetPosts(int postId = 0, int userId = 0, string searchParam = "None")
        {
            string sql = @"EXEC [TutorialAppSchema].[spPosts_Get] ";
            string parameters = "";

            if (postId != 0)
            {
                parameters = parameters + ", @PostId = " + postId.ToString();
            }
            if (userId != 0)
            {
                parameters = parameters + ", @UserId = " + userId.ToString();
            }
            if (searchParam.ToLower() != "none")
            {
                parameters += ", @SearchValue = '" + searchParam + "'";
            }

            if (parameters.Length > 0)
            {

                sql += parameters.Substring(1);
            }

            System.Console.WriteLine(sql);

            IEnumerable<Post> posts = _dapper.LoadData<Post>(sql);
            return posts;
        }
        // [HttpGet("PostSingle/{postId}")]
        // public Post GetPostSingle(int postId)
        // {
        //     string sql = @"SELECT [PostId]
        //         ,[UserId]
        //         ,[PostTitle]
        //         ,[PostContent]
        //         ,[PostCreated]
        //         ,[PostUpdated]
        //     FROM [TutorialAppSchema].[Posts] Where PostId = " + postId.ToString();
        //     return _dapper.LoadDataSingle<Post>(sql);
        // }
        // [HttpGet("PostsByUser/{userId}")]
        // public IEnumerable<Post> GetPostsByUser(int userId)
        // {
        //     string sql = @"SELECT [PostId]
        //         ,[UserId]
        //         ,[PostTitle]
        //         ,[PostContent]
        //         ,[PostCreated]
        //         ,[PostUpdated]
        //     FROM [TutorialAppSchema].[Posts] Where UserId = " + userId.ToString();
        //     IEnumerable<Post> posts = _dapper.LoadData<Post>(sql);
        //     return posts;
        // }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            string sql = @"EXEC [TutorialAppSchema].[spPosts_Get]  @UserId = " +
            this.User.FindFirst("userId")?.Value;
            IEnumerable<Post> posts = _dapper.LoadData<Post>(sql);
            return posts;
        }

        [HttpPut("UpsertPost")]
        public IActionResult AddPosts(Post postToUpsert)
        {
            string sql = @" EXEC [TutorialAppSchema].[spPosts_Upsert]
            @UserId =" + this.User.FindFirst("userId")?.Value +
            ",@PostTitle ='" + postToUpsert.PostTitle +
                    "',@PostContent = '" + postToUpsert.PostContent
                    + "'";
            if (postToUpsert.PostId > 0)
            {
                sql += ", @PostId = " + postToUpsert.PostId;
            }
            if (_dapper.ExecuteSQL(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to add Post");
        }

        // [HttpPut("EditPost")]
        // public IActionResult EditPost(PostToEdtDto postToEdt)
        // {
        //     string sql = @" UPDATE [TutorialAppSchema].[Posts] 
        //         SET [PostContent]  = '" + postToEdt.PostContent +
        //         "',[PostTitle] = '" + postToEdt.PostTitle +
        //         @"',[PostUpdated] = GETDATE())
        //             Where PostId = " + postToEdt.PostId.ToString() +
        //             "AND UserId = " + this.User.FindFirst("userId")?.Value;
        //     ;
        //     if (_dapper.ExecuteSQLwithRow(sql) > 0)
        //     {
        //         return Ok();
        //     }
        //     else
        //     {
        //         throw new Exception("Failed to edit Post");
        //     }
        // }
        [HttpDelete("DeletePost/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql = @"
            EXEC [TutorialAppSchema].[spPost_Delete]
            @PostId=" + postId.ToString()
            + ", @UserId = "+ this.User.FindFirst("userId")?.Value;
            if (_dapper.ExecuteSQLwithRow(sql) > 0)
            {
                return Ok();
            }
            else
            {
                throw new Exception("Failed to Delete Post");
            }

        }

        // [HttpGet("SearchPosts/{searchParam}")]
        // public IEnumerable<Post> SearchPosts(string searchParam)
        // {
        //     string sql = @"SELECT [PostId]
        //         ,[UserId]
        //         ,[PostTitle]
        //         ,[PostContent]
        //         ,[PostCreated]
        //         ,[PostUpdated]
        //     FROM [TutorialAppSchema].[Posts] where 
        //     PostTitle like '%" + searchParam + "%'" +
        //     " OR PostContent like '%" + searchParam + "%'";
        //     IEnumerable<Post> posts = _dapper.LoadData<Post>(sql);
        //     return posts;
        // }
    }
}