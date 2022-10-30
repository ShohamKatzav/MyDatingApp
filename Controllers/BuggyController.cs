using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using API.Data;
using Microsoft.AspNetCore.Authorization;
using API.Entities;

namespace API.Controllers
{

    public class BuggyController : BaseApiController
    {
        private readonly DataContext _context;

        public BuggyController(DataContext context)
        {
            this._context = context;
        }


        // we'll create methods to return error from different types

        // return 401 - no token sent
        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret(){
            return "secret text";
        }

        // return not found error (there's no user -1)
        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound(){
            var thing = _context.Users.Find(-1);
            if(thing == null) return NotFound();
            return Ok(thing);
        }

        // return server error
        [HttpGet("server-error")]
        public ActionResult<string> GetServerError(){
            var thing = _context.Users.Find(-1);
            var thingToReturn = thing.ToString(); // NullReferenceException
            return thingToReturn;
        }

        // return bad request error
        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest(){
            return BadRequest("Your request wasn't good enough for us");
        }

    }
}