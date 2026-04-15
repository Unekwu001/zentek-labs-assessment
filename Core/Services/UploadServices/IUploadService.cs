namespace Core.Services.UploadServices
{
    public interface IUploadService
    {
        Task<string> UploadBase64ImageAsync(string base64Image);
    }
    
}
