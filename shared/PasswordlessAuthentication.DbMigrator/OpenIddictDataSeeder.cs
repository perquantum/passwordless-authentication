using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.OpenIddict.Scopes;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace PasswordlessAuthentication.DbMigrator;

public class OpenIddictDataSeeder : ITransientDependency
{
    private readonly IConfiguration _configuration;
    private readonly ICurrentTenant _currentTenant;
    private readonly IOpenIddictApplicationRepository _openIddictApplicationRepository;
    private readonly IAbpApplicationManager _applicationManager;
    private readonly IOpenIddictScopeRepository _openIddictScopeRepository;
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly IPermissionDataSeeder _permissionDataSeeder;
    private readonly IStringLocalizer<OpenIddictResponse> L;
    private readonly ILogger<OpenIddictDataSeeder> _logger;

    public OpenIddictDataSeeder(
        IConfiguration configuration,
        ICurrentTenant currentTenant,
        IOpenIddictApplicationRepository openIddictApplicationRepository,
        IAbpApplicationManager applicationManager,
        IOpenIddictScopeRepository openIddictScopeRepository,
        IOpenIddictScopeManager scopeManager,
        IPermissionDataSeeder permissionDataSeeder,
        IStringLocalizer<OpenIddictResponse> l,
        ILogger<OpenIddictDataSeeder> logger)
    {
        _configuration = configuration;
        _currentTenant = currentTenant;
        _openIddictApplicationRepository = openIddictApplicationRepository;
        _applicationManager = applicationManager;
        _openIddictScopeRepository = openIddictScopeRepository;
        _scopeManager = scopeManager;
        _permissionDataSeeder = permissionDataSeeder;
        L = l;
        _logger = logger;
    }

    public Task SeedAsync(DataSeedContext context)
    {
        return SeedAsync();
    }

    [UnitOfWork]
    public virtual async Task SeedAsync()
    {
        using (_currentTenant.Change(null))
        {
            await CreateApiScopesAsync();
            await CreateWebGatewaySwaggerClientsAsync();
            await CreateClientsAsync();
        }
    }

    private async Task CreateApiScopesAsync()
    {
        await CreateScopesAsync("AccountService");
        await CreateScopesAsync("IdentityService");
        await CreateScopesAsync("AdministrationService");
        await CreateScopesAsync("SaasService");
        await CreateScopesAsync("ProductService");
    }

    private async Task CreateWebGatewaySwaggerClientsAsync()
    {
        await CreateSwaggerClientAsync("WebGateway",
            new[] { "AccountService", "IdentityService", "AdministrationService", "SaasService", "ProductService" });
    }

