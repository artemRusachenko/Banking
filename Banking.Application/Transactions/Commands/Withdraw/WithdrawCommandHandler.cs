#pragma warning disable CA1031
using Banking.Application.Core;
using Banking.Application.Transactions.Commands.Deposit;
using Banking.Domain.Accounts;
using Banking.Domain.Data;
using Banking.Domain.Transactions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Banking.Application.Transactions.Commands.Withdraw
{
    public class WithdrawCommandHandler(IAccountRepository accountRepository,
        ITransactionRepository transactionRepository, IUnitOfWork unitOfWork) : IRequestHandler<WithdrawCommand, Result<WithdrawResult>>
    {
        public async Task<Result<WithdrawResult>> Handle(WithdrawCommand? request, CancellationToken cancellationToken)
        {
            if (request == null)
                return ResultBuilder.Failure<WithdrawResult>(new ArgumentNullException(nameof(request)));

            if (request.Amount < 0)
                return ResultBuilder.Failure<WithdrawResult>(new ArgumentException("Amount must be greater than zero."));

            if (string.IsNullOrWhiteSpace(request.AccountNumber))
                return ResultBuilder.Failure<WithdrawResult>(new ArgumentException("Account number can't be null or empty."));

            var account = await accountRepository.GetByAccountNumberAsync(request.AccountNumber).ConfigureAwait(false);

            if (account == null)
                return ResultBuilder.Failure<WithdrawResult>(new ArgumentException("Account with this number isn't found"));

            if(account.Balance < request.Amount)
                return ResultBuilder.Failure<WithdrawResult>(new ArgumentException("Insufficient funds in the account"));

            var transaction = new Transaction(account.Id, TransactionType.Withdrawal, -request.Amount, null);

            try
            {
                await accountRepository.Withdraw(account, request.Amount).ConfigureAwait(false);

                await transactionRepository.AddAsync(transaction).ConfigureAwait(false);

                await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (ArgumentNullException ex)
            {
                return ResultBuilder.Failure<WithdrawResult>(ex);
            }
            catch (DbUpdateException dbEx)
            {
                return ResultBuilder.Failure<WithdrawResult>(new ArgumentException("An error occurred while withdrawing", dbEx));
            }
            catch (Exception ex)
            {
                return ResultBuilder.Failure<WithdrawResult>(new ArgumentException("An unexpected error occurred during the withdraw operation", ex));
            }

            return ResultBuilder.Success(new WithdrawResult(transaction.Id.ToString()));
        }
    }
}
