using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MonitorLab.Data.EntityDTOs;
using MonitorLab.Core.Contracts;
namespace MonitorLab.Core.Services
{
    public class ApplicationUserService(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager) : IUserService
    {
        private static IdentityUser CreateNewUserInstance()
        {
            IdentityUser user = Activator.CreateInstance<IdentityUser>();

            return user;
        }

        public async Task<bool> LogoutUserAsync()
        {
            await signInManager.SignOutAsync();

            return true;
        }


        public virtual LoginDTO CreateBlankLoginViewModel()
        {
            return new LoginDTO();
        }

        public virtual async Task<SignInResult> LoginUserAsync(LoginDTO dto)
        {
            IdentityUser? user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return new SignInResult();
            }

            SignInResult result =
                await signInManager
                    .PasswordSignInAsync(user.UserName!,
                        dto.Password, dto.RememberMe, lockoutOnFailure: true);

            return result;
        }

    }
}
