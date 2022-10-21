using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration configuration; // to access appsettings

        public AuthenticationController(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        [HttpPost("authenticate")] // post on api/auhtenticaton/authenticate
        public ActionResult<string> Authenticate(AuthenticationRequestBody authenticationRequestBody) // returns a token/ string
        {
            // Step 1: validate username/password
            var user = ValidateUserCredentials(
                authenticationRequestBody.UserName,
                authenticationRequestBody.Password);

            if (user == null)
            {
                return Unauthorized();
            }
            // tokens are signed we need a key to sign it
            // keys are created from secrets
            // these keys should be saved in a safe place
            // like a key vault or local

           
            var securityKey = new SymmetricSecurityKey(  // security key for token
                Encoding.ASCII.GetBytes(configuration["Authentication:SecretForKey"])
                );
            var signingCredentials = new SigningCredentials( // used for siging the token
                securityKey, SecurityAlgorithms.HmacSha256);
            // claims: identity related info about the user
            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", user.UserId.ToString())); // sub is the standardized key for the unique user identifier
            claimsForToken.Add(new Claim("given_name", user.FirstName));
            claimsForToken.Add(new Claim("family_name", user.LastName));
            claimsForToken.Add(new Claim("city", user.City));

            var jwtSecurityToken = new JwtSecurityToken(
                configuration["Authentication:Issuer"],
                configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow, // start of token
                DateTime.UtcNow.AddHours(1), // end time off token
                signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler()
                .WriteToken(jwtSecurityToken);

            return Ok(tokenToReturn); // Note token based security relies on https for security

        }


        private CityInfoUser ValidateUserCredentials(string? userName, string? password)
        {
            // In real life user data stored in a table or a seperate user database
            // If we have a user table/ db. Here we check what is passed through with what is in the db
            // For demo asume credentials are valid

            // return a new CityInfoUser (these values would normally come from the db
            return new CityInfoUser(
                1,
                userName ?? "",
                "Kevin",
                "Dockx",
                "Antwerp");
        }


        // inner class only used inside this class
        public class AuthenticationRequestBody
        {
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }

        // inner class only used here
        public class CityInfoUser
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string City { get; set; }

            public CityInfoUser(int userId, string userName, string firstName, string lastName, string city)
            {
                UserId = userId;
                UserName = userName;
                FirstName = firstName;
                LastName = lastName;
                City = city;
            }
        }

    }
}
