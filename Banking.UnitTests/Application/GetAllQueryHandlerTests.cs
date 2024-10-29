#pragma warning disable CA1707
using Banking.API.Accounts.Queries.GetAll;
using Banking.Application.Accounts.Queries.GetAll;
using Banking.Domain.Accounts;
using Moq;

namespace Banking.Tests.Application
{
    public class GetAllQueryHandlerTests
    {
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly GetAllQueryHandler _handler;

        public GetAllQueryHandlerTests()
        {
            _mockAccountRepository = new Mock<IAccountRepository>();
            _handler = new GetAllQueryHandler(_mockAccountRepository.Object);
        }

        [Fact]
        public async Task Should_Return_Failure_When_Request_Is_Null()
        {
            // Arrange
            GetAllQuery? request = null;

            // Act
            var result = await _handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ArgumentNullException>(result.Error);
        }

        [Fact]
        public async Task Should_Return_All_Accounts_When_Request_Is_Valid()
        {
            // Arrange
            var accounts = new List<Account>
            {
                new Account("User1"),
                new Account("User2")
            };

            _mockAccountRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(accounts);
            var request = new GetAllQuery();

            // Act
            var result = await _handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Accounts.Count());
        }
    }
}
