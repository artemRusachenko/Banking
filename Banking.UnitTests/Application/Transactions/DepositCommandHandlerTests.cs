using Banking.Application.Transactions.Commands.Deposit;
using Banking.Domain.Accounts;
using Banking.Domain.Data;
using Banking.Domain.Transactions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Data;
#pragma warning disable CA1707

namespace Banking.UnitTests.Application.Transactions
{
    public class DepositCommandHandlerTests
    {
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly DepositCommandHandler _handler;

        public DepositCommandHandlerTests()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _transactionRepositoryMock = new Mock<ITransactionRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new DepositCommandHandler(
                _accountRepositoryMock.Object,
                _transactionRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Should_Deposit_Amount_Successfully()
        {
            // Arrange
            var account = new Account("Test");
            var command = new DepositCommand(account.AccountNumber, 100);

            _accountRepositoryMock
                .Setup(repo => repo.GetByAccountNumberAsync(account.AccountNumber))
                .ReturnsAsync(account);

            _transactionRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Transaction>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.BeginTransaction())
                .Returns(new Mock<IDbTransaction>().Object);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.True(result.IsSuccess);
            _accountRepositoryMock.Verify(repo => repo.Deposit(account, command.Amount), Times.Once);
            _transactionRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Transaction>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Should_Return_Failure_When_Command_Is_Null()
        {
            // Act
            var result = await _handler.Handle(null, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ArgumentNullException>(result.Error);
        }

        [Fact]
        public async Task Should_Return_Failure_When_Ammount_Is_Negative()
        {
            // Arrange
            var request = new DepositCommand("1234567890", -100);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ArgumentException>(result.Error);
            Assert.Equal("Amount must be greater than zero.", result.Error.Message);
        }

        [Fact]
        public async Task Should_Return_Failure_When_Account_Number_Is_Empty()
        {
            // Arrange
            var request = new DepositCommand("", 100);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ArgumentException>(result.Error);
            Assert.Equal("Account number can't be null or empty.", result.Error.Message);
        }

        [Fact]
        public async Task Should_Return_Failure_When_Account_Is_Not_Found()
        {
            // Arrange
            var request = new DepositCommand("1234567890", 100);

            _accountRepositoryMock
                .Setup(repo => repo.GetByAccountNumberAsync(request.AccountNumber))
                .ReturnsAsync(null as Account);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ArgumentException>(result.Error);
            Assert.Equal("Account with this number isn't found", result.Error.Message);
        }

        [Fact]
        public async Task Should_Return_Failure_When_Withdraw_Throws_ArgumentNullException()
        {
            // Arrange
            var account = new Account("Test");
            var request = new DepositCommand(account.AccountNumber, 100);

            _accountRepositoryMock
                .Setup(repo => repo.GetByAccountNumberAsync(request.AccountNumber))
                .ReturnsAsync(account);

            _accountRepositoryMock
                .Setup(repo => repo.Deposit(account, request.Amount))
                .ThrowsAsync(new ArgumentNullException());

            _transactionRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Transaction>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.BeginTransaction())
                .Returns(new Mock<IDbTransaction>().Object);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ArgumentNullException>(result.Error);
        }

        [Fact]
        public async Task Should_Return_Failure_When_AddAsync_Throws_DbUpdateException()
        {
            // Arrange
            var account = new Account("Test");
            var request = new DepositCommand(account.AccountNumber, 100);

            _accountRepositoryMock
                .Setup(repo => repo.GetByAccountNumberAsync(request.AccountNumber))
                .ReturnsAsync(account);

            _accountRepositoryMock
                .Setup(repo => repo.Deposit(account, request.Amount))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.BeginTransaction())
                .Returns(new Mock<IDbTransaction>().Object);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DbUpdateException());

            // Act
            var result = await _handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ArgumentException>(result.Error);
            Assert.Equal("An error occurred while depositing", result.Error.Message);
        }

        [Fact]
        public async Task Should_Return_Failure_Any_Other_Exception_Occurs()
        {
            // Arrange
            var account = new Account("Test");
            var request = new DepositCommand(account.AccountNumber, 100);

            _accountRepositoryMock
                .Setup(repo => repo.GetByAccountNumberAsync(request.AccountNumber))
                .ReturnsAsync(account);

            _accountRepositoryMock
                .Setup(repo => repo.Deposit(account, request.Amount))
                .Returns(Task.CompletedTask);

            _transactionRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Transaction>()))
                .ThrowsAsync(new InvalidOperationException("Some unexpected operation error"));

            _unitOfWorkMock
                .Setup(uow => uow.BeginTransaction())
                .Returns(new Mock<IDbTransaction>().Object);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ArgumentException>(result.Error);
            Assert.Equal("An unexpected error occurred during the deposit operation", result.Error.Message);
        }

    }
}
