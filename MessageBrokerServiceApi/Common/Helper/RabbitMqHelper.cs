using MessageBrokerServiceApi.Common.Helper.Interfaces;
using MessageBrokerServiceApi.Common.Middlewares;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace MessageBrokerServiceApi.Common.Helper
{
    public class RabbitMqHelper : IRabbitMqHelper
    {
        private readonly string _exchangeName;
        private readonly string _exchangeNameFanout;
        private readonly string _serviceName;

        private static IConnection _connection;
        private static IModel _channel;
        
        public RabbitMqHelper()
        {
            _exchangeName = "minhaExchange";
            _exchangeNameFanout = string.Concat("minhaExchange", "_", ExchangeType.Fanout);
            _serviceName = GetServiceName();

            if (_connection == null || _channel == null)
            {
                var factory = new ConnectionFactory()
                {
                    HostName = "rabbitmq.test",
                    UserName = "admin",
                    Password = "admin"
                };

                _connection = factory.CreateConnection(_serviceName);
                _channel = _connection.CreateModel();
            }
        }

        private string GetServiceName()
        {
            var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name;
            var serviceName = assemblyName?.Split(".").Last();
            return serviceName;
        }

        public void RouteMessage(string body)
        {
            try
            {
                _channel.ExchangeDeclare(_exchangeNameFanout, ExchangeType.Fanout);
                var logId = Guid.NewGuid();
                var props = _channel.CreateBasicProperties();
                props.Headers = new Dictionary<string, object>
                {
                    { "Method", string.Empty },
                    { "MessageId", logId.ToString() }
                };

                _channel.BasicPublish(_exchangeNameFanout, string.Empty, props, SerializeToByteArray(body));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private byte[] SerializeToByteArray(object body)
        {
            if (body == null)
                return null;

            var param = body.GetType();
            if (param.IsPrimitive || param.IsValueType || (param == typeof(string) || (param == typeof(Guid))))
                return Encoding.UTF8.GetBytes(body.ToString());

            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
        }

        public void Receive(Action<Message> received, CancellationToken token)
        {
            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic);
            _channel.ExchangeDeclare(_exchangeNameFanout, ExchangeType.Fanout);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (e, ea) =>
            {
                var messageId = Guid.Empty;
                try
                {
                    var method = string.Empty;

                    if (ea.BasicProperties.Headers != null && ea.BasicProperties.Headers.TryGetValue("MessageId", out var headerValue))
                        Guid.TryParse(Encoding.UTF8.GetString((byte[])headerValue), out messageId);

                    if (ea.BasicProperties.Headers != null && ea.BasicProperties.Headers.TryGetValue("Method", out headerValue))
                        method = Encoding.UTF8.GetString((byte[])headerValue);
 
                    var body = ea.Body.ToArray();
                    var bodyMessage = Encoding.UTF8.GetString(body);

                    received.Invoke(new Message
                    {
                        Body = bodyMessage,
                        Method = method
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            var fanoutQueue = _channel.QueueDeclare(string.Concat(_serviceName, "_", ExchangeType.Fanout, "_", Guid.NewGuid().ToString()), autoDelete: false).QueueName;
            var queue = _channel.QueueDeclare(_serviceName, durable: true, exclusive: false, autoDelete: false).QueueName;
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            _channel.QueueBind(queue, _exchangeName, _serviceName);
            _channel.BasicConsume(queue, false, consumer);

            _channel.QueueBind(fanoutQueue, _exchangeNameFanout, string.Empty);
            _channel.BasicConsume(fanoutQueue, false, consumer);

            WaitHandle.WaitAny(new[] { token.WaitHandle });
        }
    }
}
