using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AthleetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;

namespace AthleetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AthleetContext _context;

        public UsersController(AthleetContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateUser([FromHeader(Name = "Authorization")] String token, [FromQuery(Name = "userName")] String userName, [FromQuery(Name = "description")] String Description)
        {
            String UID = Utilities.pullUID(token);

            var uid = new SqlParameter("@UID", UID);
            var name = new SqlParameter("@Name", userName);
            var description = new SqlParameter("@Headline", Description);

            await _context.Database.ExecuteSqlRawAsync("EXEC procInsertNewUser @UID, @Name, @Headline", name, uid, description);

            return StatusCode(201);
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.UserId == id);
        }
    }
}
