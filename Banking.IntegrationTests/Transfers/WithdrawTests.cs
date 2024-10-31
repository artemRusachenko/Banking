using Banking.Application.Transactions.Commands.Withdraw;
using Banking.Domain.Accounts;
#pragma warning disable CA1707

namespace Banking.IntegrationTests.Transfers
{
    public class WithdrawTests : IntegrationTestBase
    {
        [Fact]
        public async Task Should_Withdraw_Amount_From_Account_When_Balance_Is_Sufficient()
        {
            // Arrange
            var account = new Account("Test Account") { Balance = 1000 };
            await _accountRepository.AddAsync(account).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            var command = new WithdrawCommand(account.AccountNumber, 300);
            var handler = new WithdrawCommandHandler(_accountRepository, _transactionRepository, _unitOfWork);

            // Act
            var result = await handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.True(result.IsSuccess);

            var updatedAccount = await _accountRepository.GetByAccountNumberAsync(account.AccountNumber).ConfigureAwait(false);
            Assert.Equal(700, updatedAccount!.Balance);
        }

        [Fact]
        public async Task Should_Return_Failure_When_Insufficient_Funds()
        {
            // Arrange
            var account = new Account("Test Account") { Balance = 100 };
            await _accountRepository.AddAsync(account).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            var command = new WithdrawCommand(account.AccountNumber, 200);
            var handler = new WithdrawCommandHandler(_accountRepository, _transactionRepository, _unitOfWork);

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
            var command = new WithdrawCommand("NonExistentAccountNumber", 100);
            var handler = new WithdrawCommandHandler(_accountRepository, _transactionRepository, _unitOfWork);

            // Act
            var result = await handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Account with this number isn't found", result.Error!.Message);
        }
    }
}
