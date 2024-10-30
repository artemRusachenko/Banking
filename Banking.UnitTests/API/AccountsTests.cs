using Banking.API.Accounts.Queries.GetByAccountNumber;
using Banking.API.Controllers;
using Banking.Application.Accounts.Commands.CreateAccount;
using Banking.Application.Accounts.Queries.GetAll;
using Banking.Application.Accounts.Queries.GetByAccountNumber;
using Banking.Application.Core;
using Banking.Domain.Accounts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
#pragma warning disable CA1707

namespace Banking.UnitTests.API
{
    public class AccountsTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly AccountsController _controller;

        public AccountsTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new AccountsController(_mediatorMock.Object);
        }

        [Fact]
        public async Task Should_Returns_Created_Result_When_Successful()
        {
            // Arrange
            var command = new CreateAccountCommand("Successfully Test");
            var result = ResultBuilder.Success(Unit.Value);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(result);

            // Act
            var response = await _controller.CreateAccount(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(response);
            Assert.NotNull(createdResult);
        }

        [Fact]
        public async Task Should_Returns_BadRequest_When_Create_Account_Failed()
        {
            // Arrange
            var command = new CreateAccountCommand("");
            var result = ResultBuilder.Failure<Unit>(new ArgumentException());
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(result);

            // Act
            var response = await _controller.CreateAccount(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task Should_Returns_OkResult_When_Accounts_Exist()
        {
            // Arrange
            var accountsList = new List<Account> { new ("Test") };
            var result = ResultBuilder.Success(new GetAllResult(accountsList));
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

            // Act
            var response = await _controller.GetAll(CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task Should_Returns_NoContent_When_No_Accounts_Exist()
        {
            // Arrange
            var result = ResultBuilder.Success(new GetAllResult(new List<Account>()));
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

            // Act
            var response = await _controller.GetAll(CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task Should_Returns_BadRequest_When_GetAll_Failed()
        {
            // Arrange
            var result = ResultBuilder.Failure<GetAllResult> (new ArgumentException());
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

            // Act
            var response = await _controller.GetAll(CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task Should_Returns_OkResult_When_Account_Exists()
        {
            // Arrange
            var account = new Account("Test");
            var result = ResultBuilder.Success(new GetByAccountNumberResult(account));
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetByAccountNumberQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

            // Act
            var response = await _controller.GetByAccountNumber(account.AccountNumber, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task Should_Returns_NotFound_When_Account_DoesNot_Exist()
        {
            // Arrange
            var result = ResultBuilder.Success(new GetByAccountNumberResult(null));
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetByAccountNumberQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

            // Act
            var response = await _controller.GetByAccountNumber("Test", CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task Should_Returns_BadRequest_When_GetByAccountNumber_Failed()
        {
            // Arrange
            var accountNumber = "123456789"; 
            var result = ResultBuilder.Failure<GetByAccountNumberResult>(new ArgumentException("An error occurred while retrieving the account."));
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetByAccountNumberQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

            // Act
            var response = await _controller.GetByAccountNumber(accountNumber, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }
    }
}
