//using loginWebAPI.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using System.Data;
//using System.Data.SqlClient;

//namespace loginWebAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class RegistrationController : ControllerBase
//    {
//        private readonly IConfiguration _configuration;
//        public RegistrationController(IConfiguration configuration)

//        {
//            _configuration = configuration;

//        }





//        [HttpPost]



//        [Route("registration")]
//        public string registration(Registration registration)
//        {
//            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("ToysCon").ToString());
//            SqlCommand cmd = new SqlCommand("Insert into Registration(UserName,Password,Email,ISActive) Values('" + registration.UserName + "','" + registration.Password + "','" + registration.Email + "','" + registration.IsActive + "' )", con);
//            con.Open();
//            int i = cmd.ExecuteNonQuery();
//            con.Close();
//            if (i > 0)
//            {
//                return "Data Inserted";
//            }
//            else
//            {
//                return "Error";
//            }
//        }
//        [HttpPost]
//        [Route("login")]
//        public string login(Registration registration)
//        {
//            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("ToysCon").ToString());

//            SqlDataAdapter da = new SqlDataAdapter("Select * from Registration where Email= '" + registration.Email + "' AND Password='" + registration.Password + "' AND ISActive=1", con);
//            DataTable dt = new DataTable();
//            da.Fill(dt);
//            if (dt.Rows.Count > 0)
//            {
//                return "Valid User";
//            }
//            else
//            {
//                return "Invalid User";
//            }


//        }

//    }
//}

// authentication web api

//using loginWebAPI.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using System.Data;
//using System.Data.SqlClient;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace loginWebAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class RegistrationController : ControllerBase
//    {
//        private readonly IConfiguration _configuration;
//        private readonly JwtTokenHelper _jwtTokenHelper; // for authentication 

//        public RegistrationController(IConfiguration configuration)
//        {
//            _configuration = configuration;
//            _jwtTokenHelper = new JwtTokenHelper(configuration); // for authentication
//        }

//        [HttpPost]
//        [Route("registration")]
//        public string registration(Registration registration)
//        {
//            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("ToysCon").ToString());
//            SqlCommand cmd = new SqlCommand("Insert into Registration(UserName,Password,Email,ISActive) Values('" + registration.UserName + "','" + registration.Password + "','" + registration.Email + "','" + registration.IsActive + "' )", con);
//            con.Open();
//            int i = cmd.ExecuteNonQuery();
//            con.Close();
//            if (i > 0)
//            {
//                return "Data Inserted";
//            }
//            else
//            {
//                return "Error";
//            }
//        }

//        [HttpPost]
//        [Route("login")]
//        public IActionResult login(Registration registration)
//        {
//            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("ToysCon").ToString());

//            SqlDataAdapter da = new SqlDataAdapter("Select * from Registration where Email= '" + registration.Email + "' AND Password='" + registration.Password + "' AND ISActive=1", con);
//            DataTable dt = new DataTable();
//            da.Fill(dt);
//            if (dt.Rows.Count > 0)
//            {
//                var token = _jwtTokenHelper.GenerateToken(registration.Email);
//                return Ok(new { token });
//            }
//            else
//            {
//                return Unauthorized("Invalid User");
//            }
//        }



//        [HttpGet]
//        [Route("test")]
//        [Authorize]// attribute for implementing authorization 
//        public IActionResult Test()
//        {
//            return Ok("This is a secured endpoint.");
//        }
//    }
//}

// for excel download 

using loginWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace loginWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        // Injects IConfiguration and initializes JwtTokenHelper for handling JWT token generation.
        private readonly IConfiguration _configuration;
        private readonly JwtTokenHelper _jwtTokenHelper;
        public RegistrationController(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtTokenHelper = new JwtTokenHelper(configuration);
        }

        [HttpPost]
        [Route("registration")]
        public string Registration(Registration registration)
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("ToysCon").ToString());
            SqlCommand cmd = new SqlCommand("Insert into Registration(UserName,Password,Email,ISActive) Values(@UserName, @Password, @Email, @IsActive)", con);
            cmd.Parameters.AddWithValue("@UserName", registration.UserName);
            cmd.Parameters.AddWithValue("@Password", registration.Password);
            cmd.Parameters.AddWithValue("@Email", registration.Email);
            cmd.Parameters.AddWithValue("@IsActive", registration.IsActive);
            con.Open();
            int i = cmd.ExecuteNonQuery();                       
            con.Close();
            return i > 0 ? "Data Inserted" : "Error";
        }           

        [HttpPost]
        [Route("login")]
        public IActionResult Login(Registration registration)
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("ToysCon").ToString());
            SqlDataAdapter da = new SqlDataAdapter("Select * from Registration where Email= @Email AND Password= @Password AND ISActive=1", con);
            da.SelectCommand.Parameters.AddWithValue("@Email", registration.Email);
            da.SelectCommand.Parameters.AddWithValue("@Password", registration.Password);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                var token = _jwtTokenHelper.GenerateToken(registration.Email);
                return Ok(new { token });
            }
            else
            {
                return Unauthorized("Invalid User");
            }
        }

        [HttpGet]
        [Route("test")]
        [Authorize]
        public IActionResult Test()
        {
            return Ok("This is a secured endpoint.");
        }

        [HttpPost]
        [Route("upload")]
        [Authorize]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (!file.FileName.EndsWith(".xls") && !file.FileName.EndsWith(".xlsx"))
                return BadRequest("Invalid file type. Only Excel files are allowed.");

            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            
            // Ensure the uploads directory exists
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }

            var filePath = Path.Combine(uploadsDir, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);                                              

            }

            return Ok(new { message = "File uploaded successfully." });
        }
        
        [HttpGet]
        [Route("download")]
        [Authorize] 
        public IActionResult Download()
        {
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            var filePath = Path.Combine(uploadsDir, "example.xlsx");

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "example.xlsx");
        }
    }
}
