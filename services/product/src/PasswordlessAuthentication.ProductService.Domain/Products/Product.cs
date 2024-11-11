using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace PasswordlessAuthentication.ProductService.Products;

public class Product : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    public virtual string Name { get; protected set; } = default!;

    public virtual float Price { get; set; }

    protected Product()
    {
    }

    public Product(Guid id, string name, float price, Guid? tenantId = null)
        : base(id)
    {
        TenantId = tenantId;
        SetName(name);
        Price = price;
    }

    public void SetName(string name)
    {
        Name = Check.NotNull(name, nameof(name), ProductConsts.NameMaxLength);
    }
}
