using AthleetAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AthleetAPI.Services
{
    public class BusinessRules
    {
        public static bool isOddUserId(User user) =>
            user.UserId % 2 == 1;
    }
}
