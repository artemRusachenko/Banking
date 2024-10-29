using Banking.Application.Accounts.Queries.GetByAccountNumber;
using Banking.Application.Core;
using Banking.Domain.Accounts;
using MediatR;

namespace Banking.API.Accounts.Queries.GetByAccountNumber
{
    public class GetByAccountNumberHandler(IAccountRepository accountRepository) : IRequestHandler<GetByAccountNumberQuery, Result<GetByAccountNumberResult>>
    {
        public async Task<Result<GetByAccountNumberResult>> Handle(GetByAccountNumberQuery? request, CancellationToken cancellationToken)
        {
            if (request == null)
                return ResultBuilder.Failure<GetByAccountNumberResult>(new ArgumentNullException(nameof(request)));

            if (String.IsNullOrWhiteSpace(request.AccountNumber))
                return ResultBuilder.Failure<GetByAccountNumberResult>(
                    new ArgumentNullException(request.AccountNumber, "Account number can't be null or empty"));

            var account = await accountRepository.GetByAccountNumberAsync(request.AccountNumber).ConfigureAwait(false);

            var result = new GetByAccountNumberResult(account);
            return ResultBuilder.Success<GetByAccountNumberResult>(result);
        }
    }
}
