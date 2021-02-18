
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessageBrokerServiceApi.Domain.Services.Impl
{
    public interface IHelloWordService
    {
        void StartMessage(Guid serviceId);

        Task ReceiveMessage(string message);

        List<string> GetMessages();
    }
}
