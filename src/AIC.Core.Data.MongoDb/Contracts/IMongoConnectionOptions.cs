namespace AIC.Core.Data.MongoDb.Contracts;

public interface IMongoConnectionOptions
{
    string ConnectionString { get; }
    string DatabaseName { get; }
}