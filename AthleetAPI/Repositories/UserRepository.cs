using AthleetAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AthleetAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AthleetContext _dbContext;

        public UserRepository(AthleetContext dbContext)
        {
            _dbContext = dbContext;
        }


        public List<User> GetAllUsers()
        {
            try
            {
                return _dbContext.Users.ToList();
            }
            catch (Exception ex)
            {
                return new List<User>();
            }
        }

        public User GetUserById(int userId)
        {
            return _dbContext.Users.SingleOrDefault(u => u.UserId == userId);
        }
    }
}
