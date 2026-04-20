using MonitorLab.Data.Seed.DTOS;
using System.Text.Json;

namespace MonitorLab.Data.Seed
{
    public class JsonSeederLoader
    {
        private static readonly JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private static string ReadJSON(string fileName)
        {
            string filePath;

            filePath = Path.Combine(AppContext.BaseDirectory, fileName);
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }

            string solutionRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
            filePath = Path.Combine(solutionRoot, "MonitorLab.Data", "Seed", fileName);

            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }

            throw new FileNotFoundException(
                $"JSON файлът '{fileName}' не беше намерен нито в wwwroot, нито в локалния проект.");
        }


        public static async Task<List<MonitorSeedDTO>> LoadMonitorsAsync(string fileName)
        {
            string json = ReadJSON(fileName);
            return JsonSerializer.Deserialize<List<MonitorSeedDTO>>(json, options)!;
        }

        public static async Task<List<PortSeedDTO>> LoadPortsAsync(string fileName)
        {
            string json = ReadJSON(fileName);
            return JsonSerializer.Deserialize<List<PortSeedDTO>>(json, options)!;
        }

        public static async Task<List<MonitorPortSeedDTO>> LoadMonitorPortsAsync(string fileName)
        {
            string json = ReadJSON(fileName);
            return JsonSerializer.Deserialize<List<MonitorPortSeedDTO>>(json, options)!;
        }
    }
}
