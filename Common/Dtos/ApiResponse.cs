

using Microsoft.AspNetCore.Http;

namespace Common.Dtos
{
    public class ApiResponse<TData>
    {
        public bool Success { get; init; }
        public int StatusCode { get; init; }
        public string Message { get; init; } = string.Empty;
        public string? ErrorCode { get; init; }
        public TData? Data { get; init; }
        public IReadOnlyList<string>? Errors { get; init; }
        public object? Metadata { get; init; }



        public static ApiResponse<TData> Successful(
            TData data,
            string message = "Operation completed successfully",
            int statusCode = StatusCodes.Status200OK,
            object? metadata = null)
        {
            return new ApiResponse<TData>
            {
                Success = true,
                StatusCode = statusCode,
                Message = message,
                Data = data,
                Metadata = metadata
            };
        }




        public static ApiResponse<TData> Created(
            TData data,
            string message = "Resource created successfully",
            object? metadata = null)
        {
            return Successful(data, message, StatusCodes.Status201Created, metadata);
        }





        public static ApiResponse<TData> Error(
            string message,
            int statusCode = StatusCodes.Status400BadRequest,
            string? errorCode = null,
            IEnumerable<string>? errors = null)
        {
            return new ApiResponse<TData>
            {
                Success = false,
                StatusCode = statusCode,
                Message = message,
                ErrorCode = errorCode,
                Errors = errors?.ToList().AsReadOnly(),
                Data = default
            };
        }





        public static ApiResponse<TData> NotFound(string message = "Resource not found")
        {
            return Error(message, StatusCodes.Status404NotFound, "NOT_FOUND");
        }





        public static ApiResponse<TData> Unauthorized(
            string message = "Authentication required",
            string? errorCode = "UNAUTHORIZED")
        {
            return Error(message, StatusCodes.Status401Unauthorized, errorCode);
        }





        public static ApiResponse<TData> Forbidden(
            string message = "Access forbidden",
            string? errorCode = "FORBIDDEN")
        {
            return Error(message, StatusCodes.Status403Forbidden, errorCode);
        }





        public static ApiResponse<TData> BadRequest(
            string message = "Invalid Payload",
            string? errorCode = "BAD_REQUEST")
        {
            return Error(message, StatusCodes.Status400BadRequest, errorCode);
        }





        public static ApiResponse<TData> ValidationError(
            string message = "One or more validation errors occurred",
            IEnumerable<string>? errors = null,
            string? errorCode = "VALIDATION_ERROR")
        {
            return Error(
                message: message,
                statusCode: StatusCodes.Status400BadRequest,
                errorCode: errorCode,
                errors: errors);
        }




        public static ApiResponse<TData> ServerError(
            string message = "An unexpected error occurred. Please try again later.",
            string? errorCode = "INTERNAL_ERROR")
        {
            return Error(message, StatusCodes.Status500InternalServerError, errorCode);
        }



    }


    public class ApiResponse : ApiResponse<object?>
    {
    }
}