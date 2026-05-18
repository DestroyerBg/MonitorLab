using System.ComponentModel.DataAnnotations;
using static MonitorLab.Data.Common.ErrorMessages.Common;
namespace MonitorLab.Web.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = FieldIsRequired)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = FieldIsRequired)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
