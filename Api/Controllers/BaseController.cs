using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class BaseController : ControllerBase
    {
        protected string CurrentUserEmail
        {
            get
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrWhiteSpace(email))
                    throw new UnauthorizedAccessException("Logged in user email not found in token");

                return email;
            }
        }


        protected Guid CurrentUserId
        {
            get
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parsedId))
                    throw new UnauthorizedAccessException("Logged in user ID not found in token");

                return parsedId;
            }
        }
    }
}
