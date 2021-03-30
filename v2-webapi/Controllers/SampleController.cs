using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace v2_webapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private readonly ILogger<SampleController> logger;
        private readonly  IHubContext<NotificationHub, INotificationHub> notificationHub;

        public const string StoreName = "statestore";

        public SampleController(ILogger<SampleController> logger, IHubContext<NotificationHub, INotificationHub> notificationHub)
        {
            this.logger = logger;
            this.notificationHub = notificationHub;
        }

        [HttpGet("{customer}")]
        public ActionResult<Customer> Get([FromState(StoreName)] StateEntry<Customer> customer)
        {
            if (customer.Value is null)
            {
                return this.NotFound();
            }

            return customer.Value;
        }

        [HttpPost("create")]
        public async Task<ActionResult<Customer>> Create(Customer customer, [FromServices] DaprClient daprClient)
        {
            logger.LogDebug("Enter new Customer");
            var state = await daprClient.GetStateEntryAsync<Customer>(StoreName, customer.Id);
            state.Value ??= customer;
            state.Value.Name = customer.Name;
            await state.SaveAsync();
            return state.Value;
        }

        [Topic("pubsub", "newOrder")]
        public async Task NewOrderArrived(OrderData order, [FromServices] DaprClient daprClient)
        {
            await notificationHub.Clients.All.Notification($"Order Arrived-> Id: {order.orderId}, Product: {order.productId}, Amount: {order.amount}");
        }
    }
}
