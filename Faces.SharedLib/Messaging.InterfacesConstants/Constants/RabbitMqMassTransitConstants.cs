namespace Messaging.InterfacesConstants.Constants
{
    public class RabbitMqMassTransitConstants
    {
        public const string RabbitMqUri = "rabbitmq://rabbitmq:5672/";

        public const string Username = "guest";

        public const string Password = "guest";

        public const string RegisterOrderCommandQueue = "register.order.command";

        public const string NotificationServiceQueue = "notification.service.queue";

        public const string OrderDispatchedServiceQueue = "order.dispatch.service.queue";
    }
}