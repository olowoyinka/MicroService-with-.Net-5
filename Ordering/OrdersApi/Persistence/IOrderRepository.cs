﻿using OrdersApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrdersApi.Persistence
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderAsync(Guid id);
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task RegisterOrder(Order order);

        Order GetOrder(Guid id);
        void UpdateOrder(Order order);
    }
}