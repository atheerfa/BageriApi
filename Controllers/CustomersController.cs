using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BageriApi.Data;
using BageriApi.Models;
using BageriApi.DTOs;

namespace BageriApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly BageriContext _context;

        public CustomersController(BageriContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerCreateDto dto)
        {
            var customer = new Customer
            {
                StoreName = dto.StoreName,
                Phone = dto.Phone,
                Email = dto.Email,
                ContactPerson = dto.ContactPerson,
                DeliveryAddress = dto.DeliveryAddress,
                InvoiceAddress = dto.InvoiceAddress
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return StatusCode(201);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _context.Customers
                .Select(c => new { c.Id, c.StoreName, c.ContactPerson })
                .ToListAsync();

            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Orders)
                .ThenInclude(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null) return NotFound();

            var result = new
            {
                customer.StoreName,
                customer.Phone,
                customer.Email,
                customer.ContactPerson,
                customer.DeliveryAddress,
                customer.InvoiceAddress,
                OrderHistory = customer.Orders.Select(o => new
                {
                    o.OrderNumber,
                    o.OrderDate,
                    Items = o.OrderItems.Select(oi => new
                    {
                        oi.Product.Name,
                        oi.Quantity,
                        oi.Price,
                        oi.TotalPrice
                    })
                })
            };

            return Ok(result);
        }

        [HttpPatch("{id}/contactperson")]
        public async Task<IActionResult> UpdateContactPerson(int id, [FromBody] string newContactPerson)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            customer.ContactPerson = newContactPerson;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetCustomersByProduct(int productId)
        {
            var customers = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .Where(o => o.OrderItems.Any(oi => oi.ProductId == productId))
                .Select(o => o.Customer.StoreName)
                .Distinct()
                .ToListAsync();

            return Ok(customers);
        }
    }
}