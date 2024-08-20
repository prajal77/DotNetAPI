using DotNetApi.Data;
using DotNetApi.Dtos;
using DotNetApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotNetApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        public PostController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);

        }

        [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
        public IEnumerable<Post> GetPost(int postId = 0, int userId = 0, string searchParam="None")
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Get";

            string parameter = "";

            if(postId != 0)
            {
                parameter += ", @PostId=" + postId.ToString();
            }
            if (userId != 0)
            {
                parameter += ", @UserId=" + userId.ToString();
            }
            if (searchParam.ToLower() != "none")
            {
                parameter += ", @SearchValue='" + searchParam +"'" ;
            }
            if(parameter.Length > 0)
            {

            sql += parameter.Substring(1);
            }


            return _dapper.LoadData<Post>(sql);

        }



        [HttpGet("GetMyPost")]
        public IEnumerable<Post> GetMyPosts()
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Get @UserId =" + this.User.FindFirst("userId")?.Value;
            return _dapper.LoadData<Post>(sql);
        }

    

        [HttpPut("UpsertPost")]
        public IActionResult UpsertPost(Post postToUpsert)
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Upsert
                            @UserId =" + this.User.FindFirst("userId")?.Value +
                            @", @PostTitle ='" + postToUpsert.PostTitle +
                            @"',@PostContent ='" + postToUpsert.PostContent + "'";
            
            if(postToUpsert.PostId > 0)
            {
            sql +=", @PostID= " + postToUpsert.PostId;
            }

                     
            if (_dapper.ExecutSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to upsert post!");
        }


        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql = @"EXEC TutorialAppSchema.spPost_Delete
                                @PostId =" + postId.ToString()+ 
                ", @UserId = " + this.User.FindFirst("userId")?.Value; ;

            if (_dapper.ExecutSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to delete post!"); 
        }

    }
}
