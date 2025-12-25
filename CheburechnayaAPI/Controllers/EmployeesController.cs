using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CheburechnayaAPI.Data;
using CheburechnayaAPI.Models;
using CheburechnayaAPI.Models.DTOs;

namespace CheburechnayaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public EmployeesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            var employees = await _context.Employees
                .Include(e => e.Position)
                .Include(e => e.Orders)
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FullName = e.FullName,
                    PositionId = e.PositionId,
                    PositionTitle = e.Position.Title,
                    Salary = e.Position.Salary,
                    HireDate = e.HireDate,
                    PhoneNumber = e.PhoneNumber,
                    TotalOrders = e.Orders.Count
                })
                .OrderBy(e => e.FullName)
                .ToListAsync();

            return Ok(employees);
        }

        // GET: api/employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Position)
                .Include(e => e.Orders)
                .Where(e => e.Id == id)
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FullName = e.FullName,
                    PositionId = e.PositionId,
                    PositionTitle = e.Position.Title,
                    Salary = e.Position.Salary,
                    HireDate = e.HireDate,
                    PhoneNumber = e.PhoneNumber,
                    TotalOrders = e.Orders.Count
                })
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                return NotFound(new { message = $"Сотрудник с ID {id} не найден" });
            }

            return employee;
        }

        // POST: api/employees
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(EmployeeCreateDto employeeDto)
        {
            var employee = new Employee
            {
                FullName = employeeDto.FullName,
                PositionId = employeeDto.PositionId,
                HireDate = employeeDto.HireDate,
                PhoneNumber = employeeDto.PhoneNumber
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmployee", new { id = employee.Id }, employee);
        }

        // PUT: api/employees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, EmployeeUpdateDto employeeDto)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            employee.FullName = employeeDto.FullName;
            employee.PositionId = employeeDto.PositionId;
            employee.PhoneNumber = employeeDto.PhoneNumber;

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
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

        // DELETE: api/employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/employees/by-position/2
        [HttpGet("by-position/{positionId}")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployeesByPosition(int positionId)
        {
            var employees = await _context.Employees
                .Include(e => e.Position)
                .Include(e => e.Orders)
                .Where(e => e.PositionId == positionId)
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FullName = e.FullName,
                    PositionId = e.PositionId,
                    PositionTitle = e.Position.Title,
                    Salary = e.Position.Salary,
                    HireDate = e.HireDate,
                    PhoneNumber = e.PhoneNumber,
                    TotalOrders = e.Orders.Count
                })
                .ToListAsync();

            return Ok(employees);
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}