#pragma warning disable CA1707
using Banking.Application.Accounts.Commands.CreateAccount;
using Banking.Domain.Accounts;
using Banking.Domain.Data;
using Moq;

namespace Banking.Tests.Application
{
    public class CreateAccountCommandHandlerTests
    {
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly CreateAccountCommandHandler _handler;

        public CreateAccountCommandHandlerTests()
        {
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new CreateAccountCommandHandler(_mockAccountRepository.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Should_Return_Failure_When_Request_Is_Null()
        {
            // Arrange
            CreateAccountCommand? request = null;

            // Act
            var result = await _handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ArgumentNullException>(result.Error);
        }

        [Fact]
        public async Task Should_Return_Failure_When_HolderName_Is_Null_Or_Empty()
        {
            // Arrange
            var request = new CreateAccountCommand(string.Empty);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ArgumentException>(result.Error);
            Assert.Equal("Holder name can't be null or empty", result.Error.Message);
        }

        [Fact]
        public async Task Should_Create_Account_When_Request_Is_Valid()
        {
            // Arrange
            var request = new CreateAccountCommand("Test");

            // Act
            var result = await _handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.True(result.IsSuccess);
            _mockAccountRepository.Verify(repo => repo.AddAsync(It.IsAny<Account>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Once);
        }
    }
}
