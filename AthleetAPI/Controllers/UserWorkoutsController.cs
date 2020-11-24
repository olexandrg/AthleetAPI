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
    public class UserWorkoutsController : ControllerBase
    {
        private readonly AthleetContext _context;

        public UserWorkoutsController(AthleetContext context)
        {
            _context = context;
        }

        // GET: api/UserWorkouts
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserWorkouts>>> GetUserWorkouts()
        {
            return await _context.UserWorkouts.ToListAsync();
        }

        // GET: api/UserWorkouts/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserWorkouts>> GetUserWorkouts(int id)
        {
            var userWorkouts = await _context.UserWorkouts.FindAsync(id);

            if (userWorkouts == null)
            {
                return NotFound();
            }

            return userWorkouts;
        }

        // PUT: api/UserWorkouts/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUserWorkouts(int id, UserWorkouts userWorkouts)
        {
            if (id != userWorkouts.UserWorkoutId)
            {
                return BadRequest();
            }

            _context.Entry(userWorkouts).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserWorkoutsExists(id))
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

        // POST: api/UserWorkouts
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UserWorkouts>> PostUserWorkouts(UserWorkouts userWorkouts)
        {
            _context.UserWorkouts.Add(userWorkouts);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserWorkouts", new { id = userWorkouts.UserWorkoutId }, userWorkouts);
        }

        // DELETE: api/UserWorkouts/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<UserWorkouts>> DeleteUserWorkouts(int id)
        {
            var userWorkouts = await _context.UserWorkouts.FindAsync(id);
            if (userWorkouts == null)
            {
                return NotFound();
            }

            _context.UserWorkouts.Remove(userWorkouts);
            await _context.SaveChangesAsync();

            return userWorkouts;
        }

        private bool UserWorkoutsExists(int id)
        {
            return _context.UserWorkouts.Any(e => e.UserWorkoutId == id);
        }
    }
}
