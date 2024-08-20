using Dapper;
using DotNetApi.Data;
using DotNetApi.Dtos;
using DotNetApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

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
            DynamicParameters sqlParameter = new DynamicParameters();

            if(postId != 0)
            {
                parameter += ", @PostId= @PostIdPram";
                sqlParameter.Add("@PostIdPram", postId, DbType.Int32);

            }
            if (userId != 0)
            {
                parameter += ", @UserId= @UserIdParam";
                sqlParameter.Add("@UserIdParam", postId, DbType.Int32);

            }
            if (searchParam.ToLower() != "none")
            {
                parameter += ", @SearchValue='" + searchParam +"'" ;
                sqlParameter.Add("@SearchValueParam",searchParam, DbType.String);
            }
            if(parameter.Length > 0)
            {

            sql += parameter.Substring(1);
            }


            return _dapper.LoadDataParameters<Post>(sql, sqlParameter);

        }



        [HttpGet("GetMyPost")]
        public IEnumerable<Post> GetMyPosts()
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Get @UserId = @UserParam";
            DynamicParameters sqlParameter = new DynamicParameters();

            sqlParameter.Add("@UserParam", this.User.FindFirst("userId")?.Value, DbType.Int32);

            return _dapper.LoadDataParameters<Post>(sql,sqlParameter);
        }

    

        [HttpPut("UpsertPost")]
        public IActionResult UpsertPost(Post postToUpsert)
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Upsert
                            @UserId = @UserIdParameter, 
                            @PostTitle = @PostTitleParam,
                            @PostContent = @PostContentParam";

            DynamicParameters sqlParameter = new DynamicParameters();
            sqlParameter.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, DbType.Int32);
            sqlParameter.Add("@PostTitleParam", postToUpsert.PostTitle, DbType.Int32);
            sqlParameter.Add("@PostContentParam", postToUpsert.PostContent, DbType.Int32);


            if (postToUpsert.PostId > 0)
            {
            sql += ", @PostID= @PostIdParam";
                sqlParameter.Add("@PostIdParam", postToUpsert.PostId, DbType.Int32);
            }

                     
            if (_dapper.ExecutSqlWithParameter(sql,sqlParameter))
            {
                return Ok();
            }
            throw new Exception("Failed to upsert post!");
        }


        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql = @"EXEC TutorialAppSchema.spPost_Delete
                                @PostId =@PostIdParam , @UserId = @UserIdParam";
            DynamicParameters sqlParameter = new DynamicParameters();

            sqlParameter.Add("@PostIdParam",postId, DbType.Int32);
            sqlParameter.Add("@UserIdParam", this.User.FindFirst("userId")?.Value, DbType.Int32);

            if (_dapper.ExecutSqlWithParameter(sql,sqlParameter))
            {
                return Ok();
            }
            throw new Exception("Failed to delete post!"); 
        }

    }
}
