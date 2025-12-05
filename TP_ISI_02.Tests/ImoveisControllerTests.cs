using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TP_ISI_02.API.Controllers;
using TP_ISI_02.Domain.Interfaces;
using TP_ISI_02.Domain.Models;
using Xunit;

namespace TP_ISI_02.Tests
{
    public class ImoveisControllerTests
    {
        private readonly Mock<IImovelRepository> _repositoryMock;
        private readonly ImoveisController _controller;

        public ImoveisControllerTests()
        {
            _repositoryMock = new Mock<IImovelRepository>();
            _controller = new ImoveisController(_repositoryMock.Object);
        }

        [Fact]
        public async Task GetImoveis_DeveRetornarListaDeImoveis()
        {
            // Arrange
            var imoveis = new List<Imovel>
            {
                new Imovel { Id = 1, Titulo = "Casa 1", Preco = 100000, Localizacao = "Porto" },
                new Imovel { Id = 2, Titulo = "Casa 2", Preco = 200000, Localizacao = "Lisboa" }
            };

            _repositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(imoveis);

            // Act
            var result = await _controller.GetImoveis();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedImoveis = Assert.IsAssignableFrom<IEnumerable<Imovel>>(actionResult.Value);
            Assert.Equal(2, returnedImoveis.Count());
        }

        [Fact]
        public async Task GetImovel_ComIdValido_DeveRetornarImovel()
        {
            // Arrange
            var imovelId = 1;
            var imovel = new Imovel { Id = imovelId, Titulo = "Casa Teste", Preco = 150000, Localizacao = "Braga" };

            _repositoryMock.Setup(repo => repo.GetByIdAsync(imovelId))
                .ReturnsAsync(imovel);

            // Act
            var result = await _controller.GetImovel(imovelId);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedImovel = Assert.IsType<Imovel>(actionResult.Value);
            Assert.Equal("Casa Teste", returnedImovel.Titulo);
        }

        [Fact]
        public async Task GetImovel_ComIdInvalido_DeveRetornarNotFound()
        {
            // Arrange
            var imovelId = 99;

            _repositoryMock.Setup(repo => repo.GetByIdAsync(imovelId))
                .ReturnsAsync((Imovel)null);

            // Act
            var result = await _controller.GetImovel(imovelId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
