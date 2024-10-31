using Banking.Application.Transfers.Command;
using Banking.Domain.Accounts;
using Banking.Domain.Data;
using Banking.Domain.Transactions;
using Banking.Domain.Transfers;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Data;
#pragma warning disable CA1707

namespace Banking.UnitTests.Application.Transfers
{
    public class TransferCommandHandlerTests
    {
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<ITransferRepository> _transferRepositoryMock;
        private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly TransferCommandHandler _handler;

        public TransferCommandHandlerTests()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _transferRepositoryMock = new Mock<ITransferRepository>();
            _transactionRepositoryMock = new Mock<ITransactionRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new TransferCommandHandler(
                _accountRepositoryMock.Object,
                _transferRepositoryMock.Object,
                _transactionRepositoryMock.Object,
                _unitOfWorkMock.Object);
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
            var command = new TransferCommand("TestFrom", "TestTo", -10, null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ArgumentException>(result.Error);
            Assert.Equal("Amount must be greater than zero.", result.Error.Message);
        }

        [Fact]
        public async Task Should_Return_Failure_When_Empty_AccountNumbers()
        {
            // Arrange
            var command = new TransferCommand("", "", 10, null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ArgumentException>(result.Error);
            Assert.Equal("Account numbers can't be null or empty.", result.Error.Message);
        }

        [Fact]
        public async Task Should_Return_Failure_When_From_Account_Not_Found()
        {
            // Arrange
            var command = new TransferCommand("12345", "67890", 100, null);

            _accountRepositoryMock
                .Setup(repo => repo.GetByAccountNumberAsync(command.FromAccountNumber))
                .ReturnsAsync(null as Account);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ArgumentException>(result.Error);
            Assert.Equal($"Account with {command.FromAccountNumber} number isn't found", result.Error.Message);
        }

        [Fact]
        public async Task Should_Return_Failure_When_Insufficient_Funds()
        {
            var account = new Account("Test") { Balance = 50 };
            var command = new TransferCommand(account.AccountNumber, "67890", 100, null);

            _accountRepositoryMock.Setup(repo => repo.GetByAccountNumberAsync(It.IsAny<string>()))
                           .ReturnsAsync(account);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ArgumentException>(result.Error);
            Assert.Equal("Insufficient funds in the account", result.Error.Message);
        }

        [Fact]
        public async Task Should_Return_Failure_When_To_Account_Not_Found()
        {
            // Arrange
            var accountFrom = new Account("Test");

            var command = new TransferCommand(accountFrom.AccountNumber, "67890", 100, null);

            _accountRepositoryMock
                .Setup(repo => repo.GetByAccountNumberAsync(command.FromAccountNumber))
                .ReturnsAsync(accountFrom);

            _accountRepositoryMock
                .Setup(repo => repo.GetByAccountNumberAsync(command.ToAccountNumber))
                .ReturnsAsync(null as Account);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ArgumentException>(result.Error);
            Assert.Equal($"Account with {command.ToAccountNumber} number isn't found", result.Error.Message);
        }

        [Fact]
        public async Task Should_Return_Failure_When_Transfer_Throws_ArgumentNullException()
        {
            // Arrange
            var accountFrom = new Account("Test");
            var accountTo = new Account("Test2");
            var request = new TransferCommand(accountFrom.AccountNumber, accountTo.AccountNumber, 100, null);

            _accountRepositoryMock
                .Setup(repo => repo.GetByAccountNumberAsync(accountFrom.AccountNumber))
                .ReturnsAsync(accountFrom);
            _accountRepositoryMock
                .Setup(repo => repo.GetByAccountNumberAsync(accountTo.AccountNumber))
                .ReturnsAsync(accountTo);

            _accountRepositoryMock
                .Setup(repo => repo.Transfer(accountFrom, accountTo, request.Amount))
                .ThrowsAsync(new ArgumentNullException());

            _transactionRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Transaction>()))
                .Returns(Task.CompletedTask);

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
            var accountFrom = new Account("Test");
            var accountTo = new Account("Test2");
            var request = new TransferCommand(accountFrom.AccountNumber, accountTo.AccountNumber, 100, null);

            _accountRepositoryMock
                .Setup(repo => repo.GetByAccountNumberAsync(accountFrom.AccountNumber))
                .ReturnsAsync(accountFrom);
            _accountRepositoryMock
                .Setup(repo => repo.GetByAccountNumberAsync(accountTo.AccountNumber))
                .ReturnsAsync(accountTo);
            _accountRepositoryMock
                .Setup(repo => repo.Transfer(accountFrom, accountTo, request.Amount))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DbUpdateException());

            // Act
            var result = await _handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ArgumentException>(result.Error);
            Assert.Equal("An error occurred while transfering", result.Error.Message);
        }

        [Fact]
        public async Task Should_Return_Failure_Any_Other_Exception_Occurs()
        {
            // Arrange
            var accountFrom = new Account("Test");
            var accountTo = new Account("Test2");
            var request = new TransferCommand(accountFrom.AccountNumber, accountTo.AccountNumber, 100, null);

            _accountRepositoryMock
                .Setup(repo => repo.GetByAccountNumberAsync(accountFrom.AccountNumber))
                .ReturnsAsync(accountFrom);
            _accountRepositoryMock
                .Setup(repo => repo.GetByAccountNumberAsync(accountTo.AccountNumber))
                .ReturnsAsync(accountTo);
            _accountRepositoryMock
                .Setup(repo => repo.Transfer(accountFrom, accountTo, request.Amount))
                .Returns(Task.CompletedTask);

            _transactionRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Transaction>()))
                .ThrowsAsync(new InvalidOperationException("Some unexpected operation error"));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.IsType<ArgumentException>(result.Error);
            Assert.Equal("An unexpected error occurred during the transfer operation", result.Error.Message);
        }
    }
}
