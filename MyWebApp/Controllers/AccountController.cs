﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyWebApp.Credentials;
using MyWebApp.Data.Interfaces;
using MyWebApp.Localization.Interfaces;
using MyWebApp.Models;
using MyWebApp.Repository.Interfaces;
using MyWebApp.ViewModels;

namespace MyWebApp.Controllers
{
    public sealed class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IRandomGenerator _randomGenerator;
        private readonly ILanguageService _languageService;
        private readonly ICredentialsRepository _credentialsRepository;
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;

        public AccountController(ILogger<AccountController> logger,
            IRandomGenerator randomGenerator,
            ILanguageService languageService,
            ICredentialsRepository credentialsRepository,
            UserManager<UserModel> userManager,
            SignInManager<UserModel> signInManager)
        {
            _credentialsRepository = credentialsRepository;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _randomGenerator = randomGenerator;
            _languageService = languageService;
        }

        [HttpGet]
        [Route("Account/Login")]
        public async Task<IActionResult> Login()
        {
            var credentialsVM = await _credentialsRepository.GetLoggedInUser();
            var credentials = credentialsVM.Credentials;
            if (credentials != null &&
                credentials.IsAuthenticated())
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        [Route("Account/Login")]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            var credentialsVM = await _credentialsRepository.GetLoggedInUser();
            var credentials = credentialsVM.Credentials;
            if (credentials != null &&
                credentials.IsAuthenticated())
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = _languageService.GetKey("InputIsNotValid");

                return View(loginVM);
            }

            var user = await _userManager.FindByEmailAsync(loginVM.Email);
            if (user != null)
            {
                _logger.LogInformation($"(Account/Login) User {loginVM.Email} is trying to log in");

                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                if (passwordCheck)
                {
                    _logger.LogInformation($"(Account/Login) User {loginVM.Email}, Check password succeeded");

                    var signInResult = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                    if (signInResult.Succeeded)
                    {
                        _logger.LogInformation($"(Account/Login) User {loginVM.Email}, Log in succeeded");

                        return RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        _logger.LogInformation($"(Account/Login) User {loginVM.Email}, Attempt to log in failed");
                    }
                }
                else
                {
                    _logger.LogInformation($"(Account/Login) User {loginVM.Email}, Check password failed");
                }
            }
            else
            {
                _logger.LogInformation($"(Account/Login) Fail, User {loginVM.Email} doesn't exist");
            }

            TempData["Error"] = _languageService.GetKey("WrongCredentials");

            return View(loginVM);
        }

        [HttpGet]
        [Route("Account/Register")]
        public async Task<IActionResult> Register()
        {
            var credentialsVM = await _credentialsRepository.GetLoggedInUser();
            var credentials = credentialsVM.Credentials;
            if (credentials != null &&
                credentials.IsAuthenticated())
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [Route("Account/Register")]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            var credentialsVM = await _credentialsRepository.GetLoggedInUser();
            var credentials = credentialsVM.Credentials;
            if (credentials != null &&
                credentials.IsAuthenticated())
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = _languageService.GetKey("InputIsNotValid");

                return View(registerVM);
            }

            _logger.LogInformation($"(Account/Register) Trying to register user {registerVM.Email}");

            var user = await _userManager.FindByEmailAsync(registerVM.Email);
            if (user != null)
            {
                _logger.LogInformation($"(Account/Register) User {registerVM.Email} failed to register. E-mail already in use");
                TempData["Error"] = _languageService.GetKey("EmailAlreadyInUse");

                return View(registerVM);
            }

            var newUser = new UserModel()
            {
                Id = _randomGenerator.GetRandomId(),
                Email = registerVM.Email,
                UserName = registerVM.UserName
            };

            var newUserCreateResponse = await _userManager.CreateAsync(newUser, registerVM.Password);
            if (newUserCreateResponse.Succeeded)
            {
                _logger.LogInformation($"(Account/Register) User {registerVM.Email} has succeeded to register!");

                await _userManager.AddToRoleAsync(newUser, UserRoles.User);
                var signInResult = await _signInManager.PasswordSignInAsync(newUser, registerVM.Password, false, false);
                if (signInResult.Succeeded)
                {
                    _logger.LogInformation($"(Account/Register) User {registerVM.Email} made first log in!");

                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    _logger.LogInformation($"(Account/Register) User {registerVM.Email} can't login into new account");
                    TempData["Error"] = _languageService.GetKey("CantLoginIntoCreatedAccount");

                    return View(registerVM);
                }
            }
            else
            {
                _logger.LogInformation($"(Account/Register) User {registerVM.Email} failed to register");
            }

            TempData["Error"] = _languageService.GetKey("RegistrationFailed");

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
