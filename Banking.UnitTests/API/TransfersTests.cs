#pragma warning disable CA1707

using Banking.API.Controllers;
using Banking.Application.Core;
using Banking.Application.Transfers.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Banking.UnitTests.API
{
    public class TransfersControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly TrransfersController _controller;

        public TransfersControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new TrransfersController(_mediatorMock.Object);
        }

        [Fact]
        public async Task Should_Transfer_When_Successful()
        {
            // Arrange
            var command = new TransferCommand("SourceAccount", "DestinationAccount", 100, null);
            var result = ResultBuilder.Success(new TransferResult("1"));
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(result);

            // Act
            var response = await _controller.Deposit(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task Should_Return_BadRequest_When_Transfer_Fails()
        {
            // Arrange
            var command = new TransferCommand("SourceAccount", "DestinationAccount", 100, null);
            var result = ResultBuilder.Failure<TransferResult>(new ArgumentException());
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(result);

            // Act
            var response = await _controller.Deposit(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }
    }
}
