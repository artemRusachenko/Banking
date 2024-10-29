using Banking.Application.Accounts.Queries.GetAll;
using Banking.Application.Core;
using Banking.Domain.Accounts;
using MediatR;

namespace Banking.API.Accounts.Queries.GetAll
{
    public class GetAllQueryHandler(IAccountRepository accountRepository) : IRequestHandler<GetAllQuery, Result<GetAllResult>>
    {
        public async Task<Result<GetAllResult>> Handle(GetAllQuery? request, CancellationToken cancellationToken)
        {
            if (request == null)
                return ResultBuilder.Failure<GetAllResult>(new ArgumentNullException(nameof(request)));

            var accounts = await accountRepository.GetAllAsync().ConfigureAwait(false);

            var result = new GetAllResult(accounts);

            return ResultBuilder.Success(result);
        }
    }
}
