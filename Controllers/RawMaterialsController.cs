using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BageriApi.Data;

namespace BageriApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RawMaterialsController : ControllerBase
    {
        private readonly BageriContext _context;

        public RawMaterialsController(BageriContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var materials = await _context.RawMaterials
                .Include(r => r.SupplierRawMaterials)
                .ThenInclude(srm => srm.Supplier)
                .Select(r => new
                {
                    r.ArticleNumber,
                    r.Name,
                    Suppliers = r.SupplierRawMaterials.Select(srm => new
                    {
                        SupplierName = srm.Supplier.Name,
                        srm.PricePerKg
                    })
                })
                .ToListAsync();

            return Ok(materials);
        }

        [HttpGet("search/{name}")]
        public async Task<IActionResult> SearchRawMaterial(string name)
        {
            var material = await _context.RawMaterials
                .Include(r => r.SupplierRawMaterials)
                .ThenInclude(srm => srm.Supplier)
                .Where(r => r.Name.ToLower().Contains(name.ToLower()))
                .Select(r => new
                {
                    r.ArticleNumber,
                    r.Name,
                    Suppliers = r.SupplierRawMaterials.Select(srm => new
                    {
                        SupplierName = srm.Supplier.Name,
                        srm.PricePerKg
                    })
                })
                .ToListAsync();

            if (!material.Any())
            {
                return NotFound("Kunde inte hitta råvaran");
            }

            return Ok(material);
        }
    }
}