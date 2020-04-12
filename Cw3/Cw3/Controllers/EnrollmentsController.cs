using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cw3.Models;
using Cw3.DTOs.Requests;

namespace Cw3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            if (request.FirstName == null || request.LastName == null || request.IndexNumber == null || request.DateOfBirth == null || request.Studies == null)
                return BadRequest("Bad request");

            return Ok();
        }
    }
}