using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Count4U.Service.Core.Server.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestApi.Server.Data;
using RestApi.Shared;

namespace RestApi.Server.Controllers
{
	//[ApiController]
	//[Produces("application/json")]
	public class Test//Controller : Controller
	{
		private readonly CustomerContext _context;

		public Test(CustomerContext context)
		{
			_context = context;
		}


		// GET: api/Customer
		//[HttpGet("test/[controller]/[action]")]
		//public IEnumerable<Customer> GetCustomers()
		//{
		//	return _context.Customers;
		//}

		// GET: api/Customer/5
		//[HttpGet("test/[controller]/[action]/{id}")]
		//public async Task<IActionResult> GetCustomer([FromRoute] int id)
		//{
		//	if (!ModelState.IsValid)
		//	{
		//		return BadRequest(ModelState);
		//	}

		//	var customer = await _context.Customers.SingleOrDefaultAsync(m => m.ID == id);

		//	if (customer == null)
		//	{
		//		return NotFound();
		//	}

		//	return Ok(customer);
		//}

		//// PUT: api/Customer/5
		//[HttpPut("{id}")]
		//public async Task<IActionResult> PutCustomer([FromRoute] int id, [FromBody] Customer customer)
		//{
		//	if (!ModelState.IsValid)
		//	{
		//		return BadRequest(ModelState);
		//	}

		//	if (id != customer.ID)
		//	{
		//		return BadRequest();
		//	}

		//	_context.Entry(customer).State = EntityState.Modified;

		//	try
		//	{
		//		await _context.SaveChangesAsync();
		//	}
		//	catch (DbUpdateConcurrencyException)
		//	{
		//		if (!CustomerExists(id))
		//		{
		//			return NotFound();
		//		}
		//		else
		//		{
		//			throw;
		//		}
		//	}

		//	return NoContent();
		//}

		//// POST: api/Customer
		//[HttpPost]
		//public async Task<IActionResult> PostCustomer([FromBody] Customer customer)
		//{
		//	if (!ModelState.IsValid)
		//	{
		//		return BadRequest(ModelState);
		//	}

		//	_context.Customers.Add(customer);
		//	await _context.SaveChangesAsync();

		//	return CreatedAtAction("GetCustomer", new { id = customer.ID }, customer);
		//}

		//// DELETE: api/Customer/5
		//[HttpDelete("{id}")]
		//public async Task<IActionResult> DeleteCustomer([FromRoute] int id)
		//{
		//	if (!ModelState.IsValid)
		//	{
		//		return BadRequest(ModelState);
		//	}

		//	var customer = await _context.Customers.SingleOrDefaultAsync(m => m.ID == id);
		//	if (customer == null)
		//	{
		//		return NotFound();
		//	}

		//	_context.Customers.Remove(customer);
		//	await _context.SaveChangesAsync();

		//	return Ok(customer);
		//}

		//[NonAction]
		//private bool CustomerExists(int id)
		//{
		//	return _context.Customers.Any(e => e.ID == id);
		//}
	}
}
