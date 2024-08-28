namespace AIC.Core.Data.MongoDb.Implementations;

using AIC.Core.Data.MongoDb.Contracts;

public class MongoConnectionOptions : IMongoConnectionOptions
{
    public MongoConnectionOptions(string connectionString, string databaseName)
    {
        this.ConnectionString = connectionString;
        this.DatabaseName = databaseName;
    }

    public string ConnectionString { get; }
    public string DatabaseName { get; }
}