using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        protected UserManager<ApplicationUser> userManager { get; }
        protected IConfiguration configuration { get; }

        public AuthController(UserManager<ApplicationUser> userManager,IConfiguration configuration)
        {
            this.configuration = configuration;
            this.userManager = userManager;
        }
        [Authorize]
        [HttpPost]
        [Route("users")]
        public IActionResult getusers()
        {
            
            var users = userManager.Users.Select(u => new { u.Id,u.UserName, u.FullName,Role=JsonConvert.DeserializeObject<int[]>(u.Role),password="",u.Email,u.Unite,u.Class }).ToList();
            
            return Ok(users);
            
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> AddUser(AuthModel user)
        {
            var auser = new ApplicationUser()
            {
                Enabled = user.Enabled,
                UserName = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                DDP = user.DDP,
                Role = JsonConvert.SerializeObject(user.Role),
                Class = user.Class,
                Unite = user.Unite
            };
            auser.SetAccess(user.Access);
            var result = await userManager.CreateAsync(auser, user.Password);
            if (result.Succeeded)
            {
                var userId = await userManager.FindByNameAsync(user.Username);
                return Ok(new
                {
                    success = true,
                    userId=userId.Id
                });
                
            }
            string err = "";
            foreach(var error in result.Errors)
            {
                
                switch (error.Code)
                {
                    case "DuplicateUserName": err+="Nom d'utilisateur existant";break;
                    default:err += "  /" + error.Description;break;
                }
            }
            return Ok(new {
                success = false,
                 err
            } );
        }
        public bool hasrole(int[] Role, int re)
        {
            
            foreach (int r in Role)
            {
                if (re == r) return true;
            }
            return false;
        }
        [Authorize]
        [Route("agents")]
        [HttpGet]
        public IActionResult GetAgents()
        {
            using (var cnx = new SqlConnection(configuration.GetConnectionString("gespersAppCon").ToString()))
            {
                var agents = cnx.Query("select mdib,doti,cin,nom,sexe,Vnom,Vprenom,Mail from personel where (dsortie is null or DSORTIE>getdate())");
                return Ok(agents);
            }
        }
        [Authorize]
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> UpdateUser(UpdateModel user)
        {
            var toup = await userManager.FindByIdAsync(user.id);
            if(toup != null)
            { 
                if (user.Username != null && user.Username!="" ) toup.UserName = user.Username;
                if(user.Email!=null) toup.Email = user.Email;
                if (user.FullName != null && user.FullName != "") toup.FullName = user.FullName;
                if (user.Unite != 0) toup.Unite = user.Unite;
                if (user.Class != 0) toup.Class = user.Class;
                if (user.Role.Length != 0) toup.Role = JsonConvert.SerializeObject(user.Role);
                if (user.Password != null && user.Password != "") toup.PasswordHash = userManager.PasswordHasher.HashPassword(toup, user.Password);
                var result = await userManager.UpdateAsync(toup);
                if (result.Succeeded)
                    return Ok(new
                    {
                        success = true
                    });
                string err = "";
                foreach (var error in result.Errors)
                {
                    err += "  /" + error.Description;
                }
                return Ok(new
                {
                    success = false,
                    err
                });
            }
            else
            {
                return Ok(new { success = false, err = "Utilisateur n'existe pas" });
            }
           
        }
        [Authorize]
        [HttpGet]
        [Route("request")]
        public async Task<IActionResult> RequestAsync(string token)
        {
            var username = new JwtSecurityTokenHandler().ReadJwtToken(token.Substring(7)).Claims.First(c => c.Type == "sub").Value;
            Console.WriteLine(username);
            var user = await userManager.FindByNameAsync(username);
            if (user != null)
            {
                return Ok(new
                {
                    success = true,
                    user = new
                    {
                        user.UserName,
                        Role = JsonConvert.DeserializeObject(user.Role),
                        user.FullName,
                        user.Email,
                        user.Id,
                        password = "",
                        user.Unite,
                        user.Class
                    },
                    token = token
                });
            }
            else
            {
                return Ok(new
                {
                    success = false,
                    msg = "Utilisateur non existant"
                });
            }
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginModel login)
        {
            try
            {
                var user = await userManager.FindByNameAsync(login.Username);
                var passf = await userManager.CheckPasswordAsync(user, login.Password);
                if (user != null)
                {
                    if (await userManager.IsLockedOutAsync(user))
                    {
                        return Ok(new
                        {
                            success = false,
                            msg = "Compte bloqué pour 5 minutes à cause de plusieurs tentative erroné "
                        });
                    }
                    else
                    {
                        if (user != null && user.UserName == login.Username && passf)
                        {

                            var claims = new[]
                            {
                    new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti,user.Id)
                };

                            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("auth:secret")));
                            var token = new JwtSecurityToken(
                                issuer: "MYINRA",
                                audience: "INRA",
                                expires: DateTime.UtcNow.AddHours(configuration.GetValue<int>("auth:duration")),
                                claims: claims,
                                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                                );
                            if (user.Enabled)
                            {
                                return Ok(new
                                {
                                    success = true,
                                    user = new
                                    {
                                        user.UserName,
                                        Role=JsonConvert.DeserializeObject(user.Role),
                                        user.FullName,
                                        user.Email,
                                        user.DDP,
                                        access=user.GetAccess(),
                                        user.Id,
                                        password="",
                                        user.Unite,
                                        user.Class
                                    },
                                    token = new JwtSecurityTokenHandler().WriteToken(token)
                                });
                            }
                            else
                            {
                                return Ok(new
                                {
                                    success = false,
                                    msg = "Utilisateur non activé,un email d'activation vous sera envoyer!"
                                });
                            }
                        }
                        await userManager.AccessFailedAsync(user);
                        return Ok(new
                        {
                            success = false,
                            msg = "Utilisateur ou mot de passe incorrect",
                        });
                    }
                }
                else
                {
                    return Ok(new
                    {
                        success = false,
                        msg = "Utilisateur non existant"
                    });
                }

            }
            catch (Exception)
            {

                throw;
            }
            
            
        }
        [HttpPost]
        [Route("notify")]
        public IActionResult Notify(LoginModel login)
        {
            return Ok(true);
        }


        }
    
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
    public class AuthModel
    {
        [Required]
        public string Username { get; set; }
        public string DDP { get; set; }
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string FullName { get; set; }
        public int[] Role { get; set; }
        public string[] Access { get; set; }
        public int Unite { get; set; }
        public int Class { get; set; }
        public bool Enabled { get; set; }

    }
    public class UpdateModel
    {
        [Required]
        public string id { get; set; }
        public string DDP { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public int[] Role { get; set; }
        public string[] Access { get; set; }
        public int Unite { get; set; }
        public int Class { get; set; }
        public bool Enabled { get; set; }

    }
    public class History
    {
        protected IConfiguration configuration {get;}
        public History(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public void Historify(string user,string table, string op, string concatid)
        {
            using (var cnx = new SqlConnection(configuration.GetConnectionString("GESPERS").ToString()))
            {
                var result = cnx.Query("insert into History(userId,tableName,op,concatId) values(@user,@table,@op,@concatid)", new { user, table, op, concatid });

            }
        }
    }
}
    