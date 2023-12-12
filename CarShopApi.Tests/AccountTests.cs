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

        [Test]
        public async Task Should_return_status_code_200_when_login_user()
        {
            // Arrange
            var registerDto = new RegistrationDto { UserName = "Test123", Password = "1Password" };
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            var user = new User { UserName = registerDto.UserName, Password = hashedPassword };

            // Ensure the CheckUsername returns a non-null user
            mockRepository.Setup(repo => repo.CheckUsername(It.IsAny<string>())).ReturnsAsync(user);
            mockConfiguration.Setup(config => config.GetSection("Jwt:Token").Value).Returns("my very very top of the line secret key with a crazy long name because why not");

            // Act
            var result = await accountController.LoginUser(new RegistrationDto { UserName = registerDto.UserName, Password = registerDto.Password });

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            // Additional checks
            var token = okResult.Value as string;
            Assert.NotNull(token);
        }
    }
}
