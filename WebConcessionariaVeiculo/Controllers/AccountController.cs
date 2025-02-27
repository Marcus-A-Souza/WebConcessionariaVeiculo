using WebConcessionariasVeiculos.Models;
using WebConcessionariasVeiculos.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebConcessionariasVeiculos.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserManager<Usuario> _userManager;

        public AccountController(SignInManager<Usuario> signInManager, UserManager<Usuario> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState inválido.");
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                Console.WriteLine($"Usuário '{model.Email}' não encontrado.");
                ModelState.AddModelError("", "Usuário ou senha inválidos.");
                return View(model);
            }

            Console.WriteLine($"Usuário encontrado: {user.UserName}");

            var passwordCheck = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordCheck)
            {
                Console.WriteLine("Senha incorreta para o usuário: " + user.UserName);
                ModelState.AddModelError("", "Usuário ou senha inválidos.");
                return View(model);
            }

            Console.WriteLine("Senha correta! Tentando autenticar...");

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                Console.WriteLine("Login bem-sucedido para: " + user.UserName);
                return RedirectToAction("Index", "Home");
            }

            Console.WriteLine("Falha no login: " + result.ToString());
            ModelState.AddModelError("", "Usuário ou senha inválidos.");
            return View(model);
        }




        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new Usuario
            {
                UserName = model.Email,
                Email = model.Email,
                NomeCompleto = model.NomeCompleto
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
