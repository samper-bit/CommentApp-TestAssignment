namespace CommentApp.Application.Services.RabbitMqService;

public interface IRabbitMqService
{
    Task PublishAsync<T>(T message);
}