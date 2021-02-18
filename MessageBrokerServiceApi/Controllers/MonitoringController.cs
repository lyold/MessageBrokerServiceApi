using MessageBrokerServiceApi.Domain.Services.Impl;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessageBrokerServiceApi.Controllers
{
    [Route("[controller]")]
    public class MonitoringController : ControllerBase
    {
        private static Guid _SERVICE_ID = Guid.NewGuid();

        private readonly IHelloWordService _service;
        
        public MonitoringController(IHelloWordService service)
        {
            _service = service;
        }

        /// <summary>
        /// Método responsável por obter as mensagens recebidas do serviço
        /// </summary>
        /// <returns></returns>
        [HttpGet("messages")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<string>> GetMessages()
        {
            return Ok(_service.GetMessages());
        }

        /// <summary>
        /// Método responsável por obter as mensagens recebidas do serviço
        /// </summary>
        /// <returns></returns>
        [HttpGet("info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<string> GetServiceInfo()
        {
            var messageInfoService = $"Id do serviço : {_SERVICE_ID}";

            return Ok(messageInfoService);
        }

        /// <summary>
        /// Método responsável por iniciar o disparo de mensagens de mensages de um serviço
        /// </summary>
        /// <returns></returns>
        [HttpPost("start")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<ActionResult> StartService()
        {
            _service.StartMessage(_SERVICE_ID);

            return Accepted();
        }
    }
}
