using Entites;
using Microsoft.AspNetCore.Mvc;
using CarShopAPI.Data.Repositories;
using CarShopAPI.Data.UserRepositories;
using AutoMapper;
using CarShopAPI.Dto;

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
            _logger.LogWarning($"HEWLOOOOOOOOOOOOOOOOO{registerDto.UserName}");
            Console.WriteLine("asdasdasdasdasdasd");
            var newUser = new User
            {
                UserName = registerDto.UserName,
                Password = registerDto.Password,
            };
            try
            {
                var createdUser = await _userRepository.AddUser(newUser);
                
                if(createdUser == null)
                {
                    _logger.LogWarning($"Failed creating user");
                    return BadRequest();
                }
                _logger.LogInformation($"Succsefully created user");
                return Ok(CreatedAtAction("GetTodo", createdUser));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when trying to register"); return StatusCode(500, "Internal server error haha");
            }
        }


    }
}
