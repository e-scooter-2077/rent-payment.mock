using Azure.Messaging.ServiceBus;
using EasyDesk.CleanArchitecture.Application.Events.ExternalEvents;
using EasyDesk.CleanArchitecture.Application.Json;
using EasyDesk.CleanArchitecture.Infrastructure.Events.ServiceBus;
using EasyDesk.CleanArchitecture.Infrastructure.Json;
using EasyDesk.CleanArchitecture.Infrastructure.Time;
using EasyDesk.Tools.PrimitiveTypes.DateAndTime;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace EScooter.RentPayment
{
    public static class AuthorizeRent
    {
        public record RentRequested(Guid RentId);

        public record RentPaymentAuthorized(Guid RentId, Timestamp StartTime) : ExternalEvent;

        [Function("AuthorizeRent")]
        public static void Run(
            [ServiceBusTrigger("%TopicName%", "%SubscriptionName%", Connection = "ServiceBusConnectionString")] string messageContent,
            FunctionContext context)
        {
            var logger = context.GetLogger("AuthorizeRent");
            var serializer = CreateSerializer();
            var publisher = CreatePublisher(serializer);

            var rentId = serializer.Deserialize<RentRequested>(messageContent).RentId;
            publisher.Publish(new RentPaymentAuthorized(rentId, Timestamp.Now));

            logger.LogInformation($"Handled event of type {typeof(RentRequested).Name} for rent '{rentId}'");
        }

        private static IExternalEventPublisher CreatePublisher(IJsonSerializer jsonSerializer)
        {
            var connectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionString");
            var topicName = Environment.GetEnvironmentVariable("TopicName");
            var descriptor = AzureServiceBusSenderDescriptor.Topic(topicName);
            var client = new ServiceBusClient(connectionString);
            var serviceBusPublisher = new AzureServiceBusPublisher(client, descriptor);

            return new ExternalEventPublisher(
                serviceBusPublisher,
                new MachineDateTime(),
                jsonSerializer);
        }

        private static IJsonSerializer CreateSerializer()
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };
            return new NewtonsoftJsonSerializer(serializerSettings);
        }
    }
}
