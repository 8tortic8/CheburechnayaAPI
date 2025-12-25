using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CheburechnayaAPI.Data;
using CheburechnayaAPI.Models;
using CheburechnayaAPI.Models.DTOs;

namespace CheburechnayaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public PositionsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/positions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PositionDto>>> GetPositions()
        {
            var positions = await _context.Positions
                .Select(p => new PositionDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Salary = p.Salary,
                    EmployeeCount = p.Employees.Count
                })
                .OrderBy(p => p.Title)
                .ToListAsync();

            return Ok(positions);
        }

        // GET: api/positions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PositionDto>> GetPosition(int id)
        {
            var position = await _context.Positions
                .Where(p => p.Id == id)
                .Select(p => new PositionDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Salary = p.Salary,
                    EmployeeCount = p.Employees.Count
                })
                .FirstOrDefaultAsync();

            if (position == null)
            {
                return NotFound();
            }

            return position;
        }

        // POST: api/positions
        [HttpPost]
        public async Task<ActionResult<Position>> PostPosition(PositionCreateDto positionDto)
        {
            var position = new Position
            {
                Title = positionDto.Title,
                Salary = positionDto.Salary
            };

            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPosition", new { id = position.Id }, position);
        }

        // PUT: api/positions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPosition(int id, PositionCreateDto positionDto)
        {
            var position = await _context.Positions.FindAsync(id);
            if (position == null)
            {
                return NotFound();
            }

            position.Title = positionDto.Title;
            position.Salary = positionDto.Salary;

            _context.Entry(position).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PositionExists(id))
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

        // DELETE: api/positions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePosition(int id)
        {
            var position = await _context.Positions.FindAsync(id);
            if (position == null)
            {
                return NotFound();
            }

            // Проверяем, нет ли сотрудников на этой должности
            var hasEmployees = await _context.Employees.AnyAsync(e => e.PositionId == id);
            if (hasEmployees)
            {
                return BadRequest(new { message = "Нельзя удалить должность, на которой есть сотрудники" });
            }

            _context.Positions.Remove(position);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PositionExists(int id)
        {
            return _context.Positions.Any(e => e.Id == id);
        }
    }
}