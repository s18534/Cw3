using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Cw3.DTOs.Requests;
using Cw3.DTOs.Responses;
using Cw3.Models;
using Cw3.DTOs;

namespace Cw3.Services
{
    public interface IStudentDbService
    {
        public IEnumerable<Student> GetStudents();
        public Student GetStudent(string indexNum);
        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request);
        public EnrollStudentResponse PromoteStudent(PromoteStudentRequest request);

        public Claim[] GetClaims(string IndexNumber);
        public bool CheckPassword(LoginRequestDto request);
        public void SetRefreshToken(string token, string indexNumber);
        public string CheckRefreshToken(string token);
    }
}
