using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PasswordlessAuthentication.SaasService.EntityFrameworkCore;
using PasswordlessAuthentication.Shared.Hosting.Microservices.DbMigrations;
using Volo.Abp.Data;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using Volo.Saas.Tenants;

namespace PasswordlessAuthentication.SaasService.DbMigrations;

public class SaasServiceDatabaseMigrationEventHandler
    : DatabaseMigrationEventHandlerBase<SaasServiceDbContext>,
        IDistributedEventHandler<ApplyDatabaseMigrationsEto>
{
    public SaasServiceDatabaseMigrationEventHandler(
        ILoggerFactory loggerFactory,
        ICurrentTenant currentTenant,
        IUnitOfWorkManager unitOfWorkManager,
        ITenantStore tenantStore,
        ITenantRepository tenantRepository,
        IDistributedEventBus distributedEventBus
    ) : base(
        loggerFactory,
        currentTenant,
        unitOfWorkManager,
        tenantStore,
        tenantRepository,
        distributedEventBus,
        SaasServiceDbProperties.ConnectionStringName)
    {
    }

    public async Task HandleEventAsync(ApplyDatabaseMigrationsEto eventData)
    {
        if (eventData.DatabaseName != DatabaseName)
        {
            return;
        }

        if (eventData.TenantId != null)
        {
            return;
        }

        try
        {
            await MigrateDatabaseSchemaAsync(null);
        }
        catch (Exception ex)
        {
            await HandleErrorOnApplyDatabaseMigrationAsync(eventData, ex);
        }
    }
}
