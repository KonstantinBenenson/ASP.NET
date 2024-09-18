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
                csr.Add(PopulateCustomerResponse(customer));
            }

            return csr;
        }

        private static CustomerShortResponse PopulateCustomerResponse(Customer customer) => 
            new CustomerShortResponse { Id = customer.Id, FirstName = customer.FirstName, LastName = customer.LastName, Email = customer.Email };
    }
}
