using MessageBrokerServiceApi.Controllers;
using MessageBrokerServiceApi.Domain.Services.Impl;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MessageBrokerServiceApi.Test.Controllers
{
    public class MonitoringControllerTest
    {
        private MonitoringController _controller;
        private Mock<IHelloWordService> _service;
        
        public MonitoringControllerTest()
        {
            
        }

        [SetUp]
        public void SetUp()
        {
            _service = new Mock<IHelloWordService>();
            
            _service.Setup(s => s.StartMessage(It.IsAny<Guid>()));

            _controller = new MonitoringController(_service.Object);
        }

        [Test]
        public void GetMessagesShouldSuccess()
        {
            var id = Guid.NewGuid();

            var listMessages = new List<string>();
            listMessages.Add("message 1");
            
            _service.Setup(s => s.GetMessages()).Returns(listMessages);

            var response = _controller.GetMessages();

            Assert.AreEqual(response.Result.GetType(), typeof(OkObjectResult));
        }

        [Test]
        public void GetServiceInfoShouldSuccess()
        {
            var response = _controller.GetServiceInfo();

            Assert.AreEqual(response.Result.GetType(), typeof(OkObjectResult));
        }

        [Test]
        public void StartServiceShouldSuccess()
        {
            var response = _controller.StartService().GetAwaiter().GetResult();

            Assert.AreEqual(response.GetType(), typeof(AcceptedResult));
        }

    }
}