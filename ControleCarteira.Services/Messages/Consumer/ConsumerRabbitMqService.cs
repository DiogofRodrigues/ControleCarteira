using ControleCarteira.Domain;
using ControleCarteira.Infrastructure.Repositories;
using ControleCarteira.Services.File;
using ControleCarteira.Services.Messages.Producer;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ControleCarteira.Services.Messages.Consumer;
public class ConsumerRabbitMqService : IConsumerRabbitMqService
{
    private readonly IModel _channel;
    private readonly IProducerRabbitMqService _producerRabbitMqService;
    private readonly IOrdemRepository _ordemRepository;
    private readonly IFileManagerService _fileManager;

    private static string _queuename = null;
    private static string _queueDlqName = null;

    private Ordem _currentObject;
    private bool _processedSuccessfully = false;

    public ConsumerRabbitMqService(
        IConfiguration configuration,
        IModel channel,
        IProducerRabbitMqService producerRabbitMqService,
        IOrdemRepository ordemRepository,
        IFileManagerService fileManager)
    {
        _channel = channel;
        _producerRabbitMqService = producerRabbitMqService;
        _ordemRepository = ordemRepository;
        _fileManager = fileManager;
        _queuename = configuration["RabbitMqConfig:Queue"];
        _queueDlqName = configuration["RabbitMqConfig:QueueDlq"];
    }



    public ObjectMessageProcessedInfo Consume(bool consumeDlq = false)
    {
        EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);

        string queuename = consumeDlq ? _queueDlqName : _queuename;
        consumer.Received += Consumer_Message;
        _channel.BasicConsume(queuename, false, consumer);

        return new ObjectMessageProcessedInfo
        {
            Object = _currentObject,
            ProcessedSuccessfully = _processedSuccessfully,
            ProcessedDlqQueue = consumeDlq
        };
    }

    private void Consumer_Message(object? sender, BasicDeliverEventArgs e)
    {
        bool processouComSucesso = false;
        string messageContent = Encoding.UTF8.GetString(e.Body.ToArray());
        Ordem ordem = null;

        if (!string.IsNullOrEmpty(messageContent))
        {
            Console.WriteLine($"Conteudo da mensagem: {messageContent}");
            try
            {
                ordem = JsonConvert.DeserializeObject<Ordem>(messageContent);
                _currentObject = ordem ?? null;

                //if (ordem != null && (jogo.Id > 0 || !string.IsNullOrEmpty(jogo.Nome)))
                //{
                //    if (jogo.Id == 0 && !string.IsNullOrEmpty(jogo.Nome))
                //    {
                        _ordemRepository.Cadastrar(ordem);
                //    }
                //    else if (jogo.Id > 0 && !string.IsNullOrEmpty(jogo.Nome))
                //    {
                //        _jogoRepository.Atualizar(jogo);
                //    }
                //    else if (jogo.Id > 0)
                //    {
                //        _jogoRepository.Excluir(jogo.Id);
                //    }

                //    processouComSucesso = true;
                //    _processedSuccessfully = true;
                //    Console.WriteLine($"Jogo processado com sucesso - [{jogo.Nome.ToUpper()}]\n");
                //}

                Console.WriteLine($"Ordem processada com sucesso - [{ordem.Ticker.ToUpper()}]\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Falha no tratamento da mensagem] - {ex.Message}");

                if (ordem != null)
                {
                    string message = JsonConvert.SerializeObject(ordem);

                    if (!e.RoutingKey.ToLower().Contains("dlq"))
                        _producerRabbitMqService.Publish(message, ordem.GetType(), publishDlq: true);
                    else if (e.RoutingKey.ToLower().Contains("dlq"))
                        _fileManager.Save(message);

                    processouComSucesso = true;
                }
            }
            finally
            {
                if (processouComSucesso)
                    _channel.BasicAck(e.DeliveryTag, false);
                else
                    _channel.BasicNack(e.DeliveryTag, false, false);
            }
        }
    }

}