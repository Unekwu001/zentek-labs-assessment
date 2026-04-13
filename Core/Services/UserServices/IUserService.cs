using Common.Dtos;
namespace Core.Services.UserServices
{
    public interface IUserService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto);
        Task<AuthResponseDto> LoginAsync(LoginUserDto dto);
    }
}
