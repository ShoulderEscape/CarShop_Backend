using AutoMapper;
using CarShopAPI.Dto;
using Data.Repositories;
using Entites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;


namespace CarShopAPI.Controllers
{
    [Route("CarShop/[controller]")]
    [ApiController]

    public class ArticleController : ControllerBase
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ArticleController> _logger;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ArticleController(
            IArticleRepository articleRepository,
            IMapper mapper,
            ILogger<ArticleController> logger,
            IWebHostEnvironment hostEnvironment)

        {
            _articleRepository = articleRepository;
            _mapper = mapper;
            _logger = logger;
            _hostEnvironment = hostEnvironment;
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
        [HttpPost("CreateCar"), DisableRequestSizeLimit]
        public async Task<ActionResult<CarDto>> PostArticle([FromForm] IFormFile? imageFile, [FromForm] string brand, [FromForm] string model, [FromForm] int year,
            [FromForm] int mileAge, [FromForm] string fuelType, [FromForm] string transmission, [FromForm] string contactName, [FromForm] int contactNumber, [FromForm]
            int price, [FromForm] string? description, [FromForm] string? imagelink, [FromForm] DateTime auctionDateTime)
        {
            // Konvertera från FormData till Car-objekt
            Car car = new Car
            {
                Brand = brand,
                Model = model,
                Year = year,
                MileAge = mileAge,
                FuelType = fuelType,
                Transmission = transmission,
                ContactName = contactName,
                ContactNumber = contactNumber,
                Price = price,
                Description = description,
                Imagelink = imagelink,
                AuctionDateTime = auctionDateTime
            };

            // Hantera bilduppladdningen här
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolderPath = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolderPath))
                    Directory.CreateDirectory(uploadsFolderPath);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;

                var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                car.Imagelink = "/uploads/" + uniqueFileName; // Spara den relativa sökvägen till bilden
            }

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