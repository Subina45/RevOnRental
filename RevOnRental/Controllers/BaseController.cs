using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevOnRental.Domain.Constants;
using System.Security.Claims;

namespace RevOnRental.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class BaseController : ControllerBase
    {
        private IMediator _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        protected int CurrentUserId => int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == AuthConstants.JwtId).Value);
        protected string CurrentRole => HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value.ToLower();


    }
}
