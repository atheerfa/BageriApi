using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BageriApi.Data;
using BageriApi.Models;
using BageriApi.DTOs;

namespace BageriApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly BageriContext _context;

        public ProductsController(BageriContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                PricePerPiece = dto.PricePerPiece,
                Weight = dto.Weight,
                AmountPerPackage = dto.AmountPerPackage,
                BestBeforeDate = dto.BestBeforeDate,
                ManufacturingDate = dto.ManufacturingDate
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return StatusCode(201);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPatch("{id}/price")]
        public async Task<IActionResult> UpdatePrice(int id, [FromBody] decimal newPrice)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.PricePerPiece = newPrice;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}