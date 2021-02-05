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
    public class ExercisesController : ControllerBase
    {
        private readonly AthleetContext _context;

        public ExercisesController(AthleetContext context)
        {
            _context = context;
        }

        // GET: api/Exercises
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Exercises>>> GetExercises()
        {
            return await _context.Exercises.ToListAsync();
        }

        // GET: api/Exercises/InsertExercise
        /*[HttpGet("InsertExercise")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Workouts>>> CreateExercise(
            [FromQuery(Name = "Name")] String Name,
            [FromQuery(Name = "Description")] String Description,
            [FromQuery(Name = "DefaultReps")] int DefaultReps,
            [FromQuery(Name = "WorkoutName")] String WorkoutName,
            [FromHeader(Name = "Authorization")] String token)
        {
            String UID = Utilities.pullUID(token);

            var name = new SqlParameter("@Name", Name);
            var description = new SqlParameter("@Description", Description);
            var defaultReps = new SqlParameter("@DefaultReps", DefaultReps);
            var workoutName = new SqlParameter("@WorkoutName", WorkoutName);
            var uid = new SqlParameter("@UID", UID);
            await _context.Database.ExecuteSqlRawAsync("EXEC procInsertExercise @UID, @Name, @Description, @DefaultReps, @WorkoutName", uid, name, description, defaultReps, workoutName);
            return StatusCode(201);
        }*/

        // GET: api/Exercises/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Exercises>> GetExercises(int id)
        {
            var exercises = await _context.Exercises.FindAsync(id);

            if (exercises == null)
            {
                return NotFound();
            }

            return exercises;
        }

        // PUT: api/Exercises/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutExercises(int id, Exercises exercises)
        {
            if (id != exercises.ExerciseId)
            {
                return BadRequest();
            }

            _context.Entry(exercises).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExercisesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Exercises
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Exercises>> PostExercises(Exercises exercises)
        {
            _context.Exercises.Add(exercises);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExercises", new { id = exercises.ExerciseId }, exercises);
        }

        // DELETE: api/Exercises/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<Exercises>> DeleteExercises(int id)
        {
            var exercises = await _context.Exercises.FindAsync(id);
            if (exercises == null)
            {
                return NotFound();
            }

            _context.Exercises.Remove(exercises);
            await _context.SaveChangesAsync();

            return exercises;
        }

        private bool ExercisesExists(int id)
        {
            return _context.Exercises.Any(e => e.ExerciseId == id);
        }
    }
}
