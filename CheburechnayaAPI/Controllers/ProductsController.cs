using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CheburechnayaAPI.Data;
using CheburechnayaAPI.Models;
using CheburechnayaAPI.Models.DTOs;

namespace CheburechnayaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public ProductsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _context.Products
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Category = p.Category,
                    Price = p.Price,
                    CostPrice = p.CostPrice,
                    Profit = p.CostPrice.HasValue ? p.Price - p.CostPrice.Value : null,
                    ProfitMarginPercent = p.CostPrice.HasValue && p.CostPrice > 0 ?
                        ((p.Price - p.CostPrice.Value) / p.CostPrice.Value * 100) : null,
                    TotalSold = p.OrderItems.Sum(oi => oi.Quantity),
                    TotalRevenue = p.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice)
                })
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            return Ok(products);
        }

        // GET: api/products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _context.Products
                .Where(p => p.Id == id)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Category = p.Category,
                    Price = p.Price,
                    CostPrice = p.CostPrice,
                    Profit = p.CostPrice.HasValue ? p.Price - p.CostPrice.Value : null,
                    ProfitMarginPercent = p.CostPrice.HasValue && p.CostPrice > 0 ?
                        ((p.Price - p.CostPrice.Value) / p.CostPrice.Value * 100) : null,
                    TotalSold = p.OrderItems.Sum(oi => oi.Quantity),
                    TotalRevenue = p.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice)
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // GET: api/products/category/выпечка
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(string category)
        {
            var products = await _context.Products
                .Where(p => p.Category.ToLower() == category.ToLower())
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Category = p.Category,
                    Price = p.Price,
                    CostPrice = p.CostPrice,
                    Profit = p.CostPrice.HasValue ? p.Price - p.CostPrice.Value : null,
                    ProfitMarginPercent = p.CostPrice.HasValue && p.CostPrice > 0 ?
                        ((p.Price - p.CostPrice.Value) / p.CostPrice.Value * 100) : null,
                    TotalSold = p.OrderItems.Sum(oi => oi.Quantity),
                    TotalRevenue = p.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice)
                })
                .ToListAsync();

            return Ok(products);
        }

        // GET: api/products/popular
        [HttpGet("popular")]
        public async Task<ActionResult<IEnumerable<PopularProductDto>>> GetPopularProducts([FromQuery] int limit = 5)
        {
            var popularProducts = await _context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => oi.Order.OrderDate >= DateTime.Now.AddDays(-30))
                .GroupBy(oi => oi.Product)
                .Select(g => new PopularProductDto
                {
                    ProductId = g.Key.Id,
                    ProductName = g.Key.ProductName,
                    Category = g.Key.Category,
                    TotalSales = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Quantity * oi.UnitPrice),
                    IsPopular = g.Sum(oi => oi.Quantity) >= 10
                })
                .OrderByDescending(p => p.TotalSales)
                .Take(limit)
                .ToListAsync();

            return Ok(popularProducts);
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(ProductCreateDto productDto)
        {
            var product = new Product
            {
                ProductName = productDto.ProductName,
                Category = productDto.Category,
                Price = productDto.Price,
                CostPrice = productDto.CostPrice
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // PUT: api/products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductCreateDto productDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            product.ProductName = productDto.ProductName;
            product.Category = productDto.Category;
            product.Price = productDto.Price;
            product.CostPrice = productDto.CostPrice;

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // DELETE: api/products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // ѕровер€ем, нет ли заказов с этим продуктом
            var hasOrders = await _context.OrderItems.AnyAsync(oi => oi.ProductId == id);
            if (hasOrders)
            {
                return BadRequest(new { message = "Ќельз€ удалить продукт, который есть в заказах" });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}