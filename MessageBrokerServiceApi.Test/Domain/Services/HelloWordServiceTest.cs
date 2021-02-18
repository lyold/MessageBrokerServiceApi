using MessageBrokerServiceApi.Common.Helper.Interfaces;
using MessageBrokerServiceApi.Domain.Services.Impl;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MessageBrokerServiceApi.Test.Controllers
{
    public class HelloWordServiceTest
    {
        private HelloWordService _service;
        private Mock<IRabbitMqHelper> _rabbitMqHelper;
        
        public HelloWordServiceTest()
        {
            
        }

        [SetUp]
        public void SetUp()
        {
            _rabbitMqHelper = new Mock<IRabbitMqHelper>();

            _service = new HelloWordService(_rabbitMqHelper.Object);
        }

        [Test]
        public void StartMessageShouldSuccess()
        {
            var id = Guid.NewGuid();

            var listMessages = new List<string>();
            listMessages.Add("message 1");

            _rabbitMqHelper.Setup(s => s.RouteMessage(It.IsAny<string>()));

            Assert.DoesNotThrow(() => _service.StartMessage(id));
        }

        [Test]
        public void GetMessagesShouldSuccess()
        {
            var message = "Hello World";

            _service.ReceiveMessage(message).GetAwaiter().GetResult();

            var response = _service.GetMessages();


            Assert.AreEqual(response.FirstOrDefault(), message);
        }
    }
}