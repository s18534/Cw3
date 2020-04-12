using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cw3.Models;
using Cw3.DTOs.Requests;
using Cw3.DTOs.Responses;
using Cw3.Services;
using System.Data.SqlClient;

namespace Cw3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentDbService _service;

        public EnrollmentsController(IStudentDbService service)
        {
            _service = service;
        }


        

        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {

        }

        [HttpPost("promotions")]
        public IActionResult PromoteStudent(PromoteStudentRequest request)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                connection.Open();

                command.CommandText = "exec PromoteStudents @stud,@sem";
                command.Parameters.AddWithValue("stud", request.Studies);
                command.Parameters.AddWithValue("sem", request.Semester);

                var read = command.ExecuteReader();
                if (read.Read())
                {
                    EnrollStudentResponse response = new EnrollStudentResponse
                    {
                        IdEnroll = (int)read["IdEnrollment"],
                        Semester = (int)read["Semester"],
                        Study = (int)read["IdStudy"],
                        Date = DateTime.Parse(read["StartDate"].ToString())
                    };
                    return Ok(response);
                }
            }
            return NotFound();
        }

    }
}