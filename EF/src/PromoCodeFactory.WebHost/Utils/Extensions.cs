using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Utils
{
    public static class Extensions
    {
        public static List<CustomerShortResponse> ToCustomerShortResponseList(this IEnumerable<Customer> customers)
        {
            List<CustomerShortResponse> csr = new();
            foreach (var customer in customers)
            {
                csr.Add(customer.ToCustomerShortResponse());
            }

            return csr;
        }

        public static CustomerShortResponse ToCustomerShortResponse(this Customer customer) => 
            new CustomerShortResponse 
            { 
                Id = customer.Id, 
                FirstName = customer.FirstName, 
                LastName = customer.LastName, 
                Email = customer.Email 
            };

        public static Customer ToCustomer(this CreateOrEditCustomerRequest request, Guid? id = null)
        {
            var customerId = id ?? Guid.NewGuid();
            var customer = new Customer
            {
                Id = customerId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                CustomersPreferences = request.PreferenceIds.Count > 0 ? new List<CustomerPreference>() : null
            };

            if (customer.CustomersPreferences is not null)
            {
                foreach (var guid in request.PreferenceIds)
                {
                    customer.CustomersPreferences.Add(new CustomerPreference { CustomerId = customerId, PreferenceId = guid });
                }
            }

            return customer;
        }
    }
}
