using Domain.Messages;
using Infrastructure.AppService;

namespace Infrastructure.Messages;

public class InsightMessageHandler(IInsightService insightService, InsightsWriter insightsWriter) : IMessageHandler
{
    public string Topic => Topics.Insights;

    public Task HandleAsync(string message, CancellationToken cancellationToken)
    {
        if (insightService.TryParse(message, out var insight))
        {
            insightsWriter.Record(insight);
        }

        return Task.CompletedTask;
    }
}