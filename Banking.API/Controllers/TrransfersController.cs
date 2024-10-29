using Banking.Application.Transactions.Commands.Deposit;
using Banking.Application.Transfers.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Banking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrransfersController(IMediator mediator) : ControllerBase
    {
        [HttpPost("transfer")]
        public async Task<IActionResult> Deposit([FromBody] TransferCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken).ConfigureAwait(false);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { error = result.Error?.Message ?? "An error occurred during transfering." });
        }
    }
}
