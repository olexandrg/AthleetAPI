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
using System.Text.RegularExpressions;

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
        // only allowed to overwrite name and headline of user
        [HttpPut("{id}")]
        [Authorize]
        public ActionResult ChangeUsername(
            [FromHeader(Name = "Authorization")] String token,
            [FromBody] User user,
            int id
            )
        {
            var entity = _context.User.FirstOrDefault(user => user.UserId == id);

            if (entity == null)
                return StatusCode(204);
            try
            {
                entity.UserName = user.UserName;
                entity.UserHeadline = user.UserHeadline;
                _context.SaveChanges();
            }

            catch (Exception e)
            {
                return StatusCode(204, e.Message);
            }
            return StatusCode(200);
        }       
        
        // PUT: api/Users/blockUser
        [HttpPost("blockUser")]
        [Authorize]
        public ActionResult blockUser(
            [FromHeader(Name = "Authorization")] String token,
            [FromBody] String username)
        {
            String uid = Utilities.pullUID(token);
            User userToBlock = _context.User.FirstOrDefault(user => user.UserName == username);
            var currentUser = _context.User.FirstOrDefault(user => user.FirebaseUID == token);
            if (userToBlock == null)
                return StatusCode(204);
            try
            {
                currentUser.BlockedUsers.Concat(" " + userToBlock.UserId + " ");
                _context.SaveChanges();
                return StatusCode(200);
            }
            catch (Exception e)
            {
                return StatusCode(204, e.Message);
            }
            return StatusCode(204);
        }

        // PUT: api/Users/unblockUser
        [HttpPost("unblockUser")]
        [Authorize]
        public ActionResult unblockUser(
            [FromHeader(Name = "Authorization")] String token,
            [FromBody] string username)
        {
            User userToUnblock = _context.User.FirstOrDefault(user => user.UserName == username);
            var currentUser = _context.User.FirstOrDefault(user => user.FirebaseUID == token);
            if (userToUnblock == null)
                return StatusCode(204);
            try
            {
                currentUser.BlockedUsers.Replace(" " + userToUnblock.UserId + " ", null);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                return StatusCode(204, e.Message);
            }
            return StatusCode(200);
        }
        // GET: api/Users/BlockedUsers
        [HttpGet("BlockedUsers")]
        [Authorize]
        public ActionResult<IEnumerable<String>> GetBlockedUsers(
            [FromHeader(Name = "Authorization")] String token)
        {
            var uid = new SqlParameter("@UID", Utilities.pullUID(token));
            var user = _context.User.FromSqlRaw("SELECT * FROM dbo.[User] u where FirebaseUID = @UID",uid).First();
            var userID = new SqlParameter("@UserID", user.UserId);
            return new List<String>( _context.User.FromSqlRaw("SELECT u.UserName FROM [User] u WHERE u.UserID in (SELECT BlockedIDs FROM BlockedUsers bu WHERE UserID = @UserID)",userID).Select(x=>x.UserName));

        }

    }
}
