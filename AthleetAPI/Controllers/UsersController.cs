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
            var entity = _context.User.FirstOrDefault(u => u.UserId == id);

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
            [FromQuery(Name = "UserName")] String username)
        {
            try
            {
                String uid = Utilities.pullUID(token);

                // validate user to block
                var userToBlockModel = _context.User.Where(x => x.UserName == username).FirstOrDefault();
                if (userToBlockModel == null) return StatusCode(404);

                // validate current user
                var currentUserModel = _context.User.Where(x => x.FirebaseUID == uid).FirstOrDefault();
                if (currentUserModel == null) return StatusCode(404);
                bool BlockedUserEntryExists = _context.BlockedUsers.Any(x => x.UserID == currentUserModel.UserId && x.BlockedID == userToBlockModel.UserId);
                if (!BlockedUserEntryExists)
                {
                    var blockedUserToInsert = new BlockedUser()
                    {
                        UserID = currentUserModel.UserId,
                        BlockedID = userToBlockModel.UserId
                    };

                    _context.BlockedUsers.Add(blockedUserToInsert);
                    _context.SaveChanges();
                    return StatusCode(201);
                }
                return StatusCode(200);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        // PUT: api/Users/unblockUser
        [HttpPost("unblockUser")]
        [Authorize]
        public ActionResult unblockUser(
            [FromHeader(Name = "Authorization")] String token,
            [FromQuery(Name ="UserName")] String username)
        {
            try
            {
                String uid = Utilities.pullUID(token);

                // validate user to block
                var userToUnblockModel = _context.User.Where(x => x.UserName == username).FirstOrDefault();
                if (userToUnblockModel == null) return StatusCode(404);

                // validate current user
                var currentUserModel = _context.User.Where(x => x.FirebaseUID == uid).FirstOrDefault();
                if (currentUserModel == null) return StatusCode(404);

                BlockedUser blockedUserToRemove = _context.BlockedUsers.Where(x => x.UserID == currentUserModel.UserId && x.BlockedID == userToUnblockModel.UserId).FirstOrDefault();

                _context.BlockedUsers.Remove(blockedUserToRemove);
                _context.SaveChanges();

                return StatusCode(201);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            /*            String uid = Utilities.pullUID(token);
                        var userNameToUnblock = new SqlParameter("@UserNameToUnblock", username);
                        var currentUserFBUid = new SqlParameter("@CurrentUserFBUID", uid);
                        var userToUnblock = _context.User.FromSqlRaw("Select * from dbo.[User] u where u.UserName = @UserNameToUnblock", userNameToUnblock).First();
                        var currentUser = _context.User.FromSqlRaw("Select * from dbo.[User] u where u.FirebaseUID = @CurrentUserFBUID", currentUserFBUid).First();

                        if (userToUnblock == null) return StatusCode(204);
                        try
                        {
                            var userID = new SqlParameter("@UserID", currentUser.UserId);
                            var unblockID = new SqlParameter("@UnblockedID", userToUnblock.UserId);
                            var result = _context.BlockedUsers.FromSqlRaw("DELETE FROM [dbo].[BlockedUsers] WHERE BlockedUsers.UserID = @UserID and BlockedUsers.BlockedIDs = @UnblockedID", userID, unblockID);
                            return StatusCode(200, result.First());
                        }
                        catch (Exception e) { return StatusCode(204, e.Message); }*/
        }
        // GET: api/Users/BlockedUsers
        [HttpGet("BlockedUsers")]
        [Authorize]
        public ActionResult<IEnumerable<String>> GetBlockedUsers(
            [FromHeader(Name = "Authorization")] String token)
        {
            try
            {
                var uid = Utilities.pullUID(token);
                var user = _context.User.Where(x => x.FirebaseUID == uid).FirstOrDefault();
                var blockedUsers = _context.BlockedUsers.Where(x => x.UserID == user.UserId).ToList();
                List<String> blockedUsernames = new List<String>();
                foreach(BlockedUser b in blockedUsers)
                {
                    var u = _context.User.Where(x => x.UserId == b.BlockedID).FirstOrDefault();
                    blockedUsernames.Add(u.UserName);
                }
                return blockedUsernames;

            }
            catch(Exception e) { return StatusCode(500, e.Message); }
        }

    }
}
