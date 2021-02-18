using MessageBrokerServiceApi.Common.Helper.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MessageBrokerServiceApi.Domain.Services.Impl
{
    public class HelloWordService : IHelloWordService
    {
        private readonly IRabbitMqHelper _rabbitMQ;

        private static readonly List<string> ListMessages = new List<string>();
        
        public HelloWordService(IRabbitMqHelper rabbitMQ)
        {
            _rabbitMQ = rabbitMQ;
        }

        public List<string> GetMessages()
        {
            return ListMessages;
        }

        public async void StartMessage(Guid serviceId)
        {
            Task.Run(() => ExecuteProcess(serviceId));
        }

        public Task ReceiveMessage(string message)
        {
            ListMessages.Add(message);

            return Task.CompletedTask;
        }

        #region Private Methods

        private async Task ExecuteProcess(Guid serviceId)
        {
            while (true)
            {
                PublishMessage(serviceId);

                Thread.Sleep(5000);
            }
        }

        private void PublishMessage(Guid serviceId)
        {
            try
            {
                _rabbitMQ.RouteMessage($"Hello Word / Date:{ DateTime.Now } / Service Id : { serviceId } / Message Id : {Guid.NewGuid()}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        #endregion
    }
}
