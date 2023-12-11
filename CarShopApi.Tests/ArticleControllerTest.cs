using AutoMapper;
using CarShopAPI.Controllers;
using CarShopAPI.Dto;
using Data.Repositories;
using Entites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Hosting;

namespace CarShopApi.Tests
{
    public class Tests
    {
        private Mock<IArticleRepository> _articleRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger<ArticleController>> _loggerMock;
        private ArticleController sut;
        private readonly IWebHostEnvironment _hostEnvironment;


        [SetUp]
        public void Setup()
        {
            _articleRepositoryMock = new Mock<IArticleRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ArticleController>>();
            IWebHostEnvironment hostEnvironment;

            // Additional setups if needed

            sut = new ArticleController(_articleRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object, _hostEnvironment);
        }
        [Test]
        public async Task DeleteArticleById_ReturnsDeletedArticle_WhenArticleFound()
        {
            // ARRANGE
            int existingArticleId = 1;
            var existingArticle = new Car { Id = existingArticleId, Description = "Ford Focus" };

            _articleRepositoryMock.Setup(x => x.DeleteCarById(existingArticleId)).ReturnsAsync(existingArticle);

            // ACT
            var result = await sut.DeleteCar(existingArticleId);

            // ASSERT
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(NoContentResult), result);

            var OkResult = result as NoContentResult;
            Assert.AreEqual(204, OkResult.StatusCode);

        }
        [Test]
        public async Task DeleteTodoById_ReturnsNotFound_If_TodoNotFound()
        {
            // ARRANGE
            int id = 1;
            _articleRepositoryMock.Setup(x => x.DeleteCarById(id)).ReturnsAsync((Car)null);

            // ACT
            var result = await sut.DeleteCar(id);

            // ASSERT
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(NotFoundResult), result);

        }
        //[Test]
        //public async Task AddArticle_ReturnsNewCarDto_WhenFire()
        //{
        //    // ARRANGE 
        //    var newArticle = new Car { Description = "Volvo XC90" };
        //    var createdArticle = new Car
        //    {
        //        Id = 1,
        //        Brand = "Volvo",
        //        Model = "XC90",
        //        Year = 2021,
        //        MileAge = 4120,
        //        Transmission = "Fjantomat",
        //        Price = 610000,
        //        ContactName = "Holger",
        //        ContactNumber = 073020954,
        //        FuelType = "LaddHybrid",
        //        Description = "Fin Volvo som har agerat företagsbil",
        //        Imagelink = "",
        //        AuctionDateTime = new DateTime(2023, 11, 25, 15, 30, 0)
        //};

        //    _articleRepositoryMock.Setup(x => x.AddArticle(newArticle)).ReturnsAsync(createdArticle);

        //    // ACT
        //    var result = await sut.PostArticle(newArticle);

        //    // ASSERT
        //    Assert.IsNotNull(result);
        //    Assert.IsInstanceOf<ActionResult<CarDto>>(result);

        //    var okResult = result.Result as CreatedAtActionResult;
        //    Assert.IsNotNull(okResult);
        //    Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
        //    Assert.AreEqual(201, okResult.StatusCode);

        //}
        [Test]
        public async Task PutArticle_ReturnsUpdatedArticle()
        {
            // ARRANGE
            int articleId = 1;
            var updatedArticle = new Car { Id = articleId, Description = "Updated Description", ContactName = "Updated Name" };

            _articleRepositoryMock.Setup(x => x.UpdateCar(articleId, updatedArticle)).ReturnsAsync((Car)null);

            // ACT
            var result = await sut.PutCar(articleId, updatedArticle);

            // ASSERT
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ActionResult<CarDto>>(result);

            if (result.Result is OkObjectResult okResult)
            {
                Assert.Fail("Expected BadRequest, but received OkObjectResult.");
            }
            else
            {
                Assert.IsInstanceOf<BadRequestResult>(result.Result);
                Assert.AreEqual(400, (result.Result as BadRequestResult).StatusCode);
            }
        }
        [Test]
        public async Task PutArticle_InvalidData_ReturnsBadRequest()
        {
            //ASSERT
            int id = 1;
            _articleRepositoryMock.Setup(x => x.UpdateCar(It.IsAny<int>(), It.IsAny<Car>())).ReturnsAsync((Car)null);

            //ACT
            var actionResult = await sut.PutCar(id, new Car());

            // ASSERT
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOf<BadRequestResult>(actionResult.Result);

        }
        [Test]
        public async Task GetArticle_ReturnsArticleWithId()
        {
            //ARRANGE
            int articleId = 1;
            var createdArticle = new Car
            {
                Id = 1,
                Brand = "Volvo",
                Model = "XC90",
                Year = 2021,
                MileAge = 4120,
                Transmission = "Fjantomat",
                Price = 610000,
                ContactName = "Holger",
                ContactNumber = 073020954,
                FuelType = "LaddHybrid",
                Description = "Fin Volvo som har agerat företagsbil",
                Imagelink = ""

            };
            _articleRepositoryMock.Setup(x => x.GetArticle(articleId)).ReturnsAsync(createdArticle);

            //ACT
            var result = await sut.GetArticle(articleId);

            //ASSERT
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);

            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsInstanceOf<ActionResult<CarDto>>(result);

        }
        [Test]
        public async Task GetArticle_ReturnsNotFound_If_ArticleNotFound()
        {
            //ARRANGE
            int id = 1;
            _articleRepositoryMock.Setup(x => x.GetArticle(id)).ReturnsAsync((Car)null);

            //ACT
            var result = await sut.GetArticle(id);

            //ASSERT
            Assert.IsInstanceOf<NotFoundResult>(result.Result);

        }
        [Test]
        public async Task GetArticles_ReturnsListOfArticles()
        {
            //ARRANGE
            var expectedArticle = new List<Car>
            {
                new Car { Id = 1, Description = "Car 1", Transmission = "Manuell" },
                new Car { Id = 2, Description = "Car 2", Transmission = "Automat" },
            };
            _articleRepositoryMock.Setup(x => x.GetArticles()).ReturnsAsync(expectedArticle);
            _mapperMock.Setup(y => y.Map<List<CarDto>>(expectedArticle)).Returns(
              new List<CarDto>
              {
                    new CarDto { Id = 1, Description = "Car 1", Transmission = "Manuell" },
                    new CarDto { Id = 2, Description = "Car 2", Transmission = "Automat" },
              });
            //ACT
            var result = await sut.GetArticles();

            //ASSERT
            Assert.IsNotNull(result);

            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(200, okResult.StatusCode);

            var articleDto = okResult.Value as List<CarDto>;
            Assert.IsNotNull(articleDto);

            Assert.AreEqual(expectedArticle.Count, articleDto.Count);

        }
        [Test]
        public async Task GetArticle_ReturnsNotFound_WhenArticleNotFound()
        {
            //ARRANGE
            int nonExistedArticle = 999;
            _articleRepositoryMock.Setup(x => x.GetArticle(nonExistedArticle)).ReturnsAsync((Car)null);

            //ACT
            var result = await sut.GetArticle(nonExistedArticle);

            //ARRANGE
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundResult>(result.Result);

            var notFoundResult = result.Result as NotFoundResult;
            Assert.AreEqual(404, notFoundResult.StatusCode);

        }
    }
}

