﻿using Cw3.DTOs.Requests;
using Cw3.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw3.Models;
using Cw3.Procedure;
using System.Security.Claims;

namespace Cw3.Services
{
    public class SqlServerDbService : IStudentDbService
    {

        string connectionString = "Data Source=db-mssql;Initial Catalog=s18534;Integrated Security=True";


        public IEnumerable<Student> GetStudents()
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
                        DateOfBirth = DateTime.Parse(dr["BirthDate"].ToString()),
                        Studies = dr["Name"].ToString(),
                        Semestr = int.Parse(dr["Semester"].ToString())
                    };
                    students.Add(st);
                }
            }
            return students;
        }

        public Student GetStudent(string indexNum)
        {
            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18534;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = "SELECT *  FROM Student" +
                    " INNER JOIN Enrollment ON Student.IdEnrollment=Enrollment.IdEnrollment" +
                    "  INNER JOIN Studies ON Enrollment.IdStudy=Studies.IdStudy WHERE IndexNumber=@index";

                com.Parameters.AddWithValue("index", indexNum);
                client.Open();
                var dr = com.ExecuteReader();
                if (dr.Read())
                {
                    var st = new Student()
                    {
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        IndexNumber = dr["IndexNumber"].ToString(),
                        DateOfBirth = DateTime.Parse(dr["BirthDate"].ToString()),
                        Studies = dr["Name"].ToString(),
                        Semestr = int.Parse(dr["Semester"].ToString())
                    };
                    return st;
                }
            }
            return null;
        }


        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {

                command.Connection = connection;

                connection.Open();

                command.CommandText = "SELECT IdStudy FROM studies WHERE name=@name";
                command.Parameters.AddWithValue("name", request.Studies);

                var read = command.ExecuteReader();
                if (!read.Read())
                    throw new Exception("No such studies");

                int idStudies = (int)read["IdStudy"];
                read.Close();

                command.CommandText = "SELECT * FROM Student WHERE IndexNumber=@index";
                command.Parameters.AddWithValue("index", request.IndexNumber);

                read = command.ExecuteReader();
                if (read.Read())
                    throw new Exception("IndexNumber not unique");

                read.Close();

                command.CommandText = "SELECT * FROM enrollment WHERE Semester=@semester AND IdStudy=@idStudy";
                command.Parameters.AddWithValue("idStudy", idStudies);
                command.Parameters.AddWithValue("semester", 1);
                int idEnroll = 1;
                DateTime dateTime = DateTime.Now;
                read = command.ExecuteReader();
                if (!read.Read())
                {
                    read.Close();

                    command.CommandText = "SELECT MAX(IdEnrollment) as maximus from enrollment";
                    read = command.ExecuteReader();
                    if (read.Read())
                        idEnroll = (int)read["maximus"] + 1;

                    read.Close();
                    Console.WriteLine(idEnroll);
                    command.CommandText = "INSERT INTO Enrollment(IdEnrollment,Semester,IdStudy,StartDate) VALUES(@id, @sem, @idstud, @date)";
                    command.Parameters.AddWithValue("@id", idEnroll);
                    command.Parameters.AddWithValue("@sem", 1);
                    command.Parameters.AddWithValue("@idstud", idStudies);
                    command.Parameters.AddWithValue("@date", dateTime);
                }
                else
                {
                    idEnroll = (int)read["IdEnrollment"];
                    dateTime = (DateTime)read["StartDate"];
                }
                read.Close();
                command.CommandText = "INSERT INTO Student(IndexNumber,FirstName,LastName,BirthDate,IdEnrollment) VALUES(@indx, @firna, @laname, @date, @idEnrol)";
                command.Parameters.AddWithValue("indx", request.IndexNumber);
                command.Parameters.AddWithValue("firna", request.FirstName);
                command.Parameters.AddWithValue("laname", request.LastName);
                command.Parameters.AddWithValue("date", request.DateOfBirth);
                command.Parameters.AddWithValue("idEnrol", idEnroll);
                command.ExecuteNonQuery();

                return new EnrollStudentResponse()
                {
                    IdEnroll = idEnroll,
                    Semester = 1,
                    Study = idStudies,
                    Date = dateTime
                };
            }
        }

        public EnrollStudentResponse PromoteStudent(PromoteStudentRequest request)
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
                    return response;
                }
            }
            throw new Exception("Exception");
        }

        public bool CheckPassword(LoginRequestDto request)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                connection.Open();

                command.CommandText = "SELECT Password,Salt FROM Student WHERE IndexNumber=@number";
                command.Parameters.AddWithValue("number", request.Login);

                using var read = command.ExecuteReader();

                if (read.Read())
                {
                    return PasswordSalt.Validate(request.Haslo, read["Salt"].ToString(), read["Password"].ToString());
                }
                return false;
            }
        }

        public Claim[] GetClaims(string IndexNumber)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                connection.Open();

                command.CommandText = "select IndexNumber,FirstName,LastName,Rola from S_Rola sr Join Uprawnienia u on sr.Rola_IdRola = u.IdRola join Student s on s.IndexNumber = sr.Student_IndexNumber where s.IndexNumber=@index";
                command.Parameters.AddWithValue("index", IndexNumber);

                var read = command.ExecuteReader();

                if (read.Read())
                {
                    var claimList = new List<Claim>();
                    claimList.Add(new Claim(ClaimTypes.NameIdentifier, read["IndexNumber"].ToString()));
                    claimList.Add(new Claim(ClaimTypes.Name, read["FirstName"].ToString() + " " + read["LastName"].ToString()));
                    claimList.Add(new Claim(ClaimTypes.Role, read["Role"].ToString()));

                    while (read.Read())
                    {
                        claimList.Add(new Claim(ClaimTypes.Role, read["Role"].ToString()));
                    }
                    return claimList.ToArray<Claim>();
                }
                else return null;
            }
        }

        public void SetRefreshToken(string refreshToken, string IndexNumber)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                connection.Open();

                command.CommandText = "update student set token = @token where IndexNumber = @IndexNumber";
                command.Parameters.AddWithValue("token", refreshToken);
                command.Parameters.AddWithValue("IndexNumber", IndexNumber);
                command.ExecuteNonQuery();

            }
        }

        public string CheckRefreshToken(string refreshToken)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                connection.Open();
                Console.WriteLine(refreshToken);
                command.CommandText = "SELECT IndexNumber FROM Student WHERE token = @token";
                command.Parameters.AddWithValue("token", refreshToken);

                var read = command.ExecuteReader();

                if (read.Read())
                    return read["IndexNumber"].ToString();
                else
                    return "brak";
            }
        }
    }
}
