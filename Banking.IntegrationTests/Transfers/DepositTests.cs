using Banking.Application.Transactions.Commands.Deposit;
using Banking.Domain.Accounts;
#pragma warning disable CA1707

namespace Banking.IntegrationTests.Transfers
{
    public class DepositTests : IntegrationTestBase
    {
        [Fact]
        public async Task Should_Deposit_Amount_To_Account_When_Valid()
        {
            // Arrange
            var account = new Account("Test Account") { Balance = 500 };
            await _accountRepository.AddAsync(account).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            var command = new DepositCommand(account.AccountNumber, 200);
            var handler = new DepositCommandHandler(_accountRepository, _transactionRepository, _unitOfWork);

            // Act
            var result = await handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.True(result.IsSuccess);

            var updatedAccount = await _accountRepository.GetByAccountNumberAsync(account.AccountNumber).ConfigureAwait(false);
            Assert.Equal(700, updatedAccount!.Balance);
        }

        [Fact]
        public async Task Should_Return_Failure_When_Deposit_Amount_Is_Negative()
        {
            // Arrange
            var account = new Account("Test Account");
            await _accountRepository.AddAsync(account).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            var command = new DepositCommand(account.AccountNumber, -200);
            var handler = new DepositCommandHandler(_accountRepository, _transactionRepository, _unitOfWork);

            // Act
            var result = await handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Amount must be greater than zero.", result.Error!.Message);
        }

        [Fact]
        public async Task Should_Return_Failure_When_Account_Not_Found()
        {
            // Arrange
            var command = new DepositCommand("NonExistentAccountNumber", 200);
            var handler = new DepositCommandHandler(_accountRepository, _transactionRepository, _unitOfWork);

            // Act
            var result = await handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Account with this number isn't found", result.Error!.Message);
        }
    }
}
