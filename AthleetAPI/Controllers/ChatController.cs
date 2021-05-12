using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AthleetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AthleetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly AthleetContext _context;

        public ChatController(AthleetContext context)
        {
            _context = context;
        }
        [HttpGet("viewUserConversations")]
        [Authorize]

        public async Task<ActionResult<List<Conversation>>> ViewUserConversations(
            [FromHeader(Name = "Authorization")] String token
         )
        {
            String UID = Utilities.pullUID(token);

            try
            {
                User user = await _context.User.FirstOrDefaultAsync(x => x.FirebaseUID == UID);
                if (user == null) return StatusCode(404, "User not found");

                var conversations = await _context.Conversation.Where(x => x.UserName == user.UserName).ToListAsync();
                if (conversations == null) return StatusCode(404, "User has no conversations");

                return conversations;
            }
            catch (Exception ex) { return NotFound(ex.Message); }
        }

        [HttpGet("team")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Conversation>>> GetTeamConversation(
                [FromQuery(Name = "teamName")] String teamName
        )
        {
            var TeamName = new SqlParameter("@TeamName", teamName);
            try
            {
                var result = await _context.Conversation.FromSqlRaw("SELECT * FROM fnViewTeamConv(@TeamName)", TeamName).ToListAsync();
                if (result == null)
                {
                    return NotFound();
                }
                return result;
            }
            catch (Exception ex) { return NotFound(ex.Message); }
        }

        [HttpPost("team")]
        [Authorize]
        public async Task<ActionResult> SaveTeamConversation(
                [FromHeader(Name = "Authorization")] String token,
                [FromQuery(Name = "conversationID")] String conversationID,
                [FromQuery(Name = "content")] String content
        )
        {
            String UID = Utilities.pullUID(token);

            var uid = new SqlParameter("@UID", UID);
            var ConvID = new SqlParameter("@ConvID", conversationID);
            var Content = new SqlParameter("@Content", content);

            await _context.Database.ExecuteSqlRawAsync("EXEC procInsertNewTeamMessage @UID, @ConvID, @Content", ConvID, uid, Content);

            return StatusCode(201);           
        }
    }
}