﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.ViewModels;

namespace MyWebApp.Controllers
{
    public sealed class AccountController : Controller
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly RandomGenerator _randomGenerator;

        public AccountController(UserManager<UserModel> userManager,
            SignInManager<UserModel> signInManager,
            RandomGenerator randomGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _randomGenerator = randomGenerator;
        }

        [HttpGet]
        [Route("Account/Login")]
        public IActionResult Login()
        {
            var response = new LoginViewModel();
            return View(response);
        }

        [HttpPost]
        [Route("Account/Login")]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Input is not valid";
                return View(loginVM);
            }

            var user = await _userManager.FindByEmailAsync(loginVM.Email);
            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                if (passwordCheck)
                {
                    var signInResult = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                    if (signInResult.Succeeded)
                    {
                        return RedirectToAction("Index", "Notes");
                    }
                } 
            }

            TempData["Error"] = "Wrong credentials, please try again";
            return View(loginVM);
        }

        [HttpGet]
        [Route("Account/Register")]
        public IActionResult Register()
        {
            var response = new RegisterViewModel();
            return View(response);
        }

        [HttpPost]
        [Route("Account/Register")]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Input is not valid";
                return View(registerVM);
            }

            var user = await _userManager.FindByEmailAsync(registerVM.Email);
            if (user != null)
            {
                TempData["Error"] = "This e-mail address is already in use";
                return View(registerVM);
            }

            var newUser = new UserModel()
            {
                Id = _randomGenerator.GetRandomId(),
                Email = registerVM.Email,
                UserName = registerVM.Email
            };

            var newUserCreateResponse = await _userManager.CreateAsync(newUser, registerVM.Password);
            if (newUserCreateResponse.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);
                var signInResult = await _signInManager.PasswordSignInAsync(newUser, registerVM.Password, false, false);
                if (signInResult.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Error"] = "Can't login into created account!";
                    return View(registerVM);
                }
            }

            TempData["Error"] = "Registration failed";
            return View(registerVM);
        }

        [HttpGet]
        [Route("Account/Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
