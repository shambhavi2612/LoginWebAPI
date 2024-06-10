using loginWebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;

namespace loginWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public RegistrationController(IConfiguration configuration)

        {
            _configuration = configuration;

        }





        [HttpPost]



        [Route("registration")]
        public string registration(Registration registration)
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("ToysCon").ToString());
            SqlCommand cmd = new SqlCommand("Insert into Registration(UserName,Password,Email,ISActive) Values('" + registration.UserName + "','" + registration.Password + "','" + registration.Email + "','" + registration.IsActive + "' )", con);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            if (i > 0)
            {
                return "Data Inserted";
            }
            else
            {
                return "Error";
            }
        }
        [HttpPost]
        [Route("login")]
        public string login(Registration registration)
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("ToysCon").ToString());

            SqlDataAdapter da = new SqlDataAdapter("Select * from Registration where Email= '" + registration.Email + "' AND Password='" + registration.Password + "' AND ISActive=1", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                return "Valid User";
            }
            else
            {
                return "Invalid User";
            }


        }

    }
}


