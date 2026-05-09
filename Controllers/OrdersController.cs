using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BageriApi.Data;
using BageriApi.Models;
using BageriApi.DTOs;

namespace BageriApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly BageriContext _context;

        public OrdersController(BageriContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto dto)
        {
            var customer = await _context.Customers.FindAsync(dto.CustomerId);
            if (customer == null) return BadRequest();

            var order = new Order
            {
                OrderNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                OrderDate = DateTime.Now,
                CustomerId = dto.CustomerId
            };

            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        Price = product.PricePerPiece
                    });
                }
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return StatusCode(201);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchOrders([FromQuery] string? orderNumber, [FromQuery] DateTime? orderDate)
        {
            var query = _context.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(orderNumber))
            {
                query = query.Where(o => o.OrderNumber == orderNumber);
            }

            if (orderDate.HasValue)
            {
                query = query.Where(o => o.OrderDate.Date == orderDate.Value.Date);
            }

            var orders = await query.Select(o => new { o.OrderNumber, o.OrderDate }).ToListAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            var result = new
            {
                order.OrderNumber,
                order.OrderDate,
                CustomerName = order.Customer.StoreName,
                CustomerPhone = order.Customer.Phone,
                Products = order.OrderItems.Select(oi => new
                {
                    oi.Product.Name,
                    oi.Quantity,
                    oi.Price,
                    oi.TotalPrice
                })
            };

            return Ok(result);
        }
    }
}