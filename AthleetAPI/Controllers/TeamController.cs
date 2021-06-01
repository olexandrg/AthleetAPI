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
        public async Task<ActionResult<TeamModel>> GetTeam([FromQuery(Name = "teamName")] String teamName)
        {
            try
            {
                var TeamName = new SqlParameter("@TeamName", teamName);

                // construct new team and assign name
                TeamModel team = new TeamModel();
                team.TeamName = teamName;

                // get current team users
                IEnumerable<TeamUser> users = await _context.TeamUser.FromSqlRaw("SELECT * FROM fnViewTeamUsers(@TeamName)", TeamName).ToListAsync();

                if (users == null)
                {
                    return StatusCode(404, "Invalid team; unable to fetch users list");
                }

                team.users = users;

                // get team workouts
                var workoutNames = await _context.TeamWorkoutNames.FromSqlRaw("SELECT WorkoutName FROM fnViewTeamWorkouts(@TeamName)", TeamName).ToListAsync();

                if (workoutNames == null)
                {
                    return StatusCode(404, "Invalid team; unable to fetch workouts");
                }

                // get all workout names
                TeamWorkoutNames[] workouts = workoutNames.ToArray();
                foreach (TeamWorkoutNames workout in workouts)
                {
                    team.workoutNames.Append(workout.WorkoutName);
                }

                return team;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        //GET: api/Team/list
        [HttpGet("list")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TeamListItem>>> GetTeamList([FromHeader(Name = "Authorization")] String token)
        {
            String UID = Utilities.pullUID(token);

            var uid = new SqlParameter("@UID", UID);

            return await _context.TeamListItem.FromSqlRaw("SELECT * FROM fnViewUserTeams(@UID)", uid).ToListAsync();
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

        // PUT: api/Team/{teamName}/{userName}?isAdmin=true/false
        [HttpPut("{teamName}/{userName}")]
        [Authorize]
        public async Task<ActionResult> UpdateUserAdmin([FromHeader(Name = "Authorization")] String token, String teamName, String userName, [FromQuery(Name = "isAdmin")] bool isAdmin)
        {
            String UID = Utilities.pullUID(token);

            var uid = new SqlParameter("@UID", UID);
            var tName = new SqlParameter("@TeamName", teamName);
            var uName = new SqlParameter("@UserName", userName);
            var admin = new SqlParameter("@IsAdmin", isAdmin);

            // calls procedure procUpdateUserAdmin
            await _context.Database.ExecuteSqlRawAsync("EXEC procUpdateUserAdmin @UID, @TeamName, @UserName, @IsAdmin", tName, uid, uName, admin);

            return StatusCode(200);
        }

        // PUT: api/Team/invite/{teamName}/{userName}
        [HttpPut("invite/{teamName}/{userName}")]
        [Authorize]
        public async Task<ActionResult> InviteUserToTeam([FromHeader(Name = "Authorization")] String token, String teamName, String userName)
        {
            String UID = Utilities.pullUID(token);

            var uid = new SqlParameter("@UID", UID);
            var tName = new SqlParameter("@TeamName", teamName);
            var uName = new SqlParameter("@UserName", userName);

            // calls procedure procInviteUserToTeam
            await _context.Database.ExecuteSqlRawAsync("EXEC procInviteUserToTeam @UID, @TeamName, @UserName", tName, uid, uName);

            return StatusCode(200);
        }

        // GET: api/Team/workouts/{id}
        [HttpGet("workouts")]
        [Authorize]
        public ActionResult<IEnumerable<Workout>> GetTeamWorkouts(
            [FromQuery(Name = "TeamID")] int teamID)
        {
            var teamId = new SqlParameter("@TeamID", teamID);
            var teamWorkouts = _context.TeamWorkouts.Where(w => w.TeamID == teamID).ToList();
            if (teamWorkouts == null)
            {
                return NotFound("Team has no workouts");
            }

            var workouts = new List<Workout>();

            foreach (var w in teamWorkouts)
            {
                workouts.Add(_context.Workouts.Where(wo => wo.WorkoutId == w.WorkoutID).First());
            }

            if (workouts == null)
            {
                return NotFound();
            }

            return workouts;
        }

        [HttpPost("workout")]
        [Authorize]
        public async Task<ActionResult> CreateTeamWorkout(
            [FromBody]TeamWorkoutModel teamWorkout
            )
        {

            //generate the sql parameters
            var name = new SqlParameter("@Name", teamWorkout.Name);
            var description = new SqlParameter("@Description", teamWorkout.Description);
            var teamName = new SqlParameter("@TeamName", teamWorkout.TeamName);

            //run the query
            await _context.Database.ExecuteSqlRawAsync("EXEC procInsertTeamWorkout @Name, @Description, @TeamName", name, description, teamName);

            //return success
            return StatusCode(201);
        }

        [HttpDelete("workout")]
        [Authorize]
        public async Task<ActionResult> DeleteTeamWorkout(
            [FromBody] DeleteTeamWorkoutModel teamWorkout,
            [FromHeader(Name = "Authorization")] String token
            )
        {

            //generate the sql parameters
            var uid = new SqlParameter("@FirebaseUID", token);
            var teamName = new SqlParameter("@TeamName", teamWorkout.TeamName);
            var workoutName = new SqlParameter("@WorkoutName", teamWorkout.WorkoutName);

            //run the query
            await _context.Database.ExecuteSqlRawAsync("EXEC procDeleteTeamWorkout @FirebaseUID, @TeamName, @WorkoutName", uid, teamName, workoutName);

            //return success
            return StatusCode(204);
        }

        [HttpGet("id")]
        [Authorize]
        public ActionResult<int> getTeamId(
            [FromQuery] string teamName
            )
        {
            try
            {
                var team  = _context.Team.Where(t => t.TeamName == teamName).FirstOrDefault();
                if (team == null)
                {
                    return StatusCode(404, "Invalid team");
                }

                return team.TeamID;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }  

        [HttpGet("warning")]
        [Authorize]
        public async Task<ActionResult<DateTime>> getWarning([FromHeader(Name = "Authorization")] String token)
        {
            var uid = Utilities.pullUID(token);

            int userId = _context.User.Where(x => x.FirebaseUID == uid).Select(x => x.UserId).First();
            DateTime dateAdded = _context.Notifications.Where(x => x.UserID == userId).Select(x => x.DateAdded).Last();

            return dateAdded;
        }

        [HttpPost("warn")]
        [Authorize]
        public async Task<ActionResult> warnUser([FromHeader(Name = "Authorization")] String token, [FromQuery(Name = "Date")] DateTime warningDate)
        {
            try
            {
                var user = await _context.User.FirstOrDefaultAsync(x => x.FirebaseUID == Utilities.pullUID(token));
                if (user == null)
                {
                    return StatusCode(404, "No user with this FirebaseUID found");
                }
                _context.Notifications.Add(new Notifications { UserID = user.UserId, DateAdded = warningDate });
                _context.SaveChanges();

                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

    }
}