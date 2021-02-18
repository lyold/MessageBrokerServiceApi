using MessageBrokerServiceApi.Common.Helper.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MessageBrokerServiceApi.Common.Middlewares
{
    public class BrokerReceiver : BackgroundService
    {
        private readonly IRabbitMqHelper _rabbitMQ;
        
        public static event Action<Message> MessageReceived;

        public BrokerReceiver(IRabbitMqHelper rabbitMQ)
        {
            _rabbitMQ = rabbitMQ;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        _rabbitMQ.Receive((msg) =>
                        {
                            if (MessageReceived != null)
                                MessageReceived.Invoke(msg);

                        }, stoppingToken);
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc.Message);
                    }
                }, stoppingToken);
        }
    }

    public class Message
    {
        public string Method { get; set; }
        public string Body { get; set; }
    }
}