    private async Task CreateSwaggerClientAsync(string name, string[]? scopes = null)
    {
        var commonScopes = new List<string> {
            OpenIddictConstants.Permissions.Scopes.Address,
            OpenIddictConstants.Permissions.Scopes.Email,
            OpenIddictConstants.Permissions.Scopes.Phone,
            OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Permissions.Scopes.Roles
        };

        scopes ??= new[] { name };

        // Swagger Client
        var swaggerClientId = $"{name}_Swagger";
        if (!swaggerClientId.IsNullOrWhiteSpace())
        {
            var webGatewaySwaggerRootUrl = _configuration[$"OpenIddict:Applications:{name}:RootUrl"]?.EnsureEndsWith('/');
            var publicWebGatewayRootUrl = _configuration[$"OpenIddict:Applications:PublicWebGateway:RootUrl"]?.EnsureEndsWith('/');
            var accountServiceRootUrl = _configuration[$"OpenIddict:Resources:AccountService:RootUrl"]?.EnsureEndsWith('/');
            var identityServiceRootUrl = _configuration[$"OpenIddict:Resources:IdentityService:RootUrl"]?.EnsureEndsWith('/');
            var administrationServiceRootUrl = _configuration[$"OpenIddict:Resources:AdministrationService:RootUrl"]?.EnsureEndsWith('/');
            var saasServiceRootUrl = _configuration[$"OpenIddict:Resources:SaasService:RootUrl"]?.EnsureEndsWith('/');
            var productServiceRootUrl = _configuration[$"OpenIddict:Resources:ProductService:RootUrl"]?.EnsureEndsWith('/');

            await CreateApplicationAsync(
                name: swaggerClientId!,
                type: OpenIddictConstants.ClientTypes.Public,
                consentType: OpenIddictConstants.ConsentTypes.Implicit,
                displayName: "Swagger Client",
                secret: null,
                grantTypes: new List<string> { OpenIddictConstants.GrantTypes.AuthorizationCode, },
                scopes: commonScopes.Union(scopes).ToList(),
                redirectUris: new List<string> {
                    $"{webGatewaySwaggerRootUrl}swagger/oauth2-redirect.html", // WebGateway redirect uri
                    $"{publicWebGatewayRootUrl}swagger/oauth2-redirect.html", // PublicWebGateway redirect uri
                    $"{accountServiceRootUrl}swagger/oauth2-redirect.html", // AccountService redirect uri
                    $"{identityServiceRootUrl}swagger/oauth2-redirect.html", // IdentityService redirect uri
                    $"{administrationServiceRootUrl}swagger/oauth2-redirect.html", // AdministrationService redirect uri
                    $"{saasServiceRootUrl}swagger/oauth2-redirect.html", // SaasService redirect uri
                    $"{productServiceRootUrl}swagger/oauth2-redirect.html", // ProductService redirect uri
                }
            );
        }
    }

    private async Task CreateScopesAsync(string name)
    {
        if (await _openIddictScopeRepository.FindByNameAsync(name) == null)
        {
            await _scopeManager.CreateAsync(new OpenIddictScopeDescriptor {
                Name = name, DisplayName = name + " API", Resources = { name }
            });
        }
    }

