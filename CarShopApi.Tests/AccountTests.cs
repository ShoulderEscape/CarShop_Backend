using AutoMapper;
using CarShopAPI.Controllers;
using CarShopAPI.Data.UserRepositories;
using CarShopAPI.Dto;
using Entites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace CarShopApi.Tests
{
    public class AccountTests
    {
        private Mock<IUserRepository> mockRepository;
        private Mock<IMapper> mockMapper;
        private Mock<ILogger<AccountController>> mockLogger;
        private AccountController accountController;
        private Mock<IConfiguration> mockConfiguration;

        [SetUp]
        public void Setup()
        {

            mockRepository = new Mock<IUserRepository>();
            mockLogger = new Mock<ILogger<AccountController>>();
            mockMapper = new Mock<IMapper>();
            mockConfiguration = new Mock<IConfiguration>();
            accountController = new AccountController(mockRepository.Object, mockMapper.Object, mockLogger.Object, mockConfiguration.Object);

        }
        [Test]
        public async Task Should_return_status_code_200_when_creating_user()
        {
            //Arrange

            var registerDto = new RegistrationDto { UserName = "Test123", Password = "1Password" };
            var user = new User { UserName = registerDto.UserName, Password = registerDto.Password };

            mockRepository.Setup(repo => repo.AddUser(It.IsAny<User>())).ReturnsAsync(user);

            //Act

            var result = await accountController.PostUser(registerDto);

            var okResult = result.Result as OkObjectResult;

            Assert.AreEqual(200, okResult.StatusCode);

        }

        [Test]

        public async Task Should_Hash_password_when_user_is_created()
        {
            //Arrange

            var registerDto = new RegistrationDto { UserName = "testUser", Password = "ShouldHashThisPassword1337" };
            User savedUser = null;

            mockRepository.Setup(repo => repo.AddUser(It.IsAny<User>()))
                           .Callback<User>(u => savedUser = u)
                           .ReturnsAsync(new User()); // Returnerar ett dummy User-objekt för att efterlikna en framgångsrik skapelse

            // Act
            await accountController.PostUser(registerDto);

            // Assert
            mockRepository.Verify(repo => repo.AddUser(It.IsAny<User>()), Times.Once());

            // Separat verifiering för lösenordet
            Assert.IsNotNull(savedUser);
            Assert.IsTrue(BCrypt.Net.BCrypt.Verify("ShouldHashThisPassword1337", savedUser.Password));

        }

    }
}
