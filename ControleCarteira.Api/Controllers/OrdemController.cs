using ControleCarteira.Domain;
using ControleCarteira.Services.Messages.Producer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace ControleCarteira.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdemController : Controller

    {

        private readonly IProducerRabbitMqService _producerRabbitMqService;
        private bool _enableDlq = false;

        public OrdemController(IProducerRabbitMqService producerRabbitMqService)
        {
            _producerRabbitMqService = producerRabbitMqService;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Ordem ordem)
        {
            var message = JsonConvert.SerializeObject(ordem);
            await _producerRabbitMqService.Publish(message, typeof(Ordem), _enableDlq);
            return Ok($"Ordem processada na carteira com sucesso: {ordem.Ticker}");
        }

        //static ConnectionFactory GetConnection()
        //{

        //    ConnectionFactory factory = new ConnectionFactory();
        //    factory.HostName = "localhost";
        //    factory.VirtualHost = "/";
        //    factory.Port = 5672;
        //    factory.UserName = "guest";
        //    factory.Password = "guest";

        //    return factory;

        //}

    }
}
