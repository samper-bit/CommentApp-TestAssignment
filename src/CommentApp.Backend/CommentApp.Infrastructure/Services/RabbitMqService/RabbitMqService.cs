using System.Text;
using System.Text.Json;

namespace CommentApp.Infrastructure.Services.RabbitMqService;

public class RabbitMqService(IConnection connection) : IRabbitMqService, IAsyncDisposable
{
    private const string ExchangeName = "comment_exchange";

    public async Task PublishAsync<T>(T message)
    {
        await using var channel = await connection.CreateChannelAsync();
        await channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Fanout, durable: true);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        await channel.BasicPublishAsync(ExchangeName, string.Empty, body);
    }

    public async ValueTask DisposeAsync()
    {
        await connection.CloseAsync();
        await connection.DisposeAsync();
    }
}