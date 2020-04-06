using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopApp.WebUI.Identity;
using ShopApp.WebUI.Models;

namespace ShopApp.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View(new RegisterModel());
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                FullName = model.FullName,
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                //generate token
                //send mail
                return RedirectToAction("login", "account");
            }

            ModelState.AddModelError("", "Bilinmeyen bir hata olustu. Lutfen tekrar deneyiniz.");
            return View(model);
        }

        public IActionResult Login(string ReturnUrl = null)
        {
            return View(new LoginModel() { 
                ReturnUrl = ReturnUrl
            });
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginModel model)
        {
          
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("","Bu email'e ait hesap bulunmamaktadir.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);

            if (result.Succeeded)
            {
                // ReturnUrl null ise anasayfaya yonlendir
                return Redirect(model.ReturnUrl??"~/");
            }

            ModelState.AddModelError("","Email ya da parola yanlis.");
            return View(model);
        }
    }
}