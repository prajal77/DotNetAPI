using DotNetApi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DotNetApi.Models;
using AutoMapper;

namespace DotNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserSalaryEf : ControllerBase
    {
        DataContextEf _entity;
        IMapper _mapper;
        public UserSalaryEf(IConfiguration config)
        {
            _entity = new DataContextEf(config);
            _mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<UserSalary, UserSalary>()));
         
        }

        [HttpGet("UserSalaryGet")]
        public IEnumerable<UserSalary> Get()
        {
            IEnumerable<UserSalary> UserSalary = _entity.UserSalary.ToList();
            return UserSalary;
        }

        [HttpGet("UserSalaryGetById/{userId}")]
        public UserSalary GetById(int userId)
        {
            UserSalary? userSalary = _entity.UserSalary.Where(u => u.UserId == userId).FirstOrDefault<UserSalary>();

            if (userSalary != null)
            {

            return userSalary;
            }
            else
            {
                throw new Exception("UserSalary not found");
            }
        }

        [HttpPost("UserSalaryPost")]
        public IActionResult PostUserSalary (UserSalary userSalary )
        {
            _entity.UserSalary.Add(userSalary);
            if (_entity.SaveChanges()>0)
            {
                return Ok();
            }
            throw new Exception("Adding UserSalary failed on save");
        }

        [HttpPut("UserSalaryUpdate")]

        public IActionResult UpdateSalary(UserSalary userSalary)
        {


            UserSalary? userSalaryToUpdate = _entity.UserSalary.Where(u => u.UserId == userSalary.UserId).FirstOrDefault<UserSalary>();
            if(userSalaryToUpdate !=null)
            {
                _mapper.Map(userSalary, userSalaryToUpdate);
                if(_entity.SaveChanges()>0)
                {
                    return Ok();
                }
                throw new Exception("Updating UserSalary failed on save");
            }
            else
            {

                throw new Exception("Failed to find UserSalary to update");
            }
        }

        [HttpDelete("userSalaryDelete{userId}")]
        public IActionResult UserSalaryDelete(int userId)
        {
            UserSalary? userSalary = _entity.UserSalary.Where(u=>u.UserId == userId).FirstOrDefault();

            if(userSalary != null)
            {
                _entity.UserSalary.Remove(userSalary);
                if (_entity.SaveChanges() > 0)
                {
                    return Ok();
                }
                else
                {
                    throw new Exception("Cannot delete the userSalary");
                }
            }
            throw new Exception("Cannot find the userSalary");
            
        }

    }
}
