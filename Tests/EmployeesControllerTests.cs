using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiEmployee.Data;
using WebApiEmployee.Model;
using WebApiEmployee.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace Tests
{
    public  class EmployeesControllerTests
    {
        private readonly EmployeeContext _context;

        public EmployeesControllerTests()
        {
            var options = new DbContextOptionsBuilder<EmployeeContext>()
                .UseSqlServer("name=ConnectionStrings:EmployeeDb")
                .Options;

            _context = new EmployeeContext(options);
        }

        [Fact]
        public async Task CreateEmployee_ReturnsCreatedResult()
        {
            var controller = new EmployeeController(_context, new HttpClientFactoryMock());
            var employee = new Employee { Name = "John Doe", Salary = 50000, StartDate = DateTime.Now };

            var result = await controller.PostEmployee(employee);

            Assert.IsType<CreatedAtActionResult>(result);
        }
        [Fact]
        public async Task GetEmployee_ReturnsNotFound_WhenEmployeeDoesNotExist()
        {
            var controller = new EmployeeController(_context, new HttpClientFactoryMock());

            var result = await controller.GetEmployee(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetEmployee_ReturnsOkResult_WhenEmployeeExists()
        {
            var employee = new Employee { Name = "Jane Doe", Salary = 60000, StartDate = DateTime.Now };
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var controller = new EmployeeController(_context, new HttpClientFactoryMock());
            var result = await controller.GetEmployee(employee.Id);

            Assert.IsType<OkObjectResult>(result);
        }


        private class HttpClientFactoryMock : IHttpClientFactory
        {
            public HttpClient CreateClient(string name = null)
            {
                return new HttpClient(new FakeHttpMessageHandler());
            }
        }

    }//
}
