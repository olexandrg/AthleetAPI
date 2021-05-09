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
                [FromQuery(Name = "conversationID")] int conversationID,
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