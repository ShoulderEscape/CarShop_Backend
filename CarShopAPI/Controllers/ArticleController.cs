using AutoMapper;
using CarShopAPI.Dto;
using Data.Repositories;
using Entites;
using Microsoft.AspNetCore.Mvc;


namespace CarShopAPI.Controllers
{
    [Route("CarShop/[controller]")]
    [ApiController]

    public class ArticleController : ControllerBase
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ArticleController> _logger;

        public ArticleController(
            IArticleRepository articleRepository,
            IMapper mapper,
            ILogger<ArticleController> logger)
        {
            _articleRepository = articleRepository;
            _mapper = mapper;
            _logger = logger;
        }
        [HttpGet]
        public async Task<ActionResult<List<CarDto>>> GetArticles()
        {
            var cars = await _articleRepository.GetArticles();
            var carDto = _mapper.Map<List<CarDto>>(cars);
            return Ok(carDto);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CarDto>> GetArticle(int id)
        {
            var article = await _articleRepository.GetArticle(id);
            if (article == null)
            {
                return NotFound();
            }
            var articleDto = _mapper.Map<CarDto>(article);
            return Ok(articleDto);
        }
        [HttpPost("CreateCar")]
        public async Task<ActionResult<CarDto>> PostArticle(Car car)
        {
                
            var createdArticle = await _articleRepository.AddArticle(car);
            return CreatedAtAction("GetArticle", new { id = createdArticle.Id }, value: createdArticle);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<CarDto>> PutCar(int id, Car car)
        {
            var updatedCar = await _articleRepository.UpdateCar(id, car);
            if (updatedCar == null)
            {
                return BadRequest();
            }
            var carDto = _mapper.Map<CarDto>(updatedCar);
            return Ok(carDto);
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteCar(int id)
        {
            try
            {
                var car = await _articleRepository.DeleteCarById(id);

                if (car == null)
                {
                    _logger.LogWarning($"Article with id {id} was not found", id);
                    return NotFound();
                }
                _logger.LogInformation($"Article with id {id} was deleted", id);
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error while deleting an article with id {id}", id);
                return StatusCode(500, value: "Internal Server Error");
            }
        }
    }
}