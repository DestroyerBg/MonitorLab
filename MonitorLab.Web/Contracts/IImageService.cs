namespace MonitorLab.Web.Contracts
{
    public interface IImageService
    {
        Task<string> SaveMonitorImageAsync(
           IFormFile imageFile,
           Guid monitorId);

        void DeleteImage(string imageUrl);
    }
}
