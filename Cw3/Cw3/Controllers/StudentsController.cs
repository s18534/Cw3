using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cw3.Models;
using Cw3.DAL;
using System.Data.SqlClient;

namespace Cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;

        [HttpGet]
        public IActionResult GetStudents()
        {

            List<Student> students = new List<Student>();

            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18534;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = "SELECT *  FROM Student" +
                    " INNER JOIN Enrollment ON Student.IdEnrollment=Enrollment.IdEnrollment" +
                    "  INNER JOIN Studies ON Enrollment.IdStudy=Studies.IdStudy";

                client.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student()
                    {
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        IndexNumber = dr["IndexNumber"].ToString(),
                        DateOfBirth = DateTime.Parse(dr["BirthDate"].ToString()).ToShortDateString(),
                        Studies = dr["Name"].ToString(),
                        Semestr = int.Parse(dr["Semester"].ToString())
                    };
                    students.Add(st);
                }
            }
            return Ok(students);
        }
        [HttpGet("{indexNum}")]
        public IActionResult GetStudent(string indexNum)
        {
            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18534;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = "SELECT *  FROM Student" +
                    " INNER JOIN Enrollment ON Student.IdEnrollment=Enrollment.IdEnrollment" +
                    "  INNER JOIN Studies ON Enrollment.IdStudy=Studies.IdStudy WHERE IndexNumber=@index";

                com.Parameters.AddWithValue("index", indexNum);
                Console.WriteLine("SIUR");
                client.Open();
                var dr = com.ExecuteReader();
                if (dr.Read())
                {
                    var st = new Student()
                    {
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        IndexNumber = dr["IndexNumber"].ToString(),
                        DateOfBirth = DateTime.Parse(dr["BirthDate"].ToString()).ToShortDateString(),
                        Studies = dr["Name"].ToString(),
                        Semestr = int.Parse(dr["Semester"].ToString())
                    };
                    return Ok(st);
                }
            }
            return NotFound();
        }


        [HttpPost]
        public IActionResult CreateStudent([FromBody]Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult PutStudent(int id)
        {
            return Ok("Aktualizacja dokończona");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            return Ok("Usuwanie ukończone");
        }

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        public IActionResult GetStudents(string orderBy)
        {
            return Ok(_dbService.GetStudents());
        }

    }
}