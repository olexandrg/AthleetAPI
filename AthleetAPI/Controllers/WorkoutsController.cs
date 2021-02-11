using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        //GET: api/Workouts
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Workout>>> ListWorkouts([FromHeader(Name = "Authorization")] String token)
        {
            //pull the UID from the token
            String UID = Utilities.pullUID(token);
            var uid = new SqlParameter("@UID", UID);

            List<Workout> workouts = _context.Workouts.FromSqlRaw("SELECT * FROM fnViewUserWorkouts(@UID)", uid).ToList();
            if (workouts == null)
            {
                return StatusCode(404);
            }

            return workouts;
        }

        // POST: api/Workouts
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateWorkout(
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
    }
}
