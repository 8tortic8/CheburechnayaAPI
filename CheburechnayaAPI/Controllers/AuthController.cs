using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CheburechnayaAPI.Data;
using CheburechnayaAPI.Models;
using CheburechnayaAPI.Models.DTOs;

namespace CheburechnayaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public AuthController(DatabaseContext context)
        {
            _context = context;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginDto loginDto)
        {
            if (loginDto == null)
                return BadRequest(new { message = "Неверные данные для входа" });

            // Проверяем, существует ли сотрудник с таким ID
            var employee = await _context.Employees
                .Include(e => e.Position)
                .FirstOrDefaultAsync(e =>
                    e.Id.ToString() == loginDto.EmployeeId &&
                    e.PhoneNumber == loginDto.Password);

            if (employee == null)
            {
                return Unauthorized(new { message = "Неверные учетные данные" });
            }

            // Определяем роль на основе должности
            var role = employee.Position?.Title?.ToLower() switch
            {
                "менеджер" or "manager" => "manager",
                "повар" or "cook" => "cook",
                "официант" or "waiter" => "waiter",
                "кассир" or "cashier" => "cashier",
                "закупщик" or "buyer" => "buyer",
                _ => "employee"
            };

            var response = new LoginResponseDto
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Username = loginDto.Username,
                EmployeeId = employee.Id,
                Position = employee.Position?.Title ?? "Сотрудник",
                Salary = employee.Position?.Salary ?? 0,
                Role = role,
                HireDate = employee.HireDate,
                PhoneNumber = employee.PhoneNumber,
                IsAuthenticated = true
            };

            return Ok(response);
        }

        // GET: api/auth/check
        [HttpGet("check")]
        public IActionResult CheckAuth()
        {
            return Ok(new { message = "API работает", timestamp = DateTime.Now });
        }
    }
}