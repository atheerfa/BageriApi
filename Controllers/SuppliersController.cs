using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BageriApi.Data;
using BageriApi.Models;
using BageriApi.DTOs;

namespace BageriApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly BageriContext _context;

        public SuppliersController(BageriContext context)
        {
            _context = context;
        }

        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetSupplierProducts(int id)
        {
            var supplier = await _context.Suppliers
                .Include(s => s.SupplierRawMaterials)
                .ThenInclude(srm => srm.RawMaterial)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (supplier == null)
            {
                return NotFound("Leverantören finns inte");
            }

            var result = new
            {
                supplier.Name,
                supplier.ContactPerson,
                supplier.Email,
                Products = supplier.SupplierRawMaterials.Select(srm => new
                {
                    srm.RawMaterial.ArticleNumber,
                    srm.RawMaterial.Name,
                    srm.PricePerKg
                })
            };

            return Ok(result);
        }

        [HttpPost("{id}/products")]
        public async Task<IActionResult> AddProductToSupplier(int id, [FromBody] RawMaterialDto dto)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound("Leverantören finns inte");
            }

            var rawMaterial = await _context.RawMaterials
                .FirstOrDefaultAsync(r => r.ArticleNumber == dto.ArticleNumber);

            if (rawMaterial == null)
            {
                rawMaterial = new RawMaterial
                {
                    ArticleNumber = dto.ArticleNumber,
                    Name = dto.Name
                };
                _context.RawMaterials.Add(rawMaterial);
                await _context.SaveChangesAsync();
            }

            var existingLink = await _context.SupplierRawMaterials
                .FirstOrDefaultAsync(srm => srm.SupplierId == id && srm.RawMaterialId == rawMaterial.Id);

            if (existingLink != null)
            {
                return BadRequest("Produkten finns redan hos denna leverantör");
            }

            var link = new SupplierRawMaterial
            {
                SupplierId = id,
                RawMaterialId = rawMaterial.Id,
                PricePerKg = dto.PricePerKg
            };

            _context.SupplierRawMaterials.Add(link);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSupplierProducts), new { id = id }, dto);
        }

        [HttpPatch("{supplierId}/products/{rawMaterialId}")]
        public async Task<IActionResult> UpdateProductPrice(int supplierId, int rawMaterialId, [FromBody] PriceUpdateDto dto)
        {
            var link = await _context.SupplierRawMaterials
                .FirstOrDefaultAsync(srm => srm.SupplierId == supplierId && srm.RawMaterialId == rawMaterialId);

            if (link == null)
            {
                return NotFound("Kunde inte hitta produkten hos vald leverantör");
            }

            link.PricePerKg = dto.PricePerKg;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}