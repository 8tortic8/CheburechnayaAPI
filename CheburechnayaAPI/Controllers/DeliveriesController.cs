using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CheburechnayaAPI.Data;
using CheburechnayaAPI.Models;
using CheburechnayaAPI.Models.DTOs;

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

        // GET: api/deliveries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeliveryDto>>> GetDeliveries()
        {
            var deliveries = await _context.Deliveries
                .Include(d => d.Supplier)
                .Include(d => d.Employee)
                .Include(d => d.DeliveryItems)
                .Select(d => new DeliveryDto
                {
                    Id = d.Id,
                    SupplierId = d.SupplierId,
                    SupplierName = d.Supplier.CompanyName,
                    DeliveryDate = d.DeliveryDate,
                    EmployeeId = d.EmployeeId,
                    EmployeeName = d.Employee.FullName,
                    DriverName = d.DriverName,
                    DriverPhone = d.DriverPhone,
                    VehicleNumber = d.VehicleNumber,
                    TotalAmount = d.TotalAmount,
                    Status = d.Status,
                    DeliveryItems = d.DeliveryItems.Select(di => new DeliveryItemDto
                    {
                        Id = di.Id,
                        ProductId = di.ProductId,
                        Quantity = di.Quantity,
                        UnitPrice = di.UnitPrice,
                        Subtotal = di.Quantity * di.UnitPrice,
                        ExpiryDate = di.ExpiryDate,
                        BatchNumber = di.BatchNumber
                    }).ToList()
                })
                .OrderByDescending(d => d.DeliveryDate)
                .ToListAsync();

            return Ok(deliveries);
        }

        // GET: api/deliveries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DeliveryDto>> GetDelivery(int id)
        {
            var delivery = await _context.Deliveries
                .Include(d => d.Supplier)
                .Include(d => d.Employee)
                .Include(d => d.DeliveryItems)
                .Where(d => d.Id == id)
                .Select(d => new DeliveryDto
                {
                    Id = d.Id,
                    SupplierId = d.SupplierId,
                    SupplierName = d.Supplier.CompanyName,
                    DeliveryDate = d.DeliveryDate,
                    EmployeeId = d.EmployeeId,
                    EmployeeName = d.Employee.FullName,
                    DriverName = d.DriverName,
                    DriverPhone = d.DriverPhone,
                    VehicleNumber = d.VehicleNumber,
                    TotalAmount = d.TotalAmount,
                    Status = d.Status,
                    DeliveryItems = d.DeliveryItems.Select(di => new DeliveryItemDto
                    {
                        Id = di.Id,
                        ProductId = di.ProductId,
                        Quantity = di.Quantity,
                        UnitPrice = di.UnitPrice,
                        Subtotal = di.Quantity * di.UnitPrice,
                        ExpiryDate = di.ExpiryDate,
                        BatchNumber = di.BatchNumber
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (delivery == null)
            {
                return NotFound();
            }

            return delivery;
        }

        // GET: api/deliveries/status/pending
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<DeliveryDto>>> GetDeliveriesByStatus(string status)
        {
            var deliveries = await _context.Deliveries
                .Include(d => d.Supplier)
                .Include(d => d.Employee)
                .Where(d => d.Status.ToLower() == status.ToLower())
                .Select(d => new DeliveryDto
                {
                    Id = d.Id,
                    SupplierId = d.SupplierId,
                    SupplierName = d.Supplier.CompanyName,
                    DeliveryDate = d.DeliveryDate,
                    EmployeeId = d.EmployeeId,
                    EmployeeName = d.Employee.FullName,
                    DriverName = d.DriverName,
                    DriverPhone = d.DriverPhone,
                    VehicleNumber = d.VehicleNumber,
                    TotalAmount = d.TotalAmount,
                    Status = d.Status
                })
                .OrderByDescending(d => d.DeliveryDate)
                .ToListAsync();

            return Ok(deliveries);
        }

        // POST: api/deliveries
        [HttpPost]
        public async Task<ActionResult<Delivery>> PostDelivery(DeliveryCreateDto deliveryDto)
        {
            // Создаем поставку
            var delivery = new Delivery
            {
                SupplierId = deliveryDto.SupplierId,
                EmployeeId = deliveryDto.EmployeeId,
                DriverName = deliveryDto.DriverName,
                DriverPhone = deliveryDto.DriverPhone,
                VehicleNumber = deliveryDto.VehicleNumber,
                Status = deliveryDto.Status ?? "Pending",
                DeliveryDate = DateTime.Now,
                TotalAmount = 0 // Будет вычислено триггером
            };

            _context.Deliveries.Add(delivery);
            await _context.SaveChangesAsync(); // Сохраняем, чтобы получить ID поставки

            // Добавляем элементы поставки
            foreach (var itemDto in deliveryDto.DeliveryItems)
            {
                var product = await _context.Products.FindAsync(itemDto.ProductId);
                if (product == null)
                {
                    return BadRequest(new { message = $"Продукт с ID {itemDto.ProductId} не найден" });
                }

                var deliveryItem = new DeliveryItem
                {
                    DeliveryId = delivery.Id,
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    ExpiryDate = itemDto.ExpiryDate,
                    BatchNumber = itemDto.BatchNumber
                };

                _context.DeliveryItems.Add(deliveryItem);
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDelivery", new { id = delivery.Id }, delivery);
        }

        // PUT: api/deliveries/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateDeliveryStatus(int id, DeliveryStatusUpdateDto statusDto)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery == null)
            {
                return NotFound();
            }

            delivery.Status = statusDto.Status;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/deliveries/5
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