namespace AIC.Core.Data.MongoDb.Contracts;

public interface IHasDataPayload
{
    /// <summary>
    ///     Gets or sets the data.
    /// </summary>
    /// <value>
    ///     The data.
    /// </value>
    byte[] Data { get; set; }
}