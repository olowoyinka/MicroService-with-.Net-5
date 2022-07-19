using Microsoft.AspNetCore.Mvc;
using OrdersApi.Persistence;
using System;
using System.Threading.Tasks;

namespace OrdersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var data = await _orderRepository.GetOrdersAsync();

            return Ok(data);
        }

        [HttpGet]
        [Route("{orderId}", Name = "GetByOrderId")]
        public async Task<IActionResult> GetOrderById(string orderId)
        {
            var order = await _orderRepository.GetOrderAsync(Guid.Parse(orderId));

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }
    }
}