using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonitorLab.Core.Contracts;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using Microsoft.AspNetCore.Identity;
using MonitorLab.Web.Models.AccountViewModels;
using MonitorLab.Data.EntityDTOs;
using static MonitorLab.Data.Common.ErrorMessages.Common;
namespace MonitorLab.Web.Controllers
{
    public class AccountController(IUserService userService, 
        IMapper mapService) : Controller
    {
        
        [HttpGet]
        public IActionResult Login()
        {
            LoginViewModel model = mapService.Map<LoginDTO, LoginViewModel>(userService.CreateBlankLoginViewModel());
            
            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            LoginDTO dto = mapService.Map<LoginViewModel, LoginDTO>(model);
            SignInResult result = await userService.LoginUserAsync(dto);
            
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction("Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, InvalidLoginAttempt);
                return View(model);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await userService.LogoutUserAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Lockout()
        {
            return View();
        }

    }
}
