using Banking.API.Accounts.Queries.GetByAccountNumber;
using Banking.Application.Accounts.Commands.CreateAccount;
using Banking.Application.Accounts.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Banking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController(IMediator mediator) :ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken).ConfigureAwait(false);

            if(result.IsSuccess) 
                return Created();

            return BadRequest(new { error = result.Error?.Message ?? "An error occurred during login." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetAllQuery(), cancellationToken).ConfigureAwait(false);

            if (result.IsSuccess)
            {
                if (result.Value!.Accounts.Any())
                    return Ok(result.Value);
                else
                    return NoContent();
            }
            
            return BadRequest(new { error = result.Error?.Message ?? "An error occurred during getting all accounts." });
        }

        [HttpGet("{accountNumber}")]
        public async Task<IActionResult> GetByAccountNumber(string accountNumber, CancellationToken cancellationToken)
        {
            var query = new GetByAccountNumberQuery(accountNumber);
            var result = await mediator.Send(query, cancellationToken).ConfigureAwait(false);

            if (result.IsSuccess)
            {
                if(result.Value!.Account == null)
                    return NotFound();
                else
                    return Ok(result.Value);
            }

            return BadRequest(new { error = result.Error?.Message ??
                $"An error occurred during getting account by accountNumber: {accountNumber}." });
        }
    }
}
