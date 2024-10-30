#pragma warning disable CA1707
using Banking.API.Accounts.Queries.GetByAccountNumber;
using Banking.Domain.Accounts;

namespace Banking.IntegrationTests.Accounts
{
    public class GetByAccountNumberTests : IntegrationTestBase
    {
        [Fact]
        public async Task Should_Return_Account_By_Acc_Num()
        {
            //Arrange
            var account = new Account("Test Account");
            await _context.Accounts.AddAsync(account).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            var handler = new GetByAccountNumberHandler(_accountRepository);

            //Act
            var result = await handler.Handle(new GetByAccountNumberQuery(account.AccountNumber), CancellationToken.None).ConfigureAwait(false);

            //Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Test Account", result.Value!.Account!.HolderName);
        }
    }
}
