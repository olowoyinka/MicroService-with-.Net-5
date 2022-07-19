using Faces.WebMvc.ViewModels;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Faces.WebMvc.RestClients
{
    public interface IOrderManagementApi
    {
        [Get("/orders")]
        Task<List<OrderViewModel>> GetOrders();

        [Get("/orders/{orderId}")]
        Task<OrderViewModel> GetOrderById(Guid orderId);
    }
}