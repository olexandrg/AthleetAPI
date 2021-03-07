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

        
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateExercise(
            [FromQuery(Name = "Name")] String Name,
            [FromQuery(Name = "Description")] String Description,
            [FromQuery(Name = "DefaultReps")] int DefaultReps,
            [FromQuery(Name = "exerciseSets")] int ExerciseSets,
            [FromQuery(Name = "MeasureUnits")] String MeasureUnits,
            [FromQuery(Name = "unitCount")] decimal UnitCount,
            [FromQuery(Name = "WorkoutID")] int WorkoutID,
            [FromHeader(Name = "Authorization")] String token)
        {
            String UID = Utilities.pullUID(token);

            var name = new SqlParameter("@Name", Name);
            var description = new SqlParameter("@Description", Description);
            var defaultReps = new SqlParameter("@DefaultReps", DefaultReps);
            var sets = new SqlParameter("@ExerciseSets", ExerciseSets);
            var unitType = new SqlParameter("@MeasureUnits", MeasureUnits);
            var unitCount = new SqlParameter("@UnitCount", UnitCount);
            var workoutId = new SqlParameter("@WorkoutID", WorkoutID);
            var uid = new SqlParameter("@UID", UID);
            await _context.Database.ExecuteSqlRawAsync("EXEC procInsertExercise @UID, @Name, @Description, @DefaultReps, @exerciseSets, @MeasureUnits, @unitCount, @WorkoutID", uid, name, description, defaultReps, sets, unitType, unitCount, workoutId);
            return StatusCode(201);
        }

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


        // GET: api/Exercises/workout/{workoutId}
        [HttpGet("workout/{woID}")]
        [Authorize]
        public ActionResult<List<Exercises>> GetWorkoutExercises(int woID)
        {
            // this could be refactored into some joins probably

            // Gets the workout wanted. There is a better way but I am using this for now because it works.
            var workout = _context.Workouts.Where(w => w.WorkoutId == woID).FirstOrDefault();
            var woExercises = _context.WorkoutExercises.Where(we => we.WorkoutId == workout.WorkoutId).ToList();

            if (woExercises == null)
            {
                return NotFound();
            }

            var exercises = new List<Exercises>();

            foreach(var e in woExercises)
            {
                exercises.Add(_context.Exercises.Where(ex => ex.ExerciseId == e.ExerciseId).First());
            }

            if (exercises == null)
            {
                return NotFound();
            }

            return exercises;
        }

    }
}
