using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.WebHost.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YamlDotNet.Core.Tokens;

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
        private readonly IExtendedRepository<Customer> _customerRepo;
        private readonly IExtendedRepository<Preference> _preferenceRepo;

        public CustomersController(IExtendedRepository<Customer> customerRepo, IExtendedRepository<Preference> preferenceRepo)
        {
            _customerRepo = customerRepo;
            _preferenceRepo = preferenceRepo;
        }

        /// <summary>
        /// Получить всех Клиентов.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<CustomerShortResponse>>> GetCustomers(CancellationToken token)
        {
            var customers = await _customerRepo.GetAllAsync(token);
            return Ok(customers.Count() > 0 ? customers.ToResponseList() : []);
        }

        /// <summary>
        /// Получить Клиента по id.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomer(Guid id, CancellationToken token)
        {
            var customer = await _customerRepo.GetByIdAsync(id, token);
            if (customer == null) { return NoContent(); }
            return Ok(customer.ToResponse());
        }

        /// <summary>
        /// Создать Клиента.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CreateOrEditCustomerRequest request, CancellationToken token)
        {
            var newCustomer = request.ToCustomer();

            foreach (var customerPreference in newCustomer.CustomersPreferences)
            {
                var preferenceName = await _preferenceRepo.GetByIdAsync(customerPreference.PreferenceId, token);
                customerPreference.Preference = new() { Name = preferenceName.Name };
            }

            await _customerRepo.CreateAsync(newCustomer, token);
            return CreatedAtAction(nameof(GetCustomer), new { id = newCustomer.Id }, newCustomer.ToResponse());
        }

        /// <summary>
        /// Редактировать Клиента.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomer(Guid id, CreateOrEditCustomerRequest request, CancellationToken token)
        {
            var updateCustomer = request.ToCustomer(id);
            await _customerRepo.UpdateAsync(id, updateCustomer, token);
            return NoContent();
        }

        /// <summary>
        /// Удалить Клиента.
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(Guid id, CancellationToken token)
        {
            await _customerRepo.DeleteByIdAsync(id, token);
            return NoContent();
        }
    }
}