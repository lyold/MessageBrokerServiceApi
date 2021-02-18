using MessageBrokerServiceApi.Common.Middlewares;
using System;
using System.Threading;

namespace MessageBrokerServiceApi.Common.Helper.Interfaces
{
    public interface IRabbitMqHelper
    {
        void Receive(Action<Message> received, CancellationToken token);
        void RouteMessage(string body);
    }
}
