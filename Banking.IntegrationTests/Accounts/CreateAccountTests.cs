#pragma warning disable CA1707
using Banking.Application.Accounts.Commands.CreateAccount;
using Banking.Domain.Accounts;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Banking.IntegrationTests.Accounts
{
    public class CreateAccountTests : IntegrationTestBase
    {
        [Fact]
        public async Task Should_Add_Account_To_Database()
        {
            // Arrange
            var command = new CreateAccountCommand ("Test Test");
            var handler = new CreateAccountCommandHandler(_accountRepository, _unitOfWork);

            // Act
            var result = await handler.Handle(command, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.True(result.IsSuccess);
            var accounts = await _context.Accounts.ToListAsync().ConfigureAwait(false);
            Assert.Single(accounts);
            Assert.Equal("Test Test", accounts.First().HolderName);
        }
    }
}
