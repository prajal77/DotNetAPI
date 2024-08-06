using DotNetApi.Models;
using System.Linq;

namespace DotNetApi.Data
{
    public class UserRepository:IUserRepository
    {
        DataContextEf _entityFramework;

        public UserRepository(IConfiguration config)
        {
            _entityFramework = new DataContextEf(config);
        }

        public bool SaveChanges()
        {
            return _entityFramework.SaveChanges()>0;
        }

        public void AddEntity<T>(T entityToAdd)
        {
            if(entityToAdd != null)
            {
                _entityFramework.Add(entityToAdd);
            }
        }
        public void RemoveEntity<T>(T entityToRemove)
        {
            if (entityToRemove != null)
            {
                _entityFramework.Remove(entityToRemove);
            }
        }
        public IEnumerable<User> GetUsers()
        {

            IEnumerable<User> users = _entityFramework.Users.ToList<User>();
            return users;
        }

        public User GetSingleUsers(int userId)
        {
            User? user = _entityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault<User>();

            if (user != null)
            {
                return user;
            }
            throw new Exception("failed to Get User");
        }

        public UserSalary GetSingleUsersSalary(int userId)
        {
            UserSalary? userSalary = _entityFramework.UserSalary.Where(u => u.UserId == userId).FirstOrDefault<UserSalary>();

            if (userSalary != null)
            {
                return userSalary;
            }
            throw new Exception("failed to Get User");
        }

        public UserJobInfo GetSingleUsersJobInfo(int userId)
        {
            UserJobInfo? userJobInfo = _entityFramework.UserJobInfo.Where(u => u.UserId == userId).FirstOrDefault<UserJobInfo>();

            if (userJobInfo != null)
            {
                return userJobInfo;
            }
            throw new Exception("failed to Get User");
        }


    }
}
