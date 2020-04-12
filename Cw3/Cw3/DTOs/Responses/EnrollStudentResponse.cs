using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.DTOs.Responses
{
    public class EnrollStudentResponse
    {
        public int IdEnroll { get; set; }
        public int Semester { get; set; }
        public int Study { get; set; }
        public DateTime Date { get; set; }

    }
}
