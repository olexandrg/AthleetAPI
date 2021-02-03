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
    public class WorkoutsController : ControllerBase
    {
        private readonly AthleetContext _context;

        public WorkoutsController(AthleetContext context)
        {
            _context = context;
        }

        // GET: api/Workouts
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Workouts>>> GetWorkouts()
        {
            return await _context.Workouts.ToListAsync();
        }

        // GET: api/Workouts/InsertWorkout
        [HttpGet("InsertWorkout")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Workouts>>> CreateWorkout(
            [FromQuery(Name = "Name")] String Name,
            [FromQuery(Name = "Description")] String Description,
            [FromHeader(Name = "Authorization")] String token)
        {
            //pull the UID from the token
            String UID = Utilities.pullUID(token);

            //generate the sql parameters
            var name = new SqlParameter("@Name", Name);
            var description = new SqlParameter("@Description", Description);
            var uid = new SqlParameter("@UID", UID);
            
            //run the query
            await _context.Database.ExecuteSqlRawAsync("EXEC procInsertWorkout @UID, @Name, @Description", name, description, uid);

            //return success
            return StatusCode(201);
        }

        // GET: api/Workouts
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Workouts>>> DeleteWorkout(
            [FromQuery(Name = "Name")] String Name,
            [FromHeader(Name = "Authorization")] String token)
        {
            //pull the UID from the token
            String UID = Utilities.pullUID(token);

            //generate the sql parameters
            var name = new SqlParameter("@Name", Name);
            var uid = new SqlParameter("@UID", UID);

            //run the query
            await _context.Database.ExecuteSqlRawAsync("EXEC procedureDeleteWorkoutForUser @UID, @Name", uid, name);

            //return success
            return StatusCode(201);
        }

        // GET: api/Workouts/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Workouts>> GetWorkouts(int id)
        {
            var workouts = await _context.Workouts.FindAsync(id);

            if (workouts == null)
            {
                return NotFound();
            }

            return workouts;
        }

        // POST: api/Workouts
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Workouts>> PostWorkouts(Workouts workouts)
        {
            _context.Workouts.Add(workouts);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWorkouts", new { id = workouts.WorkoutId }, workouts);
        }

    
    }
}
