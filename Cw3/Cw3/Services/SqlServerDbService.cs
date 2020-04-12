using Cw3.DTOs.Requests;
using Cw3.DTOs.Responses;
using System;
using System.Collections.Generic;
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
                var transaction = connection.BeginTransaction();

                command.CommandText = "SELECT IdStudies FROM studies WHERE name=@name";
                command.Parameters.AddWithValue("name", request.Studies);

                var read = command.ExecuteReader();
                if (!read.Read())
                    BadRequest("No such studies");

                int idStudies = (int) read["IdStudy"];
                read.Close();

                command.CommandText = "SELECT * FROM Student WHERE IndexNumber=@index";
                command.Parameters.AddWithValue("index", request.IndexNumber);

                if (read.Read())
                    BadRequest("IndexNumber not unique");

                read.Close();

                command.CommandText = "SELECT * FROM enrollment WHERE Semester=@semester AND IdStudy=@idStudy";
                command.Parameters.AddWithValue("idStudy", idStudies);
                command.Parameters.AddWithValue("semester", 1);
                int idEnroll = 1;
                read = command.ExecuteReader();
                if (!read.Read())
                {
                    read.Close();

                    command.CommandText = "SELECT MAX(IdEnrollment) as maximus from enrollment";
                    read = command.ExecuteReader();
                    if (read.Read())
                        idEnroll = (int)read["maximus"] + 1;

                    read.Close();

                    command.CommandText = "INSERT INTO Enrollment(IdEnrollment,Semester,IdStudy,StartDate) VALUES(@id, @sem, @idstud, @date)";
                    command.Parameters.AddWithValue("@id", idEnroll);
                    command.Parameters.AddWithValue("@sem", 1);
                    command.Parameters.AddWithValue("@idstud", idStudies);
                    command.Parameters.AddWithValue("@date", DateTime.Now);
                }
                else
                    idEnroll = (int) read["IdEnrollment"];
                read.Close();

                command.CommandText = "INSERT INTO Student(IndexNumber,FirstName,LastName,BirthDate,IdEnrollment) VALUES(@index, @firna, @laname, @date, @enroll";
                command.Parameters.AddWithValue("index", request.IndexNumber);
                command.Parameters.AddWithValue("firna", request.FirstName);
                command.Parameters.AddWithValue("laname", request.LastName);
                command.Parameters.AddWithValue("date", request.DateOfBirth);
                command.Parameters.AddWithValue("enroll", idEnroll);
                command.ExecuteNonQuery();
                transaction.Commit();
                
            }
            return Ok();
        }

        public EnrollStudentResponse PromoteStudents(PromoteStudentRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
