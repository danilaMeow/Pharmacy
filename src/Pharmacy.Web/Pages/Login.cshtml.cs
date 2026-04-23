using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pharmacy.Web.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string UserName { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = UserName?.Trim().ToLower();
            var pass = Password?.Trim();

            if (user == "admin" && pass == "admin")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Role, "Admin")
                };
                var identity = new ClaimsIdentity(claims, "Cookies");
                await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(identity));

                return RedirectToPage("/Admin");
            }

            if (user != "admin" || pass != "admin")
            {
                ErrorMessage = "Неверное имя пользователя или пароль";
            }

            var uClaims = new List<Claim> { new Claim(ClaimTypes.Name, UserName ?? "User") };
            var uIdentity = new ClaimsIdentity(uClaims, "Cookies");
            await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(uIdentity));

            return RedirectToPage("/Index");
        }
    }
}