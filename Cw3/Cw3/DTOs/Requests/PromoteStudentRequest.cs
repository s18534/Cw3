using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Cw3.DTOs.Requests
{
    public class PromoteStudentRequest
    {   [Required]
        public string Studies { get; set; }
        [Required]
        public string Semester { get; set; }
    }
}
