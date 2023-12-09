using AutoMapper;
using CarShopAPI.Data.UserRepositories;
using CarShopAPI.Dto;
using Entites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarShopAPI.Controllers
{

    [Route("/api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _config;

        public AccountController(IUserRepository userRepository,
            IMapper mapper, ILogger<AccountController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _mapper = mapper;
            _userRepository = userRepository;
            _config = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> PostUser(RegistrationDto registerDto)
        {
            var existingUser = await _userRepository.CheckUsername(registerDto.UserName);
            if (existingUser  != null)
            {
                return BadRequest("Username already in use.");
            }

            var newUser = new User
            {
                UserName = registerDto.UserName,
                Password = HashPassword(registerDto.Password)
            };

            try
            {
                var createdUser = await _userRepository.AddUser(newUser);

                if (createdUser == null)
                {
                    _logger.LogWarning($"Failed creating user");
                    return BadRequest();
                }
                _logger.LogInformation($"Succsefully created user");
                return Ok(CreatedAtAction("Added User", createdUser));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when trying to register"); return StatusCode(500, "Internal server error");
            }
        }


        [HttpPost("login")]
        public async Task<ActionResult<User>> LoginUser(RegistrationDto registerDto)
        {
            var user = await _userRepository.CheckUsername(registerDto.UserName);
            if (user == null)
            {
                return BadRequest("Username not found.");
            }

            if(!BCrypt.Net.BCrypt.Verify(registerDto.Password, user.Password))
            {
                return BadRequest("Wrong password");
            }

            string token = CreateToken(user);

            return Ok(token);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim> 
            { 
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("Jwt:Token").Value!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

    }
}
