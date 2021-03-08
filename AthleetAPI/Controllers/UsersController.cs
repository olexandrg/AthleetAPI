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

        // POST: api/Users
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateUser(
            [FromHeader(Name = "Authorization")] String token,
            [FromQuery(Name = "userName")] String userName,
            [FromQuery(Name = "description")] String Description)
        {
            String UID = Utilities.pullUID(token);
            var uid = new SqlParameter("@UID", UID);
            var name = new SqlParameter("@Name", userName);
            var description = new SqlParameter("@Headline", Description);
            var result = await _context.Database.ExecuteSqlRawAsync("EXEC procInsertNewUser @UID, @Name, @Headline", name, uid, description);
            if (result > 0)
                return StatusCode(201);     //This means the user is truly new and a new db entry was added
            else
                return StatusCode(200);     //This means the user is a returning user and the db has not been modified.
        }
        // GET: api/Users
        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<User>> CheckExistingUser(
            [FromHeader(Name = "Authorization")] String token)
        {
            String UID = Utilities.pullUID(token);
            var uid = new SqlParameter("@UID", UID);

            var users = _context.User.FromSqlRaw("SELECT * FROM dbo.[User] where FirebaseUID = @UID", uid).ToList();
            if (users == null)
                return StatusCode(403);

            return users;
        }
        // PUT: api/Users/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> ChangeUsername(
            [FromHeader(Name = "Authorization")] String token,
            [FromQuery(Name = "userName")] String userName
            )
        {
            String UID = Utilities.pullUID(token);
            var uid = new SqlParameter("@UID", UID);
            var name = new SqlParameter("@Name", userName);
            var user = _context.User.FromSqlRaw("SELECT * FROM dbo.[User] where FirebaseUID = @UID", uid).First();
            if (user == null){ return NotFound();}
            int id = user.UserId;

            //user.UserName = userName;
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC procChangeUsername @UID, @Name", uid, name);
            }
            catch (Exception)
            {
                return NotFound();
            }

            return NoContent();
            //String UID = Utilities.pullUID(token);
            //var uid = new SqlParameter("@UID", UID);
            //var name = new SqlParameter("@Name", userName);
            //await _context.Database.ExecuteSqlRawAsync("EXEC procChangeUsername @UID, @Name", uid, name);

            //return NoContent();     //Updated succeeded
        }
        // PUT: api/Users
        [HttpPut]
        [Authorize]
        public async Task<ActionResult> ChangeHeadline(
            [FromHeader(Name = "Authorization")] String token,
            [FromQuery(Name = "Headline")] String headline
            )
        {
            String UID = Utilities.pullUID(token);
            var uid = new SqlParameter("@UID", UID);
            var name = new SqlParameter("@Headline", headline);
            var result = await _context.Database.ExecuteSqlRawAsync("EXEC procChangeHeadline @UID, @Headline", uid, headline);
            if (result > 0)
                return StatusCode(201);     //This means the user is truly new and a new db entry was added
            else
                return StatusCode(200);     //This means the user is a returning user and the db has not been modified.
        }
    }
}
