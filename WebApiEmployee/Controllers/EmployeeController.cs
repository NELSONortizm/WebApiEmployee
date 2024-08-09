using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.Json;
using WebApiEmployee.Data;
using WebApiEmployee.Model;

namespace WebApiEmployee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {

        private readonly EmployeeContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        //constructor
        public EmployeeController(EmployeeContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            if (string.IsNullOrEmpty(employee.Name) || employee.Name.Length > 100)
            {

                return BadRequest("Invalid name. ");
            }
            if(employee.Salary < 0 || employee.Salary > 999999.99M)
            {
                return BadRequest("Invalid Salary");
            }
            if (employee.StartDate > DateTime.Now)
            {
                return BadRequest("Start date cannot be in the future");
            }

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult> GetEmployee(int id, string currency = "USD")
        
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                {
                    return NotFound();
                }
                decimal convertedSalary = await ConvertCurrency(employee.Salary, currency);
                return Ok(new { employee.Name, Salary = convertedSalary, employee.StartDate });
            }
            catch (Exception ex)
            {
                // Manejo de la excepción
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }

        }//

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAllmployees()
        {
            var employees = await _context.Employees.ToListAsync();
            return Ok(employees);
        
        }

        public async Task<decimal> ConvertCurrency(decimal amount, string currency)
        { 
            
            var client = _httpClientFactory.CreateClient();           
            var response = await client.GetStringAsync($"https://v6.exchangerate-api.com/v6/YOUR_API_KEY/latest/USD");
            var exchangeRates = JsonSerializer.Deserialize<ExchangeRateResponse>(response);
            return exchangeRates.ExchangeRates[currency] * amount;

        }

       

    }//

    public class ExchangeRateResponse    { 
    
        public Dictionary<string, decimal> ExchangeRates { get; set; }
    }//
}
