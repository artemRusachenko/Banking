#pragma warning disable CA1707
using Banking.API.Accounts.Queries.GetByAccountNumber;
using Banking.Domain.Accounts;
using Moq;

namespace Banking.UnitTests.Application.Accounts
{
    public class GetByAccountNumberHandlerTests
    {
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly GetByAccountNumberHandler _handler;

        public GetByAccountNumberHandlerTests()
        {
            _mockAccountRepository = new Mock<IAccountRepository>();
            _handler = new GetByAccountNumberHandler(_mockAccountRepository.Object);
        }

        [Fact]
        public async Task Should_Return_Failure_When_Request_Is_Null()
        {
            // Act
            var result = await _handler.Handle(null, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ArgumentNullException>(result.Error);
        }

        [Fact]
        public async Task Should_Return_Failure_When_AccountNumber_Is_Null_Or_Empty()
        {
            // Arrange
            var request = new GetByAccountNumberQuery(string.Empty);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ArgumentNullException>(result.Error);
            Assert.Equal("Account number can't be null or empty", result.Error.Message);
        }

        [Fact]
        public async Task Handle_ShouldReturnAccount_WhenAccountNumberIsValid()
        {
            // Arrange
            var account = new Account("User1") { AccountNumber = "123456" };
            _mockAccountRepository
                .Setup(repo => repo.GetByAccountNumberAsync("123456"))
                .ReturnsAsync(account);
            var request = new GetByAccountNumberQuery("123456");

            // Act
            var result = await _handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("123456", result.Value.Account!.AccountNumber);
            Assert.Equal("User1", result.Value.Account.HolderName);
        }
    }
}
