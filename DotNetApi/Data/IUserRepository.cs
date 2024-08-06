using DotNetApi.Models;

namespace DotNetApi.Data
{
    public interface IUserRepository
    {
        public bool SaveChanges();
        public void AddEntity<T>(T entityToAdd);

        public void RemoveEntity<T>(T entityToRemove);
        public IEnumerable<User> GetUsers();
        public User GetSingleUsers(int userId);
        public UserSalary GetSingleUsersSalary(int userId);
        public UserJobInfo GetSingleUsersJobInfo(int userId);

    }
}
