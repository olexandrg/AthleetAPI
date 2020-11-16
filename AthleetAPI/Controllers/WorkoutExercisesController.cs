using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AthleetAPI.Models;

namespace AthleetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkoutExercisesController : ControllerBase
    {
        private readonly AthleetContext _context;

        public WorkoutExercisesController(AthleetContext context)
        {
            _context = context;
        }

        // GET: api/WorkoutExercises
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkoutExercises>>> GetWorkoutExercise()
        {
            return await _context.WorkoutExercises.ToListAsync();
        }

        // GET: api/WorkoutExercises/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkoutExercises>> GetWorkoutExercises(int id)
        {
            var workoutExercises = await _context.WorkoutExercises.FindAsync(id);

            if (workoutExercises == null)
            {
                return NotFound();
            }

            return workoutExercises;
        }

        // PUT: api/WorkoutExercises/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorkoutExercises(int id, WorkoutExercises workoutExercises)
        {
            if (id != workoutExercises.WorkoutExerciseId)
            {
                return BadRequest();
            }

            _context.Entry(workoutExercises).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkoutExercisesExists(id))
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

        // POST: api/WorkoutExercises
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<WorkoutExercises>> PostWorkoutExercises(WorkoutExercises workoutExercises)
        {
            _context.WorkoutExercises.Add(workoutExercises);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWorkoutExercises", new { id = workoutExercises.WorkoutExerciseId }, workoutExercises);
        }

        // DELETE: api/WorkoutExercises/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<WorkoutExercises>> DeleteWorkoutExercises(int id)
        {
            var workoutExercises = await _context.WorkoutExercises.FindAsync(id);
            if (workoutExercises == null)
            {
                return NotFound();
            }

            _context.WorkoutExercises.Remove(workoutExercises);
            await _context.SaveChangesAsync();

            return workoutExercises;
        }

        private bool WorkoutExercisesExists(int id)
        {
            return _context.WorkoutExercises.Any(e => e.WorkoutExerciseId == id);
        }
    }
}
