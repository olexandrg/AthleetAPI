using AthleetAPI.Models;
using AthleetAPI.Repositories;
using AthleetAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AthleetAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _UserRespository;

        public UserService(IUserRepository UserRepository)
        {
            _UserRespository = UserRepository;
        }





        public List<UserViewModel> GetAllUsers()
        {
            var UserViewModels = new List<UserViewModel>();

            foreach (var User in _UserRespository.GetAllUsers())
            {
                UserViewModels.Add(new UserViewModel
                {
                    UserName = User.UserName,
                    UserHeadline = User.UserHeadline,
                    Special = BusinessRules.isOddUserId(User)
                });
            }

            return UserViewModels;
        }

        public User GetUserById(int userId)
        {
            return _UserRespository.GetUserById(userId);
        }
    }
}
