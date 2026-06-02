using DiaryApp.Models;
using DiaryApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DiaryApp.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerService _service;


        public CustomersController(CustomerService service)  
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAllCustomers()
        {
            return Ok(_service.GetAll());   
        }

        [HttpGet("{id:int}")]
        public IActionResult GetCustomerById(int id)
        {
            var customer = _service.GetById(id);

            if (customer == null)
                return NotFound();   

            return Ok(customer);     
        }

        [HttpGet("search")]
        public IActionResult GetCustomerByName([FromQuery] string name)
        {
            var customer = _service.SearchByName(name);
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpGet("premium")]
        public IActionResult GetPremiumCustomers()
        {
            return Ok(_service.GetPremium());
        }

        [HttpPost]
        public IActionResult CreateCustomer([FromBody] Customer customer)
        {
            _service.Add(customer);
            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customer);
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateCustomer(int id, [FromBody] Customer customer)
        {
            var updated = _service.Update(id, customer);
            if (updated == null) return NotFound();   
            return Ok(updated);                        
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteCustomer(int id)
        {
            var deleted = _service.Delete(id);
            if (!deleted) return NotFound();         
            return NoContent();                        
        }
    }
}

