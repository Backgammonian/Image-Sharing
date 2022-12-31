using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.ViewModels;

namespace MyWebApp.Controllers
{
    public sealed class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly RandomGenerator _randomGenerator;

        public AccountController(ILogger<AccountController> logger,
            UserManager<UserModel> userManager,
            SignInManager<UserModel> signInManager,
            RandomGenerator randomGenerator)
        {
            _logger = logger;
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
                _logger.LogInformation($"(Account/Login) User {loginVM.Email} is trying to log in");

                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                if (passwordCheck)
                {
                    _logger.LogInformation($"(Account/Login) User {loginVM.Email}, Check password succeeded");

                    var signInResult = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                    if (signInResult.Succeeded)
                    {
                        _logger.LogInformation($"(Account/Login) User {loginVM.Email}, Log in succeeded");

                        return RedirectToAction("Index", "Notes");
                    }
                    else
                    {
                        _logger.LogInformation($"(Account/Login) User {loginVM.Email}, Attempt to log in failed");
                    }
                }
                else
                {
                    _logger.LogInformation($"(Account/Login) User {loginVM.Email}, Attempt to check password failed");
                }
            }
            else
            {
                _logger.LogInformation($"(Account/Login) Fail, User {loginVM.Email} doesn't exist");
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

            _logger.LogInformation($"(Account/Register) Trying to register user {registerVM.Email}");

            var user = await _userManager.FindByEmailAsync(registerVM.Email);
            if (user != null)
            {
                _logger.LogInformation($"(Account/Register) User {registerVM.Email} failed to register. Email already in use");
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
                _logger.LogInformation($"(Account/Register) User {registerVM.Email} has succeeded to register!");

                await _userManager.AddToRoleAsync(newUser, UserRoles.User);
                var signInResult = await _signInManager.PasswordSignInAsync(newUser, registerVM.Password, false, false);
                if (signInResult.Succeeded)
                {
                    _logger.LogInformation($"(Account/Register) User {registerVM.Email} made first log in!");

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _logger.LogInformation($"(Account/Register) User {registerVM.Email} can't login into new account");
                    TempData["Error"] = "Can't login into created account!";

                    return View(registerVM);
                }
            }
            else
            {
                _logger.LogInformation($"(Account/Register) User {registerVM.Email} failed to register");
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
