using AICVScanner.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace AICVScanner.Controllers;

[Route("account")]
public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
         _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost("login")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Login(
        [FromForm] string email,
        [FromForm] string password,
        [FromForm] string? returnUrl)
    {
        var result = await _signInManager.PasswordSignInAsync(
            email, password, isPersistent: true, lockoutOnFailure: false);
        
        if (result.Succeeded)
        {
            var target = !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
                ? returnUrl
                : "/dashboard";
            return LocalRedirect(target);
        }

        return Redirect("/login?error=" + Uri.EscapeDataString("Invalid email or password"));
    }

    [HttpPost("register")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Register(
        [FromForm] string fullName,
        [FromForm] string email,
        [FromForm] string password)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName
        };
        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: true);
            return LocalRedirect("/dashboard");
        }

        var message = string.Join("", result.Errors.Select(e => e.Description));
        return Redirect("/register?error=" + Uri.EscapeDataString(message));
    }

    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return LocalRedirect("/");
    }
}