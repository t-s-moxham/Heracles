using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Heracles.Web.auth;

public static class CrudRequirements
{
    public static OperationAuthorizationRequirement Create = new() { Name = nameof(Create) };

    public static OperationAuthorizationRequirement Read = new() { Name = nameof(Read) };

    public static OperationAuthorizationRequirement Update = new() { Name = nameof(Update) };

    public static OperationAuthorizationRequirement Delete = new() { Name = nameof(Delete) };
}