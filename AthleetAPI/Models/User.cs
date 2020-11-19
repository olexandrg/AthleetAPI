using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AthleetAPI.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirebaseUID { get; set; }
        public string UserHeadline { get; set; }
    }
}
