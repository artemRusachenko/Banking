#pragma warning disable CA1031
using Banking.Application.Core;
using Banking.Domain.Accounts;
using Banking.Domain.Data;
using Banking.Domain.Transactions;
using Banking.Domain.Transfers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Banking.Application.Transfers.Command
{
    public class TransferCommandHandler(IAccountRepository accountRepository,
        ITransferRepository transferRepository, ITransactionRepository transactionRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<TransferCommand, Result<TransferResult>>
    {
        public async Task<Result<TransferResult>> Handle(TransferCommand? request, CancellationToken cancellationToken)
        {
            if (request == null)
                return ResultBuilder.Failure<TransferResult>(new ArgumentNullException(nameof(request)));

            if (request.Amount < 0)
                return ResultBuilder.Failure<TransferResult>(new ArgumentException("Amount must be greater than zero."));

            if (string.IsNullOrWhiteSpace(request.FromAccountNumber) || string.IsNullOrWhiteSpace(request.ToAccountNumber))
                return ResultBuilder.Failure<TransferResult>(new ArgumentException("Account numbers can't be null or empty."));

            var accountFrom = await accountRepository.GetByAccountNumberAsync(request.FromAccountNumber).ConfigureAwait(false);

            if (accountFrom == null)
                return ResultBuilder.Failure<TransferResult>(
                    new ArgumentException($"Account with {request.FromAccountNumber} number isn't found"));

            if (accountFrom.Balance < request.Amount)
                return ResultBuilder.Failure<TransferResult>(new ArgumentException("Insufficient funds in the account"));

            var accountTo = await accountRepository.GetByAccountNumberAsync(request.ToAccountNumber).ConfigureAwait(false);

            if (accountTo == null)
                return ResultBuilder.Failure<TransferResult>(
                    new ArgumentException($"Account with {request.ToAccountNumber} number isn't found"));

            var transfer = new Transfer(accountFrom.Id, accountTo.Id, request.Amount);
            var sendTransaction = new Transaction(accountFrom.Id, TransactionType.Transfer, -request.Amount, request.description);
            var recieveTransaction = new Transaction(accountFrom.Id, TransactionType.Transfer, request.Amount, request.description);
            
            try
            {
                await accountRepository.Transfer(accountFrom, accountTo, request.Amount).ConfigureAwait(false);

                await transactionRepository.AddAsync(sendTransaction).ConfigureAwait(false);

                await transactionRepository.AddAsync(recieveTransaction).ConfigureAwait(false);

                await transferRepository.AddAsync(transfer).ConfigureAwait(false);

                await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (ArgumentNullException ex)
            {
                return ResultBuilder.Failure<TransferResult>(ex);
            }
            catch (DbUpdateException dbEx)
            {
                return ResultBuilder.Failure<TransferResult>(new ArgumentException("An error occurred while transfering", dbEx));
            }
            catch (Exception ex)
            {
                return ResultBuilder.Failure<TransferResult>(new ArgumentException("An unexpected error occurred during the transfer operation", ex));
            }

            return ResultBuilder.Success(new TransferResult(transfer.Id.ToString()));
        }
    }
}
