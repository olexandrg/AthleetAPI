using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.JsonWebTokens;

namespace AthleetAPI
{
    public class Utilities
    {
        //This method will pull a UID from a JWT token
        public static String pullUID(String token)
        {
            var webToken = new JsonWebToken(token);
            return webToken.Subject;
        }
    }
}
