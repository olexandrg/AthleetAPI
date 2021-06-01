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
        [HttpPost("user")]
        [Authorize]
        public async Task<ActionResult<int>> CreateUserConv(
            [FromHeader(Name = "Authorization")] String token,
            [FromQuery(Name = "OtherUser")] String userName
        )
        {
            String UID = Utilities.pullUID(token);

            try
            {
                User u1 = _context.User.FirstOrDefault(x => x.FirebaseUID == UID);
                if (u1 == null) return StatusCode(404, "User not found");

                User u2 = _context.User.FirstOrDefault(x => x.UserName == userName);
                if (u2 == null) return StatusCode(404, "Other User not found");

                var u1Conv = _context.UserConversations.Where(x => x.UserID == u1.UserId).Select(x => x.ConversationID).ToList();
                var u2Conv = _context.UserConversations.Where(x => x.UserID == u2.UserId).Select(x => x.ConversationID).ToList();

                var res = u2Conv.Where(x => u1Conv.Contains(x));
                if (res.Count() > 0) return StatusCode(400, "Conversation already exists");

                Conversations conv = new Conversations();
                conv.ConversationDate = DateTime.Now;
                _context.Conversations.Add(conv);
                _context.SaveChanges();

                _context.UserConversations.Add(new UserConversations { ConversationID = conv.ConversationID, UserID = u1.UserId });
                _context.UserConversations.Add(new UserConversations { ConversationID = conv.ConversationID, UserID = u2.UserId });

                _context.SaveChanges();

                return conv.ConversationID;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("user/list")]
        [Authorize]

        public ActionResult<List<ConvList>> ViewUserConversations(
            [FromHeader(Name = "Authorization")] String token
         )
        {
            String UID = Utilities.pullUID(token);

            try
            {
                User user =  _context.User.FirstOrDefault(x => x.FirebaseUID == UID);
                if (user == null) return StatusCode(404, "User not found");

                var convs = _context.UserConversations.Join(_context.Conversations, uc => uc.ConversationID, c => c.ConversationID, (uc, c) => new
                {
                    ConversationID = c.ConversationID,
                    UserID = uc.UserID
                }).Where(x => x.UserID == user.UserId).Select(x => x.ConversationID).ToList();
                if (convs == null) return StatusCode(404, "User has no conversations");

                var conversations 
                    = _context.UserConversations
                    .Where(x => convs
                        .Contains(x.ConversationID) && x.UserID != user.UserId)
                    .Join(_context.User, uc => uc.UserID, u => u.UserId, (uc, u) 
                        => new
                            {
                                ConversationID = uc.ConversationID,
                                UserName = u.UserName
                            })
                    .Select(x => new { x.UserName, x.ConversationID})
                    .ToList();

                List<ConvList> convList = new List<ConvList>();

                for (int i = 0; i < conversations.Count; i++)
                {
                    convList.Add(new ConvList{ ConversationID = conversations[i].ConversationID, UserName = conversations[i].UserName});
                }

                return convList;
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }


        [HttpGet("user")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Messages>>> GetUserConversation(
                [FromQuery(Name = "convID")] int convID
        )
        {
            var ConvID = new SqlParameter("@ConversationID", convID);
            try
            {
                var result = await _context.Messages.FromSqlRaw("SELECT * FROM fnViewUserMessages(@ConversationID)", ConvID).ToListAsync();
                if (result == null)
                {
                    return NotFound();
                }
                return result;
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

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> SaveMessage(
                [FromHeader(Name = "Authorization")] String token,
                [FromQuery(Name = "conversationID")] int conversationID,
                [FromQuery(Name = "content")] String content
        )
        {
            String UID = Utilities.pullUID(token);

            var uid = new SqlParameter("@UID", UID);
            var ConvID = new SqlParameter("@ConvID", conversationID);
            var Content = new SqlParameter("@Content", content);

            //The below only takes the convid so it works for both user and team messages
            await _context.Database.ExecuteSqlRawAsync("EXEC procInsertNewTeamMessage @UID, @ConvID, @Content", ConvID, uid, Content);

            return StatusCode(201);           
        }     

        [HttpDelete("delete")]
        [Authorize]
        public async Task<ActionResult> DeleteMessage([FromHeader(Name = "Authorization")] String token, [FromQuery(Name = "messageID")] int messageID)
        {
            String UID = Utilities.pullUID(token);

            var message = new Messages { MessageID = messageID };
            _context.Messages.Attach(message);
            _context.Messages.Remove(message);
            _context.SaveChanges();

            return StatusCode(204);
        }
    }
}