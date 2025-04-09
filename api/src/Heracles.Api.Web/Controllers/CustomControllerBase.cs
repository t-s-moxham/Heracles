using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Heracles.Web.Controllers;

[ApiController]
public class CustomControllerBase : ControllerBase
{
    protected Guid CurrentUserId
    {
        get { return new Guid(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value); }
    }
}
