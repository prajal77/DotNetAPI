﻿using DotNetApi.Data;
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

        [HttpGet("Posts")]
        public IEnumerable<Post> GetPost()
        {
            string sql = @"SELECT [PostId],
                            [UserId],
                            [PostTitle],
                            [PostContent],
                            [PostCreated],
                            [PostUpdated] FROM TutorialAppSchema.Posts";
            return _dapper.LoadData<Post>(sql);

        }

        [HttpGet("PostById/{postId}")]

        public IEnumerable<Post> GetPostById(int postId)
        {
            string sql = @"SELECT [PostId],
                            [UserId],
                            [PostTitle],
                            [PostContent],
                            [PostCreated],
                            [PostUpdated] FROM TutorialAppSchema.Posts WHERE PostId =" + postId.ToString();
            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("PostByUserId/{userId}")]

        public Post GetPostByUserId(int userId)
        {
            string sql = @"SELECT [PostId],
                            [UserId],
                            [PostTitle],
                            [PostContent],
                            [PostCreated],
                            [PostUpdated] FROM TutorialAppSchema.Posts WHERE UserId="+ userId.ToString();
            return _dapper.LoadDataSingle<Post>(sql);
        }

        [HttpGet("GetMyPost")]
        public IEnumerable<Post> GetMyPosts()
        {
            string sql = @"SELECT [PostId],
                            [UserId],
                            [PostTitle],
                            [PostContent],
                            [PostCreated],
                            [PostUpdated] FROM TutorialAppSchema.Posts WHERE UserId= " + this.User.FindFirst("userId")?.Value;
            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("PostsBySearch/{searchParam}")]
        public IEnumerable<Post> PostsBySearch(string searchParam)
        {
            string sql = @"SELECT [PostId],
                            [UserId],
                            [PostTitle],
                            [PostContent],
                            [PostCreated],
                            [PostUpdated] FROM TutorialAppSchema.Posts  
                                WHERE PostTitle LIKE '%" + searchParam + @"%'
                                OR PostContent LIKE '%" + searchParam +"%'";
            Console.WriteLine(sql);
            return _dapper.LoadData<Post>(sql);
        }

        [HttpPost("Post")]
        public IActionResult AddPost(PostToAddDto postToAdd)
        {
            string sql = @"INSERT INTO TutorialAppSchema.Posts(
                            [UserId],
                            [PostTitle],
                            [PostContent],
                            [PostCreated],
                            [PostUpdated]) VALUES(" + this.User.FindFirst("userId")?.Value
                            + ", '" + postToAdd.PostTitle
                            + "','" + postToAdd.PostContent
                            + "', GETDATE(), GETDATE() )";

            if (_dapper.ExecutSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to create new post!");
        }


        [HttpPut("Post")]
        public IActionResult EditPost(PostToEditDto postToEdit)
        {
            string sql = @"UPDATE TutorialAppSchema.Posts 
                            SET PostContent ='" + postToEdit.PostContent
                            + "',PostTitle ='" + postToEdit.PostTitle
                            + @"', PostUpdated = GETDATE()
                            WHERE PostId = " + postToEdit.PostId.ToString() +
                            "AND UserId ="+ this.User.FindFirst("userId")?.Value;

            if (_dapper.ExecutSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to edit new post!");
        }

        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql = @"DELETE FROM TutorialAppSchema.Posts WHERE PostId =" + postId.ToString()+ 
                "AND UserId = " + this.User.FindFirst("userId")?.Value; ;

            if (_dapper.ExecutSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to delete post!"); 
        }

    }
}