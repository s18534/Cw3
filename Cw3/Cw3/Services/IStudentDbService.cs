using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cw3.DTOs.Requests;
using Cw3.DTOs.Responses;
namespace Cw3.Services
{
    public interface IStudentDbService
    {
        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request);
        public EnrollStudentResponse PromoteStudents(PromoteStudentRequest request);
    }
}
