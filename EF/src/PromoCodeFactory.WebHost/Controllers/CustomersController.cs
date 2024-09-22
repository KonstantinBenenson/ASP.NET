using Mapster;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]/[Action]")]
    public class CustomersController
        : ControllerBase
    {
        private readonly IRepository<Customer> _repo;

        public CustomersController(IRepository<Customer> repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<List<CustomerShortResponse>>> GetCustomers()
        {
            var customers = await _repo.GetAllAsync();
            return Ok(customers.Count() > 0 ? customers.ToCustomerShortResponseList() : default);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomer(Guid id)
        {
            var customer = await _repo.GetByIdAsync(id);
            if (customer == null) { return BadRequest($"No {nameof(Customer)} was found"); }
            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CreateOrEditCustomerRequest request)
        {
            var newCustomer = request.ToCustomer();
            await _repo.CreateAsync(newCustomer);
            var response = newCustomer.ToCustomerShortResponse();
            return CreatedAtAction(nameof(GetCustomer), new { id = newCustomer.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomer(Guid id, CreateOrEditCustomerRequest request)
        {
            var updateCustomer = request.Adapt<Customer>();
            //TODO: Обновить данные клиента вместе с его предпочтениями
            return Ok(updateCustomer);
        }

        [HttpDelete]
        public Task<IActionResult> DeleteCustomer(Guid id)
        {
            //TODO: Удаление клиента вместе с выданными ему промокодами
            throw new NotImplementedException();
        }
    }
}