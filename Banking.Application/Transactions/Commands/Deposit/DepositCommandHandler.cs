#pragma warning disable CA1031
using Banking.Application.Core;
using Banking.Domain.Accounts;
using Banking.Domain.Data;
using Banking.Domain.Transactions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Banking.Application.Transactions.Commands.Deposit
{
    public class DepositCommandHandler(IAccountRepository accountRepository,
        ITransactionRepository transactionRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<DepositCommand, Result<DepositResult>>
    {
        public async Task<Result<DepositResult>> Handle(DepositCommand? request, CancellationToken cancellationToken)
        {
            if(request == null) 
                return ResultBuilder.Failure<DepositResult>(new ArgumentNullException(nameof(request)));

            if (request.Amount < 0)
                return ResultBuilder.Failure<DepositResult>(new ArgumentException("Amount must be greater than zero."));

            if (string.IsNullOrWhiteSpace(request.AccountNumber))
                return ResultBuilder.Failure<DepositResult>(new ArgumentException("Account number can't be null or empty."));

            var account = await accountRepository.GetByAccountNumberAsync(request.AccountNumber).ConfigureAwait(false);

            if(account == null)
                return ResultBuilder.Failure<DepositResult>(new ArgumentException("Account with this number isn't found"));

            var transaction = new Transaction(account.Id, TransactionType.Deposit, request.Amount, null);

            using var trans = unitOfWork.BeginTransaction();

            try
            {
                await accountRepository.Deposit(account, request.Amount).ConfigureAwait(false);

                await transactionRepository.AddAsync(transaction).ConfigureAwait(false);

                await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                trans.Commit();
            }
            catch (ArgumentNullException ex)
            {
                trans.Rollback();
                return ResultBuilder.Failure<DepositResult>(ex);
            }
            catch (DbUpdateException dbEx)
            {
                trans.Rollback();
                return ResultBuilder.Failure<DepositResult>(new ArgumentException("An error occurred while depositing", dbEx));
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return ResultBuilder.Failure<DepositResult>(new ArgumentException("An unexpected error occurred during the deposit operation", ex));
            }

            return ResultBuilder.Success(new DepositResult(transaction.Id.ToString()));
        }
    }
}