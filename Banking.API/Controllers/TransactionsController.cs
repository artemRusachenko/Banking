using Banking.Application.Transactions.Commands.Deposit;
using Banking.Application.Transactions.Commands.Withdraw;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Banking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController(IMediator mediator) : ControllerBase
    {
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken).ConfigureAwait(false);

            if(result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { error = result.Error?.Message ?? "An error occurred during depositing." });
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken).ConfigureAwait(false);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { error = result.Error?.Message ?? "An error occurred during withdrawing." });
        }
    }
}
