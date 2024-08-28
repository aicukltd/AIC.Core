namespace AIC.Core.Identity.Tenants.Models.MongoDb.Implementations;

using AIC.Core.Data.MongoDb.Implementations;
using AIC.Core.Identity.Tenants.Models.Contracts;
using AIC.Core.Identity.Tenants.Models.Implementations;

public class Tenant : BaseMongoDbDocument, ITenant
{
    public string Name { get; set; }
    public Guid OwnerUserId { get; set; }
    public TenantStatus Status { get; set; }
    public TenantType Type { get; set; }
}