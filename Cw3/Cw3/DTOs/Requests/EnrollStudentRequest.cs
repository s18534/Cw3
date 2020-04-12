using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.DTOs.Requests
{
    public class EnrollStudentRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IndexNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Studies { get; set; }
    }
}
