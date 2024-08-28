namespace AIC.Core.Messaging.Models.Contracts;

public interface IHasChronology
{
    DateTime Sent { get; set; }
    bool HasBeenSent => this.Sent != default;
    DateTime Delivered { get; set; }
    bool HasBeenDelivered => this.Delivered != default;
    DateTime Read { get; set; }
    bool HasBeenRead => this.Read != default;
}