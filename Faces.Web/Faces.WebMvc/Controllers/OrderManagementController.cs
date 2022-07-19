using Faces.WebMvc.RestClients;
using Faces.WebMvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Faces.WebMvc.Controllers
{
    [Route("OrderManagement")]
    public class OrderManagementController : Controller
    {
        private readonly IOrderManagementApi _ordersApi;

        public OrderManagementController(IOrderManagementApi ordersApi)
        {
            _ordersApi = ordersApi;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var Orders = await _ordersApi.GetOrders();

            foreach (var m in Orders)
            {
                string imageBase64Data = Convert.ToBase64String(m.ImageData);
                m.ImageString = string.Format("data:image/png;base64,{0}", imageBase64Data);
            }

            return View(Orders);
        }

        [Route("/Details/{orderId}")]
        public async Task<IActionResult> Details(string orderId)
        {
            var order = await _ordersApi.GetOrderById(Guid.Parse(orderId));

            order.ImageString = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(order.ImageData));

            foreach (var detail in order.OrderDetails)
            {
                detail.ImageString = string.Format("data:image/png;base64,{0}",
                Convert.ToBase64String(detail.FaceData));
            }

            return View(order);
        }
    }
}
