namespace Application.DTOs.Subscription
{
    public record TransactionDto(
        Guid Id,
        string RevenueCatEventId,
        string EventType,
        DateTime EventTimestamp,
        string EventData,
        DateTime ProcessedAt,
        bool IsProcessed
    );

}
