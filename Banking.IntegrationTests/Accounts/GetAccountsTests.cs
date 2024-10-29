#pragma warning disable CA1707
using Banking.API.Accounts.Queries.GetAll;
using Banking.Application.Accounts.Queries.GetAll;
using Banking.Domain.Accounts;

namespace Banking.IntegrationTests.Accounts
{
    public class GetAccountsTests : IntegrationTestBase
    {
        [Fact]
        public async Task Should_Return_Accounts_From_Database()
        {
            //Arrange
            await _context.Accounts.AddAsync(new Account("Test1")).ConfigureAwait(false);
            await _context.Accounts.AddAsync(new Account("Test2")).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            var handler = new GetAllQueryHandler(_accountRepository);

            //Act
            var result = await handler.Handle(new GetAllQuery(), CancellationToken.None).ConfigureAwait(false);

            //Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value!.Accounts.Count());
        }
    }
}
