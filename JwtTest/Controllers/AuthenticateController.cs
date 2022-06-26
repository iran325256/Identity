using JwtTest.IdentityAuth;
using JwtTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JwtTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        public AuthenticateController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("register"]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExist = await _userManager.FindByNameAsync(model.UserName);
            if (userExist != null)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "User Already exists" });


                ApplicationUser user = new ApplicationUser()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.UserName
                };
            var result = await _userManager.CreateAsync(user, model.Password);
            if(!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new Response { Status = "Error", Message = "User already exists!" });

            return Ok(new Response { Status = "Success", Message = "User Creat Successfully" });
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error",
                    Message = "User Already Exsits"

                });

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                {
                    Status = "Error",
                    Message = "User Creation Failed! Please Cheack user detail and try again."
                });
            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new IdentityRole("User"));

            if(await _roleManager.RoleExistsAsync("Admin"))
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }

            return Ok(new Response
            {
                Status = "Success",
                Message = "User Created Successfully"
            });
        }


        [HttpPost]
        [Route("logtin")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {

            var user = await _userManager.FindByNameAsync(model.UserName);

            if(user!=null&&await _userManager.CheckPasswordAsync(user , model.Password))
            {
                var userRole = await _userManager.GetRolesAsync(user);

                var authClaim = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                };
                
                foreach (var role in userRole)
                {
                    authClaim.Add(new Claim(ClaimTypes.Role, role));
                }

                var authSignKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token=new JwtSecurityToken(
                    issuer:_configuration[],
                    audience:_configuration[],
                    expires:DateTime.Now.AddHours(1)),
                    //Claims:authCla
            }
            return Ok(new Response
            {
                Status="Success",
                Message=""
            })
        }
    }
}
