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
    public class ViewExerciseInWorkoutController : ControllerBase
    {
        private readonly AthleetContext _context;

        public ViewExerciseInWorkoutController(AthleetContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ViewExerciseInWorkout>>> ViewWorkout([FromHeader(Name = "Authorization")] string token,
            [FromQuery(Name = "WorkoutName")] string WorkoutName)
        {
            string UID = Utilities.pullUID(token);
            var uid = new SqlParameter("@UID", UID);
            var workoutName = new SqlParameter("@WorkoutName", WorkoutName);
            return _context.ViewExerciseInWorkout.FromSqlRaw("select * from fnViewExercisesWithinWorkoutForUser(@UID, @WorkoutName)", uid, workoutName).ToList();
        }
    }
}