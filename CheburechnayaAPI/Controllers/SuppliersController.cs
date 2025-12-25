using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CheburechnayaAPI.Data;
using CheburechnayaAPI.Models;
using CheburechnayaAPI.Models.DTOs;

namespace CheburechnayaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public SuppliersController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/suppliers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierDto>>> GetSuppliers()
        {
            var suppliers = await _context.Suppliers
                .Select(s => new SupplierDto
                {
                    Id = s.Id,
                    CompanyName = s.CompanyName,
                    ContactPerson = s.ContactPerson,
                    Phone = s.Phone,
                    TotalDeliveries = s.Deliveries.Count,
                    TotalDeliveredAmount = s.Deliveries.Sum(d => d.TotalAmount)
                })
                .OrderBy(s => s.CompanyName)
                .ToListAsync();

            return Ok(suppliers);
        }

        // GET: api/suppliers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierDto>> GetSupplier(int id)
        {
            var supplier = await _context.Suppliers
                .Where(s => s.Id == id)
                .Select(s => new SupplierDto
                {
                    Id = s.Id,
                    CompanyName = s.CompanyName,
                    ContactPerson = s.ContactPerson,
                    Phone = s.Phone,
                    TotalDeliveries = s.Deliveries.Count,
                    TotalDeliveredAmount = s.Deliveries.Sum(d => d.TotalAmount)
                })
                .FirstOrDefaultAsync();

            if (supplier == null)
            {
                return NotFound();
            }

            return supplier;
        }

        // POST: api/suppliers
        [HttpPost]
        public async Task<ActionResult<Supplier>> PostSupplier(SupplierCreateDto supplierDto)
        {
            var supplier = new Supplier
            {
                CompanyName = supplierDto.CompanyName,
                ContactPerson = supplierDto.ContactPerson,
                Phone = supplierDto.Phone
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSupplier", new { id = supplier.Id }, supplier);
        }

        // PUT: api/suppliers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSupplier(int id, SupplierCreateDto supplierDto)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            supplier.CompanyName = supplierDto.CompanyName;
            supplier.ContactPerson = supplierDto.ContactPerson;
            supplier.Phone = supplierDto.Phone;

            _context.Entry(supplier).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/suppliers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            // Проверяем, нет ли поставок от этого поставщика
            var hasDeliveries = await _context.Deliveries.AnyAsync(d => d.SupplierId == id);
            if (hasDeliveries)
            {
                return BadRequest(new { message = "Нельзя удалить поставщика, у которого есть поставки" });
            }

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SupplierExists(int id)
        {
            return _context.Suppliers.Any(e => e.Id == id);
        }
    }
}