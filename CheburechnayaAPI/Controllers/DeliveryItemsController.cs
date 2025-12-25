using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CheburechnayaAPI.Data;
using CheburechnayaAPI.Models;
using CheburechnayaAPI.Models.DTOs;

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

        // GET: api/deliveryitems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeliveryItemDto>>> GetDeliveryItems()
        {
            var deliveryItems = await _context.DeliveryItems
                .Include(di => di.Delivery)
                .Include(di => di.Product)
                .Select(di => new DeliveryItemDto
                {
                    Id = di.Id,
                    ProductId = di.ProductId,
                    ProductName = di.Product.ProductName,
                    ProductCategory = di.Product.Category,
                    Quantity = di.Quantity,
                    UnitPrice = di.UnitPrice,
                    Subtotal = di.Quantity * di.UnitPrice,
                    ExpiryDate = di.ExpiryDate,
                    BatchNumber = di.BatchNumber
                })
                .ToListAsync();

            return Ok(deliveryItems);
        }

        // GET: api/deliveryitems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DeliveryItemDto>> GetDeliveryItem(int id)
        {
            var deliveryItem = await _context.DeliveryItems
                .Include(di => di.Product)
                .Where(di => di.Id == id)
                .Select(di => new DeliveryItemDto
                {
                    Id = di.Id,
                    ProductId = di.ProductId,
                    ProductName = di.Product.ProductName,
                    ProductCategory = di.Product.Category,
                    Quantity = di.Quantity,
                    UnitPrice = di.UnitPrice,
                    Subtotal = di.Quantity * di.UnitPrice,
                    ExpiryDate = di.ExpiryDate,
                    BatchNumber = di.BatchNumber
                })
                .FirstOrDefaultAsync();

            if (deliveryItem == null)
            {
                return NotFound();
            }

            return deliveryItem;
        }

        // GET: api/deliveryitems/delivery/5
        [HttpGet("delivery/{deliveryId}")]
        public async Task<ActionResult<IEnumerable<DeliveryItemDto>>> GetDeliveryItemsByDelivery(int deliveryId)
        {
            var deliveryItems = await _context.DeliveryItems
                .Include(di => di.Product)
                .Where(di => di.DeliveryId == deliveryId)
                .Select(di => new DeliveryItemDto
                {
                    Id = di.Id,
                    ProductId = di.ProductId,
                    ProductName = di.Product.ProductName,
                    ProductCategory = di.Product.Category,
                    Quantity = di.Quantity,
                    UnitPrice = di.UnitPrice,
                    Subtotal = di.Quantity * di.UnitPrice,
                    ExpiryDate = di.ExpiryDate,
                    BatchNumber = di.BatchNumber
                })
                .ToListAsync();

            return Ok(deliveryItems);
        }

        // PUT: api/deliveryitems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeliveryItem(int id, DeliveryItemCreateDto deliveryItemDto)
        {
            var deliveryItem = await _context.DeliveryItems.FindAsync(id);
            if (deliveryItem == null)
            {
                return NotFound();
            }

            deliveryItem.ProductId = deliveryItemDto.ProductId;
            deliveryItem.Quantity = deliveryItemDto.Quantity;
            deliveryItem.UnitPrice = deliveryItemDto.UnitPrice;
            deliveryItem.ExpiryDate = deliveryItemDto.ExpiryDate;
            deliveryItem.BatchNumber = deliveryItemDto.BatchNumber;

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

        // DELETE: api/deliveryitems/5
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