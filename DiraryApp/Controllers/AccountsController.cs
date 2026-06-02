using Microsoft.AspNetCore.Mvc;
using DiaryApp.Models;
using DiaryApp.Services;

namespace DiaryApp.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly AccountService _service;

        public AccountsController(AccountService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAllAccounts()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("{id:int}")]
        public IActionResult GetAccountById(int id)
        {
            var account = _service.GetById(id);
            if (account == null) return NotFound();
            return Ok(account);
        }

        [HttpGet("search")]
        public IActionResult GetAccountByName([FromQuery] string name)
        {
            return Ok(_service.SearchByName(name));
        }

        [HttpPost]
        public IActionResult CreateAccount([FromBody] Account account)
        {
            var created = _service.Add(account);
            if (created == null)
                return BadRequest($"No customer found with id {account.CustomerId}.");

            return CreatedAtAction(nameof(GetAccountById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateAccount(int id, [FromBody] Account account)
        {
            var updated = _service.Update(id, account);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteAccount(int id)
        {
            var deleted = _service.Delete(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}