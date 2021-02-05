﻿using System;
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
    public class UsersController : ControllerBase
    {
        private readonly AthleetContext _context;

        public UsersController(AthleetContext context)
        {
            _context = context;
        }

        // POST: api/Users
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateUser(
            [FromHeader(Name = "Authorization")] String token, 
            [FromQuery(Name = "userName")] String userName, 
            [FromQuery(Name = "description")] String Description)
        {
            String UID = Utilities.pullUID(token);
            var uid = new SqlParameter("@UID", UID);
            var name = new SqlParameter("@Name", userName);
            var description = new SqlParameter("@Headline", Description);
            var result = await _context.Database.ExecuteSqlRawAsync("EXEC procInsertNewUser @UID, @Name, @Headline", name, uid, description);
            if (result > 0)
                return StatusCode(201);     //This means the user is truly new and a new db entry was added
            else
                return StatusCode(200);     //This means the user is a returning user and the db has not been modified.
        }
        //[HttpGet]
        //[Authorize]
        //private async Task<ActionResult> CheckExistingUser([FromHeader(Name = "Authorization")] String token)
        //{
        //    String UID = Utilities.pullUID(token);
        //    var user =  await _context.User.FindAsync(UID);
        //    if (user == null)
        //        return StatusCode(404);
        //    else
        //        return StatusCode(200);
        //}
    }
}
