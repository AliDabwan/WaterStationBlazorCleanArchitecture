using WaterS.Application.Requests;

namespace WaterS.Application.Interfaces.Services
{
    public interface IUploadService
    {
        string UploadAsync(UploadRequest request);
    }
}