    private async Task CreateClientsAsync()
    {
        var commonScopes = new List<string> {
            OpenIddictConstants.Permissions.Scopes.Address,
            OpenIddictConstants.Permissions.Scopes.Email,
            OpenIddictConstants.Permissions.Scopes.Phone,
            OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Permissions.Scopes.Roles
        };

        //Web Client
        var webClientRootUrl = _configuration["OpenIddict:Applications:Web:RootUrl"]!.EnsureEndsWith('/');
        await CreateApplicationAsync(
            name: "Web",
            type: OpenIddictConstants.ClientTypes.Confidential,
            consentType: OpenIddictConstants.ConsentTypes.Implicit,
            displayName: "Web Client",
            secret: "1q2w3e*",
            grantTypes: new List<string> //Hybrid flow
            {
                OpenIddictConstants.GrantTypes.AuthorizationCode, OpenIddictConstants.GrantTypes.Implicit
            },
            scopes: commonScopes.Union(new[] {
                "AccountService", "IdentityService", "AdministrationService", "SaasService", "ProductService"
            }).ToList(),
            redirectUris: new List<string> { $"{webClientRootUrl}signin-oidc" },
            postLogoutRedirectUris: new List<string>() { $"{webClientRootUrl}signout-callback-oidc" },
            clientUri: webClientRootUrl,
            logoUri: "/images/clients/aspnetcore.svg"
        );

        //Blazor Client
        var blazorClientRootUrl = _configuration["OpenIddict:Applications:Blazor:RootUrl"]!.EnsureEndsWith('/');
        await CreateApplicationAsync(
            name: "Blazor",
            type: OpenIddictConstants.ClientTypes.Public,
            consentType: OpenIddictConstants.ConsentTypes.Implicit,
            displayName: "Blazor Client",
            secret: null,
            grantTypes: new List<string> { OpenIddictConstants.GrantTypes.AuthorizationCode },
            scopes: commonScopes.Union(new[] {
                "AccountService", "IdentityService", "AdministrationService", "SaasService", "ProductService"
            }).ToList(),
            redirectUris: new List<string> { $"{blazorClientRootUrl}authentication/login-callback" },
            postLogoutRedirectUris: new List<string> { $"{blazorClientRootUrl}authentication/logout-callback" },
            clientUri: blazorClientRootUrl,
            logoUri: "/images/clients/blazor.svg"
        );

        //Blazor Server Client
        var blazorServerClientRootUrl = _configuration["OpenIddict:Applications:BlazorServer:RootUrl"]!.EnsureEndsWith('/');
        await CreateApplicationAsync(
            name: "BlazorServer",
            type: OpenIddictConstants.ClientTypes.Confidential,
            consentType: OpenIddictConstants.ConsentTypes.Implicit,
            displayName: "Blazor Server Client",
            secret: "1q2w3e*",
            grantTypes: new List<string> //Hybrid flow
            {
                OpenIddictConstants.GrantTypes.AuthorizationCode, OpenIddictConstants.GrantTypes.Implicit
            },
            scopes: commonScopes.Union(new[] {
                "AccountService", "IdentityService", "AdministrationService", "SaasService", "ProductService"
            }).ToList(),
            redirectUris: new List<string> { $"{blazorServerClientRootUrl}signin-oidc" },
            postLogoutRedirectUris: new List<string> { $"{blazorServerClientRootUrl}signout-callback-oidc" },
            clientUri: blazorServerClientRootUrl,
            logoUri: "/images/clients/blazor.svg"
        );

        //Blazor Web App Client
        var blazorWebAppClientRootUrl = _configuration["OpenIddict:Applications:BlazorWebApp:RootUrl"]!.EnsureEndsWith('/');
        await CreateApplicationAsync(
            name: "BlazorWebApp",
            type: OpenIddictConstants.ClientTypes.Confidential,
            consentType: OpenIddictConstants.ConsentTypes.Implicit,
            displayName: "Blazor Web App",
            secret: "1q2w3e*",
            grantTypes: new List<string> //Hybrid flow
            {
                OpenIddictConstants.GrantTypes.AuthorizationCode, OpenIddictConstants.GrantTypes.Implicit
            },
            scopes: commonScopes.Union(new[] {
                "AccountService", "IdentityService", "AdministrationService", "SaasService", "ProductService"
            }).ToList(),
            redirectUris: new List<string> { $"{blazorWebAppClientRootUrl}signin-oidc" },
            postLogoutRedirectUris: new List<string> { $"{blazorWebAppClientRootUrl}signout-callback-oidc" },
            clientUri: blazorWebAppClientRootUrl,
            logoUri: "/images/clients/blazor.svg"
        );

        //Public Web Client
        var publicWebClientRootUrl = _configuration["OpenIddict:Applications:PublicWeb:RootUrl"]!.EnsureEndsWith('/');
        await CreateApplicationAsync(
            name: "PublicWeb",
            type: OpenIddictConstants.ClientTypes.Confidential,
            consentType: OpenIddictConstants.ConsentTypes.Implicit,
            displayName: "Public Web Client",
            secret: "1q2w3e*",
            grantTypes: new List<string> //Hybrid flow
            {
                OpenIddictConstants.GrantTypes.AuthorizationCode, OpenIddictConstants.GrantTypes.Implicit
            },
            scopes: commonScopes.Union(new[] { "AccountService", "AdministrationService", "ProductService" }).ToList(),
            redirectUris: new List<string> { $"{publicWebClientRootUrl}signin-oidc" },
            postLogoutRedirectUris: new List<string> { $"{publicWebClientRootUrl}signout-callback-oidc" },
            clientUri: publicWebClientRootUrl,
            logoUri: "/images/clients/aspnetcore.svg"
        );

        //Angular Client
        var angularClientRootUrl = _configuration["OpenIddict:Applications:Angular:RootUrl"]?.TrimEnd('/');
        await CreateApplicationAsync(
            name: "Angular",
            type: OpenIddictConstants.ClientTypes.Public,
            consentType: OpenIddictConstants.ConsentTypes.Implicit,
            displayName: "Angular Client",
            secret: null,
            grantTypes: new List<string> {
                OpenIddictConstants.GrantTypes.AuthorizationCode,
                OpenIddictConstants.GrantTypes.RefreshToken,
                OpenIddictConstants.GrantTypes.Password,
                "LinkLogin",
                "Impersonation"
            },
            scopes: commonScopes.Union(new[] {
                "AccountService", "IdentityService", "AdministrationService", "SaasService", "ProductService"
            }).ToList(),
            redirectUris: new List<string> { $"{angularClientRootUrl}" },
            postLogoutRedirectUris: new List<string> { $"{angularClientRootUrl}" },
            clientUri: angularClientRootUrl,
            logoUri: "/images/clients/angular.svg"
        );

        //Administration Service Client
        await CreateApplicationAsync(
            name: "AdministrationService",
            type: OpenIddictConstants.ClientTypes.Confidential,
            consentType: OpenIddictConstants.ConsentTypes.Implicit,
            displayName: "Administration Service Client",
            secret: "1q2w3e*",
            grantTypes: new List<string> { OpenIddictConstants.GrantTypes.ClientCredentials },
            scopes: commonScopes.Union(new[] { "IdentityService" }).ToList(),
            permissions: new List<string> { IdentityPermissions.Users.Default }
        );
    }

