using ControleCarteira.Domain;

namespace ControleCarteira.Services.Messages.Consumer;
public interface IConsumerRabbitMqService
{
    ObjectMessageProcessedInfo Consume(bool consumeDlq = false);
}
