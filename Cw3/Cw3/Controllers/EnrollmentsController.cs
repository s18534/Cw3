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
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace Cw3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentDbService _service;
        private IConfiguration Configuration;

        public EnrollmentsController(IStudentDbService service, IConfiguration configuration)
        {
            _service = service;
            Configuration = configuration;
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            try
            {
                return Ok(_service.EnrollStudent(request));
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("promotions")]
        [Authorize(Roles = "Employee")]
        public IActionResult PromoteStudent(PromoteStudentRequest request)
        {
            try
            {
                return Ok(_service.PromoteStudent(request));
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequestDto request)
        {
            if (!_service.CheckPassword(request))
                return Forbid("Bearer");
        
            var claims = _service.GetClaims(request.Login);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
                );
            var refreshToken = Guid.NewGuid();
            _service.SetRefreshToken(refreshToken.ToString(), request.Login);
            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token), refreshToken });
        }

        [HttpPost("refresh_token/{refreshToken}")]
        public IActionResult RefreshToken(string refreshToken)
        {
            var response = _service.CheckRefreshToken(refreshToken);
            if (!response.Equals("brak"))
            {
                var claims = _service.GetClaims(response);
                if (claims == null)
                {
                    throw new Exception("null claims");
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken
                (
                    issuer: "Gakko",
                    audience: "Students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds
                );
                return Ok(new { newtoken = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            return StatusCode(401);
        }

    }
}