using Banking.Application.Accounts.Queries.GetAll;
using Banking.Application.Core;
using MediatR;

namespace Banking.Application.Accounts.Queries.GetAll
{
    public record GetAllQuery() : IRequest<Result<GetAllResult>>;
}
