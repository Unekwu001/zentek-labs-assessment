using Asp.Versioning;
using Common.Dtos;
using Core.Services.UserServices;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        try
        {
            var result = await _userService.RegisterAsync(dto);

            var response = ApiResponse<AuthResponseDto>.Created(
                data: result,
                message: "User registered successfully"
            );
            
            _logger.LogInformation("User registered successfully: {@User}", result);

            return CreatedAtAction(nameof(Register), response); 
        }
        catch (ValidationException ex)
        {
            var response = ApiResponse<AuthResponseDto>.BadRequest(
                message: ex.Message
            );
            _logger.LogWarning("Validation error during registration: {@RegisterDto}, Errors: {@Errors}", dto, ex.Message);
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            var response = ApiResponse<AuthResponseDto>.ServerError(
                message: "An error occurred while registering the user."
            );
            _logger.LogError(ex, "An error occurred while registering the user: {@RegisterDto}", dto);
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }




    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
    {
        try
        {
            var result = await _userService.LoginAsync(dto);

            if (result == null)
            {
                var errorResponse = ApiResponse<AuthResponseDto>.Unauthorized(
                    message: "Invalid email or password"
                );
                _logger.LogWarning("Invalid login attempt: {@LoginDto}", dto);
                return Unauthorized(errorResponse);
            }

            var successResponse = ApiResponse<AuthResponseDto>.Successful(
                data: result,
                message: "Login successful"
            );
            _logger.LogInformation("User logged in successfully: {@LoginDto}", dto);
            return Ok(successResponse);
        }
        catch (ValidationException ex)
        {
            var response = ApiResponse<AuthResponseDto>.BadRequest(
                message: ex.Message
            );
            _logger.LogWarning("Validation error during login: {@LoginDto}, Errors: {@Errors}", dto, ex.Message);
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            var errorResponse = ApiResponse<AuthResponseDto>.ServerError(
                message: "An unexpected error occurred during login."
            );
            _logger.LogError(ex, "An error occurred during login: {@LoginDto}", dto);
            return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
        }
    }




}