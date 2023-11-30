using Entites;
using Microsoft.AspNetCore.Mvc;
using CarShopAPI.Data.Repositories;
using CarShopAPI.Data.UserRepositories;
using AutoMapper;

namespace CarShopAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AcoountController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AcoountController> _logger;

        public AcoountController(IUserRepository userRepository,
            IMapper mapper, ILogger<AcoountController> logger)
        {
            _logger = logger;
            _mapper = mapper;
            _userRepository = userRepository; 
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            try
            {
                var createdUser = await _userRepository.AddUser(user);
                
                if(user == null)
                {
                    _logger.LogWarning($"Failed creating user");
                    return BadRequest();
                }
                _logger.LogInformation($"Succsefully created user");
                return Ok(CreatedAtAction("GetTodo", new { id = user.Id }, createdUser));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when trying to register"); return StatusCode(500, "Internal server error");
            }
        }


    }
}
