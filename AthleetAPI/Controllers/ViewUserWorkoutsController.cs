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
    public class ViewUserWorkoutsController : ControllerBase
    {
        private readonly AthleetContext _context;

        public ViewUserWorkoutsController(AthleetContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ViewUserWorkouts>>> ViewWorkout([FromHeader(Name = "Authorization")] String token)
        {
            String UID = Utilities.pullUID(token);
            var uid = new SqlParameter("@UID", UID);
            return _context.ViewUserWorkouts.FromSqlRaw("select * from fnViewUserWorkouts(@UID)", uid).ToList();
        }
    }
}