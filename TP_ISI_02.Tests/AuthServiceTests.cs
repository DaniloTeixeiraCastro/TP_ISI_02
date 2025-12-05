using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using TP_ISI_02.API.Services;
using TP_ISI_02.Domain.Interfaces;
using TP_ISI_02.Domain.Models;
using Xunit;

namespace TP_ISI_02.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly IConfiguration _configuration;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();

            var myConfiguration = new Dictionary<string, string>
            {
                {"Jwt:SecretKey", "SuperSecretKeyForTestingPurposesOnly123!"},
                {"Jwt:Issuer", "TestIssuer"},
                {"Jwt:Audience", "TestAudience"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            _authService = new AuthService(_userRepositoryMock.Object, _configuration);
        }

        [Fact]
        public async Task Login_ComCredenciaisValidas_DeveRetornarToken()
        {
            // Arrange
            var username = "testuser";
            var password = "Password123!";
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Username = username,
                PasswordHash = passwordHash,
                Email = "test@example.com"
            };

            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(username))
                .ReturnsAsync(user);

            // Act
            var token = await _authService.LoginAsync(username, password);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public async Task Login_ComCredenciaisInvalidas_DeveRetornarNull()
        {
            // Arrange
            var username = "nonexistent";
            var password = "wrongpassword";

            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(username))
                .ReturnsAsync((User)null);

            // Act
            var token = await _authService.LoginAsync(username, password);

            // Assert
            Assert.Null(token);
        }

        [Fact]
        public async Task Login_ComPasswordErrada_DeveRetornarNull()
        {
            // Arrange
            var username = "testuser";
            var password = "Password123!";
            var wrongPassword = "WrongPassword!";
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Username = username,
                PasswordHash = passwordHash
            };

            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(username))
                .ReturnsAsync(user);

            // Act
            var token = await _authService.LoginAsync(username, wrongPassword);

            // Assert
            Assert.Null(token);
        }
    }
}
