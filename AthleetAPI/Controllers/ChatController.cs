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
        [HttpGet("user/list")]
        [Authorize]

        public async Task<ActionResult<List<ConvList>>> ViewUserConversations(
            [FromHeader(Name = "Authorization")] String token
         )
        {
            String UID = Utilities.pullUID(token);

            try
            {
                User user = await _context.User.FirstOrDefaultAsync(x => x.FirebaseUID == UID);
                if (user == null) return StatusCode(404, "User not found");

                var convs = await _context.UserConversations.Join(_context.Conversations, uc => uc.ConversationID, c => c.ConversationID, (uc, c) => new
                {
                    ConversationID = c.ConversationID,
                    UserID = uc.UserID
                }).Where(x => x.UserID == user.UserId).Select(x => x.ConversationID).ToListAsync();
                if (convs == null) return StatusCode(404, "User has no conversations");

                var conversations = _context.UserConversations.Where(x => convs.Contains(x.ConversationID) && x.UserID != user.UserId).Join(_context.User, uc => uc.UserID, u => u.UserId, (uc, u) => new
                {
                    ConversationID = uc.ConversationID,
                    UserName = u.UserName
                }).Select(x => new { x.UserName, x.ConversationID}).ToList();

                List<ConvList> convList = new List<ConvList>();

                for (int i = 0; i < conversations.Count; i++)
                {
                    convList.Add(new ConvList{ ConversationID = conversations[i].ConversationID, UserName = conversations[i].UserName});
                }

                return convList;
            }
            catch (Exception ex) { return NotFound(ex.Message); }
        }

        [HttpGet("team")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Messages>>> GetTeamConversation(
                [FromQuery(Name = "teamName")] String teamName
        )
        {
            var TeamName = new SqlParameter("@TeamName", teamName);
            try
            {
                var result = await _context.Messages.FromSqlRaw("SELECT * FROM fnViewTeamConv(@TeamName)", TeamName).ToListAsync();
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