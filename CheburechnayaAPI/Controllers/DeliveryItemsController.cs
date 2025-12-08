using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CheburechnayaAPI.Data;
using CheburechnayaAPI.Models;

namespace CheburechnayaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryItemsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public DeliveryItemsController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeliveryItem>>> GetDeliveryItems()
        {
            return await _context.DeliveryItems
                .Include(di => di.Delivery)
                .Include(di => di.Product)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeliveryItem>> GetDeliveryItem(int id)
        {
            var deliveryItem = await _context.DeliveryItems
                .Include(di => di.Delivery)
                .Include(di => di.Product)
                .FirstOrDefaultAsync(di => di.Id == id);

            if (deliveryItem == null)
            {
                return NotFound();
            }

            return deliveryItem;
        }

        [HttpGet("delivery/{deliveryId}")]
        public async Task<ActionResult<IEnumerable<DeliveryItem>>> GetDeliveryItemsByDelivery(int deliveryId)
        {
            return await _context.DeliveryItems
                .Include(di => di.Product)
                .Where(di => di.DeliveryId == deliveryId)
                .ToListAsync();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeliveryItem(int id, DeliveryItem deliveryItem)
        {
            if (id != deliveryItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(deliveryItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeliveryItemExists(id))
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
        public async Task<ActionResult<DeliveryItem>> PostDeliveryItem(DeliveryItem deliveryItem)
        {
            _context.DeliveryItems.Add(deliveryItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDeliveryItem", new { id = deliveryItem.Id }, deliveryItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeliveryItem(int id)
        {
            var deliveryItem = await _context.DeliveryItems.FindAsync(id);
            if (deliveryItem == null)
            {
                return NotFound();
            }

            _context.DeliveryItems.Remove(deliveryItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DeliveryItemExists(int id)
        {
            return _context.DeliveryItems.Any(e => e.Id == id);
        }
    }
}