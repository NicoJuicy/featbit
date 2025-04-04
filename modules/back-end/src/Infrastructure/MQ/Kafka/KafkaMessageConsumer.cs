using System.Text.Json;
using Confluent.Kafka;
using Domain.EndUsers;
using Domain.Messages;
using Domain.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MQ.Kafka;

public partial class KafkaMessageConsumer : BackgroundService
{
    private readonly IConsumer<Null, string> _consumer;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<KafkaMessageConsumer> _logger;

    public KafkaMessageConsumer(
        ConsumerConfig config,
        IServiceProvider serviceProvider,
        ILogger<KafkaMessageConsumer> logger)
    {
        _consumer = new ConsumerBuilder<Null, string>(config).Build();
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Factory.StartNew(
            async () => { await StartConsumerLoop(stoppingToken); },
            TaskCreationOptions.LongRunning
        );
    }

    private async Task StartConsumerLoop(CancellationToken cancellationToken)
    {
        _consumer.Subscribe(Topics.EndUser);
        _logger.LogInformation("Start consuming {Topic} messages...", Topics.EndUser);

        ConsumeResult<Null, string>? consumeResult = null;
        var message = string.Empty;
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                consumeResult = _consumer.Consume(cancellationToken);
                if (consumeResult.IsPartitionEOF)
                {
                    continue;
                }

                message = consumeResult.Message.Value;
                if (string.IsNullOrWhiteSpace(message))
                {
                    continue;
                }

                var endUserMessage =
                    JsonSerializer.Deserialize<EndUserMessage>(message, ReusableJsonSerializerOptions.Web);
                if (endUserMessage == null)
                {
                    continue;
                }

                using var scope = _serviceProvider.CreateScope();
                var endUserService = scope.ServiceProvider.GetRequiredService<IEndUserService>();

                // upsert endUser and it's properties
                var endUser = endUserMessage.AsEndUser();
                await endUserService.UpsertAsync(endUser);
                await endUserService.AddNewPropertiesAsync(endUser);
            }
            catch (ConsumeException ex)
            {
                var error = ex.Error.ToString();
                Log.FailedConsumeMessage(_logger, message, error);

                if (ex.Error.IsFatal)
                {
                    // https://github.com/edenhill/librdkafka/blob/master/INTRODUCTION.md#fatal-consumer-errors
                    break;
                }
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
            catch (Exception ex)
            {
                Log.ErrorConsumeMessage(_logger, message, ex);
            }
            finally
            {
                try
                {
                    if (consumeResult != null)
                    {
                        // store offset manually
                        _consumer.StoreOffset(consumeResult);
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorStoreOffset(_logger, ex);
                }
            }
        }
    }

    public override void Dispose()
    {
        // Commit offsets and leave the group cleanly.
        _consumer.Close();
        _consumer.Dispose();

        base.Dispose();
    }
}