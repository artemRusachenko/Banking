using Banking.Application.Transfers.Command;
using Banking.Domain.Accounts;
using Banking.Domain.Transfers;
using Banking.Infrastructure.Repositories;
#pragma warning disable CA1707

namespace Banking.IntegrationTests.Transfers
{
    public class TransferTests : IntegrationTestBase
    {
        [Fact]
        public async Task Should_Transfer_Amount_Between_Accounts()
        {
            // Arrange
            var accountFrom = new Account("Sender Account");
            var accountTo = new Account("Receiver Account");
            await _accountRepository.AddAsync(accountFrom).ConfigureAwait(false);
            await _accountRepository.AddAsync(accountTo).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            var command = new TransferCommand(accountFrom.AccountNumber, accountTo.AccountNumber, 300, "Transfer Test");
            var handler = new TransferCommandHandler(_accountRepository,
                _transferRepository, _transactionRepository, _unitOfWork);

            // Act
            var result = await handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.True(result.IsSuccess);

            var updatedAccountFrom = await _accountRepository.GetByAccountNumberAsync(accountFrom.AccountNumber).ConfigureAwait(false);
            var updatedAccountTo = await _accountRepository.GetByAccountNumberAsync(accountTo.AccountNumber).ConfigureAwait(false);

            Assert.Equal(700, updatedAccountFrom!.Balance);
            Assert.Equal(1300, updatedAccountTo!.Balance);
        }

        [Fact]
        public async Task Should_Return_Failure_When_Insufficient_Funds()
        {
            // Arrange
            var fromAccount = new Account("From Account") { Balance = 50 };
            var toAccount = new Account("To Account") { Balance = 200 };
            await _accountRepository.AddAsync(fromAccount).ConfigureAwait(false);
            await _accountRepository.AddAsync(toAccount).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            var command = new TransferCommand(fromAccount.AccountNumber, toAccount.AccountNumber, 100, "Test Transfer");
            var handler = new TransferCommandHandler(_accountRepository, _transferRepository, _transactionRepository, _unitOfWork);

            // Act
            var result = await handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Insufficient funds in the account", result.Error!.Message);
        }

        [Fact]
        public async Task Should_Return_Failure_When_Account_Not_Found()
        {
            // Arrange
            var toAccount = new Account("To Account") { Balance = 200 };
            await _accountRepository.AddAsync(toAccount).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            var command = new TransferCommand("NonExistentAccount", toAccount.AccountNumber, 100, "Test Transfer");
            var handler = new TransferCommandHandler(_accountRepository, _transferRepository, _transactionRepository, _unitOfWork);

            // Act
            var result = await handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("number isn't found", result.Error!.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}
