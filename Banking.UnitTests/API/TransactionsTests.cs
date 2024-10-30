using Banking.API.Controllers;
using Banking.Application.Core;
using Banking.Application.Transactions.Commands.Deposit;
using Banking.Application.Transactions.Commands.Withdraw;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
#pragma warning disable CA1707

namespace Banking.UnitTests.API
{
    public class TransactionsTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly TransactionsController _controller;

        public TransactionsTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new TransactionsController(_mediatorMock.Object);
        }

        [Fact]
        public async Task Should_Deposit_When_Successful()
        {
            // Arrange
            var command = new DepositCommand("Test", 100);
            var result = ResultBuilder.Success(new DepositResult("1"));
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(result);

            // Act
            var response = await _controller.Deposit(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task Should_Return_BadRequest_When_Deposit_Fails()
        {
            // Arrange
            var command = new DepositCommand("Test", 100);
            var result = ResultBuilder.Failure<DepositResult>(new ArgumentException());
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(result);

            // Act
            var response = await _controller.Deposit(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task Should_Withdraw_When_Successful()
        {
            // Arrange
            var command = new WithdrawCommand("Test", 50);
            var result = ResultBuilder.Success(new WithdrawResult("1"));
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(result);

            // Act
            var response = await _controller.Withdraw(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task Should_Return_BadRequest_When_Withdraw_Fails()
        {
            // Arrange
            var command = new WithdrawCommand("Test", 50);
            var result = ResultBuilder.Failure<WithdrawResult>(new ArgumentException());
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(result);

            // Act
            var response = await _controller.Withdraw(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }
    }
}
