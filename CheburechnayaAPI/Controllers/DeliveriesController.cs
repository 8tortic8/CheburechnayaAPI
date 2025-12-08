using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CheburechnayaAPI.Data;
using CheburechnayaAPI.Models;

namespace CheburechnayaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveriesController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public DeliveriesController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Delivery>>> GetDeliveries()
        {
            return await _context.Deliveries
                .Include(d => d.Supplier)
                .Include(d => d.Employee)
                .Include(d => d.DeliveryItems)
                    .ThenInclude(di => di.Product)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Delivery>> GetDelivery(int id)
        {
            var delivery = await _context.Deliveries
                .Include(d => d.Supplier)
                .Include(d => d.Employee)
                .Include(d => d.DeliveryItems)
                    .ThenInclude(di => di.Product)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (delivery == null)
            {
                return NotFound();
            }

            return delivery;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDelivery(int id, Delivery delivery)
        {
            if (id != delivery.Id)
            {
                return BadRequest();
            }

            _context.Entry(delivery).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeliveryExists(id))
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

        [HttpPost]
        public async Task<ActionResult<Delivery>> PostDelivery(Delivery delivery)
        {
            if (delivery.DeliveryDate == DateTime.MinValue)
            {
                delivery.DeliveryDate = DateTime.Now;
            }

            _context.Deliveries.Add(delivery);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDelivery", new { id = delivery.Id }, delivery);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDelivery(int id)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery == null)
            {
                return NotFound();
            }

            _context.Deliveries.Remove(delivery);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DeliveryExists(int id)
        {
            return _context.Deliveries.Any(e => e.Id == id);
        }
    }
}