using Faces.WebMvc.Models;
using Faces.WebMvc.ViewModels;
using MassTransit;
using Messaging.InterfacesConstants.Commands;
using Messaging.InterfacesConstants.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Faces.WebMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBusControl _busControl; 

        public HomeController(ILogger<HomeController> logger,
                                IBusControl busControl)
        {
            _logger = logger;
            _busControl = busControl;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RegisterOrder()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterOrder(OrderViewModel model)
        {
            MemoryStream memory = new MemoryStream();
            using (var uploadedFile = model.File.OpenReadStream())
            {
                await uploadedFile.CopyToAsync(memory);
            }

            model.ImageData = memory.ToArray();
            model.PictureUrl = model.File.FileName;
            model.OrderId = Guid.NewGuid();
            var sendToUri = new Uri($"{RabbitMqMassTransitConstants.RabbitMqUri}/" +$"{RabbitMqMassTransitConstants.RegisterOrderCommandQueue}");

            var endPoint = await _busControl.GetSendEndpoint(sendToUri);
            await endPoint.Send<IRegisterOrderCommand>(
               new
               {
                   model.OrderId,
                   model.UserEmail,
                   model.ImageData,
                   model.PictureUrl
               });
            ViewData["OrderId"] = model.OrderId;
            return View("Thanks");
        }

        [HttpGet]
        public IActionResult Thank()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
