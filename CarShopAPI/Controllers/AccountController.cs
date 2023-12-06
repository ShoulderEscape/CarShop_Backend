using AutoMapper;
using CarShopAPI.Data.UserRepositories;
using CarShopAPI.Dto;
using Entites;
using Microsoft.AspNetCore.Mvc;

namespace CarShopAPI.Controllers
{

    [Route("/api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserRepository userRepository,
            IMapper mapper, ILogger<AccountController> logger)
        {
            _logger = logger;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(RegistrationDto registerDto)
        {
            var usernameExists = await _userRepository.CheckUsername(registerDto.UserName);
            if (usernameExists)
            {
                return BadRequest("Username already in use.");
            }

            var newUser = new User
            {
                UserName = registerDto.UserName,
                Password = HashPassword(registerDto.Password),
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

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }


    }
}
