
using Api.Startup.Jwt;
using Data.Models;

namespace Backend._247.Api.Common.ProgramSetup.Jwt;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateAccessToken(System.Collections.Generic.List<System.Security.Claims.Claim> claims, JwtSettings settings);
}
