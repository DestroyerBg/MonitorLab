using MonitorLab.Web.Contracts;

namespace MonitorLab.Web.Services
{
    public class ImageService(
        IWebHostEnvironment environment) : IImageService
    {
        public async Task<string> SaveMonitorImageAsync(
            IFormFile imageFile,
            Guid monitorId)
        {
            string extension =
                Path.GetExtension(imageFile.FileName);

            string fileName =
                $"{monitorId}{extension}";

            string folderPath = Path.Combine(
                environment.WebRootPath,
                "images",
                "monitors");

            Directory.CreateDirectory(folderPath);

            string filePath =
                Path.Combine(folderPath, fileName);

            using FileStream stream =
                new(filePath, FileMode.Create);

            await imageFile.CopyToAsync(stream);

            return $"/images/monitors/{fileName}";
        }

        public void DeleteImage(string imageUrl)
        {
            string relativePath =
                imageUrl.TrimStart('/');

            string fullPath = Path.Combine(
                environment.WebRootPath,
                relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}