    private async Task CreateApplicationAsync(
        [NotNull] string name,
        [NotNull] string type,
        [NotNull] string consentType,
        string displayName,
        string? secret,
        List<string> grantTypes,
        List<string> scopes,
        List<string>? redirectUris = null,
        List<string>? postLogoutRedirectUris = null,
        List<string>? permissions = null,
        string? clientUri = null,
        string? logoUri = null)
    {
        if (!string.IsNullOrEmpty(secret) && string.Equals(type, OpenIddictConstants.ClientTypes.Public,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new BusinessException(L["NoClientSecretCanBeSetForPublicApplications"]);
        }

        if (string.IsNullOrEmpty(secret) && string.Equals(type, OpenIddictConstants.ClientTypes.Confidential,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new BusinessException(L["TheClientSecretIsRequiredForConfidentialApplications"]);
        }

        var client = await _openIddictApplicationRepository.FindByClientIdAsync(name);

        var application = new AbpApplicationDescriptor {
            ClientId = name,
            ClientType = type,
            ClientSecret = secret,
            ConsentType = consentType,
            DisplayName = displayName,
            ClientUri = clientUri,
            LogoUri = logoUri
        };

        Check.NotNullOrEmpty(grantTypes, nameof(grantTypes));
        Check.NotNullOrEmpty(scopes, nameof(scopes));

        if (new[] { OpenIddictConstants.GrantTypes.AuthorizationCode, OpenIddictConstants.GrantTypes.Implicit }.All(
                grantTypes.Contains))
        {
            application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeIdToken);

            if (string.Equals(type, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeIdTokenToken);
                application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeToken);
            }
        }

        if (!redirectUris.IsNullOrEmpty() || !postLogoutRedirectUris.IsNullOrEmpty())
        {
            application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Logout);
        }

        var buildInGrantTypes = new[] {
            OpenIddictConstants.GrantTypes.Implicit, OpenIddictConstants.GrantTypes.Password,
            OpenIddictConstants.GrantTypes.AuthorizationCode, OpenIddictConstants.GrantTypes.ClientCredentials,
            OpenIddictConstants.GrantTypes.DeviceCode, OpenIddictConstants.GrantTypes.RefreshToken
        };

