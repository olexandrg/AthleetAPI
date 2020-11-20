using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AthleetAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace AthleetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserWorkoutExercisesController : ControllerBase
    {
        private readonly AthleetContext _context;

        public UserWorkoutExercisesController(AthleetContext context)
        {
            _context = context;
        }

        // GET: api/UserWorkoutExercises
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserWorkoutExercises>>> GetUserWorkoutExercises()
        {
            return await _context.UserWorkoutExercises.ToListAsync();
        }

        // GET: api/UserWorkoutExercises/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserWorkoutExercises>> GetUserWorkoutExercises(int id)
        {
            var userWorkoutExercises = await _context.UserWorkoutExercises.FindAsync(id);

            if (userWorkoutExercises == null)
            {
                return NotFound();
            }

            return userWorkoutExercises;
        }

        // PUT: api/UserWorkoutExercises/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUserWorkoutExercises(int id, UserWorkoutExercises userWorkoutExercises)
        {
            if (id != userWorkoutExercises.UserWorkoutExerciseId)
            {
                return BadRequest();
            }

            _context.Entry(userWorkoutExercises).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserWorkoutExercisesExists(id))
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

        // POST: api/UserWorkoutExercises
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UserWorkoutExercises>> PostUserWorkoutExercises(UserWorkoutExercises userWorkoutExercises)
        {
            _context.UserWorkoutExercises.Add(userWorkoutExercises);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserWorkoutExercises", new { id = userWorkoutExercises.UserWorkoutExerciseId }, userWorkoutExercises);
        }

        // DELETE: api/UserWorkoutExercises/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<UserWorkoutExercises>> DeleteUserWorkoutExercises(int id)
        {
            var userWorkoutExercises = await _context.UserWorkoutExercises.FindAsync(id);
            if (userWorkoutExercises == null)
            {
                return NotFound();
            }

            _context.UserWorkoutExercises.Remove(userWorkoutExercises);
            await _context.SaveChangesAsync();

            return userWorkoutExercises;
        }

        private bool UserWorkoutExercisesExists(int id)
        {
            return _context.UserWorkoutExercises.Any(e => e.UserWorkoutExerciseId == id);
        }
    }
}
