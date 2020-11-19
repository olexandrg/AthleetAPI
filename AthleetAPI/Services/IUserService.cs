using AthleetAPI.Models;
using AthleetAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AthleetAPI.Services
{
    public interface IUserService
    {
        List<UserViewModel> GetAllUsers();

        User GetUserById(int userId);
    }
}