        foreach (var grantType in grantTypes)
        {
            if (grantType == OpenIddictConstants.GrantTypes.AuthorizationCode)
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode);
                application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Code);
            }

            if (grantType == OpenIddictConstants.GrantTypes.AuthorizationCode ||
                grantType == OpenIddictConstants.GrantTypes.Implicit)
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Authorization);
            }

            if (grantType == OpenIddictConstants.GrantTypes.AuthorizationCode ||
                grantType == OpenIddictConstants.GrantTypes.ClientCredentials ||
                grantType == OpenIddictConstants.GrantTypes.Password ||
                grantType == OpenIddictConstants.GrantTypes.RefreshToken ||
                grantType == OpenIddictConstants.GrantTypes.DeviceCode)
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
                application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Revocation);
                application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Introspection);
            }

            if (grantType == OpenIddictConstants.GrantTypes.ClientCredentials)
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.ClientCredentials);
            }

            if (grantType == OpenIddictConstants.GrantTypes.Implicit)
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Implicit);
            }

            if (grantType == OpenIddictConstants.GrantTypes.Password)
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Password);
            }

            if (grantType == OpenIddictConstants.GrantTypes.RefreshToken)
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.RefreshToken);
            }

            if (grantType == OpenIddictConstants.GrantTypes.DeviceCode)
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.DeviceCode);
                application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Device);
            }

            if (grantType == OpenIddictConstants.GrantTypes.Implicit)
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.IdToken);
                if (string.Equals(type, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
                {
                    application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.IdTokenToken);
                    application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Token);
                }
            }

            if (!buildInGrantTypes.Contains(grantType))
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.GrantType + grantType);
            }
        }

        var buildInScopes = new[] {
            OpenIddictConstants.Permissions.Scopes.Address, OpenIddictConstants.Permissions.Scopes.Email,
            OpenIddictConstants.Permissions.Scopes.Phone, OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Permissions.Scopes.Roles
        };

        foreach (var scope in scopes)
        {
            if (buildInScopes.Contains(scope))
            {
                application.Permissions.Add(scope);
            }
            else
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.Scope + scope);
            }
        }

        if (!redirectUris.IsNullOrEmpty())
        {
            foreach (var redirectUri in redirectUris!)
            {
                _logger.LogInformation($"=== {redirectUri} ===");
                if (!Uri.TryCreate(redirectUri, UriKind.Absolute, out var uri) || !uri.IsWellFormedOriginalString())
                {
                    throw new BusinessException(L["InvalidRedirectUri", redirectUri]);
                }

                if (application.RedirectUris.All(x => x != uri))
                {
                    application.RedirectUris.Add(uri);
                }
            }
        }

        if (!postLogoutRedirectUris.IsNullOrEmpty())
        {
            foreach (var postLogoutRedirectUri in postLogoutRedirectUris!)
            {
                if (!Uri.TryCreate(postLogoutRedirectUri, UriKind.Absolute, out var uri) ||
                    !uri.IsWellFormedOriginalString())
                {
                    throw new BusinessException(L["InvalidPostLogoutRedirectUri", postLogoutRedirectUri]);
                }

                if (application.PostLogoutRedirectUris.All(x => x != uri))
                {
                    application.PostLogoutRedirectUris.Add(uri);
                }
            }
        }

        if (permissions != null)
        {
            await _permissionDataSeeder.SeedAsync(
                ClientPermissionValueProvider.ProviderName,
                name,
                permissions,
                null
            );
        }

        if (client == null)
        {
            await _applicationManager.CreateAsync(application);
            return;
        }

        if (!HasSameRedirectUris(client, application))
        {
            client.RedirectUris = JsonSerializer.Serialize(application.RedirectUris.Select(q => q.ToString().TrimEnd('/')));
            client.PostLogoutRedirectUris = JsonSerializer.Serialize(application.PostLogoutRedirectUris.Select(q => q.ToString().TrimEnd('/')));

            await _applicationManager.UpdateAsync(client.ToModel());
        }

        if (!HasSameScopes(client, application))
        {
            client.Permissions = JsonSerializer.Serialize(application.Permissions.Select(q => q.ToString()));
            await _applicationManager.UpdateAsync(client.ToModel());
        }
    }

    private bool HasSameRedirectUris(OpenIddictApplication existingClient, AbpApplicationDescriptor application)
    {
        return existingClient.RedirectUris == JsonSerializer.Serialize(application.RedirectUris.Select(q => q.ToString().TrimEnd('/')));
    }

    private bool HasSameScopes(OpenIddictApplication existingClient, AbpApplicationDescriptor application)
    {
        return existingClient.Permissions == JsonSerializer.Serialize(application.Permissions.Select(q => q.ToString().TrimEnd('/')));
    }
}
