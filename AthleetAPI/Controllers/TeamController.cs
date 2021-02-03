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

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<Team>> GetTeam([FromQuery(Name = "teamName")] String teamName)
        {
            var TeamName = new SqlParameter("@TeamName", teamName);
            List<string> temp = new List<string>();

            Team team = new Team();
            team.TeamName = teamName;
            TeamUserNames[] users = _context.TeamUserNames.FromSqlRaw("SELECT * FROM fnViewTeamUsers(@TeamName)", TeamName).ToArray();
            foreach (TeamUserNames user in users)
            {
                temp.Add(user.UserName);
            }
            team.userNames = temp.ToArray();
            temp.Clear();

            TeamWorkoutNames[] workouts = _context.TeamWorkoutNames.FromSqlRaw("SELECT WorkoutName FROM fnViewTeamWorkouts(@TeamName)", TeamName).ToArray();
            foreach (TeamWorkoutNames workout in workouts)
            {
                temp.Add(workout.WorkoutName);
            }
            team.workoutNames = temp.ToArray();

            return team;
        }

        //GET: api/Team/list
        [HttpGet("list")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TeamListItem>>> GetTeamList([FromHeader(Name = "Authorization")] String token)
        {
            String UID = Utilities.pullUID(token);

            var uid = new SqlParameter("@UID", UID);

            return _context.TeamListItem.FromSqlRaw("SELECT * FROM fnViewUserTeams(@UID)", uid).ToList();
        }

        //GET: api/Team/leave
        [HttpGet("leave")]
        [Authorize]
        public async Task<ActionResult> LeaveTeam([FromHeader(Name = "Authorization")] String token, [FromQuery(Name = "teamName")] String teamName)
        {
            String UID = Utilities.pullUID(token);

            var uid = new SqlParameter("@UID", UID);
            var TeamName = new SqlParameter("@Name", teamName);

            await _context.Database.ExecuteSqlRawAsync("EXEC procLeaveTeam @UID, @Name", uid, TeamName);

            return StatusCode(200);
        }

        //POST: api/Team
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateTeam([FromHeader(Name = "Authorization")] String token, [FromQuery(Name = "teamName")] String teamName, [FromQuery(Name = "description")] String description)
        {
             String UID = Utilities.pullUID(token);

            var uid = new SqlParameter("@UID", UID);
            var name = new SqlParameter("@Name", teamName);
            var Description = new SqlParameter("@Description", description);

            await _context.Database.ExecuteSqlRawAsync("EXEC procInsertNewTeam @UID, @Name, @Description", name, uid, Description);

            return StatusCode(201);           
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
    }
}