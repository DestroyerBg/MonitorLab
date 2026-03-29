using System.ComponentModel.DataAnnotations;
using static MonitorLab.Data.Common.DatabaseConstants.Port;
namespace MonitorLab.Data.Models
{
    public class Port
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(
            NameMaxLength,
            MinimumLength = NameMinLength)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(VersionMin, VersionMax)]
        public double Version { get; set; }

    }
}
