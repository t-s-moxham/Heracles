using System.Security.Claims;
using Heracles.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Heracles.Web.auth;

public class ExerciseCrudAuthorizationHandler
    : AuthorizationHandler<OperationAuthorizationRequirement, PublicEntity>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        PublicEntity resource
    )
    {
        var userId = new Guid(
            context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value
        );

        var isOwned = userId == resource.UserId;

        switch (requirement.Name)
        {
            case nameof(CrudRequirements.Read):
                if (resource.IsOfficial || isOwned)
                {
                    context.Succeed(requirement);
                }
                break;

            case nameof(CrudRequirements.Create):
            case nameof(CrudRequirements.Update):
            case nameof(CrudRequirements.Delete):
                if (isOwned)
                {
                    context.Succeed(requirement);
                }

                break;
        }

        return Task.CompletedTask;
    }
}
