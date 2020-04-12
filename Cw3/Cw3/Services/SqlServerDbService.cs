using Cw3.DTOs.Requests;
using Cw3.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.Services
{
    public class SqlServerDbService : IStudentDbService
    {

        string connectionString = "Data Source=db-mssql;Initial Catalog=s18534;Integrated Security=True";

        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request)
        {
            using(var connection = new SqlConnection(connectionString))
            using(var command = new SqlCommand())
            {

                command.Connection = connection;

                connection.Open();

                command.CommandText = "SELECT IdStudy FROM studies WHERE name=@name";
                command.Parameters.AddWithValue("name", request.Studies);

                var read = command.ExecuteReader();
                if (!read.Read())
                    throw new Exception("No such studies");

                int idStudies = (int) read["IdStudy"];
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
    }
}
