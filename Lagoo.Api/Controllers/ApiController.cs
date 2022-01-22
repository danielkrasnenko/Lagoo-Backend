using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Lagoo.Api.Controllers;

/// <summary>
///   The base controller for others
/// </summary>
[ApiController]
public abstract class ApiController : ControllerBase
{
    protected ApiController(ISender sender)
    {
        Sender = sender;
    }

    protected ISender Sender { get; }
}