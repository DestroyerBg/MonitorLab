using Microsoft.AspNetCore.Identity;
using MonitorLab.Data.EntityDTOs;
namespace MonitorLab.Core.Contracts
{
    public interface IUserService
    {
        Task<SignInResult> LoginUserAsync(LoginDTO model);
        Task<bool> LogoutUserAsync();
        LoginDTO CreateBlankLoginViewModel();

    }
}
