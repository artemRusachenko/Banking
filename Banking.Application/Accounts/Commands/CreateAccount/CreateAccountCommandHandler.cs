using Banking.Application.Core;
using Banking.Domain.Accounts;
using Banking.Domain.Data;
using MediatR;

namespace Banking.Application.Accounts.Commands.CreateAccount
{
    public class CreateAccountCommandHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateAccountCommand, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(CreateAccountCommand? request, CancellationToken cancellationToken)
        {
            if(request == null)
                return ResultBuilder.Failure<Unit>(new ArgumentNullException(nameof(request)));

            if(String.IsNullOrWhiteSpace(request.HolderName))
                return ResultBuilder.Failure<Unit>(new ArgumentException("Holder name can't be null or empty"));

            var newAccount = new Account(request.HolderName);
            await accountRepository.AddAsync(newAccount).ConfigureAwait(false);

            await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return ResultBuilder.Success(Unit.Value);
        }
    }
}
