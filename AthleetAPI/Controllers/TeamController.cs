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
    public class TeamController : ControllerBase
    {
        private readonly AthleetContext _context;

        public TeamController(AthleetContext context)
        {
            _context = context;
        }

        // DELETE: api/Team
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> DeleteTeam([FromHeader(Name = "Authorization")] String token, [FromQuery(Name = "teamName")] String teamName)
        {
            String UID = Utilities.pullUID(token);

            var uid = new SqlParameter("@UID", UID);
            var name = new SqlParameter("@Name", teamName);

            await _context.Database.ExecuteSqlRawAsync("EXEC procDeleteTeam @UID, @Name", name, uid);

            return StatusCode(200);
        }

        private bool TeamExists(int id)
        {
            return _context.Team.Any(e => e.TeamID == id);
        }
    }
}